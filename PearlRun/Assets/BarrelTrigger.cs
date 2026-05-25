using UnityEngine;

public class BarrelTrigger : MonoBehaviour
{
	public MovingBarrelObstacle barrel;

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			Debug.Log("Player triggered barrel");

			if (barrel != null)
				barrel.StartMoving();
			else
				Debug.LogWarning("Barrel is not assigned in BarrelTrigger.");
		}
	}
}