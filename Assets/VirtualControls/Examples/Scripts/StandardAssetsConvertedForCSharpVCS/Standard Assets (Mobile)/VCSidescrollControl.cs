using UnityEngine;
using System.Collections;

/// <summary>
/// This is a C# and VCS friendly conversion of SidescrollControl.js found in
/// Unity's Standard Assets (Mobile) package.  For any questions, contact
/// support@bitbybitstudios.com.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class VCSidescrollControl : MonoBehaviour 
{
	public VCAnalogJoystickBase moveTouchPad;
	public VCAnalogJoystickBase jumpTouchPad;
	
	public Transform cameraPivot;									// The transform used for camera rotation
	
	public float forwardSpeed = 4.0f;
	public float backwardSpeed = 4.0f;
	public float jumpSpeed = 16.0f;
	public float inAirMultiplier = 0.25f;							// Limiter for ground speed while jumping
	
	private Transform thisTransform;
	private CharacterController character;
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
		jumpTouchPad.enabled = false;	
	
		// Don't allow any more control changes when the game ends
		this.enabled = false;
	}
	
	private void Update () 
	{
		var movement = Vector3.zero;

		// Apply movement from move joystick
		if ( moveTouchPad.AxisX > 0.0f )
			movement = Vector3.right * forwardSpeed * moveTouchPad.AxisX;
		else
			movement = Vector3.right * backwardSpeed * moveTouchPad.AxisX;
		
		// Check for jump
		if ( character.isGrounded )
		{		
			var jump = false;
			var touchPad = jumpTouchPad;
				
			if ( !touchPad.Dragging )
				canJump = true;
			
		 	if ( canJump && touchPad.Dragging )
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
	//		movement.z *= inAirMultiplier;
		}
			
		movement += velocity;	
		movement += Physics.gravity;
		movement *= Time.deltaTime;
		
		// Actually move the character	
		character.Move( movement );
		
		if ( character.isGrounded )
			// Remove any persistent velocity after landing	
			velocity = Vector3.zero;
	}
}
