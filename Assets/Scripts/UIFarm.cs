using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIFarm : GameController 
{
	#region variables
	public GUIStyle buttonBuy;
	public GUIStyle buttonSell;
	public GUIStyle buttonInfo;
	public GUIStyle buttonLoad;
	public GUIStyle buttonSave;
	public GUIStyle buttonSupplies;
	public GUIStyle buttonMainMenu;
	public GUIStyle buttonMart;
	public GUIStyle buttonX;
	public GUIStyle buttonExit;
	public GUIStyle buttonFeed;
	public GUIStyle buttonGrain;
	public GUIStyle buttonHay;
	public GUIStyle buttonPellets;
	public GUIStyle labelAge;
	public GUIStyle labelBreed;
	public GUIStyle labelHappiness;
	public GUIStyle labelHealth; 
	public GUIStyle labelPregnant;
	public GUIStyle labelGender;
	public GUIStyle labelWeight;
	public GUIStyle labelLoading;
	public GUIStyle customTextStyle;

	private string cowGender = "Male";
	private string cowPregnant = "No";
	
	public Image healthBar;
	public Image happinessBar;

	public Color backgroundColor = new Color(0.2f, 0.3f, 0.4f, 0.5f);
	public Color foregroundColor = new Color(0.2f, 0.3f, 0.4f, 0.5f);

	public Image[] bars = new Image[2];
	public GameObject AudioObject;
	public AudioClip buttonSound;
	public Cow cow;

	private Rect windowRect;
	private bool isLoading = false;
	private CameraController cameraControl;
	private VCAnalogJoystickBase joyStick;

	private Vector2 farmTopLeft = new Vector2(102f, 261f);
	private Vector2 farmBottomRight = new Vector2(57.2f, 242.2f);
	#endregion
	
    void Start()
    {
		if (!init) 
		{
			game = this;
			game.player.name = playerName;
			game.player.cash = 50000;
			game.player.grain = 0;
			game.player.hay = 0;
			game.player.pellet = 0;
			init = true;
		}

		bars = GetComponentsInChildren<Image>();
		healthBar = bars[0];
		happinessBar = bars[1];
        cameraControl = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        joyStick = GameObject.FindGameObjectWithTag("JoyStick").GetComponent<VCAnalogJoystickBase>();

		/*if(newGame)
		{
			for(int i = 0;i < 10; i++)
			{
				cow = CowMaker.GenerateCow();
				CowMaker.SpawnCow(cow,farmTopLeft, farmBottomRight,Vector3.zero);
				game.cows.Add(cow);
			}

			newGame = false;
		}
		else 
		{
			if(GameController.loadPlayer)
			{
				Load();
				foreach (Cow cow in game.cows)
                    CowMaker.SpawnCow(cow, farmTopLeft, farmBottomRight, Vector3.zero);
			}
		}*/

		cow = CowMaker.GenerateCow();
		CowMaker.SpawnCow(cow,farmTopLeft, farmBottomRight,Vector3.zero);
		game.cows.Add(cow);
    }

    void OnGUI()
    {
		GUI.color = foregroundColor;

		if (GlobalVars.playerUI)
        {
			windowRect = new Rect(Screen.width * .29f, Screen.height - 150f, 620, 100);
			windowRect = GUI.Window(0, windowRect, PlayerInfo, "Farm");
		}
		else if(GlobalVars.buySuppliesUI)
		{
			windowRect = new Rect(Screen.width * .29f, Screen.height - 300f, 620, 250);
			windowRect = GUI.Window(0, windowRect, BuyPlayerSupplies, "Buy Supplies");
		}
		else if (GlobalVars.cowUI && cowGameObject != null )
        {
			windowRect = new Rect(Screen.width * .29f, Screen.height - 150f, 620, 100);
			windowRect = GUI.Window(0, windowRect, CowInfo, cow.name);
		}
		else if (GlobalVars.cowMoreInfoUI && cowGameObject != null )
		{
			windowRect = new Rect(Screen.width * .4f, Screen.height - 420f, 355, 365);
			windowRect = GUI.Window(0, windowRect, CowMoreInfo, cow.name);
		}
		else if (GlobalVars.cowFeedUI && cowGameObject != null )
		{
			windowRect = new Rect(Screen.width * .32f, Screen.height - 350f, 555, 295);
			windowRect = GUI.Window(0, windowRect, CowFeed, cow.name);
		}
		else if (GlobalVars.loadSaveUI)
		{
			windowRect = new Rect(Screen.width * .29f, Screen.height - 150f, 620, 100);
			windowRect = GUI.Window(0, windowRect, LoadSaveInfo, "Load / Save");
		}
		else if(GlobalVars.sceneTransitionUI)
		{
			windowRect = new Rect(Screen.width * .29f, Screen.height - 150f, 620, 100);
			windowRect = GUI.Window(0, windowRect, SceneTransition, "Select");
		}
	}

    void PlayerInfo(int windowID)
    {
		GUI.contentColor = backgroundColor;

		GUI.Label(new Rect(20, 25, 150, 30), game.player.name, customTextStyle);
        GUI.Label(new Rect(20, 60, 150, 30), "Cash: € " + game.player.cash, customTextStyle);

		SetHealth(0);
		SetHappiness(0);
		
		if (GUI.Button(new Rect(260, 33, 180, 53), "", buttonSupplies))
		{
			GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
			GlobalVars.playerUI = false;
			GlobalVars.buySuppliesUI = true;
		}
    }

	void BuyPlayerSupplies(int windowID)	// Finish Me!
	{
		GUI.contentColor = backgroundColor;
		
		SetHealth(0);
		SetHappiness(0);
		
		GUI.Label(new Rect(85, 30, 150, 30), "Grain", customTextStyle);
		GUI.Label(new Rect(245, 30, 150, 30), "Hay", customTextStyle);
		GUI.Label(new Rect(380, 30, 150, 30), "Pellets", customTextStyle);
		GUI.Label(new Rect(85, 70, 150, 30), "€1000", customTextStyle);
		GUI.Label(new Rect(230, 70, 150, 30), "€1500", customTextStyle);
		GUI.Label(new Rect(385, 70, 150, 30), "€2500", customTextStyle);

		if (GUI.Button(new Rect(80, 105, 75, 75), "", buttonGrain))	// Add to player class / inventory
		{
			if(game.player.cash >= 1000)
			{
				GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
				game.player.grain++;
				game.player.cash = game.player.cash - 1000;
			}
		}

		if (GUI.Button(new Rect(230, 105, 75, 75), "", buttonHay))		// Add to player class / inventory
		{
			if(game.player.cash >= 1500)
			{
				GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
				game.player.hay++;
				game.player.cash = game.player.cash - 1500;
			}
		}

		if (GUI.Button(new Rect(380, 105, 75, 75), "", buttonPellets))	// Add to player class / inventory
		{
			if(game.player.cash >= 2500)
			{
				GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
				game.player.pellet++;
				game.player.cash = game.player.cash - 2500;
			}
		}

		GUI.Label(new Rect(110, 190, 150, 30), "" + game.player.grain, customTextStyle);
		GUI.Label(new Rect(260, 190, 150, 30), "" + game.player.hay, customTextStyle);
		GUI.Label(new Rect(410, 190, 150, 30), "" + game.player.pellet, customTextStyle);

		if (GUI.Button(new Rect(535, 180, 50, 50), "", buttonX))
		{
			GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
			GlobalVars.playerUI = true;
			GlobalVars.buySuppliesUI = false;
		}
	}

    void CowInfo(int windowID)
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
			GlobalVars.cowUI = false;
			GlobalVars.cowMoreInfoUI = true;
		}

		if (GUI.Button(new Rect(535, 30, 50, 50), "", buttonX))
		{
			GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
       
            cameraControl.FollowPlayer();
			GlobalVars.cowUI = false;
			GlobalVars.cowSelected = false;
			StartCoroutine(waitForCamera());
			SetHealth(0);
			SetHappiness(0);
		}
    }

    IEnumerator waitForCamera()
	{
		while(cameraControl.transitioning)
		{
			yield return new WaitForSeconds(0.2f);
		}
		joyStick.gameObject.SetActive(true);
		Movement.freeRoam = true;
	}
	
	void CowMoreInfo(int windowID)
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
		GUI.Label(new Rect(142, 245, 150, 30), "" + cow.weight, customTextStyle);

		if (GUI.Button(new Rect(38, 300, 80, 40), "", buttonFeed))
		{
			GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
			GlobalVars.cowMoreInfoUI = false;
			GlobalVars.cowFeedUI = true;
		}

		if (GUI.Button(new Rect(285, 292, 50, 50), "", buttonX))
		{
			GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
			GlobalVars.cowMoreInfoUI = false;
			GlobalVars.cowUI = true;
		}
	}

	void CowFeed(int windowID)	// Finish Me!
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

			if(game.player.grain >= 1)
			{
				game.player.grain--;
				FeedCow(1);
			}
		}
		
		if (GUI.Button(new Rect(230, 75, 75, 75), "", buttonHay))
		{
			GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);

			if(game.player.hay >= 1)
			{
				game.player.hay--;
				FeedCow(2);
			}
		}
		
		if (GUI.Button(new Rect(380, 75, 75, 75), "", buttonPellets))
		{
			GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);

			if(game.player.pellet >= 1)
			{
				game.player.pellet--;
				FeedCow(3);
			}
		}
		
		GUI.Label(new Rect(110, 160, 150, 30), "" + game.player.grain, customTextStyle);
		GUI.Label(new Rect(260, 160, 150, 30), "" + game.player.hay, customTextStyle);
		GUI.Label(new Rect(410, 160, 150, 30), "" + game.player.pellet, customTextStyle);
		
		if (GUI.Button(new Rect(475, 230, 50, 50), "", buttonX))
		{
			GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
			GlobalVars.cowMoreInfoUI = true;
			GlobalVars.cowFeedUI = false;
		}
	}

	void SceneTransition(int windowID)
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
				Save ();
				isLoading = true;
				StartCoroutine (WaitFor (0));
			}
		}
		
		if(!isLoading)
		{
			if (GUI.Button(new Rect(410, 34, 130, 48), "", buttonMart))
			{
				GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
				Save();
				isLoading = true;
				StartCoroutine(WaitFor(4));
			}
		}
	}

	void LoadSaveInfo(int windowID)
	{
		SetHealth(0);
		SetHappiness(0);

		GUI.contentColor = backgroundColor;
		
		if (GUI.Button(new Rect(50, 30, 150, 50), "", buttonLoad))
		{
			GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
			Load();
		}
		
		if (GUI.Button(new Rect(275, 30, 150, 50), "", buttonSave))
		{
			GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
			Save();
		}
	}

	void SetHealth(float health)
	{
		healthBar.fillAmount = health;
	}

	void SetHappiness(float happiness)
	{
		happinessBar.fillAmount = happiness;
	}

	private void FeedCow(int foodType)
	{
		int feedAmount;

		if(foodType == 1)
		{
			feedAmount = Random.Range(1, 2);
		}
		else if(foodType == 2)
		{
			feedAmount = Random.Range(1, 4);
		}
		else if(foodType == 3)
		{
			feedAmount = Random.Range(1, 6);
		}
		else
		{
			feedAmount = Random.Range(1, 6);
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
		}
		
		if(cow.happiness > 10)
			cow.happiness = 10;
		
		if(cow.health > 100)
			cow.health = 100;
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
			GlobalVars.sceneTransitionUI = false;
		}
	}
}