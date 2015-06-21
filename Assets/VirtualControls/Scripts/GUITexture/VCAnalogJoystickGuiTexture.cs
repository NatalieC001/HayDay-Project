//----------------------------------------
// Virtual Controls Suite for Unity
// Â© 2012 Bit By Bit Studios, LLC
// Author: sean@bitbybitstudios.com
// Use of this software means you accept the license agreement.  
// Please don't redistribute without permission :)
//---------------------------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// An Analog Joystick that uses Unity's native GUITextures for graphics.
/// If no Collider component exists on colliderObject, the collilderObject's
/// GUITexture Component's screen Rect will be hit tested against.
/// </summary>
public class VCAnalogJoystickGuiTexture : VCAnalogJoystickBase
{
	/// <summary>
	/// When hit testing against a GUITexture's ScreenRect(), the Rect will be scaled from the center
	/// by this factor in each dimension.
	/// </summary>
	public Vector2 hitRectScale = new Vector2(1.0f, 1.0f);
	
	protected GUITexture _movingPartGuiTexture;
	
	protected override bool Init ()
	{
		if (!base.Init ())
			return false;
		
		// cache for performance
		_movingPartGuiTexture = movingPart.GetComponent<GUITexture>();
		
		return true;
	}
	
	protected override void InitOriginValues ()
	{
		_touchOrigin = movingPart.transform.position;
		_touchOriginScreen = new Vector2(_touchOrigin.x * Screen.width, _touchOrigin.y * Screen.height);
		_movingPartOrigin = movingPart.transform.position;
	}
	
	protected override void UpdateDelta ()
	{
		base.UpdateDelta ();
		
		// GUITextures positions are expressed in terms of percentage of Screen dimensions, 0 - 1.
		_movingPartOffset.x = _deltaPixels.x / Screen.width;
		_movingPartOffset.y = _deltaPixels.y / Screen.height;
	}
	
	protected override bool Colliding (VCTouchWrapper tw)
	{
		if (!tw.Active)
			return false;
		
		// first try and hitTest against a collider if we have one
		if (_collider != null)
		{
			return AABBContains(tw.position);
		}
		
		// otherwise, fall back to a rect hit test on the guiTexture (this is what most people will want)
		Rect r = _movingPartGuiTexture.GetScreenRect();
		VCUtils.ScaleRect(ref r, hitRectScale);
		return r.Contains(tw.position);
	}
	
	protected override void ProcessTouch (VCTouchWrapper tw)
	{
		// if measuring relative to center, set origin to movingPart pos
		if (measureDeltaRelativeToCenter)
		{
			_touchOrigin = movingPart.transform.position;
			_touchOriginScreen.x = _touchOrigin.x * Screen.width;
			_touchOriginScreen.y = _touchOrigin.y * Screen.height;
		}
		else
		{
			// otherwise set origin to touch location
			_touchOrigin.x = tw.position.x / Screen.width;
			_touchOrigin.y = tw.position.y / Screen.height;
			_touchOrigin.z = movingPart.transform.position.z;
			
			_touchOriginScreen.x = tw.position.x;
			_touchOriginScreen.y = tw.position.y;
		}
		
		// if we need to move the joystick, do it
		if (positionAtTouchLocation)
		{
			_movingPartOrigin = _touchOrigin;
			basePart.transform.position = new Vector3(_touchOrigin.x, _touchOrigin.y, basePart.transform.position.z);
		}
	}
	
	protected override void SetVisible(bool visible, bool forceUpdate)
	{
		if (!forceUpdate && _visible == visible)
			return;
		
		// cache a list of the ui components we'll be disabling and enabling for visiblity changes
		if (_visibleBehaviourComponents == null)
		{
			_visibleBehaviourComponents = new List<Behaviour>(basePart.GetComponentsInChildren<GUITexture>());
		}
		
		foreach (var c in _visibleBehaviourComponents)
		{
			c.enabled = visible;
		}
		
		// set the moving part visibility explicitly
		_movingPartVisible = visible && !hideMovingPart;
		movingPart.GetComponent<GUITexture>().enabled = _movingPartVisible;
		
		_visible = visible;
	}
	
#if UNITY_EDITOR
	protected override Vector2 GetDebugTouchOrigin()
	{
		return new Vector2(_movingPartOrigin.x * Screen.width, _movingPartOrigin.y * Screen.height);
	}
#endif
}

