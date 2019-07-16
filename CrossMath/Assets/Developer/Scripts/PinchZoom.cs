using UnityEngine;

public class PinchZoom : MonoBehaviour
{
	[SerializeField]
	private float perspectiveZoomSpeed = 0.5f;
	[SerializeField]
	private float perspectiveZoomMinAngle = 0.1f;
	[SerializeField]
	private float perspectiveZoomMaxAngle = 179.9f;

	[SerializeField]
	private float orthoZoomSpeed = 0.5f;
	[SerializeField]
	private float orthoSizeMin = 0.1f;
	[SerializeField]
	private float orthoSizeMax = 180.0f;

	private Camera m_Camera;

	void Start()
	{
		m_Camera = this.GetComponent<Camera>();
	}

	void Update()
	{
		//If there are two touches on the device...
		if(Input.touchCount == 2)
		{
			//Store both touches
			Touch touchZero = Input.GetTouch(0);
			Touch touchOne = Input.GetTouch(1);

			//Find the position in the previous frame of each touch.
			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

			//Find the magnitude of the vector (the distacne) between the touches in each frame.
			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

			//Find the difference in the distances between each frame.
			float deltaMagnitudediff = prevTouchDeltaMag - touchDeltaMag;

			//If the camera is orthographic...
			if (m_Camera.orthographic)
			{
				// ... change the orthographic size based on the change in distance between the touches.
				m_Camera.orthographicSize += deltaMagnitudediff * orthoZoomSpeed;

				//Make sure the orthographic size is between min (0) and max.
				m_Camera.orthographicSize = Mathf.Clamp(m_Camera.orthographicSize, orthoSizeMin, orthoSizeMax);
			}
			else
			{
				//Otherwsie change the field of view based on the change in distance between the touches.
				m_Camera.fieldOfView += deltaMagnitudediff * perspectiveZoomSpeed;

				//Clamp the field of view to make sure it's between min (0) and max (180) angles.
				m_Camera.fieldOfView = Mathf.Clamp(m_Camera.fieldOfView, perspectiveZoomMinAngle, perspectiveZoomMaxAngle);
			}
		}
	}
}
