using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Renderer flagRenderer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Save checkpoint position
            CheckpointSystem.SaveCheckpoint(other.transform.position);

            // Visual effect: change color
            if (flagRenderer != null)
            {
                flagRenderer.material.color = Color.yellow;
            }

            // Visual effect: scale up
            transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);

            Debug.Log("Checkpoint Saved");
        }
    }
}