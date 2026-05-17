using UnityEngine;

public class BarrelTrigger : MonoBehaviour
{
	public MovingBarrelObstacle barrel;

	void OnTriggerEnter(Collider other)
	{
		Debug.Log("Something entered: " + other.name);

		if (other.CompareTag("Player"))
		{
			Debug.Log("Player entered, barrel should move");

			if (barrel != null)
				barrel.StartMoving();
		}
	}
}