using UnityEngine;
using System.Collections;

[RequireComponent(typeof (CharacterController))] 
[AddComponentMenu("Third Person Player/Third Person Controller")]
public class Movement : MonoBehaviour {
	public float rotationDamping = 20f;
	public float Speed = 10f;
	public int gravity = 20;
	public float jumpSpeed = 8;
	
	bool canJump;
	float moveSpeed;
	float verticalVel;  // Used for continuing momentum while in air    
	CharacterController controller;

    Camera mainCamera;

    GameObject player;

    Animator animator;

    Vector3 difVec;
	void Start()
	{
		controller = (CharacterController)GetComponent(typeof(CharacterController));

        mainCamera = (Camera)GameObject.Find("MainCamera").GetComponent<Camera>();
        animator = GetComponent<Animator>();
        player = GameObject.Find("Player");
        difVec = new Vector3(-10.3f, 16.1f, -11.6f);
        print(difVec);
	}
	float UpdateMovement()
	{

        
		// Movement
		float x = Input.GetAxis("Horizontal");
		float z = Input.GetAxis("Vertical");
		
		Vector3 inputVec = new Vector3(x, 0, z);



        Vector3 newCamVec = (mainCamera.transform.position - player.transform.position) - difVec;

		inputVec *= Speed;
		
		controller.Move((inputVec + Vector3.up * -gravity + new Vector3(0, verticalVel, 0)) * Time.deltaTime);
		
		// Rotation
		if (inputVec != Vector3.zero)
			transform.rotation = Quaternion.Slerp(transform.rotation, 
			                                      Quaternion.LookRotation(inputVec), 
			                                      Time.deltaTime * rotationDamping);


        mainCamera.transform.position = player.transform.position + difVec;
		return inputVec.magnitude;
	}
	void Update()
	{
		// Check for jump
		if (controller.isGrounded )
		{
			canJump = true;
			if ( canJump && Input.GetKeyDown("space") )
			{
				// Apply the current movement to launch velocity
				verticalVel = jumpSpeed;
				canJump = false;
			}
		}
		else
		{           
			// Apply gravity to our velocity to diminish it over time
			verticalVel += Physics.gravity.y * Time.deltaTime;
		}
		
		// Actually move the character
		moveSpeed = UpdateMovement();

      //  if (Input.GetKey("return"))
        {
            animator.SetFloat("Speed", moveSpeed, 0.01f, .01f);
        }
        

		//Debug.Log ("Move Speed: " + moveSpeed.ToString());
		
		if ( controller.isGrounded )
			verticalVel = 0f;// Remove any persistent velocity after landing
	}
}