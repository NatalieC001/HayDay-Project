using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace HayDay
{
	public class UIMart : MonoBehaviour 	
	{
		#region variables
		// GUI labels & text
		public GUIStyle buttonMainMenu;
		public GUIStyle buttonBuy;
		public GUIStyle buttonSell;
		public GUIStyle buttonFarm;
		public GUIStyle buttonSupplies;
		public GUIStyle buttonGrain;
		public GUIStyle buttonHay;
		public GUIStyle buttonPellets;
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
		public GUIStyle labelBidWon;
		public GUIStyle labelBidLost;
		public GUIStyle customTextStyle;

		// GUI health & happiness bars
		private Image[] bars = new Image[2];

		// Images bars
		private Image healthBar;
		private Image happinessBar;
		
		public Color backgroundColor = new Color(0.2f, 0.3f, 0.4f, 0.5f);
		public Color foregroundColor = new Color(0.2f, 0.3f, 0.4f, 0.5f);

		// Audio variables
		public GameObject AudioObject;
		public AudioClip buttonSound;

		// Bidding control
	    public static int startingPrice;
		public static int currentCowBid;
		public static int currentTimer;
		public static bool playerBidLast;
		public static bool biddingOver;
		public static Bidder lastBidder;
		public static bool bidding;
		public static float timeRemaining;
		public static float timeOfLastBid;
		public static UIMart uiMart;
		public static List<Bidder> bidderList;
		public static List<Cow> cowsInMart;
		public static GameObject cowList;
		public static Cow biddingCow;

		public GameObject cowGameObject;

		private Cow defaultCow;
		private GameObject scrollCowList;
		private CameraController cameraControl;
	    
		// Private local variables
		private string cowGender = "Male";
		private string cowPregnant = "No";
		private bool timerStart;
		private bool timerChecked = true;
		private bool cowInRing;
		private float doesAIBid;
		private Rect windowRect;
		private bool isLoading;

		// Spawn location for bidders
		private Vector2 martTopRight = new Vector2(110.28f, 142.24f);
		private Vector2 martBottomLeft = new Vector2(99.56f, 147f);

		// Spawn location for cows
		private Vector2 bottomLeft = new Vector2(128f, 116.82f);
		private Vector2 topRight = new Vector2(95.14f, 116.82f);
		private Vector3 forward = new Vector3(0,0,1);

		// Camera control variables
		private Vector3 bidArea = new Vector3(109f, 0f, 137f);
	    private Vector3 cameraPosition;

		// Scroll panel control & labels
		private float scrollListPaddingY;
		private float healthHappinessPaddingY;
		private int biddingCowIndex;
		private bool bidResult;
		private GUIStyle biddingResult;
		private VCAnalogJoystickBase joyStick;
		#endregion
	 
	    void Start()
	    {
	        cameraControl = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
			joyStick = GameObject.Find("VCAnalogJoystick").GetComponent<VCAnalogJoystickBase>();
	        cameraPosition = cameraControl.gameObject.transform.position;
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

	        cowsInMart = new List<Cow>();

			// Spawn new cows in the mart & store them in a list
			for (int i = 0; i < Random.Range(5, 8); i++)
	        {
				Cow newCow = CowMaker.GenerateCow();
				if(CowMaker.SpawnCow(newCow, bottomLeft, topRight, forward) == 1)
				{
					try
					{
						cowsInMart.Add(newCow);
					}
					catch(System.Exception error)
					{
						Debug.Log("Error: " + error);
					}
				}
	        }

			// Loop through cows in the mart & make them wait
	        foreach(Cow cow in cowsInMart)
	        {
				try
				{
	            	cow.cowController.Wait("Mart");
				}
				catch(System.Exception error)
				{
					print ("Error: " + error);
				}
	        }
	    }

		void OnGUI()
		{
			GUI.color = foregroundColor;
			
			if(GameController.Instance().martCowMenuUI)
			{
				windowRect = new Rect(Screen.width * .29f, Screen.height - 150f, 620, 100);
				windowRect = GUI.Window(0, windowRect, Menu, "Menu");
			}
			else if (GameController.Instance().globalPlayerUI)
			{
				windowRect = new Rect(Screen.width * .29f, Screen.height - 150f, 620, 100);
				windowRect = GUI.Window(0, windowRect, PlayerInfo, "Mart");
			}
			else if(GameController.Instance().globalSuppliesUI)
			{
				windowRect = new Rect(Screen.width * .29f, Screen.height - 300f, 620, 250);
				windowRect = GUI.Window(0, windowRect, BuyPlayerSupplies, "Buy Supplies");
			}
			else if(GameController.Instance().martCowBuyBidUI)
			{
				CheckScreenSize();
				windowRect = GUI.Window(0, windowRect, CowBuyBid, "Buying");
			}
			else if(GameController.Instance().martCowSellBidUI)
			{
				CheckScreenSize();
				windowRect = GUI.Window(0, windowRect, CowSellBid, "Selling");
			}
			else if(GameController.Instance().martSceneTransitionUI)
			{
				windowRect = new Rect(Screen.width * .29f, Screen.height - 150f, 620, 100);
				windowRect = GUI.Window(0, windowRect, SceneTransition, "Select");
			}
			
			if(timerStart)
			{
				if(!timerChecked)
				{
					StartCoroutine(TimerDec(1));
				}
			}
		}

	    void FixedUpdate()
		{
	        if (bidding)
	            currentTimer = (int)(timeRemaining - (Time.time - timeOfLastBid));

	        if (bidding && Time.time > timeOfLastBid + timeRemaining)
	        	StopBidding();
	    }

		private void Menu(int windowID)
		{
			GUI.contentColor = backgroundColor;
			
			SetHealth(0f);
			SetHappiness(0f);
			
			if(isLoading)
				GUI.Label(new Rect(200, 32, 270, 50), "", labelLoading);
			
			if(bidResult)
				GUI.Label(new Rect(190, 32, 270, 50), "", biddingResult);
			
			if(!isLoading)
			{
				if(!bidResult)
				{
					if (GUI.Button(new Rect(130, 35, 115, 55), "", buttonBuy))
					{
						GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
						GameController.Instance().martCowMenuUI = false;
						GameController.Instance().martCowBuyBidUI = true;
						joyStick.gameObject.SetActive(false);
						LookAtRing();
					}
				}
			}
			
			if (!isLoading) 
			{
				if(!bidResult)
				{
					if (GUI.Button (new Rect (330, 35, 115, 45), "", buttonSell)) 
					{
						GetComponent<AudioSource> ().PlayOneShot (buttonSound, 0.7f);
						GameController.Instance().martCowMenuUI = false;
						GameController.Instance().martCowSellBidUI = true;
						cowList.SetActive (true);
						joyStick.gameObject.SetActive(false);
						LookAtRing();
					}
				}
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
		
		private void BuyPlayerSupplies(int windowID)	// Finish Me!
		{
			GUI.contentColor = backgroundColor;
			
			SetHealth(0);
			SetHappiness(0);
			
			GUI.Label(new Rect(85, 30, 150, 30), "Grain", customTextStyle);
			GUI.Label(new Rect(245, 30, 150, 30), "Hay", customTextStyle);
			GUI.Label(new Rect(380, 30, 150, 30), "Pellets", customTextStyle);
			GUI.Label(new Rect(85, 70, 150, 30), "€500", customTextStyle);
			GUI.Label(new Rect(240, 70, 150, 30), "€800", customTextStyle);
			GUI.Label(new Rect(385, 70, 150, 30), "€1250", customTextStyle);
			
			if (GUI.Button(new Rect(80, 105, 75, 75), "", buttonGrain))	// Add to player class / inventory
			{
				if(GameController.Instance().player.cash >= 500)
				{
					GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
					GameController.Instance().player.grain++;
					GameController.Instance().player.cash = GameController.Instance().player.cash - 500;
				}
			}
			
			if (GUI.Button(new Rect(230, 105, 75, 75), "", buttonHay))		// Add to player class / inventory
			{
				if(GameController.Instance().player.cash >= 800)
				{
					GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
					GameController.Instance().player.hay++;
					GameController.Instance().player.cash = GameController.Instance().player.cash - 800;
				}
			}
			
			if (GUI.Button(new Rect(380, 105, 75, 75), "", buttonPellets))	// Add to player class / inventory
			{
				if(GameController.Instance().player.cash >= 1250)
				{
					GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
					GameController.Instance().player.pellet++;
					GameController.Instance().player.cash = GameController.Instance().player.cash - 1250;
				}
			}
			
			GUI.Label(new Rect(110, 190, 150, 30), "" + GameController.Instance().player.grain, customTextStyle);
			GUI.Label(new Rect(260, 190, 150, 30), "" + GameController.Instance().player.hay, customTextStyle);
			GUI.Label(new Rect(410, 190, 150, 30), "" + GameController.Instance().player.pellet, customTextStyle);
			
			if (GUI.Button(new Rect(535, 180, 50, 50), "", buttonX))
			{
				GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
				GameController.Instance().globalPlayerUI = true;
				GameController.Instance().globalSuppliesUI = false;
			}
		}
		
		private void CowBuyBid(int windowID)
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
			
			GUI.Label(new Rect(110, 20, 150, 30), GameController.Instance().player.name, customTextStyle);
			GUI.Label(new Rect(90, 50, 150, 30), "Cash: € " + GameController.Instance().player.cash, customTextStyle);
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
			GUI.Label(new Rect(132, 350, 150, 30), "" + biddingCow.weight + " KG", customTextStyle);
			
			GUI.Label(new Rect(65, 400, 150, 30), "Current Bid: € " + currentCowBid, customTextStyle);
			
			if(!cowInRing)
			{
				if (GUI.Button (new Rect (70, 445, 170, 35), "", buttonStartBid))
				{	
					if(cowsInMart.Count != 0)
					{
						GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
						GameController.Instance().cowIndex = Random.Range(0, cowsInMart.Count - 1);
						biddingCow = cowsInMart[GameController.Instance().cowIndex];	           
						StartBidding();
					}
				}
			}
			
			if(!playerBidLast && cowInRing)
			{
				if(GameController.Instance().player.cash >= currentCowBid)
				{
					if(GUI.Button (new Rect (125, 445, 80, 40), "", buttonBid))
					{
						GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
						BidOnCow(null, currentCowBid += 500);
						playerBidLast = true;
						LookAtRing();
					}
				}
			}
			
			if (!cowInRing)
			{
				if (GUI.Button(new Rect(295, 450, 35, 35), "", buttonX))
				{
					GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
					GameController.Instance().martCowMenuUI = true;
					GameController.Instance().martCowBuyBidUI = false;
					joyStick.gameObject.SetActive(true);
					cameraControl.WatchPlayer();
				}
			}
		}
		
		private void CowSellBid(int windowID)	// Bug with selling, camera
		{
			GUI.contentColor = backgroundColor;
			
			if(!GameController.Instance().cowSelected)
			{
				GUI.Label(new Rect(70, 30, 210, 45), "", labelSelect);
			}
			
			scrollCowList.transform.position = new Vector2 (windowRect.center.x, scrollListPaddingY);
			
			if(GameController.Instance().cowSelected && GameController.Instance().cows.Count != 0)
			{
				SetHealth(GameController.Instance().cows[GameController.Instance().cowIndex].health / 100f);
				SetHappiness(GameController.Instance().cows[GameController.Instance().cowIndex].happiness / 10f);
				
				healthBar.transform.position = new Vector2 (windowRect.center.x + 64, healthHappinessPaddingY + 35);
				happinessBar.transform.position = new Vector2 (windowRect.center.x + 64, healthHappinessPaddingY);
				
				if (!GameController.Instance().cows[GameController.Instance().cowIndex].gender == true)
				{
					cowGender = "Female";
					
					if(GameController.Instance().cows[GameController.Instance().cowIndex].pregnant == true)
						cowPregnant = "Yes";
				}
				else
				{
					cowGender = "Male";
					GameController.Instance().cows[GameController.Instance().cowIndex].pregnant = false;
					cowPregnant = "No";
				}
				
				GUI.Label(new Rect(110, 20, 150, 30), GameController.Instance().player.name, customTextStyle);
				GUI.Label(new Rect(90, 50, 150, 30), "Cash: € " + GameController.Instance().player.cash, customTextStyle);
				GUI.Label(new Rect(110, 90, 150, 30), "Going: " + currentTimer, customTextStyle);
				GUI.Label(new Rect(25, 135, 80, 25), "", labelAge);
				GUI.Label(new Rect(132, 132, 150, 30), "" + GameController.Instance().cows[GameController.Instance().cowIndex].age, customTextStyle);
				GUI.Label(new Rect(25, 170, 90, 25), "", labelBreed);
				GUI.Label(new Rect(132, 170, 150, 30), "" + GameController.Instance().cows[GameController.Instance().cowIndex].breed, customTextStyle);
				GUI.Label(new Rect(25, 205, 110, 30), "", labelHappiness);
				GUI.Label(new Rect(25, 240, 90, 25), "", labelHealth);
				GUI.Label(new Rect(25, 275, 110, 30), "", labelPregnant);
				GUI.Label(new Rect(150, 275, 150, 30), "" + cowPregnant, customTextStyle);
				GUI.Label(new Rect(25, 313, 90, 25), "", labelGender);
				GUI.Label(new Rect(132, 313, 150, 30), "" + cowGender, customTextStyle);
				GUI.Label(new Rect(25, 350, 90, 30), "", labelWeight);
				GUI.Label(new Rect(132, 350, 150, 30), "" + GameController.Instance().cows[GameController.Instance().cowIndex].weight + " KG", customTextStyle);
				
				GUI.Label(new Rect(65, 400, 150, 30), "Current Bid: € " + currentCowBid, customTextStyle);
				
				if (!cowInRing)
				{
					if (GUI.Button(new Rect(105, 445, 120, 40), "", buttonSell))
					{
						if (GameController.Instance().cows.Count != 0)
						{
							if (!cowInRing)
							{
								GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
								biddingCow = GameController.Instance().cows[GameController.Instance().cowIndex];
								CowMaker.SpawnCow(biddingCow, new Vector2(bidArea.x, bidArea.z), new Vector2(bidArea.x, bidArea.z), Vector3.zero);
								StartBidding();
								CreateScrollList.RemoveCowButton(GameController.Instance().cowIndex);
							}
						}
					}
				}
			}
			
			if (!cowInRing)
			{
				if (GUI.Button(new Rect(295, 450, 35, 35), "", buttonX))
				{
					GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
					if(GameController.Instance().cowSelected)
					{
						GameController.Instance().cowSelected = false;
						cowList.SetActive(true);
						SetHealth(0f);
						SetHappiness(0f);
					}
					else
					{
						GameController.Instance().martCowMenuUI = true;
						GameController.Instance().martCowSellBidUI = false;
						cowList.SetActive(false);
						joyStick.gameObject.SetActive(true);
						cameraControl.WatchPlayer();
					}
				}
			}
		}

		// Allows bidders to bid on cows in the mart
	    public static void BidOnCow(Bidder bidder, float bid)
	    {
			// Getting camera control
	        CameraController cameraControl = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();

	        if (bid >= currentCowBid)
	        {
	            if(bidder != null)
	            {
	            	playerBidLast = false;
	            	cameraControl.LookAt(bidder.gameObject.transform.position);
	            }
	              
	            currentCowBid = (int)bid;
	            lastBidder = bidder;
	            
	            EndBiddingRound();
	            StartNewRound();
	        }
	    }

	    private IEnumerator WaitForCow()
	    {
	        yield return new WaitForSeconds(1f);    
	        biddingCow.cowController.MoveTo(bidArea);

			while (Vector3.Distance(biddingCow.cowController.ReturnPosition(), bidArea) > 2)
	        {
	           yield return new WaitForSeconds(1f);       
	        }
			
	        timeOfLastBid = Time.time;
	        StartNewRound();
	        bidding = true;
	    }

	    private void StartBidding()
	    {
	        cowInRing = true;    
	        startingPrice = SetCowPrice(biddingCow);
	        currentCowBid = startingPrice;
	        cameraControl.WatchTarget(biddingCow.cowController.gameObject);
	        StartCoroutine(WaitForCow());   
	    }

	    private void StopBidding()
	    {
	        EndBiddingRound();
	        bidding = false;
	        timeRemaining = 10;

			// When selling, checks if bidding cow in players list
			if(GameController.Instance().cows.Contains(biddingCow) && startingPrice != currentCowBid)
	        {
	            GameController.Instance().player.cash += currentCowBid;
	            GameController.Instance().cows.Remove(biddingCow);      
	        }
			else
			{
				// Add to players cows list & Removing from cow generated list
				if(playerBidLast)
				{
					GameController.Instance().player.cash -= currentCowBid;
					GameController.Instance().cows.Add(biddingCow);
					cowsInMart.Remove(biddingCow);
					biddingResult = labelBidWon;
					bidResult = true;
					StartCoroutine(WaitForSec(3));
				}
				else
				{
					cowsInMart.Remove(biddingCow);
					biddingResult = labelBidLost;
					bidResult = true;
					StartCoroutine(WaitForSec(3));
				}
			}

	        if(playerBidLast)
	        {
	            print(GameController.Instance().player.name + " has bought " + biddingCow.name);
	        }
	        else
			{
	        	print(lastBidder + " has bought " + biddingCow.name);
			}

			GameController.Instance().martCowMenuUI = true;
			GameController.Instance().martCowBuyBidUI = false;
			GameController.Instance().martCowSellBidUI = false;
			GameController.Instance().cowSelected = false;
			joyStick.gameObject.SetActive(true);
	        Destroy(biddingCow.cowController.gameObject);
	        biddingCow = null;
			ClearStats();
			cameraControl.WatchPlayer();
	    }

		// Starting a new round
	    public static void StartNewRound()
	    {
	        timeOfLastBid = Time.time;
	        timeRemaining--;

			// Looping through bidders and call their method to consider bidding
	        foreach (Bidder bidder in bidderList)
	            if (bidder != lastBidder)
					bidder.ConsiderBidding(biddingCow, currentCowBid);
	    }

	    public static void EndBiddingRound()
	    {
	        foreach (Bidder bidder in bidderList)
	        {
	            bidder.StopBidding();
	        }
	    }

	    private int SetCowPrice(Cow cow)
	    {
	        int price = 0;

	        try
	        {
	            price = (cow.weight + (cow.health + cow.happiness) / cow.age) * 10;
	        }
	        catch(System.DivideByZeroException)
	        {
	            price = (cow.weight + (cow.health + cow.happiness)) * 10;
	        }
	        return price;
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
					StartCoroutine (WaitFor(0));
				}
			}
			
			if(!isLoading)
			{
				if (GUI.Button(new Rect(410, 34, 130, 48), "", buttonFarm))
				{
					GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
					GameController.Instance().Save();
					isLoading = true;
					StartCoroutine(WaitFor(3));
				}
			}
		}

		private void LookAtRing()
		{
			Vector3 height = new Vector3(0, 2, 0);
	        Vector3 position = new Vector3(96, 8, 142.31f);
	        cameraControl = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
	        cameraControl.MoveToLookAt(position, bidArea + height);
		}

		private void ClearStats()
		{
	        biddingCow = defaultCow;

			currentTimer = 0;
			currentCowBid = 0;

			// Clear all buttons && re-populate list
			CreateScrollList.RemoveAllButtons();
			GameController.Instance().scrollCowList.PopulateList();

			timerStart = false;
			timerChecked = true;
			cowInRing = false;
			biddingOver = true;
			playerBidLast = false;

			SetHealth(0);
			SetHappiness(0);
		}

		private IEnumerator TimerDec(int seconds) 
		{
			if(timerStart)
			{
				currentTimer -= seconds;
				timerChecked = true;
			}

			yield return new WaitForSeconds(seconds);

			timerChecked = false;
		}

		private void SetHealth(float health)
		{
			healthBar.fillAmount = health;
		}
		
		private void SetHappiness(float happiness)
		{
			happinessBar.fillAmount = happiness;
		}
		
		private void CheckScreenSize()
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
		}
		
		private IEnumerator WaitForSec(int seconds) 
		{
			yield return new WaitForSeconds(seconds);

			bidResult = false;
			biddingResult = null;
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
				
				if(level == 3)
					GameController.Instance().loadPlayer = true;
				
				Application.LoadLevel(level);
			}
		}
	}
}