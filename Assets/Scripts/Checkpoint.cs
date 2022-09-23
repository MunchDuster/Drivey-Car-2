using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
	public float checkpointNumber = 1;
	private void OnTriggerEnter(Collider other)
	{
		RaceBotCollision bot = other.gameObject.GetComponent<RaceBotCollision>();
		if (bot == null) return;
		bot.bot.CheckPoint(checkpointNumber);
	}
}
