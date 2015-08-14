using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour 
{
	public Texture backgroundTexture;
	public GUIStyle labelGameTitle;
	public GUIStyle buttonNewStyle;
	public GUIStyle buttonLoadStyle;
	public GUIStyle buttonHelpStyle;
	public GUIStyle buttonExitStyle;
	public float buttonPadding;
	public AudioClip buttonSound;

	void OnEnable()
	{
	}

	void OnGUI()
	{
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), backgroundTexture);
		GUI.Label (new Rect (Screen.width * .22f, Screen.height * (buttonPadding - .225f), Screen.width * .58f, Screen.height * .15f), "", labelGameTitle);

		if (GUI.Button (new Rect (Screen.width * .35f, Screen.height * (buttonPadding), Screen.width * .3f, Screen.height * .13f), "", buttonNewStyle)) 
		{
			GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
			StartCoroutine(WaitFor(1));	// Load player name scene
		}
		
		if (GUI.Button (new Rect (Screen.width * .35f, Screen.height * (buttonPadding * 1.8f), Screen.width * .3f, Screen.height * .13f), "", buttonLoadStyle)) 
		{	
			GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
			StartCoroutine(WaitFor(2));	// Load player farm scene
		}
		
		if (GUI.Button (new Rect (Screen.width * .33f, Screen.height * (buttonPadding * 2.5f), Screen.width * .35f, Screen.height * .15f), "", buttonHelpStyle)) 
		{
			GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
			StartCoroutine(WaitFor(4));	// Load a help menu, info on game
		}

		if (GUI.Button (new Rect (Screen.width * .35f, Screen.height * (buttonPadding * 3.2f), Screen.width * .3f, Screen.height * .13f), "", buttonExitStyle)) 
		{
			GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
			StartCoroutine(WaitFor(10));	// Exit game
		}
	}

	void Update() 
	{
		if (Input.GetKeyDown (KeyCode.Escape)) 
		{
			GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
			StartCoroutine(WaitFor(10));
		}
	}

	IEnumerator WaitFor(int level) 
	{
		yield return new WaitForSeconds(1.0f);

		if (level == 10) 
		{
			Application.Quit ();	
		} 
		else 
		{
			Application.LoadLevel (level);
		}
	}
}