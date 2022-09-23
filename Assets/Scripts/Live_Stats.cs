using UnityEngine;
using TMPro;
using System.Collections;
public class Live_Stats : MonoBehaviour
{
	public TMP_Text FPSText;

	// Update is called once per frame
	void Start()
	{
		StartCoroutine(UpdateFPS());
	}
	IEnumerator UpdateFPS()
	{
		while (true)
		{
			float fps = (1.0f / Time.smoothDeltaTime);
			if (fps < 1)
				FPSText.text = "FPS: " + (((int)fps * 10) / 10f);
			else
				FPSText.text = "FPS: " + ((int)fps);
			yield return new WaitForSeconds(0.75f);
		}
	}

}
