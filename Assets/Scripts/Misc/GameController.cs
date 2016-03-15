using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace IrishFarmSim
{
	public class GameController : MonoBehaviour
	{
		public Farmer player;
		public List<Cow> cows;
		public GameObject cowGameObject;

		// UI & other game control variables
		public bool gcInitialized;
		public bool globalPlayerUI;
		public bool farmCowUI;
		public bool farmCowMoreInfoUI;
		public bool farmCowFeedUI;
		public bool farmSceneTransitionUI;
		public bool martSceneTransitionUI;
		public bool martCowMenuUI;
		public bool martCowBuyBidUI;
		public bool martCowSellBidUI;
		public bool globalSuppliesUI;
		public bool loadSaveUI;
		public bool cowSelected;
		public bool newGame;
		public bool loadPlayer;
		public float fxLevel = 0.5f;
		public float musicLevel = 0.5f;
		public string gameDifficulty = "Normal";
		public List<GameObject> cowButtons;
		public int cowIndex;
		public CreateScrollList scrollCowList;

		public static GameController _instance { get; private set; }

		public static GameController Instance()
		{
			if(_instance == null)
				Debug.Log("GameController is Null!");

			return _instance;
		}

		public static string GetSceneName()
		{
			return Application.loadedLevelName;
		}

		void FixedUpdate()
		{
			string scene = GetSceneName();
			
			if (scene.Equals ("Farm") || scene.Equals ("Mart"))
			{
				if(Input.GetKeyDown (KeyCode.Escape)) 
				{
					Save();
					ResetMenus();
					StartCoroutine(WaitFor(0));
				}
			}
		}

		void Awake()
		{
			if(_instance == null || _instance.gcInitialized != true)
			{
				_instance = this;
				_instance.gcInitialized = true;
			}

			AudioListener.volume = fxLevel;
			DontDestroyOnLoad(gameObject);
		}

	    void OnApplicationQuit()
	    {
	        Save();
	    }

	    public void Save()
	    {
	        try
	        {
	            FileStream file;
	            BinaryFormatter bf = new BinaryFormatter();

	            // Save player data
	            file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.OpenOrCreate);
				bf.Serialize(file, _instance.player);
	            file.Close();

	            // Save cow data
	            file = File.Open(Application.persistentDataPath + "/cows.dat", FileMode.OpenOrCreate);
				bf.Serialize(file, _instance.cows);
	            file.Close();

				Debug.Log ("Saving!");
	        }
	        catch (UnityException e)
	        {
	            Debug.Log("Saving Failed! - " + e);
	        }
	    }

	    public void Load()
	    {
	        try
	        {
	            BinaryFormatter bf = new BinaryFormatter();
	            FileStream file;

	            // Load player data
	            if (File.Exists(Application.persistentDataPath + "/player.dat"))
	            {
	                file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.Open);
					_instance.player = (Farmer)bf.Deserialize(file);
	                file.Close();
	            }
	            // Load cow data
	            if (File.Exists(Application.persistentDataPath + "/cows.dat"))
	            {
	                file = File.Open(Application.persistentDataPath + "/cows.dat", FileMode.Open);
					_instance.cows = (List<Cow>)bf.Deserialize(file);
	                file.Close();
	            }

				_instance.player = player;
				_instance.cows = cows;
				_instance.gameDifficulty = player.gameDifficulty;
				_instance.fxLevel = player.fxLevel;
				_instance.loadPlayer = false;
	        }
	        catch (UnityException e)
	        {
	            Debug.Log("Loading Failed! - " + e);
	        }
	    }

		public void ResetMenus()
		{
			_instance.globalPlayerUI = false;
			_instance.farmCowUI = false;
			_instance.farmCowMoreInfoUI = false;
			_instance.farmCowFeedUI = false;
			_instance.farmSceneTransitionUI = false;
			_instance.martSceneTransitionUI = false;
			_instance.martCowMenuUI = false;
			_instance.martCowBuyBidUI = false;
			_instance.martCowSellBidUI = false;
			_instance.globalSuppliesUI = false;
			_instance.loadSaveUI = false;
			_instance.cowSelected = false;
			_instance.newGame = false;
			_instance.loadPlayer = false;
		}

		public bool IsMenuOpen()
		{
			if(_instance.globalPlayerUI || _instance.farmCowUI || _instance.farmCowMoreInfoUI || _instance.farmCowFeedUI || _instance.farmSceneTransitionUI
			   || _instance.martSceneTransitionUI || _instance.martCowMenuUI || _instance.martCowBuyBidUI || _instance.martCowSellBidUI || _instance.globalSuppliesUI)
			{
				return true;
			}
			else
			{
				return false;
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
	}
}