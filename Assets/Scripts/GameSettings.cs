using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
	public static GameSettings current;

    private void Start() 
	{
		if(current == null)
		{
			current = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}
    public bool startWithSaved = true;
    public bool isAIActive = true;
    
    public int loadIndex = -1;
}
