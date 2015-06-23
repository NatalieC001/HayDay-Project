using UnityEngine;
using System.Collections;

public class UI : GameController {

    public bool playerUI;
    public bool cowUI;

    public Cow cow;
    public GameObject cowGameObject;


    void Start()
    {
        playerUI = true;
   
    }
    void OnGUI()
    {
        if (playerUI)
        {
            Rect windowRect = new Rect(20, 10, 230, 100);
            windowRect = GUI.Window(0, windowRect, PlayerInfo, game.player.name);
        }
        else if (cowUI && cowGameObject != null )
        {
            Rect windowRect = new Rect(20, 10, 230, 100);
            windowRect = GUI.Window(0, windowRect, CowInfo, cow.name);
        }
    }



    void PlayerInfo(int windowID)
    {
        GUI.Label(new Rect(10, 20, 150, 30), "Cash: €" + game.player.cash);

        if (!cow.ownedByPlayer)
            if (GUI.Button(new Rect(80, 20, 80, 20), "Make cow"))
                CowMaker.generateCow();

        if (GUI.Button(new Rect(180, 20, 40, 20), "Load"))
            game.Load();

        if (GUI.Button(new Rect(180, 60, 40, 20), "Save"))
            game.Save();
    }


    void CowInfo(int windowID)
    {
       

        GUI.Label(new Rect(10, 20, 150, 30), "Health" + cow.health);

        GUI.Label(new Rect(10, 40, 150, 30),"Happiness" +  cow.happiness);
           

        if (!cow.ownedByPlayer)
            if (GUI.Button(new Rect(120, 20, 40, 20), "Buy"))
                cow.buy();


        if (cow.ownedByPlayer)
            if (GUI.Button(new Rect(120, 60, 40, 20), "Sell"))
            {
                cow.Sell();
                Destroy(cowGameObject);
                cow.gameObjectID = 0;
                playerUI = true;
                cowUI = false;
            }
    }
	
}
