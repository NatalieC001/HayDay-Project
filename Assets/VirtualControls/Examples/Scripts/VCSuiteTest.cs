using UnityEngine;
using System.Collections;

/// <summary>
/// This simple script looks for a joystick, dpad, and a button
/// and uses them to move a cube and have it emit particles.
/// </summary>
public class VCSuiteTest : MonoBehaviour {
	
	// our cube that will spin around
	public GameObject cube;
	
	// our container that we will translate
	public GameObject cubeContainer;
	
	// the max speed which the cube moves at
	public float moveSpeed = 5.0f;
	
	// Use this for initialization
	void Start () {
		
	}
	
	void Update ()
	{
		// Use the DPad to move the cube container
		if (cubeContainer)
		{
			// try and get the dpad
			VCDPadBase dpad = VCDPadBase.GetInstance("dpad");
			
			// if we got one, perform movement
			if (dpad)
			{
				if (dpad.Left)
					cubeContainer.transform.Translate(-moveSpeed * Time.deltaTime, 0.0f, 0.0f);
				if (dpad.Right)
					cubeContainer.transform.Translate(moveSpeed * Time.deltaTime, 0.0f, 0.0f);
				if (dpad.Up)
					cubeContainer.transform.Translate(0.0f, moveSpeed * Time.deltaTime, 0.0f);
				if (dpad.Down)
					cubeContainer.transform.Translate(0.0f, -moveSpeed * Time.deltaTime, 0.0f);
			}
			
			// do the same for the analog joystick
			VCAnalogJoystickBase joy = VCAnalogJoystickBase.GetInstance("stick");
			if (joy != null)
			{
				cubeContainer.transform.Translate(moveSpeed * Time.deltaTime * joy.AxisX, moveSpeed * Time.deltaTime * joy.AxisY, 0.0f);
			}
		}
		
		// rotate the cube for coolness effect
		if (cube)
			cube.transform.RotateAroundLocal(new Vector3(1.0f, 1.0f, 0.0f), Time.deltaTime);
		
		// and emit particles based on the time we've held down our A button (if we have one)
		VCButtonBase abtn = VCButtonBase.GetInstance("A");
		if (abtn != null && cubeContainer)
		{
			ParticleSystem particles = cubeContainer.GetComponentInChildren<ParticleSystem>();
			if (particles != null)
				particles.emissionRate = abtn.HoldTime * 50.0f;
			
			// emit some particles whenever joystick is double clicked
			VCAnalogJoystickBase joy = VCAnalogJoystickBase.GetInstance("stick");
			if (joy != null && particles != null && joy.TapCount > 1)
			{
				particles.emissionRate = 150.0f;
			}
		}
		
		// example of how to detect if press began or ended exactly on this frame
		if (abtn != null)
		{
			if (abtn.PressBeganThisFrame)
				Debug.Log("Press began on frame " + Time.frameCount);
			
			if (abtn.PressEndedThisFrame)
				Debug.Log("Press ended on frame " + Time.frameCount);
		}
	}
	
	void OnGUI () 
	{
		// if there's an analog joystick, output some info
		if (VCAnalogJoystickBase.GetInstance("stick") != null)
		{
			GUI.Label(new Rect(10, 10, 300, 20), "Joystick Axes: " + VCAnalogJoystickBase.GetInstance("stick").AxisX + " " + VCAnalogJoystickBase.GetInstance("stick").AxisY);
		}
		
		// if there's an a button, output some info
		if (VCButtonBase.GetInstance("A") != null)
		{
			GUI.Label(new Rect(10, 30, 300, 20), "Button Hold (s): " + VCButtonBase.GetInstance("A").HoldTime.ToString());
		}
		
		// if there's a dpad, output some info
		VCDPadBase dpad = VCDPadBase.GetInstance("dpad");
		if (dpad != null)
		{
			string str = "DPad: ";
			if (dpad.Left)
				str += "Left ";
			if (dpad.Right)
				str += "Right ";
			if (dpad.Up)
				str += "Up ";
			if (dpad.Down)
				str += "Down ";
			if (dpad.Pressed(VCDPadBase.EDirection.None))
				str += "(No Direction)";
			
			GUI.Label(new Rect(10, 50, 300, 20), str);
		}
		
		GUI.Label(new Rect(10, 70, 300, 20), "Move cube using controls");
		GUI.Label(new Rect(10, 90, 300, 20), "Double tap joystick / Press A for particles");
	}
}
