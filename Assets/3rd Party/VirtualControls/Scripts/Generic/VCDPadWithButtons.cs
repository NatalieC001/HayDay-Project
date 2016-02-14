//----------------------------------------
// Virtual Controls Suite for Unity
// © 2012 Bit By Bit Studios, LLC
// Author: sean@bitbybitstudios.com
// Use of this software means you accept the license agreement.  
// Please don't redistribute without permission :)
//---------------------------------------------------------------

using UnityEngine;

/// <summary>
/// VCDpad with 4 directional buttons.  Also capable of using JoystickMode.
/// </summary>
public class VCDPadWithButtons : VCDPadBase
{
	#region inspector variables
	/// <summary>
	/// GameObject with a VCButton component used for the DPad left button.
	/// </summary>
	public GameObject leftVCButtonObject;
	
	/// <summary>
	/// GameObject with a VCButton component used for the DPad right button.
	/// </summary>
	public GameObject rightVCButtonObject;
	
	/// <summary>
	/// GameObject with a VCButton component used for the DPad up button.
	/// </summary>
	public GameObject upVCButtonObject;
	
	/// <summary>
	/// GameObject with a VCButton component used for the DPad down button.
	/// </summary>
	public GameObject downVCButtonObject;
	#endregion
	
	#region public properties
	/// <summary>
	/// Gets the DPad's VCButtonBase LeftButton.
	/// </summary>
	public VCButtonBase LeftButton { get; private set; }
	
	/// <summary>
	/// Gets the DPad's VCButtonBase RightButton.
	/// </summary>
	public VCButtonBase RightButton { get; private set; }
	
	/// <summary>
	/// Gets the DPad's VCButtonBase UpButton.
	/// </summary>
	public VCButtonBase UpButton { get; private set; }
	
	/// <summary>
	/// Gets the DPad's VCButtonBase DownButton.
	/// </summary>
	public VCButtonBase DownButton { get; private set; }
	#endregion
	
	protected override bool Init ()
	{
		if (!base.Init ())
			return false;
		
		if (leftVCButtonObject)
			LeftButton = leftVCButtonObject.GetComponent<VCButtonBase>();
		
		if (rightVCButtonObject)
			RightButton = rightVCButtonObject.GetComponent<VCButtonBase>();
		
		if (upVCButtonObject)
			UpButton = upVCButtonObject.GetComponent<VCButtonBase>();
		
		if (downVCButtonObject)
			DownButton = downVCButtonObject.GetComponent<VCButtonBase>();
		
		// intialize to non pressed
		SetPressedGraphics(VCDPadBase.EDirection.Left, false);
		SetPressedGraphics(VCDPadBase.EDirection.Right, false);
		SetPressedGraphics(VCDPadBase.EDirection.Up, false);
		SetPressedGraphics(VCDPadBase.EDirection.Down, false);
		
		#region error checking
		if (JoystickMode)
		{
			if (LeftButton && !LeftButton.skipCollisionDetection)
			{
				Debug.LogWarning("When DPad is in JoystickMode, Buttons should have skipCollisionDetection set to true.  Setting it automatically for LeftButton");
				LeftButton.skipCollisionDetection = true;
			}
			if (RightButton && !RightButton.skipCollisionDetection)
			{
				Debug.LogWarning("When DPad is in JoystickMode, Buttons should have skipCollisionDetection set to true.  Setting it automatically for RightButton");
				RightButton.skipCollisionDetection = true;
			}
			if (DownButton && !DownButton.skipCollisionDetection)
			{
				Debug.LogWarning("When DPad is in JoystickMode, Buttons should have skipCollisionDetection set to true.  Setting it automatically for DownButton");
				DownButton.skipCollisionDetection = true;
			}
			if (UpButton && !UpButton.skipCollisionDetection)
			{
				Debug.LogWarning("When DPad is in JoystickMode, Buttons should have skipCollisionDetection set to true.  Setting it automatically for UpButton");
				UpButton.skipCollisionDetection = true;
			}
		}
		#endregion
		
		return true;
	}
	
	protected override void SetPressedGraphics (EDirection dir, bool pressed)
	{
		// only change pressed state if we're in joystick mode (and thus collision detection is off).
		// otherwise, the buttons handle the pressed graphics state themselves (during their collision detection).
		if (!JoystickMode)
			return;
		
		if (dir == EDirection.Left && LeftButton != null)
			LeftButton.ForcePressed = pressed;
		
		if (dir == EDirection.Right && RightButton != null)
			RightButton.ForcePressed = pressed;
		
		if (dir == EDirection.Up && UpButton != null)
			UpButton.ForcePressed = pressed;
		
		if (dir == EDirection.Down && DownButton != null)
			DownButton.ForcePressed = pressed;
	}
	
	protected override void UpdateStateNonJoystickMode ()
	{
		if (XAxisEnabled)
		{
			SetPressed(EDirection.Left, ButtonExistsAndIsPressed(LeftButton));
			SetPressed(EDirection.Right, ButtonExistsAndIsPressed(RightButton));
		}
		if (YAxisEnabled)
		{
			SetPressed(EDirection.Up, ButtonExistsAndIsPressed(UpButton));
			SetPressed(EDirection.Down, ButtonExistsAndIsPressed(DownButton));
		}
	}
	
	// utility to tell if a button is non null and pressed
	protected bool ButtonExistsAndIsPressed(VCButtonBase button)
	{
		if (button == null)
			return false;
		
		return button.Pressed;
	}
}
