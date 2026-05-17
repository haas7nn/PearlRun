using UnityEngine;

public class MovingBarrelObstacle : MonoBehaviour
{
	[Header("Movement")]
	public Transform pointA;
	public Transform pointB;
	public float speed = 3f;
	public float rollSpeed = 250f;

	[Header("Settings")]
	public bool pingPong = false;
	public float waitTime = 0f;
	public bool startOnlyWhenTriggered = true;

	private Vector3 targetPosition;
	private bool movingToB = true;
	private float waitTimer = 0f;
	private bool isWaiting = false;
	private bool canMove = false;

	void Start()
	{
		if (pointA == null || pointB == null)
		{
			Debug.LogWarning(
				"MovingBarrelObstacle: pointA or pointB not set on "
				+ gameObject.name
			);
			return;
		}

		transform.position = pointA.position;
		targetPosition = pointB.position;

		if (!startOnlyWhenTriggered)
			canMove = true;
	}

	void Update()
	{
		if (pointA == null || pointB == null)
			return;

		if (!canMove)
			return;

		if (isWaiting)
		{
			waitTimer -= Time.deltaTime;

			if (waitTimer <= 0f)
				isWaiting = false;

			return;
		}

		transform.position = Vector3.MoveTowards(
			transform.position,
			targetPosition,
			speed * Time.deltaTime
		);

		transform.Rotate(
			rollSpeed * Time.deltaTime,
			0f,
			0f,
			Space.Self
		);

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
			else
			{
				canMove = false;
			}
		}
	}

	public void StartMoving()
	{
		canMove = true;
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
				pointA.position,
				0.3f
			);
			Gizmos.DrawWireSphere(
				pointB.position,
				0.3f
			);
		}
	}
}