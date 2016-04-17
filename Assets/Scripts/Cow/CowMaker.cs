using UnityEngine;
using System.Collections;

namespace IrishFarmSim
{
	public class CowMaker : MonoBehaviour
	{
		public static int SpawnCow(Cow cow, float PosA, float PosB, Vector3 forward)
	    {
	        Vector3 spawnLocation;
	        int count = 0;

	        do
	        {
	            if (count++ > 100)
	            {
					print("Failed to Spawn Cow!");
	                return 0;
	            }

				// Setting location to spawn & getting the y position from the terrain
				Debug.Log ("Test A: " + PosA);
				Debug.Log ("Test B: " + PosB);
				spawnLocation = new Vector3(PosA, 0f, PosB);
	            spawnLocation.y = Terrain.activeTerrain.SampleHeight(spawnLocation) + 0.5f;

			} while (Physics.CheckSphere(spawnLocation + new Vector3(0f, 4f, 0f), 2));

	        GameObject cowGameObject = Instantiate(Resources.Load(cow.breed) as GameObject);

	        if (forward != Vector3.zero)
	        {
	            cowGameObject.transform.forward = forward;
	        }

	        cowGameObject.transform.position = spawnLocation;
	        cowGameObject.transform.localScale = new Vector3(Mathf.Pow(cow.weight, .125f), Mathf.Pow(cow.weight, .125f), Mathf.Pow(cow.weight, .125f));

	        CowController cowController = cowGameObject.GetComponent<CowController>();

	        cow.cowController = cowController;
	        cowController.cow = cow;

			// Spawned cow OK, returning 1
	        return 1; 
	    }

		public static Cow GenerateCow()
		{
			// Generate new cow from random selection
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

			// Create new instance with random selection, then returning the cow instance
			Cow cow = new Cow(cowType + " - Breed", Random.Range(1, 15), cowType, Random.Range(1, 10), Random.Range(5, 100), true, true, Random.Range(150, 400), GameController.GetSceneName());
			return cow;
		}
	}
}