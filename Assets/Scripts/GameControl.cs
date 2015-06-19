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


    GameObject cow;

    Vector3 spawnLocation;
    

    void Awake()
    {
        if(control == null)
        {
            DontDestroyOnLoad(gameObject);
            control = this;
            Load();
            //control.player.name = "Farmer Joe";
            //control.player.cash = 400;
            //Save();
            
        }
        else if(control != this)
        {
            Destroy(gameObject);
        }
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
            buy();

        if (GUI.Button(new Rect(120, 60, 40, 20), "Sell"))
            Sell();
        

        if (GUI.Button(new Rect(180, 20, 40, 20), "Load"))
            Load();

        if (GUI.Button(new Rect(180, 60, 40, 20), "Save"))
            Save();

    }

    public void buy()
    {
        control.player.cash -= 100;

        cow = Resources.Load("Cow") as GameObject;
        Instantiate(cow);     
    }

    void Sell()
    {
        control.player.cash += 100;
    }
    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.OpenOrCreate);

        Player player = new Player();
        player.name = control.player.name;
        player.cash = control.player.cash;

        bf.Serialize(file, player);
        file.Close();
    }
     
    public void Load()
    {
        if(File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);

            Player player = (Player)bf.Deserialize(file);
            file.Close();

            control.player.name = player.name;

            control.player.cash = player.cash;
        }
    }


    [System.Serializable]
    public class Player
    {
        public string name;
        public double cash;
    }

    [System.Serializable]
    public class Cow
    {
        int age { get; set; }

        int breed { get; set; }

        int happiness { get; set; }

        int health { get; set; }

        bool preggers { get; set; }

        bool sex { get; set; }

        float weight { get; set; }
    }


    [System.Serializable]
    public class Farm
    {
        //not sure what to put here yet
    }
}