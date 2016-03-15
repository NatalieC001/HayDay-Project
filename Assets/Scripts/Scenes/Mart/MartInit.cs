using UnityEngine;
using System.Collections.Generic;

namespace IrishFarmSim
{
	public class MartInit : MonoBehaviour 	
	{
		// Spawn location for bidders
		public Vector2 martTopRight = new Vector2(110.28f, 142.24f);
		public Vector2 martBottomLeft = new Vector2(99.56f, 147f);
		public Vector2 martTopLeftOutside = new Vector2(197f, 226f);
		public Vector2 martBottomRightOutside = new Vector2(128f, 252f);
		
		// Spawn location for cows
		public Vector2 bottomLeft = new Vector2(128f, 116.82f);
		public Vector2 topRight = new Vector2(95.14f, 116.82f);
		public Vector3 forward = new Vector3(0,0,1);

		void Start()
		{
			try
			{
				MartBidControl.bidderList = new List<Bidder>();

				for (int i = 0; i < Random.Range(3, 5); i++) 
				{
					Bidder newBidder = BidderMaker.SpawnBidder ("", martTopRight, martBottomLeft);
					MartBidControl.bidderList.Add(newBidder); 
				}
				
				for (int i = 0; i < Random.Range(10, 15); i++)
				{
					Cow newCow = CowMaker.GenerateCow();
					CowMaker.SpawnCow(newCow, Random.Range(martTopLeftOutside.x, martBottomRightOutside.x), Random.Range(martTopLeftOutside.y, martBottomRightOutside.y), forward);
				}

				MartBidControl.cowsInMart = new List<Cow>();
				
				// Spawn new cows in the mart & store them in a list
				for (int i = 0; i < Random.Range(5, 8); i++)
				{
					Cow newCow = CowMaker.GenerateCow();
					if(CowMaker.SpawnCow(newCow, Random.Range(bottomLeft.x, topRight.x), Random.Range(bottomLeft.y, topRight.y), forward) == 1)
					{
						try
						{
							newCow.cowController.Wait();
							MartBidControl.cowsInMart.Add(newCow);
						}
						catch(System.Exception error)
						{
							Debug.Log("Error: " + error);
						}
					}
				}
			}
			catch (UnityException e)
			{
				Debug.Log("Error - " + e);
			}
		}
	}
}