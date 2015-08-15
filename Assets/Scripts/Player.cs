using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
	public Texture backgroundTexture;
	public GUIStyle labelPlayer;
	public GUIStyle buttonPlayStyle;
	public GUIStyle buttonBackStyle;
	public float buttonPadding;
	public AudioClip buttonSound;
	public GUIStyle textFieldStyle;
	public string playerName = "Joe";

	void OnEnable()
	{
	}

	void OnGUI()
	{
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), backgroundTexture);
		GUI.Label (new Rect (Screen.width * .262f, Screen.height * .05f, Screen.width * .5f, Screen.height * .18f), "", labelPlayer);
		GUI.SetNextControlName ("PlayerInput");
		playerName = GUI.TextField(new Rect(0, Screen.height * .40f, Screen.width, Screen.height * .22f), playerName, textFieldStyle);

		if (GUI.Button (new Rect (Screen.width * .7f, Screen.height * .8f, Screen.width * .275f, Screen.height * .180f), "", buttonPlayStyle))
		{
			GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
			//GameController.playerName = playerName;
			StartCoroutine(WaitFor(2));	// Load farm scene
		}

		if (GUI.Button (new Rect (Screen.width * .035f, Screen.height * .79f, Screen.width * .265f, Screen.height * .170f), "", buttonBackStyle))
		{
			GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
			StartCoroutine(WaitFor(0));	// Back to main menu
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