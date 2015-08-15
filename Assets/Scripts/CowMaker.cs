using UnityEngine;
using System.Collections;

public class CowMaker : GameController
{

    public static int spawnCow(string breed, Vector2 topLeft, Vector2 bottomRight)
    {
        Vector3 spawnLocation;
        int count = 0;
        do
        {
            if (count++ > 100)
            {
                return 0;
<<<<<<< HEAD
            }
            print(topLeft.x + " " + bottomRight.y);
            spawnLocation = new Vector3(Random.Range(topLeft.x, bottomRight.x), 0, Random.Range(topLeft.y, bottomRight.y));
=======
			}

			if (!(Application.loadedLevelName.Equals ("Mart")))
			{
            	spawnLocation = new Vector3(Random.Range(50f, 100f), 0, Random.Range(223f, 263f));
			}
>>>>>>> origin/master

            spawnLocation.y = Terrain.activeTerrain.SampleHeight(spawnLocation);

        } while (Physics.CheckSphere(spawnLocation + new Vector3(0, 3.5f, 0), 3));


        GameObject cowGameObject = Instantiate(Resources.Load(breed) as GameObject);
        cowGameObject.transform.position = spawnLocation;     
        return cowGameObject.GetInstanceID();
    }


    public static Cow generateCow()
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

<<<<<<< HEAD
		Cow cow = new Cow(cowType + " - Breed", Random.Range(1, 15), cowType, Random.Range(1, 10), Random.Range(5, 100), true, true, Random.Range(250f, 600f));
		
		game.cows.Add(cow);
        return cow;
=======
		Cow cow = new Cow(cowType + " - Breed", Random.Range(1, 15), cowType, Random.Range(1, 10), Random.Range(5, 100), true, true, Random.Range(150, 400));
>>>>>>> origin/master

    }
}