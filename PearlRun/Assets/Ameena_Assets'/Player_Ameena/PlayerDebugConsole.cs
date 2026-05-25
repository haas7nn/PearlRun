using UnityEngine;

public class PlayerDebugConsole : MonoBehaviour
{
    void Start()
    {
        Debug.Log("✅ PlayerDebugConsole Started on: " + gameObject.name);
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (h != 0 || v != 0)
        {
            Debug.Log("🎮 Input Detected | H: " + h + " | V: " + v);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("💥 Collided With: " + collision.gameObject.name);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("✨ Trigger Enter: " + other.gameObject.name);
    }
}