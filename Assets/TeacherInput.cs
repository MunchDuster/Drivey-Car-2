using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


[RequireComponent(typeof(AITeacher))]
public class TeacherInput : MonoBehaviour
{	
	[Header("Inputs")]
	public TMP_InputField botsPerBatchInput;
	public TMP_InputField batchesPerGenerationInput;
	public TMP_InputField timeInput;
	public TMP_InputField timePerBatchInput;
	public TMP_InputField mutationInput;
	

	private AITeacher teacher;

    private void Start() 
	{
		teacher = GetComponent<AITeacher>();

		
		//add Input listeners
		botsPerBatchInput.onEndEdit.AddListener(SetBotsperBatch);
		batchesPerGenerationInput.onEndEdit.AddListener(SetBatchesPerGen);
		timeInput.onEndEdit.AddListener(SetTime);
		timePerBatchInput.onEndEdit.AddListener(SetTimePerBatch);
		mutationInput.onEndEdit.AddListener(SetMutations);
	}

	void SetBotsperBatch(string text)
	{
		try
		{
			teacher.botsPerBatch = int.Parse(text);
		}
		catch (System.Exception)
		{
			Debug.Log("Unable to parse.");
		}
	}
	void SetBatchesPerGen(string text)
	{
		try
		{
			teacher.batches = int.Parse(text);
		}
		catch (System.Exception)
		{
			Debug.Log("Unable to parse.");
		}
	}
	void SetTime(string text)
	{
		try
		{
			teacher.timePerBatch = int.Parse(text);
		}
		catch (System.Exception)
		{
			Debug.Log("Unable to parse.");
		}
	}
	void SetTimePerBatch(string text)
	{
		try
		{
			teacher.timeIncreasePerGen = float.Parse(text);
		}
		catch (System.Exception)
		{
			Debug.Log("Unable to parse.");
		}
	}
	void SetMutations(string text)
	{
		try
		{
			teacher.mutationChance = float.Parse(text);
		}
		catch (System.Exception)
		{
			Debug.Log("Unable to parse.");
		}
	}
}
