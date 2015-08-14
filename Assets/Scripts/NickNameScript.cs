using UnityEngine;
using System.Collections;

public class NickNameScript : MonoBehaviour 
{
	public Texture backgroundTexture;
	public GUIStyle labelNickName;
	public GUIStyle buttonPlayStyle;
	public GUIStyle textFieldStyle;
	public AudioClip buttonSound;
	public string playerName = "Anonymous";

	void OnGUI()
	{
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), backgroundTexture);
		GUI.Label (new Rect (Screen.width * .22f, Screen.height * .05f, Screen.width * .6f, Screen.height * .20f), "", labelNickName);
		GUI.SetNextControlName ("PlayerInput");
		playerName = GUI.TextField(new Rect(0, Screen.height * .40f, Screen.width, Screen.height * .22f), playerName, textFieldStyle);
			
		if (GUI.Button (new Rect (Screen.width * .7f, Screen.height * .8f, Screen.width * .275f, Screen.height * .180f), "", buttonPlayStyle)) 
		{
			GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
			StartCoroutine(WaitFor(2));
		}

		if (GUI.GetNameOfFocusedControl () == "PlayerInput")
		{
			if (playerName == "Anonymous")
			{
				playerName = "";
			}
		}
	}

	void OnDisable() 
	{
		PlayerPrefs.SetString("PlayerName", playerName);
	}
	
	void Update() 
	{
		if (Input.GetKeyDown (KeyCode.Escape)) 
		{
			GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
			StartCoroutine(WaitFor(0));
		}
	}
	
	IEnumerator WaitFor(int level) 
	{
		yield return new WaitForSeconds(1.0f);
		Application.LoadLevel (level);
	}
}