using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerConnection : MonoBehaviour
{
	public delegate void Function(PlayerInput input);
	public Function OnPlayerJoin;
	private PlayerInputManager inputManager;
	private void Start()
	{
		inputManager = GetComponent<PlayerInputManager>();

		inputManager.JoinPlayer(-1, -1, "Gameplay", InputSystem.devices[0]);

	}
	void OnPlayerJoined(PlayerInput input)
	{
		if (OnPlayerJoin != null) OnPlayerJoin(input);
	}
}
