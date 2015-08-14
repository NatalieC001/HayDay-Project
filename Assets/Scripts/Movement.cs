using UnityEngine;
using System.Collections;

[RequireComponent(typeof (CharacterController))] 
[AddComponentMenu("Third Person Player/Third Person Controller")]
public class Movement : MonoBehaviour 
{
    public float rotationDamping = 20f;
    public float Speed = 0.1f;
    public int gravity = 20;
    public float jumpSpeed = 8;

    float moveSpeed;
    CharacterController controller;

    Animator animator;

    public float speed = 0.5F;
    public float startTime;
    public float journeyLength;

    public GameObject currentFocus; 

    public static bool freeRoam;

	public AudioClip moveSound;
	AudioSource audio;

    void Start()
    {
        freeRoam = true;
        controller = (CharacterController)GetComponent(typeof(CharacterController));
        animator = GetComponent<Animator>();
		audio = GetComponent<AudioSource>();
    }

    public float UpdateMovement()
    {
        VCAnalogJoystickBase joy = VCAnalogJoystickBase.GetInstance("stick");
        Vector3 inputVec = new Vector3(joy.AxisY, 0, -joy.AxisX);
        inputVec *= Speed;

		if((inputVec.z != 0 || inputVec.x != 0))
		{
			if(!audio.isPlaying)
			{
				audio.clip = moveSound;
				audio.Play();
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