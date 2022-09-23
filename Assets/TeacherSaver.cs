using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(AITeacher))]
public class TeacherSaver : MonoBehaviour
{
	public TMP_InputField saveNameInput;


    private AITeacher teacher;
	private SaveData loadedData;
	
	private string saveName;
    private void Start() 
	{
		teacher = GetComponent<AITeacher>();
		saveNameInput.onEndEdit.AddListener(SetSaveName);
	}
	public void SetSaveName(string name)
	{
		saveName = name;
	}

    void SaveCurrentOverall()
	{
		loadedData.batch = teacher.getBatch();
		loadedData.generation = teacher.getGeneration();
		loadedData.overallBestNet = teacher.OverallBestNet;
		//Save
		SaveSystem.SaveOld(GameSettings.current.loadIndex, loadedData);
	}
	public void Save()
	{
		if (loadedData != null)
		{
			SaveCurrentOverall();
		}
		else
		{
			loadedData = new SaveData();
			loadedData.saveName = saveName;
			loadedData.batch = teacher.getBatch();
			loadedData.generation = teacher.getGeneration();
			loadedData.overallBestNet = teacher.OverallBestNet;

			SaveSystem.SaveNew(loadedData);
		}
	}
}
