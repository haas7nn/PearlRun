using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1000)]
[RequireComponent(typeof(Rigidbody))]
public class WaypointPathFollower : MonoBehaviour
{
    [Header("Path Root (parent of line_seg_01, line_seg_02, ...)")]
    public Transform pathRoot;

    [Header("Optional: only follow certain children (in this order). Leave empty to use all children.")]
    public string[] waypointChildNames;

    [Header("Movement")]
    public float speed = 8f;
    public float rotationSpeed = 10f;
    public float reachDistance = 0.8f;
    public bool loop = true;

    private Rigidbody rb;
    private readonly List<Transform> waypoints = new();
    private int index;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        waypoints.Clear();
        if (pathRoot == null) return;

        // Use only specified children (in that exact order)
        if (waypointChildNames != null && waypointChildNames.Length > 0)
        {
            foreach (var childName in waypointChildNames)
            {
                var t = pathRoot.Find(childName);
                if (t != null) waypoints.Add(t);
                else Debug.LogWarning($"Waypoint child not found: {childName}", this);
            }
        }
        else
        {
            // Use all children under pathRoot (hierarchy order)
            foreach (Transform t in pathRoot) waypoints.Add(t);
        }
    }

    Vector3 GetWaypointWorldPos(int i, Vector3 currentPos)
    {
        Transform t = waypoints[i];
        var r = t.GetComponentInChildren<Renderer>();
        Vector3 p = (r != null) ? r.bounds.center : t.position;
        p.y = currentPos.y; // keep physics vertical so jump still works
        return p;
    }

    void FixedUpdate()
    {
        if (waypoints.Count < 2) return;

        Vector3 pos = rb.position;
        Vector3 target = GetWaypointWorldPos(index, pos);

        Vector3 toTarget = target - pos;
        toTarget.y = 0f;

        if (toTarget.magnitude <= reachDistance)
        {
            index++;
            if (index >= waypoints.Count)
            {
                if (loop) index = 0;
                else return;
            }
            target = GetWaypointWorldPos(index, pos);
            toTarget = target - pos;
            toTarget.y = 0f;
        }

        Vector3 dir = toTarget.normalized;

        Vector3 v = rb.linearVelocity;
        rb.linearVelocity = new Vector3(dir.x * speed, v.y, dir.z * speed);

        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion look = Quaternion.LookRotation(dir, Vector3.up);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, look, rotationSpeed * Time.fixedDeltaTime));
        }
    }
}