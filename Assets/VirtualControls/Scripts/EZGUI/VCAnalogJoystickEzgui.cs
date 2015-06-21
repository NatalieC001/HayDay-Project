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
/// An Analog Joystick that uses AnBSoft's EZGUI for graphics.
/// EZGUI's UISprite and UIButton are supported.
/// </summary>
public class VCAnalogJoystickEzgui : VCAnalogJoystickBase
{
	// this can be a SimpleSprite or a UIButton
	protected Behaviour _movingPartBehaviorComponent;
	
	protected override bool Init ()
	{
		// make sure fake classes aren't defined
		if (!VCPluginSettings.EzguiEnabled(gameObject))
			return false;

		if (!base.Init ())
			return false;
		
		if (colliderObject == movingPart)
		{
			// EZGUI hides controls by actually modifying their size and thusly their colliders, this will interfere
			// with collision detection behavior if the collider is on one of those objects.
			Debug.LogWarning("VCAnalogJoystickEzgui may not behave properly when the colliderObject is the same as the movingPart! " +
				"You should add a Collider to a gameObject independent from the EZGUI UI components.");
		}
		
		_movingPartBehaviorComponent = GetEzguiBehavior(movingPart);
		if (_movingPartBehaviorComponent == null)
		{
			VCUtils.DestroyWithError(gameObject, "Cannot find a SimpleSprite or UIButton component on movingPart.  Destroying this control.");
			return false;
		}
		
		if (_collider == null)
		{
			VCUtils.DestroyWithError(gameObject, "No collider attached to colliderGameObject!  Destroying this control.");
			return false;
		}
		
		return true;
	}
	
	// use camera to determine origin positions
	protected override void InitOriginValues ()
	{
		_touchOrigin = _colliderCamera.WorldToViewportPoint(movingPart.transform.position);
		_touchOriginScreen = _colliderCamera.WorldToScreenPoint(movingPart.transform.position);
		_movingPartOrigin = movingPart.transform.localPosition;
	}
	
	// We don't leverage EZGUI's rayCast hit test functionality.
	// Instead we use a simple AABB hit test on our collider.
	protected override bool Colliding (VCTouchWrapper tw)
	{
		if (!tw.Active)
			return false;
		
		return AABBContains(tw.position);
	}
	
	protected override void ProcessTouch (VCTouchWrapper tw)
	{
		// if measuring delta relative to center, set origin to movingPart's pos
		if (measureDeltaRelativeToCenter)
		{
			_touchOrigin = movingPart.transform.position;
			_touchOriginScreen = _colliderCamera.WorldToScreenPoint(movingPart.transform.position);
		}
		else
		{
			// otherwise set it to the touch location
			_touchOrigin = _colliderCamera.ScreenToWorldPoint(tw.position);
			_touchOrigin.Set (_touchOrigin.x, _touchOrigin.y, basePart.transform.position.z);
			
			_touchOriginScreen.x = tw.position.x;
			_touchOriginScreen.y = tw.position.y;
		}
		
		// move to the touch location if necessary
		if (positionAtTouchLocation)
		{
			float zCache = movingPart.transform.localPosition.z;
			basePart.transform.localPosition = _touchOrigin;
			movingPart.transform.position = _touchOrigin;
			_movingPartOrigin.Set(movingPart.transform.localPosition.x, movingPart.transform.localPosition.y, zCache);
		}
	}
		
	protected override void SetVisible (bool visible, bool forceUpdate)
	{
		if (!forceUpdate && _visible == visible)
			return;
		
		// cache a list of the components we need to call Hide() on
		// for now we support UISprite and UIButton
		if (_visibleBehaviourComponents == null)
		{
			_visibleBehaviourComponents = new List<Behaviour>(basePart.GetComponentsInChildren<SimpleSprite>());
			_visibleBehaviourComponents.AddRange(basePart.GetComponentsInChildren<UIButton>());
		}
		
		foreach (var c in _visibleBehaviourComponents)
		{
			(c as SpriteRoot).Hide(!visible);
		}
		
		// set moving part explicitly
		_movingPartVisible = visible && !hideMovingPart;
		(_movingPartBehaviorComponent as SpriteRoot).Hide(!_movingPartVisible);
		
		_visible = visible;
	}
	
	// EZGUI components should be moved using localPosition
	protected override void SetPosition (GameObject go, Vector3 vec)
	{
		go.transform.localPosition = new Vector3(vec.x, vec.y, go.transform.localPosition.z);
	}
	
	/// <summary>
	/// Helper utility which attempts to find a supported EZGUI behaviour on supplied GameObject.
	/// Currently supported are SimpleSprite and UIButton.
	/// </summary>
	protected Behaviour GetEzguiBehavior(GameObject go)
	{
		if (go == null)
			return null;
		
		Behaviour retval = go.GetComponent<SimpleSprite>();
		if (retval == null)
			retval = go.GetComponent<UIButton>();
		
		return retval;
	}
}