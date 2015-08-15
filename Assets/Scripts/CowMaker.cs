using UnityEngine;
using System.Collections;

public class CowMaker : GameController
{
    public static int SpawnCow(string cowType, Vector3 spawnLocation)
    {
		GameObject cowGameObject = Instantiate(Resources.Load(cowType) as GameObject);
		
        int count = 0;

        do
		{
            if (count++ > 100)
			{
                return 0;
			}

			if (!(Application.loadedLevelName.Equals ("Mart")))
			{
            	spawnLocation = new Vector3(Random.Range(50f, 100f), 0, Random.Range(223f, 263f));
			}

            spawnLocation.y = Terrain.activeTerrain.SampleHeight(spawnLocation);

        }while (Physics.CheckSphere(spawnLocation +  new Vector3(0,3.5f,0), 3));
		
        cowGameObject.transform.position = spawnLocation;

        return cowGameObject.GetInstanceID();
    }

    public static void GenerateCow(Vector3 spawnLocation)
    {
		int cowGen = Random.Range(1, 6);
		string cowType = "Angus";

		switch(cowGen)
		{
			case 1:
				cowType = "Angus";
			break;
			case 2:
				cowType = "Brangus";
			break;
			case 3:
				cowType = "Charolais";
			break;
			case 4:
				cowType = "Hereford";
			break;
			case 5:
				cowType = "Holstein Friesian";
			break;
			case 6:
				cowType = "Shorthorn";
			break;
		}

		Cow cow = new Cow(cowType + " - Breed", Random.Range(1, 15), cowType, Random.Range(1, 10), Random.Range(5, 100), true, true, Random.Range(150, 400));

		if (Application.loadedLevelName.Equals ("Mart"))
		{
			UIMart.cowAge = cow.age;
			UIMart.breed = cow.breed;
			UIMart.happiness = cow.happiness;
			UIMart.health = cow.health;
			UIMart.preggers = cow.preggers;
			UIMart.sexMale = cow.sexMale;
			UIMart.weight = cow.weight;

			cow.gameObjectID = SpawnCow(cowType, spawnLocation);
			game.cows.Add(cow);
		}
		else
		{
			cow.gameObjectID = SpawnCow(cowType, spawnLocation);
	        game.cows.Add(cow);
		}
    }
}