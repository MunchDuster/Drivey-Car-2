using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelFollowCollider : MonoBehaviour
{
	public WheelCollider wheelC;
	private Vector3 wheelCCenter;
	private Quaternion wheelCForward;
	private RaycastHit hit;
	float rotation;

	void Update()
	{
		wheelC.GetWorldPose(out Vector3 pos, out Quaternion quat);
		transform.position = pos;
		transform.rotation = quat;

	}
}