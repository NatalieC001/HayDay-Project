using UnityEngine;
using System.Collections;

/// <summary>
/// This is a C# and VCS friendly conversion of SmoothFollow2D.js found in
/// Unity's Standard Assets (Mobile) package.  For any questions, contact
/// support@bitbybitstudios.com.
/// </summary>
public class VCSmoothFollow2D : MonoBehaviour 
{
	public Transform target;
	public float smoothTime = 0.3f;
	private Transform thisTransform;
	private Vector2 velocity;

	private void Start()
	{
		thisTransform = transform;
	}

	private void Update() 
	{
		Vector3 vec = thisTransform.position;
		vec.x = Mathf.SmoothDamp( thisTransform.position.x, 
			target.position.x, ref velocity.x, smoothTime);
		vec.y = Mathf.SmoothDamp( thisTransform.position.y, 
			target.position.y, ref velocity.y, smoothTime);
		thisTransform.position = vec;
	}
}
