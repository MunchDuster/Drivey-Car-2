using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuStand : MonoBehaviour
{
	[System.Serializable]
	public class Menu
	{
		public GameObject gameObject;
		public float angle;
	}

	public Menu[] menus;
	public AnimationCurve turnCurve;
	public Transform spinPart;
	public float lerpTime;

	private int curMenu;
	private float curAngle;
	private float targetAngle;
	private int timesAround = 0;

	private void Start()
	{
		curMenu = 0;
		spinPart.localRotation = Quaternion.Euler(0, menus[curMenu].angle, 0);
		targetAngle = menus[curMenu].angle;
		curAngle = targetAngle;
	}
	public void NextMenu()
	{

		curMenu++;
		if (curMenu == menus.Length) curMenu = 0;
		Debug.Log("NEXT");
		Debug.Log("Going to " + menus[curMenu].angle);
		Debug.Log("From " + targetAngle);

		float nextTargetAngle = menus[curMenu].angle + 360 * timesAround;
		if (nextTargetAngle < targetAngle)
		{
			timesAround++;
		}

		targetAngle = menus[curMenu].angle + 360 * timesAround;

		StartCoroutine(LerpToMenu());
	}
	public void LastMenu()
	{
		curMenu--;
		if (curMenu == -1) curMenu = menus.Length - 1;

		float nextTargetAngle = menus[curMenu].angle + 360 * timesAround;
		if (nextTargetAngle > targetAngle)
		{
			timesAround--;
		}

		targetAngle = menus[curMenu].angle + 360 * timesAround;

		StartCoroutine(LerpToMenu());
	}

	IEnumerator LerpToMenu()
	{
		Debug.Log("Started Lerping.");
		float lastAngle = curAngle;
		float myTarget = targetAngle;

		float dt = 0.03f;
		for (float l = 0; l <= 1 && myTarget == targetAngle; l += dt / lerpTime)
		{
			curAngle = Mathf.Lerp(lastAngle, targetAngle, turnCurve.Evaluate(l));
			spinPart.localRotation = Quaternion.Euler(0, curAngle, 0);
			yield return new WaitForSeconds(dt);
		}
		Debug.Log("Finished.");
	}
}
