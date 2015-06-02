using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour 
{
	public Texture backgroundTexture;
	public GUIStyle labelGameTitle;
	public GUIStyle buttonFarmStyle;
	public GUIStyle buttonMartStyle;
	public GUIStyle buttonSettingsStyle;
	public GUIStyle buttonHelpStyle;
	public float buttonPadding;
	public AudioClip buttonSound;

	void OnEnable()
	{
		//GlobalVariables.scoresChecked = false;
	}

	void OnGUI()
	{
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), backgroundTexture);
		GUI.Label (new Rect (Screen.width * .22f, Screen.height * (buttonPadding - .225f), Screen.width * .58f, Screen.height * .15f), "", labelGameTitle);

		if (GUI.Button (new Rect (Screen.width * .35f, Screen.height * (buttonPadding), Screen.width * .3f, Screen.height * .13f), "", buttonFarmStyle)) 
		{
			GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
			StartCoroutine(WaitFor(1));
		}
		
		if (GUI.Button (new Rect (Screen.width * .35f, Screen.height * (buttonPadding * 1.8f), Screen.width * .3f, Screen.height * .13f), "", buttonMartStyle)) 
		{	
			GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
			StartCoroutine(WaitFor(2));
		}
		
		if (GUI.Button (new Rect (Screen.width * .33f, Screen.height * (buttonPadding * 2.5f), Screen.width * .35f, Screen.height * .15f), "", buttonSettingsStyle)) 
		{
			GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
			StartCoroutine(WaitFor(3));
		}

		if (GUI.Button (new Rect (Screen.width * .35f, Screen.height * (buttonPadding * 3.2f), Screen.width * .3f, Screen.height * .13f), "", buttonHelpStyle)) 
		{
			GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
			StartCoroutine(WaitFor(4));
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