using UnityEngine;
using System.Collections.Generic;

namespace IrishFarmSim
{
	public class MartBidControl : MonoBehaviour 	
	{
		// Bidding control
		public static int startingPrice;
		public static int currentCowBid;
		public static int currentTimer;
		public static bool playerBidLast;
		public static bool biddingOver;
		public static Bidder lastBidder;
		public static bool bidding;
		public static float timeRemaining;
		public static float timeOfLastBid;
		public static MartUI uiMart;
		public static List<Bidder> bidderList;
		public static List<Cow> cowsInMart;
		public static GameObject cowList;
		public static Cow biddingCow;
	}
}