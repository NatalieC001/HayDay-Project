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
/// A versatile button control class that tracks Pressed and Held state.
/// Some options include allowing Presses even if the press did not originate
/// on the button, or keeping a button pressed even when the touch has gone outside
/// of the collision area.
/// 
/// VCButtonBase also has callbacks which may be registered via code to be executed
/// whenever the button is pressed, released, or every frame the button is held.
/// </summary>
public abstract class VCButtonBase : VCCollideableObject 
{
	#region static GetInstance related
	/// <summary>
	/// When this name is not null, this control may be accessed globally via the <BaseClass>.GetInstance(vcName) method.
	/// Must be unique for all objects in Scene of this base type.
	/// </summary>
	public string vcName;
	
	// static dictionary containing all the named instances of this type in the scene.
	protected static Dictionary<string, VCButtonBase> _instancesByVcName;
	
	/// <summary>
	/// When vcName is not null, adds the instance to the static dictionary for this type, so it may be retrieved
	/// via the GetInstance() method.
	/// </summary>
	protected void AddInstance()
	{
		if (string.IsNullOrEmpty(vcName))
			return;
		
		if (_instancesByVcName == null)
			_instancesByVcName = new Dictionary<string, VCButtonBase>();
		
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
	/// Gets the button with specified vcName.  The vcName must be the same as it was 
	/// when the object had Init() called on it.  Will return null if
	/// no such button exists in the scene.
	/// </summary>
	/// <returns>
	/// The instance.
	/// </returns>
	/// <param name='vcName'>
	/// vcName of the button desired.
	/// </param>
	public static VCButtonBase GetInstance(string vcName)
	{
		if (_instancesByVcName == null || !_instancesByVcName.ContainsKey(vcName))
			return null;
		
		return _instancesByVcName[vcName];
	}
	#endregion
	
	#region inspector variables
	/// <summary>
	/// When true, the button will not enter the pressed state unless the touch
	/// originated on its collider.  When false, any touch dragged into the collider
	/// area will cause a Press.
	/// </summary>
	public bool touchMustBeginOnCollider = true;
	
	/// <summary>
	/// When true, sets Pressed state to false when dragging outside of the collider.  When false,
	/// a button will stay pressed until the touch is no longer active.
	/// </summary>
	public bool releaseOnMoveOut = true;
	
	/// <summary>
	/// When true, any touch on the screen activates button.  This skips collision testing.
	/// </summary>
	public bool anyTouchActivatesControl = false;
	
	/// <summary>
	/// When true, activations of the button toggle the press state.
	/// </summary>
	public bool toggle = false;
	
	/// <summary>
	/// Skips the collision detection phase of Update.  Should be used when a button is being
	/// controlled by a VCDPadBase.
	/// </summary>
	public bool skipCollisionDetection = false;
	
	/// <summary>
	/// In Editor, when debugKeyEnabled is set true, pressing the debugTouchKey on your keyboard
	/// will set this button's Pressed state to true.
	/// </summary>
	public bool debugKeyEnabled = false;
	public KeyCode debugTouchKey = KeyCode.A;
	public bool debugTouchKeyTogglesPress = false;
	#endregion
	
	#region member variables
	protected bool _visible;
	protected bool _pressed;
	protected bool _forcePressed;
	protected bool _lastPressedState;
	
	// active touch for this control
	protected VCTouchWrapper _touch;
	#endregion
	
	#region callback related
	/// <summary>
	/// A delegate signature that takes a VCButtonBase as its sole parameter.
	/// Used for various VCButton event callbacks.
	/// </summary>
	public delegate void VCButtonDelegate(VCButtonBase button);
	
	/// <summary>
	/// Function called every frame the VCButtonBase is held.
	/// The button is passed as a reference so you may read things like
	/// VCButtonBase::HoldTime.
	/// </summary>
	public VCButtonDelegate OnHold;
	
	/// <summary>
	/// Function called once each time the VCButtonBase enters Press state.
	/// </summary>
	public VCButtonDelegate OnPress;
	
	/// <summary>
	/// Function called once each time the BCButtonBase exits Press state.
	/// </summary>
	public VCButtonDelegate OnRelease;
	#endregion
	
	#region abstract methods
	protected abstract void ShowPressedState(bool pressed);
	protected abstract bool Colliding(VCTouchWrapper tw);
	#endregion
	
	/// <summary>
	/// Sets Pressed state to false.  This is a no-op if the Pressed state was already false.
	/// </summary>
	public void ForceRelease()
	{
		Pressed = false;
	}
	
	protected void Start () 
	{
		Init();
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
		
		_lastPressedState = true; // start true so Pressed = false; will execute
		Pressed = false;
		HoldTime = 0.0f;
		
		AddInstance();
		
		return true;
	}
	
		protected void Update () 
	{
		PressBeganThisFrame = false;
		PressEndedThisFrame = false;
		
#if UNITY_EDITOR
		UpdateDebugKeys();
#endif
		
		if (!skipCollisionDetection)
		{
			// handle a current touch
			if (Pressed)
			{
				// end of a current touch
				if (PressEnded)
				{
					if (!toggle)
						Pressed = false;
				}
			}
			
			if (!Pressed || toggle)
			{
				// look for a new touch
				VCTouchWrapper tw;
				for (int i = 0; i < VCTouchController.Instance.touches.Count; i++)
				{
					tw = VCTouchController.Instance.touches[i];
					if (!tw.Active || (touchMustBeginOnCollider && tw.phase != TouchPhase.Began))
						continue;
					
					if (anyTouchActivatesControl || Colliding(tw))
					{
						_touch = tw;
						if (toggle)
							Pressed = !Pressed;
						else
							Pressed = true;
					}
				}
			}
		}
		
		// don't add hold time on the first frame in which a press began
		if (Pressed && !PressBeganThisFrame)
		{
			HoldTime += Time.deltaTime;
			if (OnHold != null)
				OnHold(this);
		}
		
		UpdateVisibility();
	}

#if UNITY_EDITOR
	protected void UpdateDebugKeys()
	{
		if (!debugKeyEnabled)
			return;
		
		if (debugTouchKeyTogglesPress)
		{
			if (Input.GetKeyDown(debugTouchKey))
			{
				ForcePressed = !Pressed;
			}
		}
		else
		{
			ForcePressed = Input.GetKey(debugTouchKey);
		}
	}
#endif
	
	protected virtual void UpdateVisibility ()
	{
		if (Pressed == _lastPressedState)
			return;
		
		_lastPressedState = Pressed;
		
		if (Pressed)
		{
			// show pressed state
			ShowPressedState(true);
		}
		else
		{
			// show up state
			ShowPressedState(false);
		}
	}
	
	#region public properties
	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="VCButtonBase"/> is pressed.
	/// </summary>
	/// <value>
	/// <c>true</c> if pressed; otherwise, <c>false</c>.
	/// </value>
	public bool Pressed 
	{ 
		get { return _pressed; }
		private set 
		{
			if (_pressed == value)
				return;
			
			_pressed = value;
			
			if (_pressed)
			{
				if (OnPress != null)
					OnPress(this);
				
				PressBeganThisFrame = true;
			}
			else
			{
				if (OnRelease != null)
					OnRelease(this);
				
				PressEndedThisFrame = true;
				
				HoldTime = 0.0f;
				_touch = null;
			}
		}
	}
	
	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="VCButtonBase"/> force pressed.
	/// When a button is ForcePressed, it always stays in Pressed state.  When ForcePressed is set false,
	/// press state is evaluated and reset as normal.
	/// </summary>
	/// <value>
	/// <c>true</c> if force pressed; otherwise, <c>false</c>.
	/// </value>
	public bool ForcePressed 
	{
		get { return _forcePressed; }
		set
		{
			_forcePressed = value;
			if (_forcePressed)
				Pressed = true;
			else
				Pressed = !PressEnded;
		}
	}
	
	/// <summary>
	/// Gets the game time (in seconds) that the button has been Held for.
	/// Counting begins on the frame immediately after an initial Press.
	/// If a button is not in press state, HoldTime returns 0.
	/// </summary>
	public float HoldTime { get; private set; }
	
	/// <summary>
	/// The TouchWrapper currently interacting with this control.  May be null.
	/// </summary>
	public VCTouchWrapper TouchWrapper
	{
		get { return _touch; }
	}
	
	/// <summary>
	/// Returns true if the button left non-pressed state and entered pressed state this frame.
	/// </summary>
	public bool PressBeganThisFrame { get; private set; }
	
	/// <summary>
	/// Returns true if the button left pressed state and entered non-pressed state this frame.
	/// </summary>
	public bool PressEndedThisFrame { get; private set; }
	#endregion
	
	#region protected properties
	// evaluate whether or not a press ended
	protected bool PressEnded
	{
		get 
		{
			// special case
			if (ForcePressed)
				return false;
			
			if (_touch == null || !_touch.Active)
				return true;
			
			if (!anyTouchActivatesControl && releaseOnMoveOut && !Colliding(_touch))
				return true;
			
			return false;
		}
	}
	#endregion
}
