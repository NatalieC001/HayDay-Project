using UnityEngine;
using System.Collections;

/// <summary>
/// A simple script that stores all the properties of a VCDPad
/// in inspector-visible variables so that a Playmaker GetProperty action
/// can be used to inspect a DPad.
/// NOTE: You should use Unity's Edit > Project Settings > Script Execution Order
/// for your project to make sure this script gets executed AFTER VCDPadBase.
/// otherwise data will be 1 frame delayed.
/// </summary>
public class VCDPadPlaymakerUpdater : MonoBehaviour 
{
	public VCDPadBase dpad;
	
	#region Inspector variables
	public bool up;
	public bool down;
	public bool left;
	public bool right;
	#endregion
	
	// Use this for initialization
	void Start () 
	{
		if (dpad == null)
		{
			// try to find it on this gameObject.
			dpad = gameObject.GetComponent<VCDPadBase>();
			if (dpad == null)
			{
				VCUtils.DestroyWithError(gameObject, "You must specify a button for VCDPadPlaymakerUpdater to function.  Destroying this object.");
				return;
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		up = dpad.Up;
		down = dpad.Down;
		left = dpad.Left;
		right = dpad.Right;
	}
}
