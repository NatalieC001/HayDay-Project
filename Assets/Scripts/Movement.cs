using UnityEngine;
using System.Collections;

[RequireComponent(typeof (CharacterController))] 
[AddComponentMenu("Third Person Player/Third Person Controller")]
public class Movement : MonoBehaviour 
{
    public float rotationDamping = 20f;
    public float Speed = 0.1f;
    public int gravity = 20;
	public GameObject currentFocus;
	public AudioClip moveSound;
	public static bool freeRoam;

    float moveSpeed;
    CharacterController controller;
    Animator animator;
	AudioSource audioSource;
	string currScene;

    void Start()
    {
        freeRoam = true;
        controller = (CharacterController)GetComponent(typeof(CharacterController));
        animator = GetComponent<Animator>();
		audioSource = GetComponent<AudioSource>();
		currScene = Application.loadedLevelName;
    }

    public float UpdateMovement()
    {
		Vector3 inputVec = new Vector3();

        VCAnalogJoystickBase joy = VCAnalogJoystickBase.GetInstance("stick");

		if(currScene.Equals("Farm"))
        	inputVec = new Vector3(joy.AxisY, 0, -joy.AxisX);

		if(currScene.Equals("Mart"))
			inputVec = new Vector3(joy.AxisX, 0, joy.AxisY);

        inputVec *= Speed;

		if((inputVec.z != 0 || inputVec.x != 0))
		{
			if(!audioSource.isPlaying)
			{
				audioSource.clip = moveSound;
				audioSource.Play();
			}
		}
		else
		{
			GetComponent<AudioSource>().Stop();
		}

        controller.Move((inputVec + Vector3.up * -gravity + new Vector3(0, 0, 0)) * Time.deltaTime);

        if (freeRoam)
        {
	        // Rotation
	        if (inputVec != Vector3.zero)
	            transform.rotation = Quaternion.Slerp(transform.rotation,
	                                                  Quaternion.LookRotation(inputVec),
	                                                  Time.deltaTime * rotationDamping);
        }
        else if (currentFocus != null)
        {
            Quaternion lookRotation;
            Vector3 direction;

            direction = (currentFocus.transform.position - transform.position).normalized;
            lookRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationDamping/3);   
        }
        else
        {
            freeRoam = true;
        }
		
        return inputVec.magnitude;
    }

    public void lookAt(GameObject target)
    {
        currentFocus = target;     
        freeRoam = false;  
    }

    void Update()
    {
        moveSpeed = UpdateMovement();
        animator.SetFloat("Speed", moveSpeed, 0.1f, Time.deltaTime);
    }
}