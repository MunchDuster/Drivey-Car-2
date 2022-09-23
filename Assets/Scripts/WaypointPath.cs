using System.Collections.Generic;
using UnityEngine;

public class WaypointPath : MonoBehaviour
{
	[Header("Get closest point settings")]
	public float maxAngle = 60;
	public float minDistance = 3;

	[Header("Debug settings")]
	public bool showLinesBetweenWaypoints = true;
	public bool showLineBetweenTransforms = true;
	public bool showTransforms = true;
	public bool showForwards = true;

	private Waypoint[] points;

	// Awake is called when the gameObject is activated
	private void Awake()
	{
		InitWaypoints();
	}

	//Populates waypoints from children transforms
	private void InitWaypoints()
	{
		//Init points array
		points = transform.GetComponentsInChildren<Waypoint>();

		//Set each point's nextPoint
		Waypoint lastPoint = null;
		for (int i = 0; i + 1 < transform.childCount; i++)
		{
			points[i].nextPoint = points[i + 1];
			points[i + 1].lastPoint = points[i];

			lastPoint = points[i];
		}

		//Set last point's nextPoint and lastPoint
		points[points.Length - 1].nextPoint = points[0];
		points[0].lastPoint = points[points.Length - 1];

	}

	//Set the closestWaypoint to subject
	public Waypoint GetClosestWaypoint(Transform subject)
	{
		//Create list copy of points array
		List<Waypoint> availablePoints = new List<Waypoint>();

		//Fill list with points in front of transform (within certian angle from front direction)
		foreach (Waypoint thispoint in points)
		{
			Vector3 forward = thispoint.forward;
			Vector3 direction = (thispoint.position - subject.position).normalized;

			float dot = Vector3.Dot(direction, forward);
			float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

			if (Mathf.Abs(angle) < maxAngle)
			{
				availablePoints.Add(thispoint);
			}
		}

		//Find the closest point
		Waypoint closest = availablePoints[0];
		float shortestDistance = 1000000000000000; //Some very large number
		bool hasSet = false;

		foreach (Waypoint thispoint in availablePoints)
		{
			float distance = (thispoint.position - subject.position).magnitude;
			if (distance < shortestDistance && distance > minDistance)
			{
				shortestDistance = distance;
				closest = thispoint;
				hasSet = true;
			}
		}


		//Return if was set, other wise there waas a problem, none was found
		if (!hasSet)
		{
			Debug.LogError("Waypoint path error in " + gameObject.name + ": No closest point to" + subject.gameObject.name);
		}
		return closest;
	}


	//Find point x distance ahead of subject
	public Vector3 GetPointAhead(Transform subject, Waypoint targetPoint, float distance, float maxAngle)
	{
		float totalDistance = 0;
		Vector3 point = Vector3.zero;


		//Distance to waypoint
		float wayPointDistance = (targetPoint.position - subject.position).magnitude;//Player to waypoint

		if (wayPointDistance >= distance)
		{
			//Go from car to target by distance
			Vector3 direction = (targetPoint.position - targetPoint.lastPoint.position).normalized;

			float distanceThrough = Vector3.Project(subject.position - targetPoint.lastPoint.position, direction).z;

			point = targetPoint.lastPoint.position + direction * (distanceThrough + distance);
		}
		else
		{
			Debug.DrawLine(subject.position, targetPoint.position, new Color(0, 1, 1, 1));

			while (true)
			{
				//Set total as current
				totalDistance = wayPointDistance;
				//Update current
				wayPointDistance += (targetPoint.nextPoint.position - targetPoint.position).magnitude;
				//Debug
				Debug.DrawLine(targetPoint.position, targetPoint.nextPoint.position, new Color(0, 1, 1, 1));

				//Exit check
				if (wayPointDistance >= distance) break;

				//Increment targetPoint
				targetPoint = targetPoint.nextPoint;//Increment
			}

			//Distance from waypoint to next
			float distanceBetweenWaypoints = (targetPoint.nextPoint.position - targetPoint.position).magnitude;
			float distanceLeft = distance - totalDistance;

			//Go from target to target.next by distanceLeft
			Vector3 direction = (targetPoint.nextPoint.position - targetPoint.position).normalized;
			point = targetPoint.position + direction * distanceLeft;

			Debug.DrawLine(targetPoint.position, point, new Color(0, 1, 1, 1));
		}

		return point;
	}

	// OnDrawGizmos is called every editor update
	private void OnDrawGizmos()
	{
		InitWaypoints();

		foreach (Waypoint point in points)
		{
			point.Draw(showLinesBetweenWaypoints, showLineBetweenTransforms, showTransforms, showForwards);
		}
	}
}
