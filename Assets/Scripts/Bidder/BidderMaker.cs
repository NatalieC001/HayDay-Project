using UnityEngine;
using System.Collections;

namespace IrishFarmSim
{
	public class BidderMaker : MonoBehaviour 
	{
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

	        } while (Physics.CheckSphere(spawnLocation + new Vector3(0, 3.5f, 0), 3));

			
			GameObject bidderGameObject = Instantiate(Resources.Load(BidderPick()) as GameObject);
			bidderGameObject.transform.position = spawnLocation;
			bidderGameObject.transform.LookAt(new Vector3(106f, 0f, 143f));
			return bidderGameObject.GetComponent<Bidder>(); 
	    }

		private static string BidderPick()
		{
			int bidderGen = Random.Range(1, 4);
			
			switch(bidderGen)
			{
				case 1:
					return("Bidder 1");
				case 2:
				    return("Bidder 2");
				case 3:
				    return("Bidder 3");
				case 4:
					return("Bidder 4");
				default:
					return("Bidder 1");
			}
		}
	}
}