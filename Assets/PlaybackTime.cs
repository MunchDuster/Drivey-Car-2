using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaybackTime : MonoBehaviour
{
	public InputManager input;
	// Start is called before the first frame update
	private void Start()
	{
		input.OnBrakePressed += PlayPause;
	}

	private bool isPlaying = false;
	private void PlayPause(bool isPressed)
	{
		if (!isPressed) return;

		isPlaying = !isPlaying;
	}
}
