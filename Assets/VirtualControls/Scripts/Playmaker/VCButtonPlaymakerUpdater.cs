using UnityEngine;
using System.Collections;

/// <summary>
/// A simple script that stores all the properties of a VCButton
/// in inspector-visible variables so that a Playmaker GetProperty action
/// can be used to inspect a button.
/// NOTE: You should use Unity's Edit > Project Settings > Script Execution Order
/// for your project to make sure this script gets executed AFTER the VCButton type you're using.
/// otherwise data will be 1 frame delayed.
/// </summary>
public class VCButtonPlaymakerUpdater : MonoBehaviour 
{
	public VCButtonBase button;
	
	#region Inspector variables
	public bool pressed;
	public bool forcePressed;
	public float holdTime;
	#endregion
	
	void Start () 
	{
		if (button == null)
		{
			// try to find it on this gameObject.
			button = gameObject.GetComponent<VCButtonBase>();
			if (button == null)
			{
				VCUtils.DestroyWithError(gameObject, "You must specify a button for VCButtonPlaymakerUpdater to function.  Destroying this object.");
				return;
			}
		}
	}
	
	void Update () 
	{
		pressed = button.Pressed;
		forcePressed = button.ForcePressed;
		holdTime = button.HoldTime;
	}
}
