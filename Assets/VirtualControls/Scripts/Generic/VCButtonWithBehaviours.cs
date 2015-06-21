//----------------------------------------
// Virtual Controls Suite for Unity
// © 2012 Bit By Bit Studios, LLC
// Author: sean@bitbybitstudios.com
// Use of this software means you accept the license agreement.  
// Please don't redistribute without permission :)
//---------------------------------------------------------------

using UnityEngine;

/// <summary>
/// A Button that has a Up and Pressed Behaviors (taken as Components from a GameObject) associated with it.
/// Most UI-specific VCButton classes will want to extend this class.
/// </summary>
public class VCButtonWithBehaviours : VCButtonBase 
{
	#region inspector variables
	/// <summary>
	/// GameObject with a Component that will be visible only when not Pressed.
	/// </summary>
	public GameObject upStateObject;
	
	/// <summary>
	/// GameObject with a Component that will be visible only when Pressed.
	/// </summary>
	public GameObject pressedStateObject;
	
	/// <summary>
	/// GameObject with a Collider that will be hit tested in order to evaluate Pressed state.
	/// </summary>
	public GameObject colliderObject;
	#endregion
	
	#region protected members
	// Behaviour attached to upStateObject
	protected Behaviour _upBehaviour;
	
	// Behaviour attached to pressedStateObject
	protected Behaviour _pressedBehavior;
	
	// when true, will cause an error during Init if no collider is present
	protected bool _requireCollider = true;
	#endregion
	
	protected override bool Init ()
	{
		if (!base.Init ())
			return false;
		
		InitGameObjects();
		InitBehaviours();
		
		return true;
	}
	
	protected void InitGameObjects()
	{
		if (upStateObject == null && pressedStateObject == null)
		{
			Debug.LogWarning("No up or pressed state GameObjects specified! Setting upStateObject to this.gameObject.");
			upStateObject = this.gameObject;
		}
		
		if (colliderObject == null)
		{
			// set colliderObject to the first non null GameObject in this order:
			colliderObject = upStateObject ?? pressedStateObject ?? this.gameObject;
		}
		
		InitCollider(colliderObject);
		
		if (_requireCollider && _collider == null)
		{
			VCUtils.DestroyWithError(this.gameObject, "colliderObject must have a Collider component!  Destroying this control.");
			return;
		}
	}
	
	
	/// <summary>
	/// Should be overridden in order to set references to your particular UI solution's 
	/// components from the upStateObject and pressedStateObject.
	/// </summary>
	protected virtual void InitBehaviours()
	{
		
	}
	
	protected override bool Colliding (VCTouchWrapper tw)
	{
		return AABBContains(tw.position);
	}
	
	protected override void ShowPressedState (bool pressed)
	{
		if (pressed)
		{
			// show pressed state
			if (_upBehaviour != null)
				_upBehaviour.enabled = false;
			
			if (_pressedBehavior != null)
				_pressedBehavior.enabled = true;
		}
		else
		{
			// show up state
			if (_upBehaviour != null)
				_upBehaviour.enabled = true;
			
			if (_pressedBehavior != null)
				_pressedBehavior.enabled = false;
		}
	}
}
