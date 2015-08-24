using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour 
{
	public Texture backgroundTexture;
	public Texture backgroundLoading;
	public GUIStyle labelGameTitle;
	public GUIStyle buttonNewStyle;
	public GUIStyle buttonLoadStyle;
	public GUIStyle buttonHelpStyle;
	public GUIStyle buttonExitStyle;
	public float buttonPadding;
	public AudioClip buttonSound;

	bool isLoading = false;

	void Start()
	{
		//isLoading = false;
	}

	void OnGUI()
	{
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), backgroundTexture);

		if(!isLoading)
			GUI.Label (new Rect (Screen.width * .23f, Screen.height * (buttonPadding - .225f), Screen.width * .58f, Screen.height * .15f), "", labelGameTitle);

		if(!isLoading)
			if (GUI.Button (new Rect (Screen.width * .28f, Screen.height * (buttonPadding * 0.8f), Screen.width * .45f, Screen.height * .17f), "", buttonNewStyle)) 
			{
				GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
				StartCoroutine(WaitFor(1));	// Load player name scene
			}

		if(!isLoading)
			if (GUI.Button (new Rect (Screen.width * .28f, Screen.height * (buttonPadding * 1.64f), Screen.width * .45f, Screen.height * .17f), "", buttonLoadStyle)) 
			{	
				GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
				StartCoroutine(WaitFor(2));	// Load player farm scene
				isLoading = true;
				backgroundTexture = backgroundLoading;
			}

		if(!isLoading)
			if (GUI.Button (new Rect (Screen.width * .37f, Screen.height * (buttonPadding * 2.49f), Screen.width * .25f, Screen.height * .16f), "", buttonHelpStyle)) 
			{
				GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
				StartCoroutine(WaitFor(4));	// Load a help menu, info on game
			}

		if(!isLoading)
			if (GUI.Button (new Rect (Screen.width * .37f, Screen.height * (buttonPadding * 3.25f), Screen.width * .25f, Screen.height * .16f), "", buttonExitStyle)) 
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