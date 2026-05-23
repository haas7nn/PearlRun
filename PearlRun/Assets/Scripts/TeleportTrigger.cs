using System.Collections;
using UnityEngine;

public class TeleportTrigger : MonoBehaviour
{
    [Header("Teleport")]
    public Transform destination;                 // Drag LoopStart here
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
    public Transform cameraToSnap;                // Leave null to use Camera.main
    public Vector3 cameraOffset = new Vector3(5f, 3f, -10f); // match your CameraController offset

    private bool onCooldown;

    private void OnTriggerEnter(Collider other)
    {
        if (onCooldown) return;
        if (!other.CompareTag("Player")) return;
        if (destination == null) return;

        // Compute offset relative to this trigger (optional)
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

        // --- Teleport player (Rigidbody or CharacterController or Transform) ---
        Rigidbody rb = other.attachedRigidbody;
        CharacterController cc = other.GetComponent<CharacterController>();

        Vector3 savedVel = Vector3.zero;
        if (rb != null)
            savedVel = rb.linearVelocity; // if your Unity complains, change to rb.linearVelocity

        if (cc != null)
        {
            // CharacterController needs disable -> move -> enable
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

        // --- Restore/adjust velocity ---
        if (rb != null)
        {
            if (preserveVelocity)
            {
                if (zeroVerticalVelocity) savedVel.y = 0f;
                rb.linearVelocity = savedVel; // if your Unity complains, change to rb.linearVelocity
            }
            else
            {
                rb.linearVelocity = Vector3.zero; // if your Unity complains, change to rb.linearVelocity
            }
        }

        // --- Snap camera to avoid smooth-lag after teleport ---
        if (snapCamera)
        {
            Transform camT = cameraToSnap != null ? cameraToSnap : (Camera.main != null ? Camera.main.transform : null);
            if (camT != null)
                camT.position = newPos + cameraOffset;
        }

        StartCoroutine(Cooldown());
    }

    private IEnumerator Cooldown()
    {
        onCooldown = true;
        yield return new WaitForSeconds(cooldownSeconds);
        onCooldown = false;
    }
}