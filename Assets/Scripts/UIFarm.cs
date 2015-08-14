using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIFarm : GameController 
{
    public bool playerUI;
    public bool cowUI;
	public bool loadSaveUI;

	public GUIStyle buttonBuy;
	public GUIStyle buttonSell;
	public GUIStyle buttonLoad;
	public GUIStyle buttonSave;
	public GUIStyle buttonCattle;
	public GUIStyle buttonExit;
	public GUIStyle customTextStyle;

    public Cow cow;
    public GameObject cowGameObject;
	
	public static Image healthBar;
	public static Image happinessBar;

	public Color backgroundColor = new Color(0.2f, 0.3f, 0.4f, 0.5f);
	public Color foregroundColor = new Color(0.2f, 0.3f, 0.4f, 0.5f);

	public Image[] bars = new Image[2];

	public GameObject AudioObject;

	Rect windowRect;

    void Start()
    {
        playerUI = true;
		//loadSaveUI = true;
		//bars = new Image[2];
		bars = GetComponentsInChildren<Image>();
		healthBar = bars[0];
		happinessBar = bars[1];

		AudioObject = GameObject.Find("Background Music");
		Destroy(AudioObject);
    }

    void OnGUI()
    {
		windowRect = new Rect(Screen.width * .29f, Screen.height - 150f, 620, 100);

		GUI.color = foregroundColor;
		
        if (playerUI)
        {
			windowRect = GUI.Window(0, windowRect, PlayerInfo, "Farm");
		}
		else if (cowUI && cowGameObject != null )
        {
			windowRect = GUI.Window(0, windowRect, CowInfo, cow.name);
		}
		else if (loadSaveUI)
		{
			windowRect = GUI.Window(0, windowRect, LoadSaveInfo, "Load / Save");
		}
	}

    void PlayerInfo(int windowID)
    {
		GUI.contentColor = backgroundColor;

		GUI.Label(new Rect(20, 25, 150, 30), game.player.name, customTextStyle);
        GUI.Label(new Rect(20, 60, 150, 30), "Cash: € " + game.player.cash, customTextStyle);
		
		if (GUI.Button(new Rect(270, 30, 150, 50), "", buttonCattle))
		{
			Vector3 spawnLocation = new Vector3(Random.Range(50f, 100f), 0, Random.Range(223f, 263f));
			CowMaker.GenerateCow(spawnLocation);
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

        if (!cow.ownedByPlayer)
			if (GUI.Button(new Rect(50, 30, 80, 50), "", buttonBuy))
				cow.Buy();
		
        if (cow.ownedByPlayer)
		{
            if (GUI.Button(new Rect(50, 30, 80, 50), "", buttonSell))
            {
                cow.Sell();
                Destroy(cowGameObject);
                cow.gameObjectID = 0;
                playerUI = true;
                cowUI = false;
            }
		}

		if (GUI.Button(new Rect(520, 30, 68, 50), "", buttonExit))
		{
			CameraController.ResetCamera(GameObject.Find("Main Camera").transform.position);
			playerUI = true;
			cowUI = false;
			Movement.freeRoam = true;
		}
    }

	void LoadSaveInfo(int windowID)
	{
		GUI.contentColor = backgroundColor;
		
		if (GUI.Button(new Rect(50, 30, 150, 50), "", buttonLoad))
			game.Load();
		
		if (GUI.Button(new Rect(275, 30, 150, 50), "", buttonSave))
			game.Save();
	}

	public static void SetHealth(float health)
	{
		healthBar.fillAmount = health;
	}

	public static void SetHappiness(float happiness)
	{
		happinessBar.fillAmount = happiness;
	}
}