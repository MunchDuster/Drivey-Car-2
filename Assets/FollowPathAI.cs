using UnityEngine;

public class FollowPathAI : InputManager
{
	[Header("Obstacle avoidance settings")]
	public float reverseTime = 2;
	public float obstacleDistance = 5;
	public float timeBlockedUntilMove = 1;


	[Header("Track follow settings")]
	public float completedPointDistance = 0.5f;
	public float steerSensitivity = 5;
	public float maxDistanceFromTrackCenter = 1;
	public float followPointMaxAngle = 45;

	[Space(10)]
	public float secondsAhead = 4;
	public float minDistanceAhead = 10;
	public float maxDistanceAhead = 50;

	[Space(10)]
	public Rigidbody rb;
	public WaypointPath path;
	public Transform centerOfCar;

	private Waypoint waypoint;

	private bool isStopped;
	private float timeStopped;

	private bool isReversing;
	private float timeStartedReversing;


	// Start is called before the first frame update
	private void Start()
	{
		waypoint = path.GetClosestWaypoint(centerOfCar);
		vertical = 1;
	}

	// FixedUpdate is called every physics update
	private void FixedUpdate()
	{
		//State control
		if (isReversing)
		{
			if (timeStartedReversing >= reverseTime)
			{
				StopReversing();
			}
		}
		else
		{
			if (IsPathBlocked())
			{
				if (isStopped)
				{
					if (timeStopped >= timeBlockedUntilMove)
					{
						StartReversing();
					}
				}
				else
				{
					Stop();
				}
			}
			else
			{
				if (AtWaypoint())
				{
					waypoint = waypoint.nextPoint;
				}
				else
				{
					SteerToPoint();
				}
			}
		}
	}

	private void StartReversing()
	{
		Debug.Log("Started reversing");
		isReversing = true;

		horizontal = 1;
		vertical = -1;
	}
	private void StopReversing()
	{
		Debug.Log("Stopped reversing");

		isReversing = false;
		vertical = 1;
	}
	private void Stop()
	{
		isStopped = true;
		timeStopped = Time.time;
	}

	private bool AtWaypoint()
	{
		//Check if is within range
		Vector3 offset = waypoint.position - centerOfCar.position;
		bool closeEnough = offset.magnitude <= completedPointDistance;

		//Check if have passed line
		float dotProduct = Vector3.Dot(offset.normalized, centerOfCar.forward);
		bool passedAlready = dotProduct < 0;

		return closeEnough || passedAlready;
	}

	private bool IsPathBlocked()
	{
		//Fix at some waypoint
		return false;
		// float speed = rb.velocity.magnitude;
		// return Physics.Raycast(centerOfCar.position, centerOfCar.forward, obstacleDistance * speed);
	}

	private void SteerToPoint()
	{

		float distance = Mathf.Clamp(rb.velocity.magnitude * secondsAhead, minDistanceAhead, maxDistanceAhead);

		Vector3 point = path.GetPointAhead(centerOfCar, waypoint, distance, followPointMaxAngle);

		Debug.DrawRay(point, Vector3.up * 5, Color.white);

		Vector3 offset = point - centerOfCar.position;
		Vector3 relativeDirection = centerOfCar.InverseTransformDirection(offset);

		horizontal = Mathf.Clamp(relativeDirection.x * steerSensitivity, -1, 1);

	}
}
