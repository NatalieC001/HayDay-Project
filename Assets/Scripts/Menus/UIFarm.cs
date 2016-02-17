using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace HayDay
{
	public class UIFarm : MonoBehaviour
	{
		#region variables
		// GUI labels & text
		public GUIStyle buttonBuy;
		public GUIStyle buttonSell;
		public GUIStyle buttonInfo;
		public GUIStyle buttonLoad;
		public GUIStyle buttonSave;
		public GUIStyle buttonSupplies;
		public GUIStyle buttonMainMenu;
		public GUIStyle buttonMart;
		public GUIStyle buttonGrain;
		public GUIStyle buttonHay;
		public GUIStyle buttonPellets;
		public GUIStyle buttonX;
		public GUIStyle buttonExit;
		public GUIStyle buttonFeed;
		public GUIStyle labelAge;
		public GUIStyle labelBreed;
		public GUIStyle labelHappiness;
		public GUIStyle labelHealth; 
		public GUIStyle labelPregnant;
		public GUIStyle labelGender;
		public GUIStyle labelWeight;
		public GUIStyle labelLoading;
		public GUIStyle customTextStyle;

		// Cow default spawn info & location area
		private string cowGender = "Male";
		private string cowPregnant = "No";
		private Vector2 farmTopLeft = new Vector2(102f, 261f);
		private Vector2 farmBottomRight = new Vector2(57.2f, 242.2f);

		// GUI health & happiness bars
		private Image[] bars = new Image[2];

		// Images bars
		private Image healthBar;
		private Image happinessBar;

		public Color backgroundColor = new Color(0.2f, 0.3f, 0.4f, 0.5f);
		public Color foregroundColor = new Color(0.2f, 0.3f, 0.4f, 0.5f);

		// Audio variables
		private GameObject AudioObject;
		public AudioClip buttonSound;

		public Cow cow;
		public GameObject cowGameObject;

		// Menu, camera & loading control variables
		private Rect windowRect;
		private bool isLoading = false;
		private CameraController cameraControl;
		private VCAnalogJoystickBase joyStick;
		#endregion
		
	    void Start()
	    {
			// If new game, then give player new cows & store them in a list
			if(GameController.Instance().newGame)
			{
				GameController.Instance().cows.Clear();

				for(int i = 0;i < 2; i++)
				{
					// Generate cow instance, spawn cow with location variables
					Cow cow = CowMaker.GenerateCow();
					// Adding to list & cow not currently in mart bool set
					if(CowMaker.SpawnCow(cow, farmTopLeft, farmBottomRight,Vector3.zero) == 1)
						GameController.Instance().cows.Add(cow);
				}
				
				GameController.Instance().newGame = false;
			}
			else 
			{
				// Else load player data from file
				if(GameController.Instance().loadPlayer)
				{
					// Loading from file & spawning cows by looping through loaded list of cows
					GameController.Instance().Load();
					foreach (Cow cow in GameController.Instance().cows)
					{
						// Using location variables to spawn cows
						if(CowMaker.SpawnCow(cow, farmTopLeft, farmBottomRight,Vector3.zero) == 1)
							cow.cowController.Wait("Farm");
					}
				}
			}

			bars = GetComponentsInChildren<Image>();
			healthBar = bars[0];
			happinessBar = bars[1];

	        cameraControl = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();	
			joyStick = GameObject.Find("VCAnalogJoystick").GetComponent<VCAnalogJoystickBase>();
	    }

	    void OnGUI()
	    {
			GUI.color = foregroundColor;

			if (GameController.Instance().globalPlayerUI)
	        {
				windowRect = new Rect(Screen.width * .29f, Screen.height - 150f, 620, 100);
				windowRect = GUI.Window(0, windowRect, PlayerInfo, "Farm");
			}
			else if(GameController.Instance().globalSuppliesUI)
			{
				windowRect = new Rect(Screen.width * .29f, Screen.height - 250f, 620, 200);
				windowRect = GUI.Window(0, windowRect, PlayerSupplies, "Supplies");
			}
			else if (GameController.Instance().farmCowUI && cowGameObject != null )
	        {
				windowRect = new Rect(Screen.width * .29f, Screen.height - 150f, 620, 100);
				windowRect = GUI.Window(0, windowRect, CowInfo, cow.name);
			}
			else if (GameController.Instance().farmCowMoreInfoUI && cowGameObject != null )
			{
				windowRect = new Rect(Screen.width * .4f, Screen.height - 420f, 355, 365);
				windowRect = GUI.Window(0, windowRect, CowMoreInfo, cow.name);
			}
			else if (GameController.Instance().farmCowFeedUI && cowGameObject != null )
			{
				windowRect = new Rect(Screen.width * .32f, Screen.height - 350f, 555, 295);
				windowRect = GUI.Window(0, windowRect, CowFeed, cow.name);
			}
			else if (GameController.Instance().loadSaveUI)
			{
				windowRect = new Rect(Screen.width * .29f, Screen.height - 150f, 620, 100);
				windowRect = GUI.Window(0, windowRect, LoadSaveInfo, "Load / Save");
			}
			else if(GameController.Instance().farmSceneTransitionUI)
			{
				windowRect = new Rect(Screen.width * .29f, Screen.height - 150f, 620, 100);
				windowRect = GUI.Window(0, windowRect, SceneTransition, "Select");
			}
		}

	    private void PlayerInfo(int windowID)
	    {
			GUI.contentColor = backgroundColor;

			GUI.Label(new Rect(20, 25, 150, 30), GameController.Instance().player.name, customTextStyle);
			GUI.Label(new Rect(20, 60, 150, 30), "Cash: € " + GameController.Instance().player.cash, customTextStyle);

			SetHealth(0);
			SetHappiness(0);
			
			if (GUI.Button(new Rect(260, 33, 180, 53), "", buttonSupplies))
			{
				GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
				GameController.Instance().globalPlayerUI = false;
				GameController.Instance().globalSuppliesUI = true;
			}
	    }

		private void PlayerSupplies(int windowID)	// Finish Me!
		{
			GUI.contentColor = backgroundColor;
			
			SetHealth(0);
			SetHappiness(0);
			
			GUI.Label(new Rect(85, 30, 150, 30), "Grain", customTextStyle);
			GUI.Label(new Rect(245, 30, 150, 30), "Hay", customTextStyle);
			GUI.Label(new Rect(380, 30, 150, 30), "Pellets", customTextStyle);

			GUI.Label (new Rect (80, 70, 75, 75), "", buttonGrain);
			GUI.Label (new Rect (230, 70, 75, 75), "", buttonHay);
			GUI.Label (new Rect (380, 70, 75, 75), "", buttonPellets);

			GUI.Label(new Rect(110, 150, 150, 30), "" + GameController.Instance().player.grain, customTextStyle);
			GUI.Label(new Rect(260, 150, 150, 30), "" + GameController.Instance().player.hay, customTextStyle);
			GUI.Label(new Rect(410, 150, 150, 30), "" + GameController.Instance().player.pellet, customTextStyle);

			if (GUI.Button(new Rect(555, 135, 50, 50), "", buttonX))
			{
				GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
				GameController.Instance().globalPlayerUI = true;
				GameController.Instance().globalSuppliesUI = false;
			}
		}

		private void CowInfo(int windowID)
	    {
			GUI.contentColor = backgroundColor;

	        GUI.Label(new Rect(165, 25, 150, 30), "Health:", customTextStyle);
			GUI.Label(new Rect(165, 60, 150, 30), "Happiness:", customTextStyle);

			SetHealth(cow.health / 100f);
			SetHappiness(cow.happiness / 10f);

			healthBar.transform.position = new Vector2 (windowRect.center.x + 100, 110);
			happinessBar.transform.position = new Vector2 (windowRect.center.x + 100, 75);

			if (GUI.Button(new Rect(40, 30, 90, 50), "", buttonInfo))
			{
				GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
				GameController.Instance().farmCowUI = false;
				GameController.Instance().farmCowMoreInfoUI = true;
			}

			if (GUI.Button(new Rect(535, 30, 50, 50), "", buttonX))
			{
				GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
	            cameraControl.FollowPlayer();
				GameController.Instance().farmCowUI = false;
				GameController.Instance().cowSelected = false;
				StartCoroutine(WaitForCamera());
				SetHealth(0);
				SetHappiness(0);
			}
	    }

		private void CowMoreInfo(int windowID)
		{
			GUI.contentColor = backgroundColor;

			SetHealth(cow.health / 100f);
			SetHappiness(cow.happiness / 10f);

			healthBar.transform.position = new Vector2 (windowRect.center.x + 70, 300);
			happinessBar.transform.position = new Vector2 (windowRect.center.x + 70, 265);
			
			if (!cow.gender == true)
			{
				cowGender = "Female";

				if(cow.pregnant == true)
					cowPregnant = "Yes";
			}
			else
			{
				cowGender = "Male";
				cow.pregnant = false;
				cowPregnant = "No";
			}

			GUI.Label(new Rect(35, 40, 80, 25), "", labelAge);
			GUI.Label(new Rect(142, 40, 150, 30), "" + cow.age, customTextStyle);
			GUI.Label(new Rect(35, 72, 90, 25), "", labelBreed);
			GUI.Label(new Rect(142, 72, 150, 30), "" + cowGender, customTextStyle);
			GUI.Label(new Rect(35, 105, 110, 30), "", labelHappiness);
			GUI.Label(new Rect(35, 140, 90, 25), "", labelHealth);
			GUI.Label(new Rect(35, 175, 110, 30), "", labelPregnant);
			GUI.Label(new Rect(160, 175, 150, 30), "" + cowPregnant, customTextStyle);
			GUI.Label(new Rect(35, 210, 90, 25), "", labelGender);
			GUI.Label(new Rect(142, 210, 150, 30), "" + cowGender, customTextStyle);
			GUI.Label(new Rect(35, 245, 90, 30), "", labelWeight);
			GUI.Label(new Rect(142, 245, 150, 30), "" + cow.weight + " KG", customTextStyle);

			if (GUI.Button(new Rect(38, 300, 80, 40), "", buttonFeed))
			{
				GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
				GameController.Instance().farmCowMoreInfoUI = false;
				GameController.Instance().farmCowFeedUI = true;
			}

			if (GUI.Button(new Rect(285, 292, 50, 50), "", buttonX))
			{
				GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
				GameController.Instance().farmCowMoreInfoUI = false;
				GameController.Instance().farmCowUI = true;
			}
		}

		private void CowFeed(int windowID)	// Finish Me!
		{
			GUI.contentColor = backgroundColor;
			
			SetHealth(cow.health / 100f);
			SetHappiness(cow.happiness / 10f);

			GUI.Label(new Rect(30, 215, 110, 30), "", labelHappiness);
			GUI.Label(new Rect(30, 250, 90, 25), "", labelHealth);

			healthBar.transform.position = new Vector2 (windowRect.center.x - 30, 120);
			happinessBar.transform.position = new Vector2 (windowRect.center.x - 30, 85);

			GUI.Label(new Rect(85, 40, 150, 30), "Grain", customTextStyle);
			GUI.Label(new Rect(245, 40, 150, 30), "Hay", customTextStyle);
			GUI.Label(new Rect(380, 40, 150, 30), "Pellets", customTextStyle);
			
			if (GUI.Button(new Rect(80, 75, 75, 75), "", buttonGrain))
			{
				GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);

				if(GameController.Instance().player.grain >= 1)
				{
					GameController.Instance().player.grain--;
					FeedCow(1);
				}
			}
			
			if (GUI.Button(new Rect(230, 75, 75, 75), "", buttonHay))
			{
				GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);

				if(GameController.Instance().player.hay >= 1)
				{
					GameController.Instance().player.hay--;
					FeedCow(2);
				}
			}
			
			if (GUI.Button(new Rect(380, 75, 75, 75), "", buttonPellets))
			{
				GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);

				if(GameController.Instance().player.pellet >= 1)
				{
					GameController.Instance().player.pellet--;
					FeedCow(3);
				}
			}
			
			GUI.Label(new Rect(110, 160, 150, 30), "" + GameController.Instance().player.grain, customTextStyle);
			GUI.Label(new Rect(260, 160, 150, 30), "" + GameController.Instance().player.hay, customTextStyle);
			GUI.Label(new Rect(410, 160, 150, 30), "" + GameController.Instance().player.pellet, customTextStyle);
			
			if (GUI.Button(new Rect(475, 230, 50, 50), "", buttonX))
			{
				GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
				GameController.Instance().farmCowMoreInfoUI = true;
				GameController.Instance().farmCowFeedUI = false;
			}
		}

		private void SceneTransition(int windowID)
		{
			GUI.contentColor = backgroundColor;
			
			SetHealth(0);
			SetHappiness(0);

			if(isLoading)
				GUI.Label(new Rect(200, 32, 270, 50), "", labelLoading);

			if (!isLoading) 
			{
				if (GUI.Button (new Rect (50, 32, 270, 50), "", buttonMainMenu)) 
				{
					GetComponent<AudioSource> ().PlayOneShot (buttonSound, 0.7f);
					GameController.Instance().Save();
					isLoading = true;
					StartCoroutine(WaitFor(0));
				}
			}
			
			if(!isLoading)
			{
				if (GUI.Button(new Rect(410, 34, 130, 48), "", buttonMart))
				{
					GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
					GameController.Instance().Save();
					isLoading = true;
					StartCoroutine(WaitFor(4));
				}
			}
		}

		private void LoadSaveInfo(int windowID)
		{
			SetHealth(0);
			SetHappiness(0);

			GUI.contentColor = backgroundColor;
			
			if (GUI.Button(new Rect(50, 30, 150, 50), "", buttonLoad))
			{
				GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
				GameController.Instance().Load();
			}
			
			if (GUI.Button(new Rect(275, 30, 150, 50), "", buttonSave))
			{
				GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
				GameController.Instance().Save();
			}
		}

		private void SetHealth(float health)
		{
			healthBar.fillAmount = health;
		}

		private void SetHappiness(float happiness)
		{
			happinessBar.fillAmount = happiness;
		}

		private void FeedCow(int foodType)
		{
			int feedAmount;

			switch(foodType)
			{
				case 1:
					feedAmount = Random.Range(1, 2);
					break;
				case 2:
					feedAmount = Random.Range(1, 3);
					break;
				case 3:
					feedAmount = Random.Range(2, 6);
					break;
				default:
					feedAmount = Random.Range(2, 6);
					break;
			}
			
			switch(feedAmount)
			{
				case 1:
					cow.happiness = cow.happiness + 1;
					cow.health = cow.health + 5;
					break;
				case 2:
					cow.happiness = cow.happiness + 1;
					cow.health = cow.health + 8;
					break;
				case 3:
					cow.happiness = cow.happiness + 2;
					cow.health = cow.health + 11;
					break;
				case 4:
					cow.happiness = cow.happiness + 3;
					cow.health = cow.health + 14;
					break;
				case 5:
					cow.happiness = cow.happiness + 4;
					cow.health = cow.health + 17;
					break;
				case 6:
					cow.happiness = cow.happiness + 4;
					cow.health = cow.health + 20;
					break;
				default:
					cow.happiness = cow.happiness + 4;
					cow.health = cow.health + 20;
					break;
			}
			
			if(cow.happiness > 10)
				cow.happiness = 10;
			
			if(cow.health > 100)
				cow.health = 100;
		}

		private IEnumerator WaitForCamera()
		{
			while(cameraControl.transitioning)
			{
				yield return new WaitForSeconds(0.2f);
			}
			joyStick.gameObject.SetActive(true);
			Movement.freeRoam = true;
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
				GameController.Instance().ResetMenus();
				Application.LoadLevel (level);
			}
		}
	}
}