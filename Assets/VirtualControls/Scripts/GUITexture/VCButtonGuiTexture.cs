//----------------------------------------
// Virtual Controls Suite for Unity
// Â© 2012 Bit By Bit Studios, LLC
// Author: sean@bitbybitstudios.com
// Use of this software means you accept the license agreement.  
// Please don't redistribute without permission :)
//---------------------------------------------------------------

using UnityEngine;

/// <summary>
/// A Button that uses Unity's native GUITextures for graphics.
/// </summary>
public class VCButtonGuiTexture : VCButtonWithBehaviours 
{
	
	/// <summary>
	/// When hit testing against a GUITexture's ScreenRect(), the Rect will be scaled from the center
	/// by this factor in each dimension.
	/// </summary>
	public Vector2 hitRectScale = new Vector2(1.0f, 1.0f);
	
	// cached guiTexture of the colliderObject for performance
	protected GUITexture _colliderGuiTexture;
	
	protected override bool Init ()
	{
		// we don't require a collider if the colliderObject has a guiTexture
		_requireCollider = false;
		
		if (!base.Init ())
			return false;
		
		if (_collider == null)
		{
			// use the colliderObject's guiTexture for hit detection
			_colliderGuiTexture = colliderObject.GetComponent<GUITexture>();
			
			if (_colliderGuiTexture == null)
			{
				VCUtils.DestroyWithError(gameObject, "There is no Collider attached to colliderObject, as well as no GUITexture, attach one or the other.  Destroying this control.");
				return false;
			}
		}
		
		return true;
	}
	
	protected override void InitBehaviours ()
	{
		if (upStateObject != null)
			_upBehaviour = upStateObject.GetComponent<GUITexture>();
		
		if (pressedStateObject != null)
			_pressedBehavior = pressedStateObject.GetComponent<GUITexture>();
	}
	
	protected override bool Colliding (VCTouchWrapper tw)
	{
		// hit test against a collider if we have one
		if (_collider != null)
		{
			return AABBContains(tw.position);
		}
		
		// otherwise, fall back to a rect hit test on the guiTexture
		Rect r = _colliderGuiTexture.GetScreenRect();
		VCUtils.ScaleRect(ref r, hitRectScale);
		return r.Contains(tw.position);
	}
}
