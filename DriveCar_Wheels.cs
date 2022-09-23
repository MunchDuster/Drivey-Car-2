using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DriveCar_Wheels : MonoBehaviour
{
	[System.Serializable]
	public struct Wheel
	{
		public WheelCollider collider;
		public Transform mesh;
		public bool meshFlipped;
	}

	public float speed = 10;
	public float acceleration = 20;
	public float jumpForce = 1;
	public float turnSpeed = 5;
	public float turnAngle = 20;
	public float brakeForce = 100000;
	public Vector3 centerOfMass = Vector3.zero;
	public InputManager input;
	public Wheel[] driveWheels;
	public Wheel[] steerWheels;

	private Vector3[] wheelpoints;
	private Rigidbody rb;
	private float throttle = 0;
	private float steer = 0;
	private Dictionary<WheelCollider, WheelFrictionCurve> originalCurves = new Dictionary<WheelCollider, WheelFrictionCurve>();
	private Wheel[] allWheels;

	void Throttle()
	{

		//THROTTLE - forward, backward and brakes
		foreach (Wheel wheel in driveWheels)
		{
			float targetTorque = throttle * speed;

			float direction = targetTorque - wheel.collider.motorTorque;

			float addTorque = Mathf.Clamp(direction, -acceleration, acceleration);

			wheel.collider.motorTorque = Mathf.Clamp(wheel.collider.motorTorque + addTorque * Time.fixedDeltaTime, -speed, speed);
		}
	}
	void Steer()
	{
		foreach (Wheel wheelT in steerWheels)
		{
			WheelCollider wheel = wheelT.collider;
			//if user not pressing side key then return to center if wheel is turned    
			float steerDirection = (Mathf.Abs(steer) > 0.5f) ? steer : (Mathf.Abs(wheel.steerAngle) > 1f) ? -wheel.steerAngle / Mathf.Abs(wheel.steerAngle) : 0;

			float deltaSteerAngle = Time.fixedDeltaTime * turnSpeed * steerDirection * turnAngle;

			wheel.steerAngle = Mathf.Clamp(wheel.steerAngle + deltaSteerAngle, -turnAngle, turnAngle);
		}
	}

	void Brake(bool isPressed)
	{
		if (isPressed)
		{
			foreach (Wheel wheel in driveWheels)
			{
				wheel.collider.brakeTorque = brakeForce;
			}
		}
		else
		{
			foreach (Wheel wheel in driveWheels)
			{
				wheel.collider.brakeTorque = 0;
			}
		}
	}
	void Reset(bool isPressed)
	{
		if (!isPressed)
			return;
		//go back to start position
		transform.position = Vector3.up * 3;
		transform.rotation = Quaternion.identity;
		//remove velocity
		rb.AddForce(-rb.velocity, ForceMode.VelocityChange);
		//remove angular velocity
		rb.AddTorque(-rb.angularVelocity, ForceMode.VelocityChange);
	}
	void Jump(bool isPressed)
	{
		if (!isPressed)
			return;

		rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
	}
	void Drift(bool isPressed)
	{
		if (isPressed)
		{
			foreach (Wheel wheel in driveWheels)
			{
				WheelFrictionCurve wheelSlideCurve = new WheelFrictionCurve();
				wheelSlideCurve.asymptoteSlip = 1;
				wheelSlideCurve.asymptoteValue = 1;
				wheelSlideCurve.extremumSlip = 1;
				wheelSlideCurve.extremumValue = 1;
				wheelSlideCurve.stiffness = 1;

				wheel.collider.sidewaysFriction = wheelSlideCurve;
			}
		}
		else
		{
			foreach (Wheel wheel in driveWheels)
			{
				wheel.collider.sidewaysFriction = originalCurves[wheel.collider];
			}
		}
	}
	void OnThrottleChange(bool isPressed)
	{
		if (isPressed)
		{

		}
		else
		{

		}
	}
	// Main functions
	void Start()
	{
		rb = GetComponent<Rigidbody>();

		rb.centerOfMass = centerOfMass;

		List<Wheel> totalWheels = new List<Wheel>();
		foreach (Wheel wheel in driveWheels)
		{
			if (!totalWheels.Contains(wheel)) totalWheels.Add(wheel);
		}
		foreach (Wheel wheel in steerWheels)
		{
			if (!totalWheels.Contains(wheel)) totalWheels.Add(wheel);
		}
		allWheels = totalWheels.ToArray();

		foreach (Wheel wheel in driveWheels)
		{
			originalCurves.Add(wheel.collider, wheel.collider.sidewaysFriction);
		}

		input.OnJumpPressed += Jump;
		input.OnBrakePressed += Brake;
		input.OnDriftPressed += Drift;
		input.OnResetPressed += Reset;
		input.OnMovePressed += OnThrottleChange;


		List<Vector3> wheelpointsList = new List<Vector3>();
		foreach (Wheel wheel in driveWheels)
		{
			Vector3 point = transform.position - wheel.collider.gameObject.transform.position;
			if (!wheelpointsList.Contains(point)) wheelpointsList.Add(point);
		}
		foreach (Wheel wheel in steerWheels)
		{
			Vector3 point = transform.position - wheel.collider.gameObject.transform.position;
			if (!wheelpointsList.Contains(point)) wheelpointsList.Add(point);
		}
		wheelpoints = wheelpointsList.ToArray();
	}
	void FixedUpdate()
	{
		//Player input
		throttle = input.vertical;
		steer = input.horizontal;


		//Apply main physics
		Throttle();
		Steer();
	}
	void Update()
	{
		//update wheel mesh positions
		foreach (Wheel wheel in allWheels)
		{
			wheel.collider.GetWorldPose(out Vector3 pos, out Quaternion quat);
			wheel.mesh.position = pos;
			wheel.mesh.rotation = quat;
			if (wheel.meshFlipped)
			{
				wheel.mesh.rotation *= Quaternion.Euler(0, 180, 0);
			}
		}
	}
}