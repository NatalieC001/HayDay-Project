using UnityEngine;
using System.Collections;

[RequireComponent(typeof (CharacterController))] 
[AddComponentMenu("Third Person Player/Third Person Controller")]
public class Movement : MonoBehaviour 
{
    public float rotationDamping = 20f;
    public float Speed = 10f;
    public int gravity = 20;
    public float jumpSpeed = 8;

    float moveSpeed;
    CharacterController controller;

    Animator animator;

    void Start()
    {
        controller = (CharacterController)GetComponent(typeof(CharacterController));
        animator = GetComponent<Animator>();
    }

    public float UpdateMovement()
    {
        // These values are for keyboard use, enable them if needed
        //float z = Input.GetAxis("Horizontal");
        //float x = Input.GetAxis("Vertical");

        // Get input from the joystick
        VCAnalogJoystickBase joy = VCAnalogJoystickBase.GetInstance("stick");

        Vector3 inputVec = new Vector3(joy.AxisY, 0, -joy.AxisX);

        inputVec *= Speed;

        controller.Move((inputVec + Vector3.up * -gravity + new Vector3(0, 0, 0)) * Time.deltaTime);

        // Rotation
        if (inputVec != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                  Quaternion.LookRotation(inputVec),
                                                  Time.deltaTime * rotationDamping);

        return inputVec.magnitude;
    }

    void Update()
    {
        // Actually move the character
        moveSpeed = UpdateMovement();

        animator.SetFloat("Speed", moveSpeed, 0.1f, Time.deltaTime);
    }
}