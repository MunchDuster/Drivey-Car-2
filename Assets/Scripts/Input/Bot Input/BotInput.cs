using UnityEngine;
using UnityEngine.Events;

public abstract class BotInput : InputManager
{
	[HideInInspector]
	public NeuralNetwork network;
	[Header("Raycast settings")]

	public bool inputEnabled = false;
	public MeshRenderer bodyRenderer;

	public Rigidbody rb;


	private void Update()
	{
		//Car input
		if (inputEnabled)
		{
			float[] outputs = network.FeedForward(getInputs());
			vertical = outputs[0];
			horizontal = (outputs[1] < 0) ? 0 : outputs[1] * 2 - 1;
		}
		else
		{
			vertical = 0;
			horizontal = 0;
		}

		//Nueral Network fitness
		UpdateFitness();
	}
	public abstract float[] getInputs();
	public abstract void UpdateFitness();
	public abstract int getInputLength();
}
