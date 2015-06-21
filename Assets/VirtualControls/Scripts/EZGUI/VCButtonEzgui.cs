//----------------------------------------
// Virtual Controls Suite for Unity
// © 2012 Bit By Bit Studios, LLC
// Author: sean@bitbybitstudios.com
// Use of this software means you accept the license agreement.  
// Please don't redistribute without permission :)
//---------------------------------------------------------------

using UnityEngine;

/// <summary>
/// A Button that uses AnBSoft's EZGUI for graphics.
/// EZGUI UIButton and SimpleSprite are supported.
/// </summary>
public class VCButtonEzgui : VCButtonWithBehaviours 
{
	protected override bool Init ()
	{
		// make sure fake classes aren't defined
		if (!VCPluginSettings.EzguiEnabled(gameObject))
			return false;
		
		if (!base.Init ())
			return false;
		
		if (colliderObject == upStateObject || colliderObject == pressedStateObject)
		{
			// EZGUI hides controls by actually modifying their size and thusly their colliders, this will interfere
			// with pressing behavior if the collider used by VCButton is on one of those objects.
			Debug.LogWarning("VCButtonEZGUI may not behave properly when the hitTestGameObject is the same as the up " +
				"or pressedStateGameObject!  You should add a Collider to a gameObject independent from the EZGUI UI components.");
		}
		
		return true;
	}
	
	protected override void InitBehaviours ()
	{
		_upBehaviour = GetEzguiBehavior(upStateObject);
		_pressedBehavior = GetEzguiBehavior(pressedStateObject);
	}
	
	protected override void ShowPressedState (bool pressed)
	{
		if (pressed)
		{
			if (_upBehaviour != null)
			{
				(_upBehaviour as SpriteRoot).Hide(true);
			}
			
			if (_pressedBehavior != null)
			{
				(_pressedBehavior as SpriteRoot).Hide(false);
			}
		}
		else
		{
			if (_upBehaviour != null)
			{
				(_upBehaviour as SpriteRoot).Hide(false);
			}
			
			if (_pressedBehavior != null)
			{
				(_pressedBehavior as SpriteRoot).Hide(true);
			}
		}
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
