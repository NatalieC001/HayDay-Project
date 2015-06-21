//----------------------------------------
// Virtual Controls Suite for Unity
// © 2012 Bit By Bit Studios, LLC
// Author: sean@bitbybitstudios.com
// Use of this software means you accept the license agreement.  
// Please don't redistribute without permission :)
//---------------------------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A DPad controller that may be controlled via a joystick or simply with directional buttons.
/// May be configured to use one or two axes at a time.
/// </summary>
public abstract class VCDPadBase : MonoBehaviour 
{
	#region static GetInstance related
	/// <summary>
	/// When this name is not null, this control may be accessed globally via the <BaseClass>.GetInstance(vcName) method.
	/// Must be unique for all objects in Scene of this base type.
	/// </summary>
	public string vcName;
	
	// static dictionary containing all the named instances of this type in the scene.
	private static Dictionary<string, VCDPadBase> _instancesByVcName;
	
	/// <summary>
	/// When vcName is not null, adds the instance to the static dictionary for this type, so it may be retrieved
	/// via the GetInstance() method.
	/// </summary>
	protected void AddInstance()
	{
		if (string.IsNullOrEmpty(vcName))
			return;
		
		if (_instancesByVcName == null)
			_instancesByVcName = new Dictionary<string, VCDPadBase>();
		
		while (_instancesByVcName.ContainsKey(vcName))
		{
			// handle old references that got deleted by something like a level load
			if (_instancesByVcName[vcName] == null)
			{
				_instancesByVcName.Remove(vcName);
				continue;
			}
			
			vcName += "_copy";
			Debug.LogWarning("Attempting to add instance with duplicate VCName!\nVCNames must be unique -- renaming this instance to "
				+ vcName);
		}
		
		_instancesByVcName.Add(vcName, this);
	}
	
	/// <summary>
	/// Gets the DPad with specified vcName.  The vcName must be the same as it was 
	/// when the object had Init() called on it.  Will return null if
	/// no such DPad exists in the scene.
	/// </summary>
	/// <returns>
	/// The instance.
	/// </returns>
	/// <param name='vcName'>
	/// vcName of the DPad desired.
	/// </param>
	public static VCDPadBase GetInstance(string vcName)
	{
		if (_instancesByVcName == null || !_instancesByVcName.ContainsKey(vcName))
			return null;
		
		return _instancesByVcName[vcName];
	}
	#endregion

	#region inspector variables
	/// <summary>
	/// When set to a VCAnalogJoystickBase, this DPad will function in JoystickMode.
	/// When in JoystickMode, directional state of the DPad is calculated by reading the
	/// VCAnalogJoystickBase::AxisX and VCAnalogJoystickBase::AxisY properties from the joystick.
	/// This is useful for when you want a DPad control that allows for fluid movement as well as
	/// for possibly adding 4 directions to your DPad (Up & Right, Up & Left, Down & Right, and Down & Left).
	/// </summary>
	public VCAnalogJoystickBase joystick;
	
	/// <summary>
	/// When false, no movement in the X axis is measured.
	/// </summary>
	public bool XAxisEnabled = true;
	
	/// <summary>
	/// When false, no movement in the Y axis is measured.
	/// </summary>
	public bool YAxisEnabled = true;
	
	/// <summary>
	/// In Editor only, you may use the below debug keyboard keys to control the DPad.
	/// Setting debugKeysEnabled to true disables regular DPad control via the mouse.
	/// </summary>
	public bool debugKeysEnabled = false;
	public bool debugTouchKeysTogglesPress = false;
	public KeyCode debugLeftKey = KeyCode.Keypad4;
	public KeyCode debugRightKey = KeyCode.Keypad6;
	public KeyCode debugUpKey = KeyCode.Keypad8;
	public KeyCode debugDownKey = KeyCode.Keypad5;
	#endregion
	
	// bitfield describing the pressed directions
	protected int _pressedField;
	
	/// <summary>
	/// Enum describing possible DPad directions.
	/// </summary>
	public enum EDirection
	{
		None = 0,
		Up = 1 << 0,
		Down = 1 << 1,
		Left = 1 << 2,
		Right = 1 << 3
	};
	
	#region abstract methods
	protected abstract void SetPressedGraphics(EDirection dir, bool pressed);
	#endregion
	
	/// <summary>
	/// Returns true if the specified EDirection is currently pressed, otherwise, false.  In the case of 
	/// Direction.None, will return true only if Left, Right, Down, and Up are all NOT pressed.
	/// </summary>
	public bool Pressed(EDirection dir)
	{
		// if query is for None direction, return true if no directions are pressed
		if (dir == EDirection.None)
			return _pressedField == 0;
		
		// otherwise do a bitwise comparison
		return (_pressedField & (int)dir) != 0;
	}
	
