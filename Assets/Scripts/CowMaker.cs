using UnityEngine;
using System.Collections;

public class CowMaker : GameController
{
    public static int SpawnCow(Cow cow, Vector2 topLeft, Vector2 bottomRight, Vector3 forward)
    {
        Vector3 spawnLocation;
        int count = 0;
        do
        {
            if (count++ > 100)
            {
                print("Failed to spawn cow!");
                return 0;
            }

            spawnLocation = new Vector3(Random.Range(topLeft.x, bottomRight.x), 0, Random.Range(topLeft.y, bottomRight.y));

            spawnLocation.y =  Terrain.activeTerrain.SampleHeight(spawnLocation) + .5f;

        } while (Physics.CheckSphere(spawnLocation + new Vector3(0, 3.5f, 0), 2));
		
        GameObject cowGameObject = Instantiate(Resources.Load(cow.breed) as GameObject);

        if (forward != Vector3.zero)
        {
            cowGameObject.transform.forward = forward;
        }
        cowGameObject.transform.position = spawnLocation;

        CowController cowController = cowGameObject.GetComponent<CowController>();

        cow.cowController = cowController;
        cowController.cow = cow;

        return 1; 
    }

	public static Cow GenerateCow()
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
		return cow;
	}
}