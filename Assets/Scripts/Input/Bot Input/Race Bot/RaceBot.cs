using UnityEngine;

public class RaceBot : BotInput
{
	public RaceBotSettings settings;

	public Transform raycastPoint;

	int lastCheckpoint = 0;

	public override void UpdateFitness()
	{

		//not moving fast enough to stabily calculate dotproduct 
		if (rb.velocity.magnitude < 1) return;

		//get dot product of movement dirction compared to car forward direction
		Vector3 movementDirection = Vector3.Scale(rb.velocity.normalized, new Vector3(1, 0, 1));
		Vector3 forwardDirection = Vector3.Scale(transform.forward, new Vector3(1, 0, 1));
		float dotProduct = Vector3.Dot(movementDirection, forwardDirection);

		//if dot product is less than zero car is moving backwards
		//I will only reward car if it is moving forwards (or sideways for them sweet drifts)
		if (dotProduct > 0)
		{
			//get distance since last update
			float distance = rb.velocity.magnitude * Time.deltaTime;

			network.AddFitness(distance * settings.distanceScoreMultplier);
		}
	}
	public override int getInputLength()
	{
		return settings.noOfRays + 2;
	}
	public override float[] getInputs()
	{
		float[] inputs = new float[settings.noOfRays + 2];
		int i;

		for (i = 0; i < settings.noOfRays; i++)
		{
			float yAngle = i * 180 / (settings.noOfRays - 1) - 90;
			Vector3 rawDirection = Quaternion.Euler(settings.downAngle.Evaluate((float)i / (settings.noOfRays - 1)) * settings.downAngleMultiplier, yAngle, 0) * Vector3.forward;//new Vector3(-settings.maxLength, Mathf.Cos(180 * (i - 1) * Mathf.Deg2Rad),0);
			Vector3 direction = transform.TransformDirection(rawDirection);

			//Vector3 direction = (Quaternion.Euler(0, 180 * (i - 1) - 90, 0) * transform.forward);

			if (Physics.Raycast(raycastPoint.position, direction, out RaycastHit hit, settings.maxLength, settings.layerMask))
			{
				if (settings.drawRays) Debug.DrawRay(raycastPoint.position, hit.distance * direction, Color.red);
				inputs[i] = 1;
			}
			else
			{
				if (settings.drawRays) Debug.DrawRay(raycastPoint.position, direction * settings.maxLength, Color.green);
				inputs[i] = -1;
			}
		}


		inputs[i++] = (rb.angularVelocity.y);
		inputs[i++] = rb.velocity.magnitude / 2 - 1;
		return inputs;
	}
	public void LosePoints(float amount)
	{
		if (!inputEnabled) return;
		network.AddFitness(-amount * Time.deltaTime);
	}
	public void CheckPoint(float checkpointNumber)
	{
		if (!inputEnabled) return;
		if (checkpointNumber <= lastCheckpoint) return;

		network.AddFitness(settings.checkpointBonus * checkpointNumber);
	}
}
