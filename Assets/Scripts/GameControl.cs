using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameControl : MonoBehaviour
{
    public static GameControl control;



    public Player player;
    public Farm farm;
    public Dictionary<int, Cow> cowIndex;
    Vector3 spawnLocation;

    void Awake()
    {
        if (control == null)
        {
            DontDestroyOnLoad(gameObject);
            control = this;
            cowIndex = new Dictionary<int, Cow>();
            //Load();
            //control.player.name = "Farmer Joe";
            control.player.cash = 500;
        }
        else if (control != this)
        {
            Destroy(gameObject);
        }
    }

    void OnApplicationQuit()
    {
        Save();
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
            //	if (control.player.cash > 0)
            {
                control.player.cash -= 100;
                //Prolly need a method to randomly create a cow here
                Cow cow = new Cow("Carlos", 1, 1, 10, 100, true, true, 250f);
                SpawnCow(cow);
            }
        }
        catch (UnityException e)
        {
            Debug.Log("Buying Failed! - " + e);
        }
    }

    void Sell()
    {
        try
        {
            if (cowIndex.Values.Count > 0)
            {
                control.player.cash += 100;

                //Need to get the instanceID of the cow in order to delete/sell them now
                // Destroy(cowIndex(selectedCowGameObject.instanceID));

                //   cowIndex.Remove(selectedCowGameObject.instanceID);
                //cows.RemoveAt(cows.Count - 1);
            }
        }
        catch (UnityException e)
        {
            Debug.Log("Selling Failed! - " + e);
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

            Debug.Log("Number in list: " + cowIndex.Values.Count);

            foreach (Cow cow in cowIndex.Values)
                player.cows.Add(cow);


            bf.Serialize(file, player);
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
            if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);

                Player player = (Player)bf.Deserialize(file);
                file.Close();

                control.player.name = player.name;
                control.player.cash = player.cash;

                Debug.Log("Number in list: " + cowIndex.Values.Count);

                if (player.cows != null)
                {
                    foreach (Cow cow in player.cows)
                    {
                        SpawnCow(cow);
                    }
                }
            }
        }
        catch (UnityException e)
        {
            Debug.Log("Loading Failed! - " + e);
        }
    }

    public void SpawnCow(Cow cow)
    {
        GameObject cowGameObject = Instantiate(Resources.Load("Cow") as GameObject);

        cowIndex.Add(cowGameObject.GetInstanceID(), cow);

        spawnLocation = new Vector3(Random.Range(50f, 100f), Random.Range(-10.0f, 10.0f), Random.Range(223f, 263f));

        spawnLocation.y = Terrain.activeTerrain.SampleHeight(spawnLocation);

        cowGameObject.transform.position = spawnLocation;
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
        public string name { get; set; }
        public int age { get; set; }
        public int breed { get; set; }
        public int happiness { get; set; }
        public int health { get; set; }
        public bool preggers { get; set; }
        public bool sexMale { get; set; }
        public float weight { get; set; }

        public Cow(string name)
        {
            this.name = name;
        }

        public Cow(string name, int age, int breed, int happiness, int health, bool preggers, bool sexMale, float weight)
        {
            this.name = name;
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