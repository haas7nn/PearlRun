using System.Collections;
using UnityEngine;

public class TeleportTrigger : MonoBehaviour
{
    [Header("Teleport")]
    public Transform destination;
    public float cooldownSeconds = 0.5f;

    [Header("Keep Offset (recommended: keep only Z for lane offset)")]
    public bool keepOffset = true;
    public bool keepOffsetX = false;
    public bool keepOffsetY = false;
    public bool keepOffsetZ = true;

    [Header("Velocity")]
    public bool preserveVelocity = true;
    public bool zeroVerticalVelocity = false;

    [Header("Camera Snap (optional)")]
    public bool snapCamera = true;
    public Transform cameraToSnap;
    public Vector3 cameraOffset = new Vector3(5f, 3f, -10f);

    [Header("Optional UI Warning (enable ONLY on Circuit1 -> Circuit2 trigger)")]
    public bool showWarning = false;
    public LapWarningUI warningUI;
    [TextArea] public string warningText = "FINAL LAP STARTING — GET READY";
    public float warningDelay = 0f;

    private bool onCooldown;

    private void OnTriggerEnter(Collider other)
    {
        if (onCooldown) return;
        if (!other.CompareTag("Player")) return;
        if (destination == null) return;

        Debug.Log($"Teleport fired on: {gameObject.name} | showWarning={showWarning} | warningUI={(warningUI == null ? "NULL" : warningUI.name)}", this);

        Vector3 offset = Vector3.zero;
        if (keepOffset)
        {
            Vector3 raw = other.transform.position - transform.position;
            offset = new Vector3(
                keepOffsetX ? raw.x : 0f,
                keepOffsetY ? raw.y : 0f,
                keepOffsetZ ? raw.z : 0f
            );
        }

        Vector3 newPos = destination.position + offset;

        Rigidbody rb = other.attachedRigidbody;
        CharacterController cc = other.GetComponent<CharacterController>();

        Vector3 savedVel = Vector3.zero;
        if (rb != null)
            savedVel = rb.linearVelocity; // if your Unity version uses rb.velocity, swap to that

        if (cc != null)
        {
            cc.enabled = false;
            other.transform.position = newPos;
            cc.enabled = true;
        }
        else if (rb != null)
        {
            rb.position = newPos;
        }
        else
        {
            other.transform.position = newPos;
        }

        if (rb != null)
        {
            if (preserveVelocity)
            {
                if (zeroVerticalVelocity) savedVel.y = 0f;
                rb.linearVelocity = savedVel;
            }
            else
            {
                rb.linearVelocity = Vector3.zero;
            }
        }

        if (snapCamera)
        {
            Transform camT = cameraToSnap != null ? cameraToSnap : (Camera.main != null ? Camera.main.transform : null);
            if (camT != null)
                camT.position = newPos + cameraOffset;
        }

        // Show warning ONLY if enabled on this trigger
        if (showWarning)
        {
            if (warningUI != null)
                StartCoroutine(ShowWarningAfterDelay());
            else
                Debug.LogWarning("TeleportTrigger: showWarning is ON but warningUI is NOT assigned on " + gameObject.name, this);
        }

        StartCoroutine(Cooldown());
    }

    [ContextMenu("TEST Show Warning")]
    private void TestShowWarning()
    {
        if (warningUI != null)
            warningUI.Show(warningText);
        else
            Debug.LogWarning("TeleportTrigger: warningUI is NOT assigned on " + gameObject.name, this);
    }

    private IEnumerator ShowWarningAfterDelay()
    {
        if (warningDelay > 0f)
            yield return new WaitForSecondsRealtime(warningDelay);

        warningUI.Show(warningText);
    }

    private IEnumerator Cooldown()
    {
        onCooldown = true;
        yield return new WaitForSecondsRealtime(cooldownSeconds);
        onCooldown = false;
    }
}