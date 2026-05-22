using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(1000)]
[RequireComponent(typeof(Rigidbody))]
public class WaypointPathFollower : MonoBehaviour
{
    [Header("Parent of line_seg_01, line_seg_02, ...")]
    public Transform linesRoot;

    [Header("Milestones in the order you want (example: 4,15,1,12,2,13,3)")]
    public int[] milestoneOrder;

    [Header("Movement")]
    public float speed = 8f;
    public float rotationSpeed = 10f;
    public float reachDistance = 0.6f;
    public bool loop = false;

    Rigidbody rb;

    // Final expanded waypoint list (includes in-between segments)
    readonly List<Transform> waypoints = new();
    int index = 0;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        BuildWaypointList();
    }

    void BuildWaypointList()
    {
        waypoints.Clear();
        if (linesRoot == null) return;

        // Build dictionary: segNumber -> Transform
        Dictionary<int, Transform> segs = new();

        foreach (Transform t in linesRoot)
        {
            if (!t.name.StartsWith("line_seg_", StringComparison.OrdinalIgnoreCase))
                continue;

            string s = t.name.Substring("line_seg_".Length);
            if (int.TryParse(s, out int n))
                segs[n] = t;
        }

        if (segs.Count < 2) return;

        int minSeg = segs.Keys.Min();
        int maxSeg = segs.Keys.Max();

        // If no milestones given, just follow all segments in numeric order
        if (milestoneOrder == null || milestoneOrder.Length == 0)
        {
            foreach (var n in segs.Keys.OrderBy(x => x))
                waypoints.Add(segs[n]);
            return;
        }

        // Expand between milestones using the shortest direction around the loop
        int current = milestoneOrder[0];
        if (!segs.ContainsKey(current))
        {
            Debug.LogError($"Milestone {current} not found under {linesRoot.name}");
            return;
        }

        waypoints.Add(segs[current]);

        for (int m = 1; m < milestoneOrder.Length; m++)
        {
            int target = milestoneOrder[m];
            if (!segs.ContainsKey(target))
            {
                Debug.LogError($"Milestone {target} not found under {linesRoot.name}");
                return;
            }

            // choose direction (+1 or -1 with wrap) that uses fewer steps
            int forwardSteps = StepsForward(current, target, minSeg, maxSeg);
            int backwardSteps = StepsBackward(current, target, minSeg, maxSeg);
            int step = (forwardSteps <= backwardSteps) ? +1 : -1;

            // walk along segments until we reach target, adding every intermediate segment
            while (current != target)
            {
                current = NextIndex(current, step, minSeg, maxSeg);
                if (segs.TryGetValue(current, out var t))
                    waypoints.Add(t);
            }
        }
    }

    int NextIndex(int current, int step, int minSeg, int maxSeg)
    {
        int next = current + step;
        if (next > maxSeg) next = minSeg;
        if (next < minSeg) next = maxSeg;
        return next;
    }

    int StepsForward(int from, int to, int minSeg, int maxSeg)
    {
        int steps = 0;
        int cur = from;
        while (cur != to && steps < 10000)
        {
            cur = NextIndex(cur, +1, minSeg, maxSeg);
            steps++;
        }
        return steps;
    }

    int StepsBackward(int from, int to, int minSeg, int maxSeg)
    {
        int steps = 0;
        int cur = from;
        while (cur != to && steps < 10000)
        {
            cur = NextIndex(cur, -1, minSeg, maxSeg);
            steps++;
        }
        return steps;
    }

    Vector3 WaypointWorldPos(int i, Vector3 currentPos)
    {
        var t = waypoints[i];
        var r = t.GetComponentInChildren<Renderer>();
        Vector3 p = (r != null) ? r.bounds.center : t.position;
        p.y = currentPos.y; // keep jumping physics
        return p;
    }

    void FixedUpdate()
    {
        if (waypoints.Count < 2) return;

        if (!loop && index >= waypoints.Count)
        {
            // stop horizontal motion, keep vertical
            var v0 = rb.linearVelocity;
            rb.linearVelocity = new Vector3(0f, v0.y, 0f);
            return;
        }

        if (index >= waypoints.Count)
            index = 0;

        Vector3 pos = rb.position;
        Vector3 target = WaypointWorldPos(index, pos);

        Vector3 toTarget = target - pos;
        toTarget.y = 0f;

        if (toTarget.magnitude <= reachDistance)
        {
            index++;
            return;
        }

        Vector3 dir = toTarget.normalized;

        // move
        Vector3 v = rb.linearVelocity;
        rb.linearVelocity = new Vector3(dir.x * speed, v.y, dir.z * speed);

        // rotate
        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion look = Quaternion.LookRotation(dir, Vector3.up);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, look, rotationSpeed * Time.fixedDeltaTime));
        }
    }
}