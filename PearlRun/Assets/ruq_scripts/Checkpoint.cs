using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Renderer flagRenderer;
    private bool activated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (activated) return;

        if (other.CompareTag("Player"))
        {
            activated = true;

            CheckpointSystem.SaveCheckpoint(other.transform.position);

            if (flagRenderer != null)
            {
                flagRenderer.material.color = Color.yellow;
            }

            transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);

            Debug.Log("Checkpoint Saved");
        }
    }
}