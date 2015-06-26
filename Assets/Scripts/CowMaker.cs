using UnityEngine;
using System.Collections;

public class CowMaker : GameController{

    public static int SpawnCow()
    {
        GameObject cowGameObject = Instantiate(Resources.Load("Cow") as GameObject);

        Vector3 spawnLocation = new Vector3(Random.Range(50f, 100f), Random.Range(-10.0f, 10.0f), Random.Range(223f, 263f));

        spawnLocation.y = Terrain.activeTerrain.SampleHeight(spawnLocation);

        cowGameObject.transform.position = spawnLocation;

        return cowGameObject.GetInstanceID();
    }

    public static void generateCow()
    {
        Cow cow = new Cow("Carlos", 1, 1, 10, 100, true, true, 250f);

        cow.gameObjectID = SpawnCow();

        game.cows.Add(cow);
    }
}