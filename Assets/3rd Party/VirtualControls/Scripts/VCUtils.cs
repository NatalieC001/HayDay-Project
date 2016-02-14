//----------------------------------------
// Virtual Controls Suite for Unity
// © 2012 Bit By Bit Studios, LLC
// Author: sean@bitbybitstudios.com
// Use of this software means you accept the license agreement.  
// Please don't redistribute without permission :)
//---------------------------------------------------------------

using UnityEngine;

/// <summary>
/// Static utilities class used by Virtual Controls Suite.
/// </summary>
public class VCUtils
{
	
	public static bool GetActive(GameObject go)
	{
#if UNITY_2_6 || UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
		return go.active;
#else
		return go.activeSelf;
#endif
	}
	
	public static void SetActive(GameObject go, bool active)
	{
#if UNITY_2_6 || UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
		go.active = active;
#else
		go.SetActive(active);
#endif
	}
	
	/// <summary>
	/// Returns 1.0f if a number is zero or positive, and -1.0f if negative.
	/// </summary>
	public static float GetSign(float val)
	{
		if (val < 0)
			return -1.0f;
		
		return 1.0f;
	}
	
	/// <summary>
	/// Returns the first camera (but perhaps not the only one!) that draws the specified GameObject.
	/// </summary>
	public static Camera GetCamera(GameObject go)
	{
		foreach (Camera c in GameObject.FindObjectsOfType(typeof(Camera)))
		{
			if ((c.cullingMask & (1 << go.layer)) != 0)
			{
				return c;
			}
		}
		
		return null;
	}
	
	/// <summary>
	/// Scales the passed Rect from its center by a specified amount.
	/// </summary>
	public static void ScaleRect(ref Rect r, Vector2 scale)
	{
		ScaleRect(ref r, scale.x, scale.y);
	}
	
	/// <summary>
	/// Scales the passed Rect from its center by a specified amount.
	/// </summary>
	public static void ScaleRect(ref Rect r, float scaleX, float scaleY)
	{
		Vector2 center = r.center;
		r.width *= scaleX;
		r.height *= scaleY;
		r.center = center;
	}
	
	/// <summary>
	/// Outputs an error and then Destroys a GameObject.
	/// </summary>
	public static void DestroyWithError(GameObject go, string message)
	{
		Debug.LogError(message);
		
		// reset our singleton instance for touch controller if this about to be destroyed GO owned it.
		if (VCTouchController.Instance != null && VCTouchController.Instance.gameObject == go)
			VCTouchController.ResetInstance();
		
		GameObject.Destroy(go);
	}
	
	/// <summary>
	/// Adds VCTouchController component to specified GameObject.
	/// </summary>
	public static void AddTouchController(GameObject go)
	{
		go.AddComponent<VCTouchController>();
	}
}

