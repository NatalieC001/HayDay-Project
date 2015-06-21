using UnityEngine;
using System.Collections;

/// <summary>
/// This is a C# and VCS friendly conversion of CameraRelativeControl.js found in
/// Unity's Standard Assets (Mobile) package.  For any questions, contact
/// support@bitbybitstudios.com.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class VCCameraRelativeControl : MonoBehaviour 
{
	public VCAnalogJoystickBase moveJoystick;
	public VCAnalogJoystickBase rotateJoystick;
	
	public Transform cameraPivot;								// The transform used for camera rotation
	public Transform cameraTransform;							// The actual transform of the camera
	
	public float speed = 5.0f;									// Ground speed
	public float jumpSpeed = 8.0f;
	public float inAirMultiplier = .25f;						// Limiter for ground speed while jumping		
	public Vector2 rotationSpeed = new Vector2(50.0f, 25.0f);   // Camera rotation speed for each axis
	
	private Transform thisTransform;
	private CharacterController character;
	private Vector3 velocity;									// Used for continuing momentum while in air
	private bool canJump = true;
	
	private void Start()
	{
		// Cache component lookup at startup instead of doing this every frame	
		thisTransform = GetComponent<Transform>();
		character = GetComponent<CharacterController>();	
		
		// Move the character to the correct start position in the level, if one exists
		var spawn = GameObject.Find( "PlayerSpawn" );
		if ( spawn != null )
			thisTransform.position = spawn.transform.position;	
	}
	
	public void FaceMovementDirection()
	{
		var horizontalVelocity = character.velocity;
		horizontalVelocity.y = 0; // Ignore vertical movement
		
		// If moving significantly in a new direction, point that character in that direction
		if ( horizontalVelocity.magnitude > 0.1 )
			thisTransform.forward = horizontalVelocity.normalized;
	}
	
	public void OnEndGame()
	{
		// Disable joystick when the game ends	
		moveJoystick.enabled = false;
		rotateJoystick.enabled = false;
		
		// Don't allow any more control changes when the game ends
		this.enabled = false;
	}
	
	private void Update()
	{
		var movement = cameraTransform.TransformDirection( new Vector3( moveJoystick.AxisX, 0, moveJoystick.AxisY ) );
		// We only want the camera-space horizontal direction
		movement.y = 0;
		movement.Normalize(); // Adjust magnitude after ignoring vertical movement
		
		// Let's use the largest component of the joystick position for the speed.
		// VCS Note: It's better to instead use MagnitudeSqr.
		//var absJoyPos = new Vector2( Mathf.Abs( moveJoystick.AxisX ), Mathf.Abs( moveJoystick.AxisY ) );
		//movement *= speed * ( ( absJoyPos.x > absJoyPos.y ) ? absJoyPos.x : absJoyPos.y );
		movement *= speed * moveJoystick.MagnitudeSqr;
		
		// Check for jump
		if ( character.isGrounded )
		{
			if ( !rotateJoystick.Dragging )
				canJump = true;
			
			if ( canJump && rotateJoystick.TapCount == 2 )
			{
				// Apply the current movement to launch velocity
				velocity = character.velocity;
				velocity.y = jumpSpeed;
				canJump = false;
			}
		}
		else
		{			
			// Apply gravity to our velocity to diminish it over time
			velocity.y += Physics.gravity.y * Time.deltaTime;
			
			// Adjust additional movement while in-air
			movement.x *= inAirMultiplier;
			movement.z *= inAirMultiplier;
		}
		
		movement += velocity;
		movement += Physics.gravity;
		movement *= Time.deltaTime;
		
		// Actually move the character
		character.Move( movement );
		
		if ( character.isGrounded )
			// Remove any persistent velocity after landing
			velocity = Vector3.zero;
		
		// Face the character to match with where she is moving
		FaceMovementDirection();	
		
		// Scale joystick input with rotation speed
		Vector2 camRotation = new Vector2();
		camRotation.x = rotateJoystick.AxisX * rotationSpeed.x;
		camRotation.y = rotateJoystick.AxisY * rotationSpeed.y;
		camRotation *= Time.deltaTime;
		
		// Rotate around the character horizontally in world, but use local space
		// for vertical rotation
		cameraPivot.Rotate( 0, camRotation.x, 0, Space.World );
		cameraPivot.Rotate( camRotation.y, 0, 0 );
	}
}
