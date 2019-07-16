using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchToMove : MonoBehaviour
{
	[SerializeField]
	private float CameraSpeed = 0.5f;

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
			Touch touchy = Input.GetTouch(0);
		}
    }
}
