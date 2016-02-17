using UnityEngine;
using System.Collections;
using System.IO;

namespace HayDay
{
	public class MainMenu : MonoBehaviour
	{
		// GUI labels & text
		public Texture backgroundTexture;
		public Texture backgroundLoading;
		public Texture backgroundNoGame;
		public GUIStyle labelGameTitle;
		public GUIStyle buttonNewStyle;
		public GUIStyle buttonLoadStyle;
		public GUIStyle buttonExitStyle;
		public float buttonPadding;
		public AudioClip buttonSound;

		private bool isLoading = false;
		private Texture backgroundTemp;

		void Start()
		{
			checkGCExists("GameController");
		}

		void Awake()
		{
			checkGCExists("GameController");
		}

		void OnGUI()
		{
			GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), backgroundTexture);

			if(!isLoading)
				GUI.Label (new Rect (Screen.width * .23f, Screen.height * (buttonPadding - .225f), Screen.width * .58f, Screen.height * .16f), "", labelGameTitle);

			if(!isLoading)
			{
				if (GUI.Button (new Rect (Screen.width * .28f, Screen.height * (buttonPadding * 1.05f), Screen.width * .45f, Screen.height * .15f), "", buttonNewStyle)) 
				{
					GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
					StartCoroutine(WaitFor(1));	// Load player name scene
				}
			}

			if(!isLoading)
			{
				if (GUI.Button (new Rect (Screen.width * .28f, Screen.height * (buttonPadding * 1.92f), Screen.width * .45f, Screen.height * .15f), "", buttonLoadStyle)) 
				{	
					GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
					GameController.Instance().loadPlayer = true;

					bool fileTest1 = File.Exists(Application.persistentDataPath + "/player.dat");
					bool fileTest2 = File.Exists(Application.persistentDataPath + "/cows.dat");

					if (fileTest1 || fileTest2)
					{
						StartCoroutine(WaitFor(2));		// Load player farm scene
						isLoading = true;
						backgroundTexture = backgroundLoading;
					}
					else
					{
						StartCoroutine(WaitForSec(3));
						isLoading = true;
						backgroundTemp = backgroundTexture;
						backgroundTexture = backgroundNoGame;
					}
				}
			}

			if(!isLoading)
			{
				if (GUI.Button (new Rect (Screen.width * .37f, Screen.height * (buttonPadding * 2.9f), Screen.width * .225f, Screen.height * .145f), "", buttonExitStyle)) 
				{
					GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
					StartCoroutine(WaitFor(10));	// Exit game
				}
			}
		}

		void Update() 
		{
			if (Input.GetKeyDown (KeyCode.Escape)) 
			{
				GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
				StartCoroutine(WaitFor(10));	// Exit game
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

		private IEnumerator WaitForSec(int seconds) 
		{
			yield return new WaitForSeconds(seconds);
			isLoading = false;
			backgroundTexture = backgroundTemp;
		}

		private void checkGCExists(string gcName)
		{
			if(GameObject.Find(gcName) == null)
			{
				GameObject cowGameObject = Instantiate(Resources.Load(gcName) as GameObject);
				cowGameObject.name = gcName;
			}
		}
	}
}