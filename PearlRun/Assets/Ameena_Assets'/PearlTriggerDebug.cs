using UnityEngine;

public class PearlTriggerDebug : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("Pearl debug started on " + gameObject.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("PEARL TRIGGER touched by: " + other.name + " | tag: " + other.tag);
    }
}