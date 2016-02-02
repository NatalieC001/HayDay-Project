using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace HayDay
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

		void Awake()
		{
			if(_instance == null || _instance.gcInitialized != true)
			{
				_instance = this;
				_instance.gcInitialized = true;
			}

			DontDestroyOnLoad(gameObject);
		}

	    void OnApplicationQuit()
	    {
	        Save();
	    }

		void Update()
		{
			if(Input.GetKeyDown (KeyCode.Escape)) 
			{
				Save();
				StartCoroutine(WaitFor(0));
			}
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
				_instance.loadPlayer = false;
	        }
	        catch (UnityException e)
	        {
	            Debug.Log("Loading Failed! - " + e);
	        }
	    }

		public bool isMenuOpen()
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