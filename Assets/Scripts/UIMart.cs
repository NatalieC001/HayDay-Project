using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class UIMart : GameController 	
{
	#region variables
	public bool cowMenuUI = true;
	public bool cowBuyBidUI;
	public bool cowSellBidUI;

	public GUIStyle buttonMainMenu;
	public GUIStyle buttonBuy;
	public GUIStyle buttonSell;
	public GUIStyle buttonFarm;
	public GUIStyle buttonBid;
	public GUIStyle buttonStartBid;
	public GUIStyle buttonX;
	public GUIStyle labelAge;
	public GUIStyle labelBreed;
	public GUIStyle labelHappiness;
	public GUIStyle labelHealth;
	public GUIStyle labelPregnant;
	public GUIStyle labelGender;
	public GUIStyle labelWeight;
	public GUIStyle labelLoading;
	public GUIStyle labelSelect;
	public GUIStyle customTextStyle;

	public Image healthBar;
	public Image happinessBar;

	public Color backgroundColor = new Color(0.2f, 0.3f, 0.4f, 0.5f);
	public Color foregroundColor = new Color(0.2f, 0.3f, 0.4f, 0.5f);

	public Image[] bars = new Image[2];

	public GameObject AudioObject;
	public AudioClip buttonSound;

	public static int currentCowBid = 0;
	public static int currentTimer = 0;
	public static bool playerBidLast = false;
	public static bool biddingOver = false;
	public static bool cowSelected = false;
	public static int cowIndex = 0;
	public static Bidder lastBidder;
	public static UIMart uiMart;
	public static List<Bidder> bidderList;
	public static List<Cow> cowsInMart;
	public static GameObject cowList;

	static Cow biddingCow;

	Cow defaultCow;
	GameObject scrollCowList;
	CameraController cameraControl;
    
	string cowGender = "Male";
	string cowPregnant = "No";
	bool timerStart = false;
	bool timerChecked = true;
	bool cowInRing = false;
	float doesAIBid;

	Rect windowRect;
	bool isLoading = false;

    static bool bidding;
    static float timeRemaining;
	static float timeOfLastBid;

	static Vector2 martTopRight = new Vector2(101f, 243f);
	static Vector2 martBottomLeft = new Vector2(57f, 261f);

    Vector3 bidArea = new Vector3(109f, 0f, 137f);

	private float scrollListPaddingY;
	private float healthHappinessPaddingY;

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
        defaultCow = new Cow("", 0, "", 0, 0, false, false, 0);
        biddingCow = defaultCow;
		cowList = GameObject.Find ("CowSelectList");
		scrollCowList = cowList.transform.Find ("Panel").gameObject;
		cowList.SetActive (false);
        bidderList = new List<Bidder>();
        timeRemaining = 10;

        for (int i = 0; i < Random.Range(3, 6); i++) 
		{
			Bidder newBidder = BidderMaker.SpawnBidder ("", martTopRight, martBottomLeft);
			bidderList.Add (newBidder); 
		}

        Vector2 bottomLeft = new Vector2(128f, 116.82f);
        Vector2 topRight = new Vector2(95.14f, 116.82f);
        cowsInMart = new List<Cow>();
        Vector3 forward = new Vector3(0,0,1);
        for (int i = 0; i < 5; i++)
        {
			Cow newCow = CowMaker.GenerateCow();
            cowsInMart.Add(newCow);
            CowMaker.SpawnCow(newCow, bottomLeft, topRight, forward);
           
        }

        foreach(Cow cow in cowsInMart)
        {
            cow.cowController.Wait();
        }
    }

    void FixedUpdate()
    {
        if (bidding)
            currentTimer = (int)(timeRemaining - (Time.time - timeOfLastBid));

        if (bidding && Time.time > timeOfLastBid + timeRemaining)
             StopBidding();       
    }

    public static void bid(Bidder bidder,float bid)
    {
        if (bid > currentCowBid)
        {
            if(bidder != null)
            { 
                playerBidLast = false; 
            }
              
            currentCowBid = (int)bid;
            lastBidder = bidder;
            
            endBiddingRound();
            StartNewRound();
        }
    }

    IEnumerator WaitForCow()
    {      
        while (Vector3.Distance(biddingCow.cowController.ReturnPosition(), bidArea) > 2)
        {
           yield return new WaitForSeconds(1f);       
        }

        timeOfLastBid = Time.time;
        StartNewRound();
        bidding = true;
    }

    void StartBidding(Cow cow)
    {
       biddingCow = cow;
       cowInRing = true;
       currentCowBid = setCowPrice(cow);
       StartCoroutine(WaitForCow());   
    }

    void StopBidding()
    {
        endBiddingRound();
        bidding = false;
        timeRemaining = 10;

		// When selling, checks if bidding cow in players list
        if(game.cows.Contains(biddingCow))
        {
            game.player.cash += currentCowBid;
            game.cows.Remove(biddingCow);      
        }
		else
		{
			// Add to players cows list
			game.cows.Add(cowsInMart[0]);
			// Removing from cow generated list
			cowsInMart.RemoveAt(0);
		}

        if(playerBidLast)
        {
            print(game.player.name + " has bought " + biddingCow.name);
        }
        else
		{
        	print(lastBidder + " has bought " + biddingCow.name);
		}

        cowMenuUI = true;
        cowBuyBidUI = false;
		cowSelected = false;
        Destroy(biddingCow.cowController.gameObject);
		ClearStats();
		cameraControl.WatchPlayer();
    }

    public static void StartNewRound()
    {
        timeOfLastBid = Time.time;
        timeRemaining--;

        foreach (Bidder bidder in bidderList)
            if (bidder != lastBidder)
                bidder.CondisderBidding(cowsInMart[0], currentCowBid);
    }

    public static void endBiddingRound()
    {
        foreach (Bidder bidder in bidderList)
        {
            bidder.stopBidding();
        }
    }

    int setCowPrice(Cow cow)
    {
        return (cow.weight + (cow.health +  cow.happiness) / cow.age) * 10;
    }

    void OnGUI()
    {
		GUI.color = foregroundColor;
		
		if(cowMenuUI)
		{
			windowRect = new Rect(Screen.width * .29f, Screen.height - 150f, 620, 100);
			windowRect = GUI.Window(0, windowRect, Menu, "Menu");
		}
		else if(cowBuyBidUI)
		{
			if(Screen.width < 900)
			{
				windowRect = new Rect(Screen.width * .54f, Screen.height - 525f, 350, 500);
				scrollListPaddingY = 240;
				healthHappinessPaddingY = 270;
			}
			
			if(Screen.width > 900)
			{
				windowRect = new Rect(Screen.width * .57f, Screen.height - 540f, 350, 500);
				scrollListPaddingY = 260;
				healthHappinessPaddingY = 285;
			}
			
			if(Screen.width > 1000)
			{
				windowRect = new Rect(Screen.width * .6f, Screen.height - 555f, 350, 500);
				scrollListPaddingY = 275;
				healthHappinessPaddingY = 300;
			}
			
			if(Screen.width > 1100)
			{
				windowRect = new Rect(Screen.width * .64f, Screen.height - 570f, 350, 500);
				scrollListPaddingY = 290;
				healthHappinessPaddingY = 315;
			}
			
			if(Screen.width > 1200)
			{
				windowRect = new Rect(Screen.width * .67f, Screen.height - 585f, 350, 500);
				scrollListPaddingY = 305;
				healthHappinessPaddingY = 330;
			}
			
			if(Screen.width > 1300)
			{
				windowRect = new Rect(Screen.width * .7f, Screen.height - 600f, 350, 500);
				scrollListPaddingY = 320;
				healthHappinessPaddingY = 348;
			}
			
			if(Screen.width > 1400)
			{
				windowRect = new Rect(Screen.width * .74f, Screen.height - 615f, 350, 500);
				scrollListPaddingY = 330;
				healthHappinessPaddingY = 360;
			}
			
			if(Screen.width > 1500)
			{
				windowRect = new Rect(Screen.width * .77f, Screen.height - 630f, 350, 500);
				scrollListPaddingY = 350;
				healthHappinessPaddingY = 375;
			}
			
			windowRect = GUI.Window(0, windowRect, CowBuyBid, "Buying");
		}
		else if(cowSellBidUI)
		{
			if(Screen.width < 900)
			{
				windowRect = new Rect(Screen.width * .54f, Screen.height - 525f, 350, 500);
				scrollListPaddingY = 240;
				healthHappinessPaddingY = 270;
			}
			
			if(Screen.width > 900)
			{
				windowRect = new Rect(Screen.width * .57f, Screen.height - 540f, 350, 500);
				scrollListPaddingY = 260;
				healthHappinessPaddingY = 285;
			}
			
			if(Screen.width > 1000)
			{
				windowRect = new Rect(Screen.width * .6f, Screen.height - 555f, 350, 500);
				scrollListPaddingY = 275;
				healthHappinessPaddingY = 300;
			}
			
			if(Screen.width > 1100)
			{
				windowRect = new Rect(Screen.width * .64f, Screen.height - 570f, 350, 500);
				scrollListPaddingY = 290;
				healthHappinessPaddingY = 315;
			}
			
			if(Screen.width > 1200)
			{
				windowRect = new Rect(Screen.width * .67f, Screen.height - 585f, 350, 500);
				scrollListPaddingY = 305;
				healthHappinessPaddingY = 330;
			}
			
			if(Screen.width > 1300)
			{
				windowRect = new Rect(Screen.width * .7f, Screen.height - 600f, 350, 500);
				scrollListPaddingY = 320;
				healthHappinessPaddingY = 348;
			}
			
			if(Screen.width > 1400)
			{
				windowRect = new Rect(Screen.width * .74f, Screen.height - 615f, 350, 500);
				scrollListPaddingY = 330;
				healthHappinessPaddingY = 360;
			}
			
			if(Screen.width > 1500)
			{
				windowRect = new Rect(Screen.width * .77f, Screen.height - 630f, 350, 500);
				scrollListPaddingY = 350;
				healthHappinessPaddingY = 375;
			}
			
			windowRect = GUI.Window(0, windowRect, CowSellBid, "Selling");
		}

		if(timerStart)
		{
			if(!timerChecked)
			{
				StartCoroutine(TimerDec(1));
			}
		}
	}

	void Menu(int windowID)		// Fix this stuff, finish me!
	{
		GUI.contentColor = backgroundColor;
		
		SetHealth(0f);
		SetHappiness(0f);
		
		if(isLoading)
			GUI.Label(new Rect(200, 32, 270, 50), "", labelLoading);
		
		if(!isLoading)
		{
			if (GUI.Button(new Rect(45, 35, 115, 55), "", buttonBuy))
			{
				GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
				cowMenuUI = false;
				cowBuyBidUI = true;
				LookAtRing();
                biddingCow = cowsInMart[Random.Range(0, cowsInMart.Count - 1)];
                biddingCow.cowController.MoveTo(bidArea);

			}
		}
		
		if (!isLoading) 
		{
			if (GUI.Button (new Rect (220, 35, 115, 45), "", buttonSell)) 
			{
				GetComponent<AudioSource> ().PlayOneShot (buttonSound, 0.7f);
				cowMenuUI = false;
				cowSellBidUI = true;
				cowList.SetActive (true);
				LookAtRing ();
			}
		}
		
		if (!isLoading) 
		{
			if (GUI.Button (new Rect (400, 35, 160, 42), "", buttonFarm)) 
			{	
				GetComponent<AudioSource> ().PlayOneShot (buttonSound, 0.7f);
				Save ();
				isLoading = true;
				StartCoroutine (WaitFor (3));
			}
		}
	}

	void CowBuyBid(int windowID)
    {
		GUI.contentColor = backgroundColor;

		SetHealth(biddingCow.health / 100f);
		SetHappiness(biddingCow.happiness / 10f);
		
		healthBar.transform.position = new Vector2 (windowRect.center.x + 64, healthHappinessPaddingY + 35);
		happinessBar.transform.position = new Vector2 (windowRect.center.x + 64, healthHappinessPaddingY);
		
		if (!biddingCow.gender == true)
		{
			cowGender = "Female";
			
			if(biddingCow.pregnant == true)
				cowPregnant = "Yes";
		}
		else
		{
			cowGender = "Male";
			biddingCow.pregnant = false;
			cowPregnant = "No";
		}

		GUI.Label(new Rect(110, 20, 150, 30), game.player.name, customTextStyle);
		GUI.Label(new Rect(90, 50, 150, 30), "Cash: € " + game.player.cash, customTextStyle);
		GUI.Label(new Rect(110, 90, 150, 30), "Going: " + currentTimer, customTextStyle);
		GUI.Label(new Rect(25, 135, 80, 25), "", labelAge);
		GUI.Label(new Rect(132, 132, 150, 30), "" + biddingCow.age, customTextStyle);
		GUI.Label(new Rect(25, 170, 90, 25), "", labelBreed);
		GUI.Label(new Rect(132, 170, 150, 30), "" + biddingCow.breed, customTextStyle);
		GUI.Label(new Rect(25, 205, 110, 30), "", labelHappiness);
		GUI.Label(new Rect(25, 240, 90, 25), "", labelHealth);
		GUI.Label(new Rect(25, 275, 110, 30), "", labelPregnant);
		GUI.Label(new Rect(150, 275, 150, 30), "" + cowPregnant, customTextStyle);
		GUI.Label(new Rect(25, 313, 90, 25), "", labelGender);
		GUI.Label(new Rect(132, 313, 150, 30), "" + cowGender, customTextStyle);
		GUI.Label(new Rect(25, 350, 90, 30), "", labelWeight);
		GUI.Label(new Rect(132, 350, 150, 30), "" + biddingCow.weight, customTextStyle);

		GUI.Label(new Rect(65, 400, 150, 30), "Current Bid: € " + currentCowBid, customTextStyle);

		if(!cowInRing)
		{
			if (GUI.Button (new Rect (70, 445, 170, 35), "", buttonStartBid))
			{	
				GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);

				cowInRing = true;


                StartBidding(biddingCow);
			}
		}

		if(!playerBidLast && cowInRing)
		{
			if(game.player.cash >= currentCowBid)
			{
				if(GUI.Button (new Rect (125, 445, 80, 40), "", buttonBid))
				{
					GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
	                bid(null, currentCowBid += 1000);
	                playerBidLast = true;
				}
			}
		}

		if (!cowInRing)
		{
			if (GUI.Button(new Rect(295, 450, 35, 35), "", buttonX))
			{
				GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
				cowMenuUI = true;
				cowBuyBidUI = false;
	            cameraControl.WatchPlayer();
			}
		}
    }

	void CowSellBid(int windowID)
	{
		GUI.contentColor = backgroundColor;

		if(!cowSelected)
		{
			GUI.Label(new Rect(70, 30, 210, 45), "", labelSelect);
		}

		scrollCowList.transform.position = new Vector2 (windowRect.center.x, scrollListPaddingY);

		if(cowSelected && game.cows.Count != 0)
		{
			SetHealth(game.cows[cowIndex].health / 100f);
			SetHappiness(game.cows[cowIndex].happiness / 10f);
			
			healthBar.transform.position = new Vector2 (windowRect.center.x + 64, healthHappinessPaddingY + 35);
			happinessBar.transform.position = new Vector2 (windowRect.center.x + 64, healthHappinessPaddingY);
			
			if (!game.cows[cowIndex].gender == true)
			{
				cowGender = "Female";
				
				if(game.cows[cowIndex].pregnant == true)
					cowPregnant = "Yes";
			}
			else
			{
				cowGender = "Male";
				game.cows[cowIndex].pregnant = false;
				cowPregnant = "No";
			}
			
			GUI.Label(new Rect(110, 20, 150, 30), game.player.name, customTextStyle);
			GUI.Label(new Rect(90, 50, 150, 30), "Cash: € " + game.player.cash, customTextStyle);
			GUI.Label(new Rect(110, 90, 150, 30), "Going: " + currentTimer, customTextStyle);
			GUI.Label(new Rect(25, 135, 80, 25), "", labelAge);
			GUI.Label(new Rect(132, 132, 150, 30), "" + game.cows[cowIndex].age, customTextStyle);
			GUI.Label(new Rect(25, 170, 90, 25), "", labelBreed);
			GUI.Label(new Rect(132, 170, 150, 30), "" + game.cows[cowIndex].breed, customTextStyle);
			GUI.Label(new Rect(25, 205, 110, 30), "", labelHappiness);
			GUI.Label(new Rect(25, 240, 90, 25), "", labelHealth);
			GUI.Label(new Rect(25, 275, 110, 30), "", labelPregnant);
			GUI.Label(new Rect(150, 275, 150, 30), "" + cowPregnant, customTextStyle);
			GUI.Label(new Rect(25, 313, 90, 25), "", labelGender);
			GUI.Label(new Rect(132, 313, 150, 30), "" + cowGender, customTextStyle);
			GUI.Label(new Rect(25, 350, 90, 30), "", labelWeight);
			GUI.Label(new Rect(132, 350, 150, 30), "" + game.cows[cowIndex].weight, customTextStyle);
			
			GUI.Label(new Rect(65, 400, 150, 30), "Current Bid: € " + currentCowBid, customTextStyle);

            if (!cowInRing)
			{
                if (GUI.Button(new Rect(105, 445, 120, 40), "", buttonSell))
                {
                    if (game.cows.Count != 0)
                    {
                        GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);

                        Vector2 spawnLocation = new Vector3(120.79f, 149.84f);
                        CowMaker.SpawnCow(game.cows[cowIndex], spawnLocation, spawnLocation, Vector3.zero);

                        game.cows[cowIndex].cowController.MoveTo(bidArea);
                        StartBidding(game.cows[cowIndex]);

                        CreateScrollList.RemoveCowButton(cowIndex);
                    }
                }
			}
		}

		if (!cowInRing)
		{
			if (GUI.Button(new Rect(295, 450, 35, 35), "", buttonX))
			{
				GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);

				if(cowSelected)
				{
					cowSelected = false;
					cowList.SetActive(true);
					SetHealth(0f);
					SetHappiness(0f);
				}
				else
				{
					cowSellBidUI = false;
					cowMenuUI = true;
					cowList.SetActive(false);
					cameraControl.WatchPlayer();
				}
			}
		}
	}

	void LookAtRing()
	{
		//cowsInMart[0].cowController.transform.position + cowsInMart[0].cowController.transform.forward * 10;

		Vector3 height = new Vector3(0, 2, 0);
        Vector3 position = new Vector3(100, 5, 148.5f); 


		cameraControl = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        cameraControl.MoveToLookAt(position + height, bidArea + height);
	}

	public void ClearStats()
	{
        biddingCow = defaultCow;

		currentTimer = 0;
		currentCowBid = 0;

		// Clear all buttons && re-populate list
		CreateScrollList.RemoveAllButtons();
		CreateScrollList.access.PopulateList();

		timerStart = false;
		timerChecked = true;
		cowInRing = false;
		biddingOver = true;
		playerBidLast = false;

		SetHealth(0);
		SetHappiness(0);
	}

	IEnumerator TimerDec(int seconds) 
	{
		if(timerStart)
		{
			currentTimer -= seconds;
			timerChecked = true;
		}

		yield return new WaitForSeconds(seconds);

		timerChecked = false;
	}

	void SetHealth(float health)
	{
		healthBar.fillAmount = health;
	}
	
	void SetHappiness(float happiness)
	{
		happinessBar.fillAmount = happiness;
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
			loadPlayer = true;
		}
	}
}