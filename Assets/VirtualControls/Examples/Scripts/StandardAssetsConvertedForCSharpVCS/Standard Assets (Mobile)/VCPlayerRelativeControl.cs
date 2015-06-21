using UnityEngine;
using System.Collections;

/// <summary>
/// This is a C# and VCS friendly conversion of PlayerRelativeControl.js found in
/// Unity's Standard Assets (Mobile) package.  For any questions, contact
/// support@bitbybitstudios.com.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class VCPlayerRelativeControl : MonoBehaviour 
{
	public VCAnalogJoystickBase moveJoystick;
	public VCAnalogJoystickBase rotateJoystick;
	
	public Transform cameraPivot;									// The transform used for camera rotation
	
	public float forwardSpeed = 4.0f;
	public float backwardSpeed = 1.0f;
	public float sidestepSpeed = 1.0f;
	public float jumpSpeed = 8.0f;
	public float inAirMultiplier = 0.25f;							// Limiter for ground speed while jumping
	public Vector2 rotationSpeed = new Vector2( 50.0f, 25.0f );		// Camera rotation speed for each axis
	
	private Transform thisTransform;
	private CharacterController character;
	private Vector3 cameraVelocity;
	private Vector3 velocity;										// Used for continuing momentum while in air
	
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
		moveJoystick.enabled = false;
		rotateJoystick.enabled = false;
		
		// Don't allow any more control changes when the game ends
		this.enabled = false;
	}
	
	private void Update () 
	{
		var movement = thisTransform.TransformDirection( new Vector3( moveJoystick.AxisX, 0.0f, moveJoystick.AxisY ) );

		// We only want horizontal movement
		movement.y = 0.0f;
		movement.Normalize();
	
		var cameraTarget = Vector3.zero;
	
		// Apply movement from move joystick
		var absJoyPos = new Vector2( Mathf.Abs( moveJoystick.AxisX ), Mathf.Abs( moveJoystick.AxisY ) );	
		if ( absJoyPos.y > absJoyPos.x )
		{
			if ( moveJoystick.AxisY > 0.0f )
				movement *= forwardSpeed * absJoyPos.y;
			else
			{
				movement *= backwardSpeed * absJoyPos.y;
				cameraTarget.z = moveJoystick.AxisY * 0.75f;
			}
		}
		else
		{
			movement *= sidestepSpeed * absJoyPos.x;
			
			// Let's move the camera a bit, so the character isn't stuck under our thumb
			cameraTarget.x = -moveJoystick.AxisX * 0.5f;
		}
		
		// Check for jump
		if ( character.isGrounded )
		{
			if ( rotateJoystick.TapCount == 2 )
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
			
			// Move the camera back from the character when we jump
			cameraTarget.z = -jumpSpeed * 0.25f;
			
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
		
		// Seek camera towards target position
		var pos = cameraPivot.localPosition;
		pos.x = Mathf.SmoothDamp( pos.x, cameraTarget.x, ref cameraVelocity.x, 0.3f );
		pos.z = Mathf.SmoothDamp( pos.z, cameraTarget.z, ref cameraVelocity.z, 0.5f );
		cameraPivot.localPosition = pos;
	
		// Apply rotation from rotation joystick
		if ( character.isGrounded )
		{
			Vector2 camRotation = new Vector2();
			camRotation.x = rotateJoystick.AxisX * rotationSpeed.x;
			camRotation.y = rotateJoystick.AxisY * rotationSpeed.y;
			camRotation *= Time.deltaTime;
			
			// Rotate the character around world-y using x-axis of joystick
			thisTransform.Rotate( 0.0f, camRotation.x, 0.0f, Space.World );
			
			// Rotate only the camera with y-axis input
			cameraPivot.Rotate( camRotation.y, 0.0f, 0.0f );
		}
	}
}
