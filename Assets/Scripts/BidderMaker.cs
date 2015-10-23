using UnityEngine;
using System.Collections;

public class BidderMaker : GameController {

    public static Bidder SpawnBidder(string bidderType, Vector2 topLeft, Vector2 bottomRight)
    {
        Vector3 spawnLocation;
        int count = 0;
        do
        {
            if (count++ > 100)
            {
                return null;
            }
            spawnLocation = new Vector3(Random.Range(topLeft.x, bottomRight.x), 0, Random.Range(topLeft.y, bottomRight.y));

            spawnLocation.y = Terrain.activeTerrain.SampleHeight(spawnLocation);

        } while (Physics.CheckSphere(spawnLocation + new Vector3(0,3.5f, 0), 3));
		
         GameObject bidderGameObject = Instantiate(Resources.Load("Bidder") as GameObject);
         bidderGameObject.transform.position = spawnLocation;
         bidderGameObject.transform.LookAt(new Vector3(106f, 0f, 143f));
         return bidderGameObject.GetComponent<Bidder>(); 
    }
}