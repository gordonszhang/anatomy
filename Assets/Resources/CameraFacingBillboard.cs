using UnityEngine;
using System.Collections;
 
public class CameraFacingBillboard : MonoBehaviour
{
    public Camera m_Camera;

	public float objectScale = 1.0f; 
	private Vector3 initialScale; 

 
	// scale object relative to distance from camera plane
	void Update () 
	{
		Plane plane = new Plane(m_Camera.transform.forward, m_Camera.transform.position); 
		float dist = plane.GetDistanceToPoint(transform.position); 
		transform.localScale = initialScale * dist * objectScale; 
	}

	void Start() {
		m_Camera = GameObject.Find("OVRCameraRig").transform.Find("TrackingSpace").transform.Find("CenterEyeAnchor").GetComponent<Camera>();
		GetComponent<Canvas>().worldCamera = m_Camera; 
		initialScale = transform.localScale; 
	}
 
    //Orient the camera after all movement is completed this frame to avoid jittering
    void LateUpdate()
    {
        transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward,
            m_Camera.transform.rotation * Vector3.up);
    }
}