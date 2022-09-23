using UnityEngine;
using UnityEngine.Events;

public class RaceBotCollision : MonoBehaviour
{
	public float losePointsSpeed = 5;
	public float checkpointBonus = 10;
	public RaceBot bot;
	public UnityEvent OnFreeze;


	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.tag == "Ground")
		{
			OnFreeze.Invoke();
		}
	}
}
