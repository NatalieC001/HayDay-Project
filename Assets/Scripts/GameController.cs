using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameController : MonoBehaviour
{
    public static GameController game;
	public static string playerName = "Joe";
	public Player player;
    public Farm farm;
    public List<Cow> cows;

	public Cow cow;
	public GameObject cowGameObject;

	public static bool loadPlayer = false;

    void Start()
    {
        game = this;

		game.player.name = playerName;
		game.player.cash = 50000;
		game.player.grain = 0;
		game.player.hay = 0;
		game.player.pellet = 0;

		if(loadPlayer)
		{
			Load();
			loadPlayer = false;
		}
    }

    void OnApplicationQuit()
    {
        Save();
    }

	void OnDisable()
	{
		Save();
	}

	void Update()
	{
		if(Input.GetKeyDown (KeyCode.Escape)) 
		{
			StartCoroutine(WaitFor(0));
		}
	}

    public void Save()
    {
        try
        {
            FileStream file;
            BinaryFormatter bf = new BinaryFormatter();

            //save player data
            file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.OpenOrCreate);
            bf.Serialize(file, game.player);
            file.Close();

            //save cow data
            file = File.Open(Application.persistentDataPath + "/cows.dat", FileMode.OpenOrCreate);
			bf.Serialize(file, game.cows);
            file.Close();

            //save farm data
            file = File.Open(Application.persistentDataPath + "/farm.dat", FileMode.OpenOrCreate);
			bf.Serialize(file, game.farm);
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
            //load player data
            if (File.Exists(Application.persistentDataPath + "/player.dat"))
            {
                file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.Open);
                player = (Player)bf.Deserialize(file);
                file.Close();
            }
            //load cow data
            if (File.Exists(Application.persistentDataPath + "/cows.dat"))
            {
                file = File.Open(Application.persistentDataPath + "/cows.dat", FileMode.Open);
                cows = (List<Cow>)bf.Deserialize(file);
                file.Close();
            }
            //load farm data
            if (File.Exists(Application.persistentDataPath + "/farm.dat"))
            {
                file = File.Open(Application.persistentDataPath + "/farm.dat", FileMode.Open);
                farm = (Farm)bf.Deserialize(file);
                file.Close();
            }

			game.player = player;
			game.cows = cows;
			game.farm = farm;

			for(int i = 0;i < game.cows.Count; i++)
			{
				Vector3 spawnLocation = new Vector3(Random.Range(55f, 105f), 0, Random.Range(220f, 265f));
				game.cows[i].gameObjectID = CowMaker.SpawnCow(game.cows[i].breed, spawnLocation);
			}
        }
        catch (UnityException e)
        {
            Debug.Log("Loading Failed! - " + e);
        }
    }

    [System.Serializable]
    public class Player
    {
        public string name;
        public double cash;
		public int grain;
		public int hay;
		public int pellet;
    }

    [System.Serializable]
    public class Cow
    {
        public string name { get; set; }
        public int gameObjectID { get; set; }
        public bool ownedByPlayer { get; set; }
        public int age { get; set; }
        public string breed { get; set; }
        public int happiness { get; set; }
        public int health { get; set; }
		public bool pregnant { get; set; }
        public bool gender { get; set; }
        public int weight { get; set; }
		public int estimatedValue { get; set; }

        public Cow(string name)
        {
            this.name = name;
        }

		public Cow(string name, int age, string breed, int happiness, int health, bool pregnant, bool gender, int weight)
        {
            this.name = name;
            this.age = age;
            this.breed = breed;
            this.happiness = happiness;
            this.health = health;
			this.pregnant = pregnant;
			this.gender = gender;
            this.weight = weight;
        }
		
        public void Buy()
        {
            if (game.player.cash >= 10000)
            {
                print("You bought " + name + "!");
                ownedByPlayer = true;
				game.player.cash -= 10000;
            }
        }
		
        public void Sell()
        {
            if (ownedByPlayer)
            {
                print("You Sold " + name + " :(");
                ownedByPlayer = false;
				game.player.cash += 10000;
            }
        }
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
		}
	}

	[System.Serializable]
	public class Farm
	{
	}
}