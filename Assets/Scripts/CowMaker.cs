using UnityEngine;
using System.Collections;

public class CowMaker : GameController
{
	public static int SpawnCow(string cowType, Vector3 spawnLocation)
	{	
		int count = 0;
		
		do
		{
			if (count++ > 1000)
			{
				return 0;
			}
			
			spawnLocation.y = Terrain.activeTerrain.SampleHeight(spawnLocation);
			
		}while (Physics.CheckSphere(spawnLocation +  new Vector3(0,3.5f,0), 3));

		GameObject cowGameObject = Instantiate(Resources.Load(cowType) as GameObject);
		cowGameObject.transform.position = spawnLocation;
		return cowGameObject.GetInstanceID();
	}

	public static Cow GenerateCow(Vector3 spawnLocation)
	{
		int cowGen = Random.Range(1, 6);
		string cowType = "Angus";
		double valueGen = 0;
		
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

		// Generate the price of the cow here
		valueGen = cow.age + cow.happiness + cow.health + cow.weight;
		cow.estimatedValue = ((int)valueGen / 5) * 1000;

		cow.gameObjectID = SpawnCow(cowType, spawnLocation);
		game.cows.Add(cow);
		return cow;
	}
}