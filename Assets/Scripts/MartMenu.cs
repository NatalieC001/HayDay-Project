using UnityEngine;
using System.Collections;

public class MartMenu : MonoBehaviour 
{
	public Texture backgroundTexture;
	public GUIStyle labelGameTitle;
	public GUIStyle buttonBuyStyle;
	public GUIStyle buttonSellStyle;
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

		if (GUI.Button (new Rect (Screen.width * .35f, Screen.height * (buttonPadding + .01f), Screen.width * .3f, Screen.height * .13f), "", buttonBuyStyle)) 
		{
			GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
			//StartCoroutine(WaitFor(0));
		}
		
		if (GUI.Button (new Rect (Screen.width * .35f, Screen.height * (buttonPadding * 2), Screen.width * .3f, Screen.height * .13f), "", buttonSellStyle)) 
		{	
			GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
			//StartCoroutine(WaitFor(0));
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