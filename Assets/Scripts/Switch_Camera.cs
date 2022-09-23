using UnityEngine;

public class Switch_Camera : MonoBehaviour
{
	public Transform[] CameraPoints;

	private int currentCameraIndex;
	// Start is called before the first frame update
	void Start()
	{
		currentCameraIndex = 0;
		UpdateCamPos();
	}

	// Update is called once per frame
	public void Next()
	{
		currentCameraIndex++;
		if (currentCameraIndex == CameraPoints.Length) currentCameraIndex = 0;

		UpdateCamPos();
	}
	public void Last()
	{
		currentCameraIndex--;
		if (currentCameraIndex == -1) currentCameraIndex = CameraPoints.Length - 1;

		UpdateCamPos();
	}
	private void UpdateCamPos()
	{
		transform.position = CameraPoints[currentCameraIndex].position;
		transform.rotation = CameraPoints[currentCameraIndex].rotation;
	}
}
