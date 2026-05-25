using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
	public float speed = 0.1f;

	void Update()
	{
		transform.position += Vector3.left * speed * Time.deltaTime;
	}
}