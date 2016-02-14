using UnityEngine;
using System.Collections;

/// <summary>
/// A simple script that stores all the properties of a VCAnalogJoystick
/// in inspector-visible variables so that a Playmaker GetProperty action
/// can be used to inspect a joystick.
/// NOTE: You should use Unity's Edit > Project Settings > Script Execution Order
/// for your project to make sure this script gets executed AFTER the VCAnalogJoystick type you're using.
/// otherwise data will be 1 frame delayed.
/// </summary>
public class VCAnalogJoystickPlaymakerUpdater : MonoBehaviour 
{
	public VCAnalogJoystickBase joystick;
	
	#region Inspector variables
	public Vector3 axis; // playmaker reportedly doesn't like Vector2
	public Vector3 axisRaw; // playmaker reportedly doesn't like Vector2
	public int tapCount;
	
	// not implemented as they aren't very popular,
	// uncomment here and in Update() if you want to use
	
	//public float magnitudeSqr;
	//public float angleRadians;
	//public float angleDegrees;
	#endregion
	
	void Start () 
	{
		if (joystick == null)
		{
			// try to find it on this gameObject.
			joystick = gameObject.GetComponent<VCAnalogJoystickBase>();
			if (joystick == null)
			{
				VCUtils.DestroyWithError(gameObject, "You must specify a joystick for VCAnalogJoystickPlaymakerUpdater to function.  Destroying this object.");
				return;
			}
		}
	}
	
	void Update () 
	{
		axis.x = joystick.AxisX;
		axis.y = joystick.AxisY;
		
		axisRaw.x = joystick.AxisXRaw;
		axisRaw.y = joystick.AxisYRaw;
		
		tapCount = joystick.TapCount;
		
		// uncomment to have these updated each frame
		//magnitudeSqr = joystick.MagnitudeSqr;
		//angleRadians = joystick.AngleRadians;
		//angleDegrees = joystick.AngleDegrees;
	}
}
