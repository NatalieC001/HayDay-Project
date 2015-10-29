using UnityEngine;
using System.Collections;

public class Help : MonoBehaviour
{
	public Texture backgroundTexture;
	public Texture backgroundLoading;
	public GUIStyle labelGameTitle;
	public GUIStyle buttonBackStyle;
	public GUIStyle customTextStyle;
	public float buttonPadding;
	public AudioClip buttonSound;

	private bool isLoading = false;

	void OnGUI()
	{
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), backgroundTexture);

		GUI.Label (new Rect (Screen.width * .072f, Screen.height * .22f, Screen.width * .8f, Screen.height * .16f), "1: Buy supplies to feed your cattle to improve their stats, health and happiness!", customTextStyle);
		GUI.Label (new Rect (Screen.width * .072f, Screen.height * .45f, Screen.width * .8f, Screen.height * .16f), "2: Tap on your cattle to view stats and feed them!", customTextStyle);
		GUI.Label (new Rect (Screen.width * .072f, Screen.height * .63f, Screen.width * .8f, Screen.height * .16f), "3: Make some profit at the market place!", customTextStyle);

		if(!isLoading)
			GUI.Label (new Rect (Screen.width * .23f, Screen.height * (buttonPadding - .225f), Screen.width * .58f, Screen.height * .16f), "", labelGameTitle);

		if(!isLoading)
		{
			if (GUI.Button (new Rect (Screen.width * .035f, Screen.height * .79f, Screen.width * .240f, Screen.height * .145f), "", buttonBackStyle)) 
			{
				GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
				StartCoroutine(WaitFor(0));	// Exit game
			}
		}
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