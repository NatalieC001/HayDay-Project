using UnityEngine;
using System.Collections;

/// <summary>
/// This is a C# and VCS friendly conversion of FirstPersonControl.js found in
/// Unity's Standard Assets (Mobile) package.  For any questions, contact
/// support@bitbybitstudios.com.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class VCFirstPersonControl : MonoBehaviour 
{
	public VCAnalogJoystickBase moveTouchPad;
	public VCAnalogJoystickBase rotateTouchPad;						// If unassigned, tilt is used
	
	public Transform cameraPivot;									// The transform used for camera rotation
	
	public float forwardSpeed = 4.0f;
	public float backwardSpeed = 1.0f;
	public float sidestepSpeed = 1.0f;
	public float jumpSpeed = 8.0f;
	public float inAirMultiplier = 0.25f;							// Limiter for ground speed while jumping
	public Vector2 rotationSpeed = new Vector2( 50.0f, 25.0f );		// Camera rotation speed for each axis
	public float tiltPositiveYAxis = 0.6f;
	public float tiltNegativeYAxis = 0.4f;
	public float tiltXAxisMinimum = 0.1f;
	
	private Transform thisTransform;
	private CharacterController character;
	private Vector3 cameraVelocity;
	private Vector3 velocity;										// Used for continuing momentum while in air
	private bool canJump = true;
	
	private void Start () 
	{
		// Cache component lookup at startup instead of doing this every frame		
		thisTransform = GetComponent<Transform>();
		character = GetComponent<CharacterController>();	
	
		// Move the character to the correct start position in the level, if one exists
		var spawn = GameObject.Find( "PlayerSpawn" );
		if ( spawn != null )
			thisTransform.position = spawn.transform.position;
	}
	
	public void OnEndGame()
	{
		// Disable joystick when the game ends	
		moveTouchPad.enabled = false;
		
		if ( rotateTouchPad != null )
			rotateTouchPad.enabled = false;	
	
		// Don't allow any more control changes when the game ends
		this.enabled = false;
	}
	
	private void Update () 
	{
		var movement = thisTransform.TransformDirection( new Vector3( moveTouchPad.AxisX, 0.0f, moveTouchPad.AxisY ) );

		// We only want horizontal movement
		movement.y = 0.0f;
		movement.Normalize();
	
		// Apply movement from move joystick
		var absJoyPos = new Vector2( Mathf.Abs( moveTouchPad.AxisX ), Mathf.Abs( moveTouchPad.AxisY ) );	
		if ( absJoyPos.y > absJoyPos.x )
		{
			if ( moveTouchPad.AxisY > 0.0f )
				movement *= forwardSpeed * absJoyPos.y;
			else
				movement *= backwardSpeed * absJoyPos.y;
		}
		else
			movement *= sidestepSpeed * absJoyPos.x;		
		
		// Check for jump
		if ( character.isGrounded )
		{		
			var jump = false;
			
			VCAnalogJoystickBase touchPad = rotateTouchPad;
			
			if ( rotateTouchPad == null)
				touchPad = moveTouchPad;
		
			if ( !touchPad.Dragging )
				canJump = true;
			
		 	if ( canJump && touchPad.TapCount >= 2 )
		 	{
				jump = true;
				canJump = false;
		 	}	
			
			if ( jump )
			{
				// Apply the current movement to launch velocity		
				velocity = character.velocity;
				velocity.y = jumpSpeed;	
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
		
		// Apply rotation from rotation joystick
		if ( character.isGrounded )
		{
			var camRotation = Vector2.zero;
			
			if ( rotateTouchPad != null )
			{
				camRotation.x = rotateTouchPad.AxisX;
				camRotation.y = rotateTouchPad.AxisY;
			}
			else
			{
				// Use tilt instead
	//			print( iPhoneInput.acceleration );
				var acceleration = Input.acceleration;
				var absTiltX = Mathf.Abs( acceleration.x );
				if ( acceleration.z < 0.0f && acceleration.x < 0.0f )
				{
					if ( absTiltX >= tiltPositiveYAxis )
						camRotation.y = (absTiltX - tiltPositiveYAxis) / (1.0f - tiltPositiveYAxis);
					else if ( absTiltX <= tiltNegativeYAxis )
						camRotation.y = -( tiltNegativeYAxis - absTiltX) / tiltNegativeYAxis;
				}
				
				if ( Mathf.Abs( acceleration.y ) >= tiltXAxisMinimum )
					camRotation.x = -(acceleration.y - tiltXAxisMinimum) / (1.0f - tiltXAxisMinimum);
			}
			
			camRotation.x *= rotationSpeed.x;
			camRotation.y *= rotationSpeed.y;
			camRotation *= Time.deltaTime;
			
			// Rotate the character around world-y using x-axis of joystick
			thisTransform.Rotate( 0.0f, camRotation.x, 0.0f, Space.World );
			
			// Rotate only the camera with y-axis input
			cameraPivot.Rotate( -camRotation.y, 0.0f, 0.0f );
		}
	}
}
