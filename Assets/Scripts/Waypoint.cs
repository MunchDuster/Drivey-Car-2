using UnityEngine;

public class Waypoint : MonoBehaviour
{
	public float width = 10;
	[Range(-1f, 1f)] public float offset = 0;

	[HideInInspector] public Waypoint nextPoint;
	[HideInInspector] public Waypoint lastPoint;

	public Vector3 position { get { return transform.position; } }
	public Vector3 right { get { return transform.right; } }
	public Vector3 forward { get { return transform.forward; } }

	public Vector3 rightPoint { get { return transform.position + transform.right * width * (0.5f + offset / 2) / 2; } }
	public Vector3 leftPoint { get { return transform.position - transform.right * width * (0.5f - offset / 2) / 2; } }

	//Used by path
	public void Draw(bool showLinesBetweenWaypoints, bool showLineBetweenTransforms, bool showTransforms, bool showForwards)
	{
		//Spheres around transforms
		if (showTransforms)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, 1);
		}

		//Line between transforms
		if (showLineBetweenTransforms)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(rightPoint, leftPoint);
		}

		//Line forward
		if (showForwards)
		{
			Gizmos.color = new Color(1, 1, 0, 1);
			Gizmos.DrawRay(position, forward);
		}

		//Line to next point
		if (showLinesBetweenWaypoints)
		{
			Gizmos.color = Color.white;
			if (nextPoint != null) Gizmos.DrawLine(position, nextPoint.position);
		}
	}
}
