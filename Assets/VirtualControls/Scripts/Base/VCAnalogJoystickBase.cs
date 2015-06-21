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
/// The Base class for the Virtual Control AnalogJoystick.
/// May be dragged with touch or mouse input and have its X and Y axis values read from.
/// Options include the ability to show only when being dragged, snap to touch location
/// on start of drag, enable drag only while colliding, and more.
/// </summary>
public abstract class VCAnalogJoystickBase : VCCollideableObject 
{
	#region static GetInstance related
	/// <summary>
	/// When this name is not null, this control may be accessed globally via the <BaseClass>.GetInstance(vcName) method.
	/// Must be unique for all objects in Scene of this base type.
	/// </summary>
	public string vcName;
	
	// static dictionary containing all the named instances of this type in the scene.
	protected static Dictionary<string, VCAnalogJoystickBase> _instancesByVcName;
	
	public static List<VCTouchWrapper> touchesInUse = new List<VCTouchWrapper>();
	
	/// <summary>
	/// When vcName is not null, adds the instance to the static dictionary for this type, so it may be retrieved
	/// via the GetInstance() method.
	/// </summary>
	protected void AddInstance()
	{
		if (string.IsNullOrEmpty(vcName))
			return;
		
		if (_instancesByVcName == null)
			_instancesByVcName = new Dictionary<string, VCAnalogJoystickBase>();
		
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
	/// Gets the joystick with specified vcName.  The vcName must be the same as it was 
	/// when the object had Init() called on it.  Will return null if
	/// no such joystick exists in the scene.
	/// </summary>
	/// <returns>
	/// The instance.
	/// </returns>
	/// <param name='vcName'>
	/// vcName of the joystick desired.
	/// </param>
	public static VCAnalogJoystickBase GetInstance(string vcName)
	{
		if (_instancesByVcName == null || !_instancesByVcName.ContainsKey(vcName))
			return null;
		
		return _instancesByVcName[vcName];
	}
	#endregion
	
	#region inspector variables
	/// <summary>
	/// GameObject that represents the moving part of the joystick.
	/// </summary>
	public GameObject movingPart;
	
	/// <summary>
	/// Optional GameObject (including children) that represent the base area of the joystick.
	/// </summary>
	public GameObject basePart;
	
	/// <summary>
	/// GameObject that will be used to determine whether or not the joystick is being touched.
	/// </summary>
	public GameObject colliderObject;
	
	/// <summary>
	/// When false, the joystick will not be visible unless being dragged.
	/// </summary>
	public bool visibleWhenNotActive = true;
	
	/// <summary>
	/// When true, the basePart and movingPart will move to the touch location before dragging.
	/// </summary>
	public bool positionAtTouchLocation = false;
	
	/// <summary>
	/// The upper left coordinate (in percentage) where the joystick may be moved to
	/// when positionAtTouchLocation is true.  If a touch occurs outside of this area,
	/// it will not affect the joystick.
	/// </summary>
	public Vector2 positionAtTouchLocationAreaMin = new Vector2(0.0f, 0.0f);
	
	/// <summary>
	/// The bottom right coordinate (in percentage) where the joystick may be moved to
	/// when positionAtTouchLocation is true.  If a touch occurs outside of this area,
	/// it will not affect the joystick.
	/// </summary>
	public Vector2 positionAtTouchLocationAreaMax = new Vector2(1.0f, 1.0f);
	
	/// <summary>
	/// When true, if a touch is no longer colliding with colliderObject, joystick dragging will stop.
	/// </summary>
	public bool stopDraggingOnMoveOut = false;
	
	/// <summary>
	/// When true, any touch on the screen activates joystick.  This skips collision testing.
	/// </summary>
	public bool anyTouchActivatesControl = false;
	
	/// <summary>
	/// When true, drag delta is measured from the movingPart origin instead of the touch origin.
	/// This is useful when using a joystick for DPad's joystick mode.
	/// </summary>
	public bool measureDeltaRelativeToCenter = false;
	
	/// <summary>
	/// When true, the moving part GameObject will not be visible.
	/// </summary>
	public bool hideMovingPart = false;
	
	/// <summary>
	/// When true, update for this joystick will happen during Unity's LateUpdate phase.
	/// </summary>
	public bool useLateUpdate = false;
	
	/// <summary>
	/// When true, this control will not process touches already being used by other VCAnalogJoystickBase instances.
	/// </summary>
	public bool requireExclusiveTouch = false;
	
	/// <summary>
	/// The maximum length of the distance the movingPart may be dragged away from origin, in pixels.
	/// </summary>
	public float dragDeltaMagnitudeMaxPixels = 50.0f;
	
	/// <summary>
	/// The period of time in seconds after which TapCount is reset to 0.
	/// </summary>
	public float tapCountResetTime = 0.2f;
	
	/// <summary>
	/// Multiplies actual drag distance.  
	/// For instance, a value of .5 will move the joystick only half the distance the user has moved.
	/// </summary>
	public Vector2 dragScaleFactor = new Vector2(1.0f, 1.0f);
	
	/// <summary>
	/// Raw values below this number will be ignored.  Use this to create a lower limit "dead zone".
	/// </summary>
	public Vector2 rangeMin = new Vector2(0.0f, 0.0f);
	
	/// <summary>
	/// Raw values above this number will be ignored.  Use this to create an upper limit "dead zone".
	/// </summary>
	public Vector2 rangeMax = new Vector2(1.0f, 1.0f);
	
	/// <summary>
	/// In Editor, when debugKeysEnabled is set true, you may press the debugTouchKey to simulate a
	/// touch occuring in the center of the joystick.  From there, you may use the debug directional
	/// keys to move the simulated touch in various directions.  Releasing the debugTouchKey will end
	/// the simulated touch, unless debugTouchKeyTogglesTouch is set true, in which case the touch will
	/// not end until the debugTouchKey is pressed again.
	/// </summary>
	public bool debugKeysEnabled = false;
	public float debugTouchMovementSpeedPixels = 100.0f;
	public KeyCode debugTouchKey = KeyCode.LeftControl;
	public bool debugTouchKeyTogglesTouch = false;
	public KeyCode debugLeftKey = KeyCode.LeftArrow;
	public KeyCode debugRightKey = KeyCode.RightArrow;
	public KeyCode debugUpKey = KeyCode.UpArrow;
	public KeyCode debugDownKey = KeyCode.DownArrow;
	#endregion
	
	// the square of the inspector set dragDeltaMagnitudeMaxPixels, cached for performance
	protected float _dragDeltaMagnitudeMaxSq;
	
	// positions used to track the magnitude of joystick drag distance
	// origin is expressed, depending on the UI implementation, 
	// in viewport or world coordinates, except where "Screen" is denoted.
	protected Vector3 _movingPartOrigin;
	protected Vector3 _touchOrigin;
	protected Vector2 _touchOriginScreen;
	
	// tracks if user was dragging joystick last frame
	protected bool _wasDragging = false;
	
	// current drag delta in pixels
	protected Vector2 _deltaPixels;
	
	// amount to move the movingPart GO by, based on _deltaPixels.
	// in most cases, this is simply deltaPixels, but for UI solutions like GUITexture,
	// the offset must be expressed in Viewport instead of Screen coordinates
	protected Vector2 _movingPartOffset;
	
	#region visibility handling
	protected bool _visible = true;
	protected bool _movingPartVisible = true;
	protected List<Behaviour> _visibleBehaviourComponents;
	#endregion
	
	// the active touch for this control
	protected VCTouchWrapper _touch;
	
	public delegate void VCJoystickDelegate(VCAnalogJoystickBase joystick);
	public VCJoystickDelegate OnDoubleTap;
	private float _tapTime;
	
	#region abstract methods
	// sets up origin values based on UI location
	protected abstract void InitOriginValues();
	
	// returns true if touch is colliding with this joystick
	protected abstract bool Colliding(VCTouchWrapper tw);
	
	// handles an active touch
	protected abstract void ProcessTouch(VCTouchWrapper tw);
	
	// sets visibility for joystick and base gameObjects
	protected abstract void SetVisible(bool visible, bool forceUpdate);
	#endregion
	
	protected void Start ()
	{
		Init();
	}
	
	// initializes the object
	protected virtual bool Init()
	{
		// don't execute OnGUI behavior, for performance
		this.useGUILayout = false;
		
		#region error checking
		// make sure we have a VCTouchController
		if (VCTouchController.Instance == null)
		{
			Debug.LogWarning("Cannot find VCTouchController!\nVirtualControls requires a gameObject which has VCTouchController component attached in scene. Adding one for you...");
			//gameObject.AddComponent<VCTouchController>();
			VCUtils.AddTouchController(gameObject);
		}
		
		// make sure the movingPart is specified
		if (!movingPart)
		{
			VCUtils.DestroyWithError(gameObject, "movingPart is null, VCAnalogJoystick requires it to be assigned to a gameObject! Destroying this control.");
			return false;
		}
		
		// make sure rangeX is positive
		if (this.RangeX <= 0.0f)
		{
			VCUtils.DestroyWithError(gameObject, "rangeMin must be less than rangeMax!  Destroying this control.");
			return false;
		}
		
		// make sure rangeY is positive
		if (this.RangeY <= 0.0f)
		{
			VCUtils.DestroyWithError(gameObject, "rangeMin must be less than rangeMax!  Destroying this control.");
			return false;
		}
		#endregion
		
		if (basePart == null)
			basePart = this.gameObject;
		
		if (colliderObject == null)
			colliderObject = movingPart;
		
		_deltaPixels = new Vector2();
		_dragDeltaMagnitudeMaxSq = dragDeltaMagnitudeMaxPixels * dragDeltaMagnitudeMaxPixels;
		
		InitCollider(colliderObject);
		InitOriginValues();
		TapCount = 0;
		
		AddInstance();
		
		return true;
	}
	
	// sets the position of a gameObject to the specified world location
	protected virtual void SetPosition(GameObject go, Vector3 vec)
	{
		go.transform.position = vec;
	}
	
	protected virtual void LateUpdate ()
	{
		if (useLateUpdate)
			PerformUpdate();
	}
	
	// updates drag state
	protected virtual void Update () 
	{
		if (!useLateUpdate)
			PerformUpdate();
	}
	
	protected void PerformUpdate()
	{
#if UNITY_EDITOR
		UpdateDebugTouch();
#endif
		
		// reset tapcount if necessary
		if (Time.time - _tapTime >= tapCountResetTime)
			TapCount = 0;
		
		// if we are aleady dragging, remeasure delta and update visual position
		if (this.Dragging)
		{
			UpdateDelta();
			
			Vector3 vec = new Vector3(_movingPartOrigin.x + _movingPartOffset.x, _movingPartOrigin.y + _movingPartOffset.y, movingPart.transform.position.z);
			SetPosition(movingPart, vec);
			
			// if we don't want to continue dragging when not colliding, collision test
			if (stopDraggingOnMoveOut)
			{
				if (!Colliding(_touch))
				{
					StopDragging();
					return;
				}
			}
		}
		else
		// we aren't dragging, let's see if we can see if we can
		{
			// clean up if we just finished dragging
			if (_wasDragging)
			{
				StopDragging();
			}
			
			// see if there are any touches
			VCTouchWrapper tw;
			for (int i = 0; i < VCTouchController.Instance.touches.Count; i++)
			{
				tw = VCTouchController.Instance.touches[i];
				
				// we only care about touches that began this frame
				if (tw.phase != TouchPhase.Began)
					continue;
				
				// if we don't require collision, just take the first touch
				if (anyTouchActivatesControl)
				{
					if (SetTouch(tw))
						break;
				}
				
				// if positioning at touch location, take first touch inside area
				if (positionAtTouchLocation)
				{
					Vector2 positionPercentage = new Vector2(tw.position.x / Screen.width, tw.position.y / Screen.height);
					
					// keep searching if touch is outside of area
					if (positionPercentage.x < positionAtTouchLocationAreaMin.x || positionPercentage.x > positionAtTouchLocationAreaMax.x)
						continue;
					if (positionPercentage.y < positionAtTouchLocationAreaMin.y || positionPercentage.y > positionAtTouchLocationAreaMax.y)
						continue;
					
					if (SetTouch(tw))
						break;
				}
					
				if (Colliding(tw))
				{
					if (SetTouch (tw))
						break;
				}
			}
			
			// if we found a touch, process it
			if (_touch != null)
			{
				ProcessTouch(_touch);
			}
		}
		
		// set our visiblity accordingly
		SetVisible(visibleWhenNotActive || this.Dragging, _movingPartVisible == hideMovingPart);
	}
	
// debug touch code
#if UNITY_EDITOR
	protected void UpdateDebugTouch()
	{

		if (!debugKeysEnabled)
			return;
		
		if (debugTouchKeyTogglesTouch)
		{
			if (Input.GetKeyDown(debugTouchKey))
			{
				// if we were touching, stop
				if (!StopDebugTouch())
					StartDebugTouch();
			}
		}
		else if (Input.GetKey(debugTouchKey))
		{
			if (StartDebugTouch())
				return;
		}
		else 
		{
			if (StopDebugTouch())
				return;
		}
		
		if (_touch != null && _touch.debugTouch)
		{
			TouchPhase newPhase = TouchPhase.Stationary;
			
			// update keys
			if (Input.GetKey(debugLeftKey))
			{
				_touch.position.x -= debugTouchMovementSpeedPixels * Time.deltaTime;
				newPhase = TouchPhase.Moved;
			}
			
			if (Input.GetKey(debugRightKey))
			{
				_touch.position.x += debugTouchMovementSpeedPixels * Time.deltaTime;
				newPhase = TouchPhase.Moved;
			}
			
			if (Input.GetKey(debugUpKey))
			{
				_touch.position.y += debugTouchMovementSpeedPixels * Time.deltaTime;
				newPhase = TouchPhase.Moved;
			}
			
			if (Input.GetKey(debugDownKey))
			{
				_touch.position.y -= debugTouchMovementSpeedPixels * Time.deltaTime;
				newPhase = TouchPhase.Moved;
			}
			
			_touch.phase = newPhase;
		}
		
	}
	
	public bool StopDebugTouch()
	{
		if (_touch != null && _touch.debugTouch)
		{
			StopDragging();
			return true;
		}
		
		return false;
	}
	
	public bool StartDebugTouch()
	{
		if (_touch == null)
		{
			SetTouch(new VCTouchWrapper(GetDebugTouchOrigin()));
			_touch.debugTouch = true;
			return true;
		}
		
		return false;
	}
	
	protected virtual Vector2 GetDebugTouchOrigin()
	{
		return new Vector2(_movingPartOrigin.x, _movingPartOrigin.y);
	}
#endif
	
	// updates drag delta
	protected virtual void UpdateDelta()
	{
		// update our position
		_deltaPixels.x = (_touch.position.x - _touchOriginScreen.x) * dragScaleFactor.x;
		_deltaPixels.y = (_touch.position.y - _touchOriginScreen.y) * dragScaleFactor.y;
		
		// clamp delta magnitude to user-specified max
		if (_deltaPixels.sqrMagnitude > _dragDeltaMagnitudeMaxSq)
		{
			_deltaPixels = _deltaPixels.normalized * dragDeltaMagnitudeMaxPixels;
		}
		
		_movingPartOffset.x = _deltaPixels.x;
		_movingPartOffset.y = _deltaPixels.y;
	}
	
	protected bool SetTouch(VCTouchWrapper tw)
	{
		if (requireExclusiveTouch)
		{
			// don't do anything if this touch is already in use
			foreach (VCTouchWrapper twInUse in touchesInUse)
			{
				if (twInUse.fingerId == tw.fingerId)
					return false;
			}
		}
		
		_touch = tw;
		TapCount++;
		if (TapCount == 1)
		{
			_tapTime = Time.time;
		}
		else if (TapCount > 0 && (TapCount % 2 == 0) && OnDoubleTap != null)
		{
			OnDoubleTap(this);
		}
		
		touchesInUse.Add(tw);
		
		_wasDragging = true;
		
		return true;
	}
	
	// ceases dragging and repositions joystick
	protected void StopDragging()
	{
		SetPosition(movingPart, _movingPartOrigin);
		_deltaPixels = Vector2.zero;
		
		_wasDragging = false;
		
		touchesInUse.Remove(_touch);
		_touch = null;
	}
	
	// maps the actual axis value to the appropriate value for our user-specified range.
	protected float RangeAdjust(float val, float min, float max)
	{
		float range = max - min;
		
		float absValue = Mathf.Abs(val);
		
		if (absValue < min)
			return 0.0f;
		
		if (absValue > max)
			return 1.0f * VCUtils.GetSign(val);
		
		return (absValue - min) / range * VCUtils.GetSign(val);
	}
	
	#region public properties
	
	/// <summary>
	/// Gets the current X Axis Value.
	/// </summary>
	/// <value>
	/// Returns a float between -1.0f and 1.0f inclusive.
	/// </value>
	public float AxisX
	{
		get { return RangeAdjust(_deltaPixels.x / dragDeltaMagnitudeMaxPixels, rangeMin.x, rangeMax.x); }
	}
	
	/// <summary>
	/// Gets the current Y Axis Value.
	/// </summary>
	/// <value>
	/// Returns a float between -1.0f and 1.0f inclusive.
	/// </value>
	public float AxisY
	{
		get { return RangeAdjust(_deltaPixels.y / dragDeltaMagnitudeMaxPixels, rangeMin.y, rangeMax.y); }
	}
	
	/// <summary>
	/// Gets the current X Axis value without adjusting for rangeMin and rangeMax.
	/// </summary>
	/// <value>
	/// Returns a float between -1.0f and 1.0f inclusive.
	/// </value>
	public float AxisXRaw
	{
		get { return _deltaPixels.x / dragDeltaMagnitudeMaxPixels; }
	}
	
	/// <summary>
	/// Gets the current Y Axis value without adjusting for rangeMin and rangeMax.
	/// </summary>
	/// <value>
	/// Returns a float between -1.0f and 1.0f inclusive.
	/// </value>
	public float AxisYRaw
	{
		get { return _deltaPixels.y / dragDeltaMagnitudeMaxPixels; }
	}
	
	/// <summary>
	/// Gets the square Magnitude of the joystick vector.
	/// </summary>
	/// <value>
	/// Returns a float between 0 and dragDeltaMagnitudeMaxPixels.
	/// </value>
	public float MagnitudeSqr
	{
		get { return AxisX * AxisX + AxisY * AxisY; }
	}
	
	/// <summary>
	/// Gets the angle of the joystick vector in Radians.
	/// </summary>
	/// <value>
	/// The angle in radians.
	/// </value>
	public float AngleRadians
	{
		get { return Mathf.Atan2(AxisY, AxisX); }
	}
	
	/// <summary>
	/// Gets the angle of the joystick vector in Degrees.
	/// </summary>
	/// <value>
	/// The angle in degrees.
	/// </value>
	public float AngleDegrees
	{
		get { return 180.0f / Mathf.PI * Mathf.Atan2(AxisY, AxisX); }
	}
	
	/// <summary>
	// Returns true if the joystick is being dragged
	/// </summary>
	public bool Dragging
	{
		get { return (_touch != null && _touch.Active); }
	}
	/// <summary>
	/// Number of times control has been tapped during the current tap interval, which
	/// is specified by tapCountResetTime.
	/// </summary>
	public int TapCount { get; private set; }
	
	/// <summary>
	/// The TouchWrapper currently interacting with this control.  May be null.
	/// </summary>
	public VCTouchWrapper TouchWrapper
	{
		get { return _touch; }
	}
	#endregion
	
	#region protected properties
	protected float RangeX
	{
		get
		{
			return rangeMax.x - rangeMin.x;
		}
	}
	
	protected float RangeY
	{
		get
		{
			return rangeMax.y - rangeMin.y;
		}
	}
	#endregion
}