	protected void Start () 
	{
		Init ();
	}
	
	protected virtual bool Init()
	{
		// disable OnGUI calls for performance
		this.useGUILayout = false;
		
		// make sure we have a VCTouchController
		if (VCTouchController.Instance == null)
		{
			Debug.LogWarning("Cannot find VCTouchController!\nVirtualControls requires a gameObject which has VCTouchController component attached in scene. Adding one for you...");
			gameObject.AddComponent<VCTouchController>();
		}
		
		if (JoystickMode)
		{
			if (!joystick.measureDeltaRelativeToCenter)
			{
				Debug.LogWarning("DPad in joystickMode may not function correctly when joystick's measureDeltaRelativeToCenter is not true.");
			}
		}
		
		AddInstance();
		
		return true;
	}
	
	virtual protected void Update () 
	{
#if UNITY_EDITOR
		if (UpdateDebugKeys())
			return;
#endif
		
		if (JoystickMode)
		{
			UpdateStateJoystickMode();
		}
		else
		{
			// do collision tests
			UpdateStateNonJoystickMode();
		}
	}
	
#if UNITY_EDITOR
	protected bool UpdateDebugKeys()
	{
		if (!debugKeysEnabled)
			return false;
		
		if (debugTouchKeysTogglesPress)
		{
			if (Input.GetKeyDown(debugLeftKey))
				SetPressed(EDirection.Left, !Left);
			if (Input.GetKeyDown(debugRightKey))
				SetPressed(EDirection.Right, !Right);
			if (Input.GetKeyDown(debugUpKey))
				SetPressed(EDirection.Up, !Up);
			if (Input.GetKeyDown(debugDownKey))
				SetPressed(EDirection.Down, !Down);
		}
		else 
		{
			SetPressed(EDirection.Left, Input.GetKey(debugLeftKey));
			SetPressed(EDirection.Right, Input.GetKey(debugRightKey));
			SetPressed(EDirection.Up, Input.GetKey(debugUpKey));
			SetPressed(EDirection.Down, Input.GetKey(debugDownKey));
		}
		
		return true;
	}
#endif
	
	protected virtual void UpdateStateJoystickMode()
	{
		SetPressed(EDirection.Right, joystick.AxisX > 0.0f && XAxisEnabled);
		SetPressed(EDirection.Left, joystick.AxisX < 0.0f && XAxisEnabled);
		SetPressed(EDirection.Up, joystick.AxisY > 0.0f && YAxisEnabled);
		SetPressed(EDirection.Down, joystick.AxisY < 0.0f && YAxisEnabled);
	}
	
	protected virtual void UpdateStateNonJoystickMode()
	{
		// base joystick only handles joystick mode
	}
	
	// sets the pressed state for a direction, which includes both 
	// the data state and the graphical state.
	protected virtual void SetPressed(EDirection dir, bool pressed)
	{
		// do nothing if setting to the same value
		if (Pressed(dir) == pressed)
			return;
		
		SetBitfield(dir, pressed);
		
		SetPressedGraphics(dir, pressed);
	}
	
	// sets the bitfield's pressed state for the specified direction
	protected void SetBitfield(EDirection dir, bool pressed)
	{
		// clear the bit
		_pressedField &= ~((int)dir);
		
		if (pressed)
		{
			// set it
			_pressedField |= (int)dir;
		}
	}
		
	// utility to get the opposite direction of a specified direction
	protected EDirection GetOpposite(EDirection dir)
	{
		switch (dir)
		{
			case EDirection.Up: return EDirection.Down;
			case EDirection.Down: return EDirection.Up;
			case EDirection.Left: return EDirection.Right;
			case EDirection.Right: return EDirection.Left;
		}
		
		return EDirection.None;
	}
			
	#region public properties
	/// <summary>
	/// Returns true if Up is pressed, false if not.
	/// </summary>
	public bool Up
	{
		get	{ return Pressed(EDirection.Up); }
	}
	
	/// <summary>
	/// Returns true if Down is pressed, false if not.
	/// </summary>
	public bool Down
	{
		get	{ return Pressed(EDirection.Down); }
	}
	
	/// <summary>
	/// Returns true if Left is pressed, false if not.
	/// </summary>
	public bool Left
	{
		get	{ return Pressed(EDirection.Left); }
	}
	
	/// <summary>
	/// Returns true if Right is pressed, false if not.
	/// </summary>
	public bool Right
	{
		get	{ return Pressed(EDirection.Right); }
	}
	#endregion
	
	#region protected properties
	protected bool JoystickMode
	{
		get { return joystick != null; }
	}
	#endregion
}
