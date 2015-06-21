using UnityEngine;
using System.Collections;

/// <summary>
/// This is a C# and VCS friendly conversion of ZoomCamera.js found in
/// Unity's Standard Assets (Mobile) package.  For any questions, contact
/// support@bitbybitstudios.com.
/// </summary>
public class VCZoomCamera : MonoBehaviour 
{
	public Transform origin; // What is considered the origin to the camera
	public float zoom;
	public float zoomMin = -5.0f;
	public float zoomMax = 5.0f;
	public float seekTime = 1.0f;
	public bool smoothZoomIn = false;
	
	private Vector3 defaultLocalPosition;
	private Transform thisTransform;
	private float currentZoom;
	private float targetZoom;
	private float zoomVelocity;

	private void Start()
	{
		// Cache component instead of looking it up every frame
		thisTransform = transform;
		
		// The default position is the position that is set in the editor
		defaultLocalPosition = thisTransform.localPosition;
		
		// Default the current zoom to what was set in the editor 
		currentZoom = zoom;
	}

	private void Update() 
	{
		// The zoom set externally must still be within the min-max range
		zoom = Mathf.Clamp( zoom, zoomMin, zoomMax );
		
		// Only collide with non-Player (8) layers
		var layerMask = ~((1 << 8) | (1 << 2));
	
		RaycastHit hit;
		var start = origin.position;
		var zoomedPosition = defaultLocalPosition + thisTransform.parent.InverseTransformDirection( thisTransform.forward * zoom );
		var end = thisTransform.parent.TransformPoint( zoomedPosition );
		
		// Cast a line from the origin transform to the camera and find out if we hit anything in-between
		if ( Physics.Linecast( start, end, out hit, layerMask ) ) 
		{
			// We hit something, so translate this to a zoom value
			var position = hit.point + thisTransform.TransformDirection( Vector3.forward );
			var difference = position - thisTransform.parent.TransformPoint( defaultLocalPosition );
			targetZoom = difference.magnitude;
		}
		else
			// We didn't hit anything, so the camera should use the zoom set externally
			targetZoom = zoom;
		
		// Clamp target zoom to our min-max range
		targetZoom = Mathf.Clamp( targetZoom, zoomMin, zoomMax );
		
		if ( !smoothZoomIn && ( targetZoom - currentZoom ) > 0 )
		{
			// Snap the current zoom to our target if it is closer. This is useful if
			// some object is between the camera and the origin
			currentZoom = targetZoom;
		}
		else
		{
			// Smoothly seek towards our target zoom value
			currentZoom = Mathf.SmoothDamp( currentZoom, targetZoom, ref zoomVelocity, seekTime );
		}
		
		// Set the position of the camera
		zoomedPosition = defaultLocalPosition + thisTransform.parent.InverseTransformDirection( thisTransform.forward * currentZoom );
		thisTransform.localPosition = zoomedPosition;
	}
}
