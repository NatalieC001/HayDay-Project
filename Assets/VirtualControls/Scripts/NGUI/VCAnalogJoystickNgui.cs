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
/// An Analog Joystick that uses Tasharen Entertainment's NGUI for graphics.
/// NGUI's UISprite class is supported.
/// </summary>
public class VCAnalogJoystickNgui : VCAnalogJoystickBase
{
	protected UISprite _movingPartSprite;
	
	protected override bool Init ()
	{
		// make sure fake classes aren't defined
		if (!VCPluginSettings.NguiEnabled(gameObject))
			return false;
		
		if (!base.Init ())
			return false;
	
		// we require a collider
		if (_collider == null)
		{
			VCUtils.DestroyWithError(gameObject, "No collider attached to colliderGameObject!  Destroying this gameObject.");
			return false;
		}
		
		_movingPartSprite = movingPart.GetComponent<UISprite>();
		if (_movingPartSprite == null)
			_movingPartSprite = movingPart.GetComponentInChildren<UISprite>();
		
		return true;
	}
	
	// use the camera to set origin values
	protected override void InitOriginValues ()
	{
		_touchOrigin = _colliderCamera.WorldToViewportPoint(movingPart.transform.position);
		_touchOriginScreen = _colliderCamera.WorldToScreenPoint(movingPart.transform.position);
		_movingPartOrigin = movingPart.transform.localPosition;
	}
	
	protected override bool Colliding (VCTouchWrapper tw)
	{
		if (!tw.Active)
			return false;
		
		// NGUI's default OnHover and OnPress behavior doesn't update target info until
		// the mouse button or touch is released, which is not compatible with some VCAnalogJoystick behavior.
		// instead, we use a simple AABB hit test.
		return AABBContains(tw.position);
	}
	
	protected override void ProcessTouch (VCTouchWrapper tw)
	{
		if (measureDeltaRelativeToCenter)
		{
			_touchOrigin = movingPart.transform.position;
			_touchOriginScreen = _colliderCamera.WorldToScreenPoint(movingPart.transform.position);
		}
		else
		{
			_touchOrigin = _colliderCamera.ScreenToWorldPoint(tw.position);
			_touchOriginScreen.x = tw.position.x;
			_touchOriginScreen.y = tw.position.y;
		}
		
		if (positionAtTouchLocation)
		{
			float zCache = movingPart.transform.localPosition.z;
			Vector3 targetPos = _touchOrigin;
			targetPos.z = basePart.transform.position.z;
			basePart.transform.position = targetPos;
			targetPos.z = movingPart.transform.position.z;
			movingPart.transform.position = targetPos;
			_movingPartOrigin.Set(movingPart.transform.localPosition.x, movingPart.transform.localPosition.y, zCache);
		}
	}
	
	protected override void SetVisible (bool visible, bool forceUpdate)
	{
		if (!forceUpdate && _visible == visible)
			return;
		
		// cache a list of the ui components we'll be disabling and enabling for visiblity changes
		if (_visibleBehaviourComponents == null)
		{
			_visibleBehaviourComponents = new List<Behaviour>(basePart.GetComponentsInChildren<UISprite>());
		}
		
		foreach (var c in _visibleBehaviourComponents)
		{
			c.enabled = visible;
		}
		
		// set the moving part explicitly
		_movingPartVisible = visible && !hideMovingPart;
		_movingPartSprite.enabled = _movingPartVisible;
		
		_visible = visible;
	}
	
	// NGUI works with everything in localPosition, so set localPosition instead of position.
	protected override void SetPosition (GameObject go, Vector3 vec)
	{
		go.transform.localPosition = new Vector3(vec.x, vec.y, go.transform.localPosition.z);
	}
	
	protected override void UpdateDelta ()
	{
		base.UpdateDelta ();
	}
}

