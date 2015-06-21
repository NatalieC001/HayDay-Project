using UnityEngine;
using System.Collections;

/// <summary>
/// This is a C# and VCS friendly conversion of FPSInputController.js found in
/// Unity's Character Controllers package.  For any questions, contact
/// support@bitbybitstudios.com.
/// </summary>
[RequireComponent(typeof(VCCharacterMotor))]
public class VCFPSInputController : MonoBehaviour 
{
	public VCAnalogJoystickBase moveJoystick;
	public VCButtonBase jumpButton;
	
	private VCCharacterMotor motor;
	
	private void Awake()
	{
		motor = GetComponent<VCCharacterMotor>();	
		
		bool error = false;
		if (moveJoystick == null)
		{
			Debug.LogError("VCFPSInputController's moveJoystick is unassigned!  Assign it before using this Component.  Disabling Component for now.");
			error = true;
		}
		if (jumpButton == null)
		{
			Debug.LogError("VCFPSInputController's jumpButton is unassigned!  Assign it before using this Component.  Disabling Component for now.");
			error = true;
		}
		
		// disable this script if we had an error
		this.enabled = !error;
	}
	
	void Update () 
	{
		var directionVector = new Vector3(moveJoystick.AxisX, 0.0f, moveJoystick.AxisY);
	
		if (directionVector != Vector3.zero) 
		{
			// Get the length of the directon vector and then normalize it
			// Dividing by the length is cheaper than normalizing when we already have the length anyway
			var directionLength = directionVector.magnitude;
			directionVector = directionVector / directionLength;
			
			// Make sure the length is no bigger than 1
			directionLength = Mathf.Min(1.0f, directionLength);
			
			// Make the input vector more sensitive towards the extremes and less sensitive in the middle
			// This makes it easier to control slow speeds when using analog sticks
			directionLength = directionLength * directionLength;
			
			// Multiply the normalized direction vector by the modified length
			directionVector = directionVector * directionLength;
		}
		
		// Apply the direction to the CharacterMotor
		motor.inputMoveDirection = transform.rotation * directionVector;
		motor.inputJump = jumpButton.Pressed;
	}
}
