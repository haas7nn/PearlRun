using UnityEngine;

public class EnemyBase : MonoBehaviour
{
	public int health = 1;

	private bool isDead = false;
	private Rigidbody rb;
	private Collider col;
	private Renderer rend;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		col = GetComponent<Collider>();
		rend = GetComponentInChildren<Renderer>();
	}

	public void TakeDamage(int damage)
	{
		if (isDead) return;

		health -= damage;

		if (health <= 0)
		{
			Die();
		}
	}

	void Die()
	{
		isDead = true;

		if (AudioManager.instance != null)
			AudioManager.instance.PlayDeath();

		MovingObstacle move = GetComponent<MovingObstacle>();
		if (move != null)
			move.enabled = false;

		if (rb != null)
		{
			rb.isKinematic = false;
			rb.useGravity = true;

			rb.constraints = RigidbodyConstraints.FreezePositionZ |
							 RigidbodyConstraints.FreezeRotationX |
							 RigidbodyConstraints.FreezeRotationY;

			rb.AddForce(Vector3.right * 3f, ForceMode.Impulse);
			rb.AddTorque(Vector3.forward * 5f, ForceMode.Impulse);
		}

		if (col != null)
			col.enabled = false;

		StartCoroutine(DestroyWhenInvisible());
	}

	System.Collections.IEnumerator DestroyWhenInvisible()
	{
		yield return new WaitForSeconds(2f);

		while (true)
		{
			if (rend != null && !rend.isVisible)
			{
				Destroy(gameObject);
				yield break;
			}

			yield return null;
		}
	}
}