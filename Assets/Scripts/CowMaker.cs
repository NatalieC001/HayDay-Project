using UnityEngine;
using System.Collections;

public class CowMaker : GameController{

    public static int SpawnCow()
    {
        GameObject cowGameObject = Instantiate(Resources.Load("Cow") as GameObject);

        Vector3 spawnLocation;
        int count=0;

        do{

            if (count++ > 100)
                return 0;

            spawnLocation = new Vector3(Random.Range(50f, 100f), 0, Random.Range(223f, 263f));

            spawnLocation.y = Terrain.activeTerrain.SampleHeight(spawnLocation);

        }while (Physics.CheckSphere(spawnLocation +  new Vector3(0,3.5f,0), 3));


        cowGameObject.transform.position = spawnLocation;

        return cowGameObject.GetInstanceID();
    }

    public static void generateCow()
    {
		Cow cow = new Cow("Carlos", 1, 1, Random.Range(1, 10), Random.Range(5, 100), true, true, 250f);

        cow.gameObjectID = SpawnCow();

        game.cows.Add(cow);
    }
}