//----------------------------------------
// Virtual Controls Suite for Unity
// © 2012 Bit By Bit Studios, LLC
// Author: sean@bitbybitstudios.com
// Use of this software means you accept the license agreement.  
// Please don't redistribute without permission :)
//---------------------------------------------------------------

using UnityEngine;
using System.Reflection;

/// <summary>
/// This class uses defines to create skeleton classes for supported
/// UI kits in Virtual Controls.  This way, VCS will compile without
/// requiring these kits to be installed.\n\n
/// 
/// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
/// ==============================================
/// 
/// README IF YOU WOULD LIKE TO USE EZGUI OR NGUI
/// 
/// ==============================================
/// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
/// Simply change the #if true line below for the desired UI package
/// to #if false.  If using NGUI, you will also need to comment out line 72: public class UIButton...
/// </summary>
public class VCPluginSettings 
{
	// used in VC code to determine which fake classes are enabled
	public const string kFakeMemberName = "fakeMember";
	
	public static bool EzguiEnabled(GameObject go)
	{
#if UNITY_WP8
		// Note: WP8 Metro doesn't support Type.GetMember(), and will error out during building.
		// We just assume EzguiEnabled in this case.
		return true;
#else
		if (typeof(SpriteRoot).GetMember(VCPluginSettings.kFakeMemberName).Length == 0)
			return true;
		
		VCUtils.DestroyWithError(go, "An EZGUI Virtual Control is being used, but EZGUI is not properly enabled!\n" +
				"In order to use EZGUI, open VCPluginSettings.cs and edit line 63 to #if false.\n" +
				"See that file for further instruction.  Destroying this control."); 
		
		return false;
#endif
	}
	
	public static bool NguiEnabled(GameObject go)
	{
#if UNITY_WP8
		// Note: WP8 Metro doesn't support Type.GetMember(), and will error out during building.
		// We just assume EzguiEnabled in this case.
		return true;
#else
		if (typeof(UISprite).GetMember(VCPluginSettings.kFakeMemberName).Length == 0)
			return true;
		
		VCUtils.DestroyWithError(go, "An NGUI Virtual Control is being used, but NGUI is not properly enabled!\n" +
				"In order to use NGUI, open VCPluginSettings.cs and edit line 82 to #if false.\n" +
				"See that file for further instruction.  Destroying this control."); 
		
		return false;
#endif
	}
}

//==============
// EZGUI - change the #if true below to: 
// #if false
// if you want to use EZGUI Virtual Controls
//==============

#if true

// classes used to allow compilation without EZGUI installed.
public class SpriteRoot : MonoBehaviour 
{
	public bool fakeMember;
	public void Hide(bool val) {}
}
public class SimpleSprite : SpriteRoot {}
public class UIButton : SpriteRoot {}
#endif

//==============
// NGUI - change the #if true below to: 
// #if false
// AND comment out the public class UIButton... line on line 84
// if you want to use NGUI Virtual Controls.
// Change it back to #if true to and uncomment line 84 to disable NGUI.
//==============

#if true

// classes used to allow compilation without NGUI installed.
public class UISprite : MonoBehaviour 
{
	public bool fakeMember;
	public FakePanel panel;
}

public class FakePanel : MonoBehaviour
{
	public bool widgetsAreStatic;
	public void Refresh() {}
}
#endif

