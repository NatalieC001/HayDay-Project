using UnityEngine;
using System.Collections;

public class UI : GameController {

    public bool playerUI;
    public bool cowUI;

    public Cow cow;
    public GameObject cowGameObject;

	public Color backgroundColor = new Color(0.2f, 0.3f, 0.4f, 0.5f);
	public Color foregroundColor = new Color(0.2f, 0.3f, 0.4f, 0.5f);

    void Start()
    {
        playerUI = true;
    }

    void OnGUI()
    {
		GUI.color = foregroundColor;

        if (playerUI)
        {
			Rect windowRect = new Rect(Screen.width * .4f, Screen.height * .75f, 350, 62);
            windowRect = GUI.Window(0, windowRect, PlayerInfo, game.player.name);
        }
        else if (cowUI && cowGameObject != null )
        {
			Rect windowRect = new Rect(Screen.width * .4f, Screen.height * .75f, 230, 72);
            windowRect = GUI.Window(0, windowRect, CowInfo, cow.name);
        }
    }

    void PlayerInfo(int windowID)
    {
		GUI.contentColor = backgroundColor;
		GUI.backgroundColor = Color.red;

        GUI.Label(new Rect(20, 30, 150, 30), "Cash: €" + game.player.cash);

        if (!cow.ownedByPlayer)
            if (GUI.Button(new Rect(105, 23, 80, 30), "Make cow"))
                CowMaker.generateCow();

        if (GUI.Button(new Rect(200, 23, 60, 30), "Load"))
            game.Load();

        if (GUI.Button(new Rect(275, 23, 60, 30), "Save"))
            game.Save();
    }

    void CowInfo(int windowID)
    {
		GUI.contentColor = backgroundColor;
		GUI.backgroundColor = Color.red;

        GUI.Label(new Rect(10, 20, 150, 30), "Health: " + cow.health);
        GUI.Label(new Rect(10, 40, 150, 30), "Happiness: " +  cow.happiness);
           
        if (!cow.ownedByPlayer)
            if (GUI.Button(new Rect(130, 25, 60, 30), "Buy"))
                cow.Buy();
		
        if (cow.ownedByPlayer)
            if (GUI.Button(new Rect(130, 25, 60, 30), "Sell"))
            {
                cow.Sell();
                Destroy(cowGameObject);
                cow.gameObjectID = 0;
                playerUI = true;
                cowUI = false;
            }
    }
}