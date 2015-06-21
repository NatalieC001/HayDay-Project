using UnityEngine;
using System.Collections;

/// <summary>
/// This is a C# and VCS friendly conversion of RotationConstraint.js found in
/// Unity's Standard Assets (Mobile) package.  For any questions, contact
/// support@bitbybitstudios.com.
/// </summary>
public class VCRotationConstraint : MonoBehaviour 
{
	private const int kAxisX = 0;
	private const int kAxisY = 1;
	private const int kAxisZ = 2;

	public int axis; 			// Rotation around this axis is constrained
	public float min;						// Relative value in degrees
	public float max;						// Relative value in degrees
	private Transform thisTransform;
	private Vector3 rotateAround;
	private Quaternion minQuaternion;
	private Quaternion maxQuaternion;
	private float range;

	private void Start()
	{
		thisTransform = transform;
	
		// Set the axis that we will rotate around
		switch ( axis )
		{
			case kAxisX:
				rotateAround = Vector3.right;
				break;
				
			case kAxisY:
				rotateAround = Vector3.up;
				break;
				
			case kAxisZ:
				rotateAround = Vector3.forward;
				break;
		}
		
		// Set the min and max rotations in quaternion space
		var axisRotation = Quaternion.AngleAxis( thisTransform.localRotation.eulerAngles[axis], rotateAround );
		minQuaternion = axisRotation * Quaternion.AngleAxis( min, rotateAround );
		maxQuaternion = axisRotation * Quaternion.AngleAxis( max, rotateAround );
		range = max - min;
	}

	// We use LateUpdate to grab the rotation from the Transform after all Updates from
	// other scripts have occured
	private void  LateUpdate() 
	{
		// We use quaternions here, so we don't have to adjust for euler angle range [ 0, 360 ]
		var localRotation = thisTransform.localRotation;
		var axisRotation = Quaternion.AngleAxis( localRotation.eulerAngles[ axis ], rotateAround );
		var angleFromMin = Quaternion.Angle( axisRotation, minQuaternion );
		var angleFromMax = Quaternion.Angle( axisRotation, maxQuaternion );
			
		if ( angleFromMin <= range && angleFromMax <= range )
			return; // within range
		else
		{		
			// Let's keep the current rotations around other axes and only
			// correct the axis that has fallen out of range.
			var euler = localRotation.eulerAngles;			
			if ( angleFromMin > angleFromMax )
				euler[ axis ] = maxQuaternion.eulerAngles[ axis ];
			else
				euler[ axis ] = minQuaternion.eulerAngles[ axis ];
	
			thisTransform.localEulerAngles = euler;		
		}
	}
}
