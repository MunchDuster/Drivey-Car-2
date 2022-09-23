using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public NeuralNetwork overallBestNet;
    public int batch;
    public int generation;
    public string saveName;
}
