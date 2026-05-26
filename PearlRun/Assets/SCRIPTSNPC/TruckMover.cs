using UnityEngine;

public class TruckMover : MonoBehaviour
{
    public float speed = 40f;  

    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;
    }
}