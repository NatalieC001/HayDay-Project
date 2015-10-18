using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameController : MonoBehaviour
{
	public Farmer player;
    public Farm farm;
    public List<Cow> cows;

	public GameObject cowGameObject;

	private Vector2 farmTopLeft = new Vector2(102f, 261f);
	private Vector2 farmBottomRight = new Vector2(57.2f, 242.2f);

    void Start()
    {
		if (!GlobalVars.init) 
		{
			GlobalVars.game = this;
			GlobalVars.game.player.name = GlobalVars.playerName;
			GlobalVars.game.player.cash = 50000;
			GlobalVars.game.player.grain = 0;
			GlobalVars.game.player.hay = 0;
			GlobalVars.game.player.pellet = 0;
			GlobalVars.init = true;
		}

		if(GlobalVars.newGame)
		{
			for(int i = 0;i < 3; i++)
			{
				Cow cow = CowMaker.GenerateCow();
				if(CowMaker.SpawnCow(cow, farmTopLeft, farmBottomRight,Vector3.zero) == 1)
				{
					GlobalVars.game.cows.Add(cow);
					cow.cowController.inMart = false;
				}
			}
			
			GlobalVars.newGame = false;
		}
		else 
		{
			if(GlobalVars.loadPlayer)
			{
				Load();
				foreach (Cow cow in GlobalVars.game.cows)
				{
					if(CowMaker.SpawnCow(cow, farmTopLeft, farmBottomRight,Vector3.zero) == 1)
						cow.cowController.inMart = false;
				}
			}
		}
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

            //save player data
            file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.OpenOrCreate);
            bf.Serialize(file, GlobalVars.game.player);
            file.Close();

            //save cow data
            file = File.Open(Application.persistentDataPath + "/cows.dat", FileMode.OpenOrCreate);
			bf.Serialize(file, GlobalVars.game.cows);
            file.Close();

            //save farm data
            file = File.Open(Application.persistentDataPath + "/farm.dat", FileMode.OpenOrCreate);
			bf.Serialize(file, GlobalVars.game.farm);
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
                player = (Farmer)bf.Deserialize(file);
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

			GlobalVars.game.player = player;
			GlobalVars.game.cows = cows;
			GlobalVars.game.farm = farm;  
        }
        catch (UnityException e)
        {
            Debug.Log("Loading Failed! - " + e);
        }
    }

    [System.Serializable]
    public class Farmer
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
        [System.NonSerialized]
        public CowController cowController;
        public string name { get; set; }    
        public bool ownedByPlayer { get; set; }
        public int age { get; set; }
        public string breed { get; set; }
        public int happiness { get; set; }
        public int health { get; set; }
        public bool pregnant { get; set; }
        public bool gender { get; set; }
        public int weight { get; set; }
        public int estimatedValue { get; set; }
		
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
            ownedByPlayer = true;
        }

        public void Sell()
        {
            if (ownedByPlayer)
            {
                ownedByPlayer = false;
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