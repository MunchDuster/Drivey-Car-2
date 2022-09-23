using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	[Header("Lerping")]
	public float moveSpeed = 10;
	public float turnSpeed = 20;

	[Header("References")]
	public Transform trackBody;

	//private vars
	private Vector3 offset;

	void Start()
	{
		offset = transform.position - trackBody.position;
	}

	// Update is called once per frame
	private void LateUpdate()
	{
		//POSITION
		Vector3 _pos = trackBody.TransformPoint(offset);
		transform.position = Vector3.Lerp(transform.position, _pos, moveSpeed * Time.deltaTime);

		//ROTATION
		Vector3 lookDirection = trackBody.position - transform.position;
		Quaternion _rot = Quaternion.LookRotation(lookDirection.normalized, Vector3.up);
		transform.rotation = Quaternion.Lerp(transform.rotation, _rot, turnSpeed * Time.deltaTime);
	}
}