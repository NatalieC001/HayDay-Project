using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI : GameController 
{
    public bool playerUI;
    public bool cowUI;

	public GUIStyle buttonBuy;
	public GUIStyle buttonSell;
	public GUIStyle buttonCattle;
	public GUIStyle customTextStyle;

    public Cow cow;
    public GameObject cowGameObject;

	public Image healthBar;
	public Image happinessBar;

	public Color backgroundColor = new Color(0.2f, 0.3f, 0.4f, 0.5f);
	public Color foregroundColor = new Color(0.2f, 0.3f, 0.4f, 0.5f);

	Rect windowRect;

    void Start()
    {
        playerUI = true;
		//healthBar = GetComponent<Image>();
		//happinessBar = GetComponent<Image>();
    }

    void OnGUI()
    {
		windowRect = new Rect(Screen.width * .29f, Screen.height * .75f, 500, 100); 

		GUI.color = foregroundColor;
		
        if (playerUI)
        {
			windowRect = GUI.Window(0, windowRect, PlayerInfo, "");
		}
		else if (cowUI && cowGameObject != null )
        {
			windowRect = GUI.Window(0, windowRect, CowInfo, cow.name);
		}
	}

    void PlayerInfo(int windowID)
    {
		GUI.contentColor = backgroundColor;

		GUI.Label(new Rect(20, 25, 150, 30), game.player.name, customTextStyle);
        GUI.Label(new Rect(20, 60, 150, 30), "Cash: €" + game.player.cash, customTextStyle);

        if (!cow.ownedByPlayer)
			if (GUI.Button(new Rect(210, 30, 150, 50), "", buttonCattle))
				CowMaker.generateCow();

        /*if (GUI.Button(new Rect(200, 23, 60, 30), "Load"))
            game.Load();

        if (GUI.Button(new Rect(275, 23, 60, 30), "Save"))
            game.Save();*/
    }

    void CowInfo(int windowID)
    {
		GUI.contentColor = backgroundColor;

        GUI.Label(new Rect(165, 25, 150, 30), "Health:", customTextStyle);
		GUI.Label(new Rect(165, 60, 150, 30), "Happiness:", customTextStyle);

		//healthBar.fillAmount = cow.health / 100f;
		//happinessBar.fillAmount = cow.happiness / 10f;

		HealthBar.SetHealth(cow.health / 100f);
		HappinessBar.SetHappiness(cow.happiness / 10f);
		
		//healthBar.transform.position = new Vector2 (185, 25);
		//happinessBar.transform.position = new Vector2 (185, 60);

        if (!cow.ownedByPlayer)
			if (GUI.Button(new Rect(50, 30, 80, 50), "", buttonBuy))
				cow.Buy();
		
        if (cow.ownedByPlayer)
            if (GUI.Button(new Rect(50, 30, 80, 50), "", buttonSell))
            {
                cow.Sell();
                Destroy(cowGameObject);
                cow.gameObjectID = 0;
                playerUI = true;
                cowUI = false;
            }
    }
}