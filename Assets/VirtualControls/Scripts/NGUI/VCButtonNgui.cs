//----------------------------------------
// Virtual Controls Suite for Unity
// © 2012 Bit By Bit Studios, LLC
// Author: sean@bitbybitstudios.com
// Use of this software means you accept the license agreement.  
// Please don't redistribute without permission :)
//---------------------------------------------------------------

using UnityEngine;

/// <summary>
/// A Button that uses Tasharen Entertainment's NGUI for graphics.
/// NGUI's UISprite is supported.
/// </summary>
public class VCButtonNgui : VCButtonWithBehaviours
{
	protected override bool Init ()
	{
		// make sure fake classes aren't defined
		if (!VCPluginSettings.NguiEnabled(gameObject))
			return false;
		
		return base.Init ();
	}
	
	protected override void InitBehaviours ()
	{
		if (upStateObject != null)
		{
			_upBehaviour = upStateObject.GetComponent<UISprite>();
		}
		
		if (pressedStateObject != null)
		{
			_pressedBehavior = pressedStateObject.GetComponent<UISprite>();
		}
	}
	
	protected override void ShowPressedState (bool pressed)
	{
		base.ShowPressedState(pressed);
		
		// if the panel has "widgetsAreStatic" marked, then we won't see a change
		// unless we force one, so lets do that
		if (this.Pressed)
		{
			UISprite pressedSprite = _pressedBehavior as UISprite;
			if (pressedSprite != null && pressedSprite.panel.widgetsAreStatic)
			{
				pressedSprite.panel.Refresh();
			}
		}
		else
		{
			UISprite upSprite = _upBehaviour as UISprite;
			if (upSprite != null && upSprite.panel.widgetsAreStatic)
			{
				upSprite.panel.Refresh();
			}
		}
	}
	
	protected override bool Colliding (VCTouchWrapper tw)
	{
		return AABBContains(tw.position);
	}
}
