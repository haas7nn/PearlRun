using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
	[Header("Movement")]
	public Transform pointA;
	public Transform pointB;
	public float speed = 3f;

	[Header("Settings")]
	public bool pingPong = true;
	public float waitTime = 0f;

	private Vector3 targetPosition;
	private bool movingToB = true;
	private float waitTimer = 0f;
	private bool isWaiting = false;

	void Start()
	{
		if (pointA == null || pointB == null)
		{
			Debug.LogWarning(
				"MovingObstacle: pointA or pointB not set on "
				+ gameObject.name
			);
			return;
		}

		transform.position = pointA.position;
		targetPosition = pointB.position;
	}

	void Update()
	{
		if (pointA == null || pointB == null)
			return;

		if (isWaiting)
		{
			waitTimer -= Time.deltaTime;

			if (waitTimer <= 0f)
				isWaiting = false;

			return;
		}

		// Move toward target
		transform.position = Vector3.MoveTowards(
			transform.position,
			targetPosition,
			speed * Time.deltaTime
		);

		// Check if reached target
		if (Vector3.Distance(
			transform.position,
			targetPosition) < 0.01f)
		{
			if (pingPong)
			{
				movingToB = !movingToB;
				targetPosition = movingToB ?
					pointB.position :
					pointA.position;

				if (waitTime > 0f)
				{
					isWaiting = true;
					waitTimer = waitTime;
				}
			}
		}
	}

	void OnDrawGizmosSelected()
	{
		if (pointA != null && pointB != null)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(
				pointA.position,
				pointB.position
			);
			Gizmos.DrawWireSphere(
				pointA.position, 0.3f
			);
			Gizmos.DrawWireSphere(
				pointB.position, 0.3f
			);
		}
	}
}