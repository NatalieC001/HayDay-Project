using UnityEngine;
using System.Collections;

/// <summary>
/// This is a C# and VCS friendly conversion of FollowTransform.js found in
/// Unity's Standard Assets (Mobile) package.  For any questions, contact
/// support@bitbybitstudios.com.
/// </summary>
public class VCFollowTransform : MonoBehaviour {

	public Transform targetTransform;		// Transform to follow
	public bool faceForward = false;		// Match forward vector?
	private Transform thisTransform;

	private void  Start()
	{
		// Cache component lookup at startup instead of doing this every frame
		thisTransform = transform;
	}

	private void  Update () 
	{
		thisTransform.position = targetTransform.position;
	
		if ( faceForward )
			thisTransform.forward = targetTransform.forward;
	}
}
