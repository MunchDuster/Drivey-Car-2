using UnityEngine;

public class DriveCar : MonoBehaviour
{
	[Header("Main Settings")]
	public float maxSpeed = 20;
	public float maxReverseSpeed = 10;
	public float turnSpeed = 45;
	public float acceleration = 10;
	public float brakeTorque = 40;
	public float maxTurnAngle = 20;
	public Transform centerOfMass;

	[Header("References")]
	public InputManager input;
	public WheelCollider[] drivingWheels;
	public WheelCollider[] steeringWheels;
	public Rigidbody rb;

	public float speed { get { return _speed; } }
	public float turn { get { return _turn; } }


	private float _speed;
	private float _turn;
	private float targetTurn;

	// Start is called before the first frame update
	private void Start()
	{
		rb.centerOfMass = centerOfMass.localPosition;
	}

	//FixedUpdate is called once per physics frame
	private void FixedUpdate()
	{
		DriveWheels();
		SteeringWheels();
	}

	//Controls motorTorque of driving wheels
	private void DriveWheels()
	{
		if (rb.velocity.magnitude > maxSpeed)
		{
			foreach (WheelCollider wheel in drivingWheels)
			{
				wheel.brakeTorque = brakeTorque;
				wheel.motorTorque = 0;
			}
		}
		else
		{
			_speed = Mathf.Clamp(speed + input.vertical * acceleration * maxSpeed * Time.fixedDeltaTime, -maxReverseSpeed, maxSpeed);

			foreach (WheelCollider wheel in drivingWheels)
			{
				wheel.brakeTorque = 0;
				wheel.motorTorque = _speed;
			}
		}

	}

	//Controls steerAngle of steering Wheels
	private void SteeringWheels()
	{
		targetTurn = input.horizontal * maxTurnAngle;

		_turn = Mathf.Lerp(_turn, targetTurn, turnSpeed * Time.fixedDeltaTime);

		foreach (WheelCollider wheel in steeringWheels)
		{
			wheel.steerAngle = turn;
		}
	}
}
