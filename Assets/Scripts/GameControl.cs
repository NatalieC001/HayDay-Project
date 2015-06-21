using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameControl : MonoBehaviour
{
    public static GameControl control;

    public Player player;
    public List<Cow> cows;
    public Farm farm;

    Vector3 spawnLocation;

    void Awake()
    {
        if (control == null)
        {
            DontDestroyOnLoad(gameObject);
            control = this;
            Load();
            //control.player.name = "Farmer Joe";
            //control.player.cash = 500;
        }
        else if (control != this)
        {
            Destroy(gameObject);
        }
    }

	void OnApplicationQuit() {
		Save ();
	}

    void OnGUI()
    {
        Rect windowRect = new Rect(20, 10, 230, 100);
        windowRect = GUI.Window(0, windowRect, userInterface, control.player.name);
    }

    void userInterface(int windowID)
    {
        GUI.Label(new Rect(10, 20, 150, 30), "Cash: €" + control.player.cash);

        if (GUI.Button(new Rect(120, 20, 40, 20), "Buy"))
            Buy();

        if (GUI.Button(new Rect(120, 60, 40, 20), "Sell"))
            Sell();
		
        if (GUI.Button(new Rect(180, 20, 40, 20), "Load"))
            Load();

        if (GUI.Button(new Rect(180, 60, 40, 20), "Save"))
            Save();
    }

    public void Buy()
    {
        try
		{
			if (control.player.cash > 0)
	        { 
	            control.player.cash -= 100;

				SpawnCow(Instantiate(Resources.Load("Cow")) as GameObject);
			}
        }
		catch(UnityException e)
		{
			Debug.Log ("Buying Failed! - " + e);
		}
    }

    void Sell()
	{
		try
		{
			if(cows.Count > 0)
			{
				control.player.cash += 100;
				Destroy(cows[cows.Count - 1].cowGameObject, .5f);
				cows.RemoveAt(cows.Count - 1);
			}
		}
		catch(UnityException e)
		{
			Debug.Log ("Selling Failed! - " + e);
		}
	}

    public void Save()
    {
        try
		{
			BinaryFormatter bf = new BinaryFormatter();
	        FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.OpenOrCreate);

	        Player player = new Player();
	        player.name = control.player.name;
	        player.cash = control.player.cash;

			player.cows = new List<Cow>();

			Debug.Log ("Number in list: " + cows.Count);

			if(cows.Count > 0)
			{
				//if(player.cows.Count > 0)
				{
					//player.cows.Clear();
				
					for(int i = 0;i > cows.Count;i++)
						player.cows.Add(cows[i]);
				}
			}

	        bf.Serialize(file, player);
	        file.Close();
		}
		catch(UnityException e)
		{
			Debug.Log ("Saving Failed! - " + e);
		}
    }

    public void Load()
    {
		try
		{
	        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
	        {
	            BinaryFormatter bf = new BinaryFormatter();
	            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);

	            Player player = (Player)bf.Deserialize(file);
	            file.Close();

	            control.player.name = player.name;
	            control.player.cash = player.cash;

				Debug.Log ("Number in list: " + player.cows.Count);

				if(player.cows != null)
				{
					for(int i = 0;i > player.cows.Count;i++)
					{
						cows.Add(cows[i]);
						SpawnCow(cows[i].cowGameObject);
					}
				}
	        }
		}
		catch(UnityException e)
		{
			Debug.Log ("Loading Failed! - " + e);
		}
    }

	public void SpawnCow(GameObject cowGameObject)
	{
		spawnLocation = new Vector3 (Random.Range (50f, 100f), Random.Range (-10.0f, 10.0f), Random.Range (223f, 263f));
		
		spawnLocation.y = Terrain.activeTerrain.SampleHeight(spawnLocation);
		
		cowGameObject.transform.position = spawnLocation;
		
		Cow cow = new Cow("Tom", cowGameObject, 1, 1, 10, 100, true, true, 250f);
		
		cows.Add(cow);
	}

    [System.Serializable]
    public class Player
    {
        public string name;
        public double cash;
		public List<Cow> cows;
    }

    [System.Serializable]
    public class Cow
    {
        public GameObject cowGameObject { get; set; }
        public string name { get; set; }
        public int age { get; set; }
        public int breed { get; set; }
        public int happiness { get; set; }
        public int health { get; set; }
        public bool preggers { get; set; }
        public bool sexMale { get; set; }
        public float weight { get; set; }

        public Cow(string name, GameObject cow)
        {
            this.name = name;
            this.cowGameObject = cow;
        }

        public Cow(string name, GameObject cow, int age, int breed, int happiness, int health, bool preggers, bool sexMale, float weight)
        {
            this.name = name;
            this.cowGameObject = cow;
            this.age = age;
            this.breed = breed;
            this.happiness = happiness;
            this.health = health;
            this.sexMale = sexMale;
            this.preggers = preggers;
            this.weight = weight;
        }
    }

    [System.Serializable]
    public class Farm
    {
        // Maybe save state of the farm here, the amount of cows the player has...etc
    }
}