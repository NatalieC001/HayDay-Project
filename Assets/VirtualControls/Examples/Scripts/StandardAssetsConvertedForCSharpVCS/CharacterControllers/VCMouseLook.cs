using UnityEngine;
using System.Collections;

/// <summary>
/// This is a C# and VCS friendly conversion of MouseLook.cs found in
/// Unity's Character Controllers package.  It uses VCAnalogJoystickBase input
/// instead of mouse input.  For any questions, contact
/// support@bitbybitstudios.com.
/// </summary>
public class VCMouseLook : MonoBehaviour 
{
	public VCAnalogJoystickBase lookJoystick;
	
	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;

	public float minimumX = -360F;
	public float maximumX = 360F;

	public float minimumY = -60F;
	public float maximumY = 60F;

	float rotationY = 0F;

	void Update ()
	{
		if (axes == RotationAxes.MouseXAndY)
		{
			float rotationX = transform.localEulerAngles.y + lookJoystick.AxisX * sensitivityX;
			
			rotationY += lookJoystick.AxisY * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
		}
		else if (axes == RotationAxes.MouseX)
		{
			transform.Rotate(0, lookJoystick.AxisX * sensitivityX, 0);
		}
		else
		{
			rotationY += lookJoystick.AxisY * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
		}
	}
	
	void Start ()
	{
		// Make the rigid body not change rotation
		if (GetComponent<Rigidbody>())
			GetComponent<Rigidbody>().freezeRotation = true;
	}
			
}
