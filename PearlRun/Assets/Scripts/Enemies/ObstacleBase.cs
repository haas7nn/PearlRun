using UnityEngine;

public class ObstacleBase : MonoBehaviour
{
	[Header("Obstacle Settings")]
	public bool isBreakable = false;
	public bool isDeadly = false;

	void OnCollisionEnter(Collision collision)
	{
		HandleContact(collision.gameObject);
	}

	void OnTriggerEnter(Collider other)
	{
		HandleContact(other.gameObject);
	}

	void HandleContact(GameObject other)
	{
		if (!other.CompareTag("Player"))
			return;

		if (isDeadly)
		{
			if (GameManager.instance != null)
				GameManager.instance.PlayerDied();
		}
		else
		{
			PlayerController player =
				other.GetComponent<PlayerController>();

			if (player != null)
				player.TakeDamage();
		}
	}

	public void Break()
	{
		if (isBreakable)
			Destroy(gameObject);
	}
}