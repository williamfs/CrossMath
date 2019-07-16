using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchToMove : MonoBehaviour
{
	[SerializeField]
	private float CameraSpeed = 0.005f;
	[SerializeField]
	private float CameraMinX = -1.5f;
	[SerializeField]
	private float CameraMaxX = 1.5f;
	[SerializeField]
	private float CameraMinY = -1.5f;
	[SerializeField]
	private float CameraMaxY = 1.5f;

	private Camera m_Camera;
    // Start is called before the first frame update
    void Start()
    {
		m_Camera = this.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
		//If only one finger is touching the screen...
        if (Input.touchCount == 1)
		{

			Vector3 CamPosition = m_Camera.transform.position;
			Touch touchy = Input.GetTouch(0);
			float newX = touchy.position.x;
			float newY = touchy.position.y;
			float newZ = CamPosition.z;

			//map the camera position to the min/max range values.

			//Make sure the orthographic size is between min (0) and max.
			//newX = Mathf.Clamp(newX, CameraMinX, CameraMaxX);
			//newY = Mathf.Clamp(newY, CameraMinY, CameraMaxY);
			newX = map(Screen.safeArea.x, Screen.safeArea.width, CameraMinX, CameraMaxX, touchy.position.x);
			newX = map(Screen.safeArea.y, Screen.safeArea.height, CameraMinY, CameraMaxY, touchy.position.y);
			Vector3 newPos = new Vector3(newX, newY, newZ);

			CamPosition = newPos;

		}
    }

	public float map(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue)
	{

		float OldRange = (OldMax - OldMin);
		float NewRange = (NewMax - NewMin);
		float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;

		return (NewValue);
	}
}
