using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeRecorder : MonoBehaviour
{
	private static List<TimeRecorder> recorders;

	private List<Record> records = new List<Record>();
	private struct Record
	{
		private Vector3 position;
		private Quaternion rotation;
		private Transform transform;

		public Record(Transform transform)
		{
			position = transform.position;
			rotation = transform.rotation;

			this.transform = transform;
		}
		public void Play()
		{
			transform.position = position;
			transform.rotation = rotation;
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		records = new List<Record>();
	}

	private static bool isPlaying = false;

	public static void Play()
	{
		isPlaying = true;
	}
	public static void Stop()
	{
		isPlaying = false;
		foreach (TimeRecorder recorder in recorders)
		{

		}
	}

	// Update is called once per frame
	void LateUpdate()
	{
		if (isPlaying)
		{
			records[0].Play();
			records.RemoveAt(0);
		}
		else
		{
			records.Add(new Record(transform));
		}
	}
}
