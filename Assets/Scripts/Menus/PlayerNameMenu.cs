using UnityEngine;
using System.Collections;

namespace HayDay
{
	public class PlayerNameMenu : MonoBehaviour 
	{
		public Texture backgroundTexture;
		public Texture backgroundLoading;
		public GUIStyle labelPlayer;
		public GUIStyle buttonPlayStyle;
		public GUIStyle buttonBackStyle;
		public GUIStyle textFieldStyle;
		public AudioClip buttonSound;

		private string playerName = "Joe";
		private bool isLoading = false;

		void OnGUI()
		{
			GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), backgroundTexture);

			if(!isLoading)
			{
				GUI.Label (new Rect (Screen.width * .272f, Screen.height * .05f, Screen.width * .45f, Screen.height * .16f), "", labelPlayer);
				GUI.SetNextControlName ("PlayerInput");
				// Getting player name from text field
				playerName = GUI.TextField(new Rect(0, Screen.height * .40f, Screen.width, Screen.height * .22f), playerName, textFieldStyle);
			}

			if(!isLoading)
			{
				if (GUI.Button (new Rect (Screen.width * .725f, Screen.height * .81f, Screen.width * .24f, Screen.height * .16f), "", buttonPlayStyle))
				{
					// Now going to load new scene with new player data
					isLoading = true;
					GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
					GameController._instance.player = new Farmer ("Farmer", 25000, 0, 0, 0);
					GameController.Instance().player.name = playerName;
					GameController.Instance().newGame = true;
					StartCoroutine(WaitFor(3));
					backgroundTexture = backgroundLoading;
				}
			}

			if(!isLoading)
			{
				if (GUI.Button (new Rect (Screen.width * .035f, Screen.height * .82f, Screen.width * .240f, Screen.height * .13f), "", buttonBackStyle))
				{
					// Now loading menu scene
					GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
					StartCoroutine(WaitFor(0));
				}
			}
		}

		void Update() 
		{
			if (Input.GetKeyDown (KeyCode.Escape)) 
			{
				// If esc key pressed, then load menu scene 
				GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
				StartCoroutine(WaitFor(0));
			}
		}

		private IEnumerator WaitFor(int level) 
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
}