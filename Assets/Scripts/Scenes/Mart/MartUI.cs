using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace IrishFarmSim
{
	public class MartUI : MartInit
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

		//public GameObject cowGameObject;

		//private Cow defaultCow;
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

		private int grainCost;
		private int hayCost;
		private int pelletsCost;
		#endregion
	 
	    void Start()
	    {
			try
			{
				InitUIControl();
				InitBidControl();
				InitSuppliesCost();
			}
			catch (UnityException e)
			{
				Debug.Log("Error - " + e);
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
			if (MartBidControl.bidding)
				MartBidControl.currentTimer = (int)(MartBidControl.timeRemaining - (Time.time - MartBidControl.timeOfLastBid));

			if (MartBidControl.bidding && Time.time > MartBidControl.timeOfLastBid + MartBidControl.timeRemaining)
	        	StopBidding();
	    }

		// Initialization of UI control
		private void InitUIControl()
		{
			cameraControl = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
			joyStick = GameObject.Find("VCAnalogJoystick").GetComponent<VCAnalogJoystickBase>();
			bars = GetComponentsInChildren<Image>();
			healthBar = bars[0];
			happinessBar = bars[1];
			cameraPosition = cameraControl.gameObject.transform.position;
		}

		// Initialization of bidding control
		private void InitBidControl()
		{
			MartBidControl.cowList = GameObject.Find("CowSelectList");
			MartBidControl.cowList.SetActive(false);
			MartBidControl.bidderList = new List<Bidder>();
			MartBidControl.timeRemaining = 10;
			MartBidControl.biddingCow = new Cow();
			scrollCowList = MartBidControl.cowList.transform.Find ("Panel").gameObject;
		}

		// Initialization of supplies costs
		private void InitSuppliesCost()
		{
			switch(GameController.Instance().gameDifficulty)
			{
				case "Easy":
					grainCost = 125;
					hayCost = 300;
					pelletsCost = 500;
				break;
				case "Normal":
					grainCost = 225;
					hayCost = 400;
					pelletsCost = 600;
				break;
				case "Hard":
					grainCost = 325;
					hayCost = 500;
					pelletsCost = 750;
				break;
				default:
					grainCost = 225;
					hayCost = 400;
					pelletsCost = 600;
				break;
			}
		}

		// Menu for controlling to either buy or sell animals
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
						GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.6f);
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
						GetComponent<AudioSource> ().PlayOneShot (buttonSound, 0.6f);
						GameController.Instance().martCowMenuUI = false;
						GameController.Instance().martCowSellBidUI = true;
						MartBidControl.cowList.SetActive(true);
						joyStick.gameObject.SetActive(false);
						LookAtRing();
					}
				}
			}
		}

		// Menu for current player status, cash & name
		private void PlayerInfo(int windowID)
		{
			GUI.contentColor = backgroundColor;
			
			GUI.Label(new Rect(20, 25, 150, 30), GameController.Instance().player.name, customTextStyle);
			GUI.Label(new Rect(20, 60, 150, 30), "Cash: € " + GameController.Instance().player.cash, customTextStyle);
			
			SetHealth(0);
			SetHappiness(0);
			
			if (GUI.Button(new Rect(260, 33, 180, 53), "", buttonSupplies))
			{
				GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.6f);
				GameController.Instance().globalPlayerUI = false;
				GameController.Instance().globalSuppliesUI = true;
			}
		}
		
		private void BuyPlayerSupplies(int windowID)
		{
			GUI.contentColor = backgroundColor;
			
			SetHealth(0);
			SetHappiness(0);


			
			GUI.Label(new Rect(85, 30, 150, 30), "Grain", customTextStyle);
			GUI.Label(new Rect(245, 30, 150, 30), "Hay", customTextStyle);
			GUI.Label(new Rect(380, 30, 150, 30), "Pellets", customTextStyle);
			GUI.Label(new Rect(85, 70, 150, 30), "€" + grainCost, customTextStyle);
			GUI.Label(new Rect(240, 70, 150, 30), "€" + hayCost, customTextStyle);
			GUI.Label(new Rect(385, 70, 150, 30), "€" + pelletsCost, customTextStyle);
			
			if (GUI.Button(new Rect(80, 105, 75, 75), "", buttonGrain))	// Add to player class / inventory
			{
				if(GameController.Instance().player.cash >= grainCost)
				{
					GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.6f);
					GameController.Instance().player.grain++;
					GameController.Instance().player.cash = GameController.Instance().player.cash - grainCost;
				}
			}
			
			if (GUI.Button(new Rect(230, 105, 75, 75), "", buttonHay))		// Add to player class / inventory
			{
				if(GameController.Instance().player.cash >= hayCost)
				{
					GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.6f);
					GameController.Instance().player.hay++;
					GameController.Instance().player.cash = GameController.Instance().player.cash - hayCost;
				}
			}
			
			if (GUI.Button(new Rect(380, 105, 75, 75), "", buttonPellets))	// Add to player class / inventory
			{
				if(GameController.Instance().player.cash >= pelletsCost)
				{
					GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.6f);
					GameController.Instance().player.pellet++;
					GameController.Instance().player.cash = GameController.Instance().player.cash - pelletsCost;
				}
			}
			
			GUI.Label(new Rect(110, 190, 150, 30), "" + GameController.Instance().player.grain, customTextStyle);
			GUI.Label(new Rect(260, 190, 150, 30), "" + GameController.Instance().player.hay, customTextStyle);
			GUI.Label(new Rect(410, 190, 150, 30), "" + GameController.Instance().player.pellet, customTextStyle);
			
			if (GUI.Button(new Rect(535, 180, 50, 50), "", buttonX))
			{
				GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.6f);
				GameController.Instance().globalPlayerUI = true;
				GameController.Instance().globalSuppliesUI = false;
			}
		}
		
		private void CowBuyBid(int windowID)
		{
			GUI.contentColor = backgroundColor;
			
			SetHealth(MartBidControl.biddingCow.health / 100f);
			SetHappiness(MartBidControl.biddingCow.happiness / 10f);
			
			healthBar.transform.position = new Vector2 (windowRect.center.x + 64, healthHappinessPaddingY + 35);
			happinessBar.transform.position = new Vector2 (windowRect.center.x + 64, healthHappinessPaddingY);
			
			if (!MartBidControl.biddingCow.gender == true)
			{
				cowGender = "Female";
				
				if(MartBidControl.biddingCow.pregnant == true)
					cowPregnant = "Yes";
			}
			else
			{
				cowGender = "Male";
				MartBidControl.biddingCow.pregnant = false;
				cowPregnant = "No";
			}
			
			GUI.Label(new Rect(110, 20, 150, 30), GameController.Instance().player.name, customTextStyle);
			GUI.Label(new Rect(90, 50, 150, 30), "Cash: € " + GameController.Instance().player.cash, customTextStyle);
			GUI.Label(new Rect(110, 90, 150, 30), "Going In: " + MartBidControl.currentTimer + "s", customTextStyle);
			GUI.Label(new Rect(25, 135, 80, 25), "", labelAge);
			GUI.Label(new Rect(132, 132, 150, 30), "" + MartBidControl.biddingCow.age, customTextStyle);
			GUI.Label(new Rect(25, 170, 90, 25), "", labelBreed);
			GUI.Label(new Rect(132, 170, 150, 30), "" + MartBidControl.biddingCow.breed, customTextStyle);
			GUI.Label(new Rect(25, 205, 110, 30), "", labelHappiness);
			GUI.Label(new Rect(25, 240, 90, 25), "", labelHealth);
			GUI.Label(new Rect(25, 275, 110, 30), "", labelPregnant);
			GUI.Label(new Rect(150, 275, 150, 30), "" + cowPregnant, customTextStyle);
			GUI.Label(new Rect(25, 313, 90, 25), "", labelGender);
			GUI.Label(new Rect(132, 313, 150, 30), "" + cowGender, customTextStyle);
			GUI.Label(new Rect(25, 350, 90, 30), "", labelWeight);
			GUI.Label(new Rect(132, 350, 150, 30), "" + MartBidControl.biddingCow.weight + " KG", customTextStyle);
			
			GUI.Label(new Rect(65, 400, 150, 30), "Current Bid: € " + MartBidControl.currentCowBid, customTextStyle);
			
			if(!cowInRing)
			{
				if (GUI.Button (new Rect (70, 445, 170, 35), "", buttonStartBid))
				{	
					if(MartBidControl.cowsInMart.Count != 0)
					{
						GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.6f);
						GameController.Instance().cowIndex = Random.Range(0, MartBidControl.cowsInMart.Count - 1);
						MartBidControl.biddingCow = MartBidControl.cowsInMart[GameController.Instance().cowIndex];	           
						StartBidding();
					}
				}
			}
			
			if(!MartBidControl.playerBidLast && cowInRing)
			{
				if(GameController.Instance().player.cash >= MartBidControl.currentCowBid)
				{
					if(GUI.Button (new Rect (125, 445, 80, 40), "", buttonBid))
					{
						GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.6f);
						BidOnCow(null, MartBidControl.currentCowBid += 500);
						MartBidControl.playerBidLast = true;
						LookAtRing();
					}
				}
			}
			
			if (!cowInRing)
			{
				if (GUI.Button(new Rect(295, 450, 35, 35), "", buttonX))
				{
					GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.6f);
					GameController.Instance().martCowMenuUI = true;
					GameController.Instance().martCowBuyBidUI = false;
					joyStick.gameObject.SetActive(true);
					cameraControl.FollowPlayer();
				}
			}
		}
		
		private void CowSellBid(int windowID)
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
				GUI.Label(new Rect(110, 90, 150, 30), "Going In: " + MartBidControl.currentTimer + "s", customTextStyle);
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
				
				GUI.Label(new Rect(65, 400, 150, 30), "Current Bid: € " + MartBidControl.currentCowBid, customTextStyle);
				
				if (!cowInRing)
				{
					if (GUI.Button(new Rect(105, 445, 120, 40), "", buttonSell))
					{
						if (GameController.Instance().cows.Count != 0)
						{
							if (!cowInRing)
							{
								GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.6f);
								MartBidControl.biddingCow = GameController.Instance().cows[GameController.Instance().cowIndex];
								CowMaker.SpawnCow(MartBidControl.biddingCow, 109f, 137f, Vector3.zero);
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
					GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.6f);
					if(GameController.Instance().cowSelected)
					{
						GameController.Instance().cowSelected = false;
						MartBidControl.cowList.SetActive(true);
						SetHealth(0f);
						SetHappiness(0f);
					}
					else
					{
						GameController.Instance().martCowMenuUI = true;
						GameController.Instance().martCowSellBidUI = false;
						MartBidControl.cowList.SetActive(false);
						joyStick.gameObject.SetActive(true);
						cameraControl.FollowPlayer();
					}
				}
			}
		}

		// Allows bidders to bid on cows in the mart
	    public static void BidOnCow(Bidder bidder, float bid)
	    {
			// Getting camera control
	        CameraController cameraControl = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();

			if (bid >= MartBidControl.currentCowBid)
	        {
	            if(bidder != null)
	            {
					MartBidControl.playerBidLast = false;
	            	cameraControl.LookAt(bidder.gameObject.transform.position);
	            }
	              
				MartBidControl.currentCowBid = (int)bid;
				MartBidControl.lastBidder = bidder;
	            
	            EndBiddingRound();
	            StartNewRound();
	        }
	    }

	    private void StartBidding()
	    {
	        cowInRing = true;    
			MartBidControl.startingPrice = SetCowPrice(MartBidControl.biddingCow);
			MartBidControl.currentCowBid = MartBidControl.startingPrice;
			cameraControl.WatchTarget(MartBidControl.biddingCow.cowController.gameObject);
	        StartCoroutine(WaitForCow());   
	    }

	    private void StopBidding()
	    {
	        EndBiddingRound();
			MartBidControl.bidding = false;
			MartBidControl.timeRemaining = 10;

			// When selling, checks if bidding cow in players list
			if(GameController.Instance().cows.Contains(MartBidControl.biddingCow) && MartBidControl.startingPrice != MartBidControl.currentCowBid)
	        {
				GPGController.CheckAchiev_2();
				GameController.Instance().player.cash += MartBidControl.currentCowBid;
				GPGController.CheckAchiev_4(GameController.Instance().player.cash);
				GPGController.CheckAchiev_5(GameController.Instance().player.cash);
				GameController.Instance().cows.Remove(MartBidControl.biddingCow);      
	        }
			else
			{
				// Add to players cows list & Removing from cow generated list
				if(MartBidControl.playerBidLast)
				{
					GameController.Instance().player.cowsBought++;
					GPGController.CheckAchiev_1();
					GameController.Instance().player.cash -= MartBidControl.currentCowBid;
					GameController.Instance().cows.Add(MartBidControl.biddingCow);
					GPGController.CheckAchiev_3(GameController.Instance().player.cowsBought);
					GPGController.CheckAchiev_6(GameController.Instance().cows.Count);
					MartBidControl.cowsInMart.Remove(MartBidControl.biddingCow);
					biddingResult = labelBidWon;
					bidResult = true;
					StartCoroutine(WaitForSec(3));
				}
				else
				{
					// AI bidder won the bidding
					MartBidControl.cowsInMart.Remove(MartBidControl.biddingCow);
					biddingResult = labelBidLost;
					bidResult = true;
					StartCoroutine(WaitForSec(3));
				}
			}

			if(MartBidControl.playerBidLast)
	        {
				print(GameController.Instance().player.name + " has bought " + MartBidControl.biddingCow.name);
	        }
	        else
			{
				print(MartBidControl.lastBidder + " has bought " + MartBidControl.biddingCow.name);
			}

			GameController.Instance().martCowMenuUI = true;
			GameController.Instance().martCowBuyBidUI = false;
			GameController.Instance().martCowSellBidUI = false;
			GameController.Instance().cowSelected = false;
			joyStick.gameObject.SetActive(true);
			Destroy(MartBidControl.biddingCow.cowController.gameObject);
			MartBidControl.biddingCow = null;
			ClearStats();
			cameraControl.FollowPlayer();
	    }

		// Starting a new round
	    public static void StartNewRound()
	    {
			MartBidControl.timeOfLastBid = Time.time;
			MartBidControl.timeRemaining--;

			// Looping through bidders and call their method to consider bidding
			foreach (Bidder bidder in MartBidControl.bidderList)
				if (bidder != MartBidControl.lastBidder)
					bidder.ConsiderBidding(MartBidControl.biddingCow, MartBidControl.currentCowBid);
	    }

	    public static void EndBiddingRound()
	    {
			foreach (Bidder bidder in MartBidControl.bidderList)
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
					GetComponent<AudioSource> ().PlayOneShot (buttonSound, 0.6f);
					GameController.Instance().Save();
					isLoading = true;
					StartCoroutine (WaitFor(0));
				}
			}
			
			if(!isLoading)
			{
				if (GUI.Button(new Rect(410, 34, 130, 48), "", buttonFarm))
				{
					GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.6f);
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
			MartBidControl.biddingCow = new Cow();

			MartBidControl.currentTimer = 0;
			MartBidControl.currentCowBid = 0;

			// Clear all buttons && re-populate list
			CreateScrollList.RemoveAllButtons();
			GameController.Instance().scrollCowList.PopulateList();

			timerStart = false;
			timerChecked = true;
			cowInRing = false;
			MartBidControl.biddingOver = true;
			MartBidControl.playerBidLast = false;

			SetHealth(0);
			SetHappiness(0);
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

		private IEnumerator WaitForCow()
		{
			yield return new WaitForSeconds(1f);    
			MartBidControl.biddingCow.cowController.MoveTo(bidArea);
			
			while (Vector3.Distance(MartBidControl.biddingCow.cowController.ReturnPosition(), bidArea) > 2)
			{
				yield return new WaitForSeconds(1f);       
			}
			
			MartBidControl.timeOfLastBid = Time.time;
			StartNewRound();
			MartBidControl.bidding = true;
		}
		
		private IEnumerator TimerDec(int seconds) 
		{
			if(timerStart)
			{
				MartBidControl.currentTimer -= seconds;
				timerChecked = true;
			}
			
			yield return new WaitForSeconds(seconds);
			
			timerChecked = false;
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