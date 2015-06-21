using UnityEngine;
using System.Collections;

/// <summary>
/// This is a C# and VCS friendly conversion of PlatformInputController.js found in
/// Unity's Character Controllers package.  For any questions, contact
/// support@bitbybitstudios.com.
/// </summary>
[RequireComponent(typeof(VCCharacterMotor))]
public class VCPlatformInputController : MonoBehaviour 
{
	public VCAnalogJoystickBase moveJoystick;
	public VCButtonBase jumpButton;
	
	public bool autoRotate = true;
	public float maxRotationSpeed = 360.0f;
	
	private VCCharacterMotor motor;
	
	// Use this for initialization
	private void Awake () 
	{
		motor = GetComponent<VCCharacterMotor>();
		
		bool error = false;
		if (moveJoystick == null)
		{
			Debug.LogError("VCPlatformInputController's moveJoystick is unassigned!  Assign it before using this Component.  Disabling Component for now.");
			error = true;
		}
		if (jumpButton == null)
		{
			Debug.LogError("VCPlatformInputController's jumpButton is unassigned!  Assign it before using this Component.  Disabling Component for now.");
			error = true;
		}
		
		// disable this script if we had an error
		this.enabled = !error;
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Get the input vector from VCS Joystick
		var directionVector = new Vector3(moveJoystick.AxisX, moveJoystick.AxisY, 0);
		
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
		
		// Rotate the input vector into camera space so up is camera's up and right is camera's right
		directionVector = Camera.main.transform.rotation * directionVector;
		
		// Rotate input vector to be perpendicular to character's up vector
		var camToCharacterSpace = Quaternion.FromToRotation(-Camera.main.transform.forward, transform.up);
		directionVector = (camToCharacterSpace * directionVector);
		
		// Apply the direction to the CharacterMotor
		motor.inputMoveDirection = directionVector;
		motor.inputJump = jumpButton.Pressed;
		
		// Set rotation to the move direction	
		if (autoRotate && directionVector.sqrMagnitude > 0.01) {
			Vector3 newForward = ConstantSlerp(
				transform.forward,
				directionVector,
				maxRotationSpeed * Time.deltaTime
			);
			newForward = ProjectOntoPlane(newForward, transform.up);
			transform.rotation = Quaternion.LookRotation(newForward, transform.up);
		}
	}
	
	public Vector3 ProjectOntoPlane(Vector3 v, Vector3 normal)
	{
		return v - Vector3.Project(v, normal);
	}
	
	public Vector3 ConstantSlerp(Vector3 fromVec, Vector3 toVec, float angle)
	{
		float val = Mathf.Min(1.0f, angle / Vector3.Angle(fromVec, toVec));
		return Vector3.Slerp(fromVec, toVec, val);
	}
}
