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

    private float moveSpeed;
	private CharacterController controller;
	private Animator animator;
	private AudioSource audioSource;
	private bool inMart;

	private VCAnalogJoystickBase joy;
    private CameraController cameraController;
    void Start()
    {
        freeRoam = true;
        controller = (CharacterController)GetComponent(typeof(CharacterController));
        animator = GetComponent<Animator>();
		audioSource = GetComponent<AudioSource>();
		inMart = Application.loadedLevelName.Equals("Mart");
        cameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
    }

    public float UpdateMovement()
    {
		Vector3 inputVec = new Vector3();

        joy = VCAnalogJoystickBase.GetInstance("stick");


        //print(cameraController.transform.forward.x);

        //int joyVec = 1;
        //if(cameraController.transform.forward.x< 0 ) joyVec =-1;
        //inputVec = new Vector3(joy.AxisY * joyVec, 0, joy.AxisX * -joyVec);
        if (!inMart)
            inputVec = new Vector3(joy.AxisY, 0, -joy.AxisX);

        if (inMart)
            inputVec = new Vector3(-joy.AxisX, 0, -joy.AxisY);

        inputVec *= Speed;

		if(!inMart)
		{
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