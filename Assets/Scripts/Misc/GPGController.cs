using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

namespace IrishFarmSim
{
	public class GPGController : MonoBehaviour 
	{
		public static void LoginIntoGPG()
		{
			Debug.Log("Checking Login");
			Social.localUser.Authenticate((bool result) => {
				Debug.Log("Logged in?: " + result);
			});
		}

		public static void PostScore(double amount){
			Debug.Log("Submitting Score");
			Social.ReportScore((long)amount, "CgkIuaLFq6kIEAIQBg", (bool result) => {
				Debug.Log("Score submitted?: " + result);
			});
		}

		public static void CheckAchiev_1()
		{
			Debug.Log("Checking CheckAchiev_1");
			Social.ReportProgress(GoogleCon.achievement_buy_your_first_cow, 100.0f, (bool result) => {
				Debug.Log("Achievement unlocked?: " + result);
			});
		}

		public static void CheckAchiev_2()
		{
			Debug.Log("Checking CheckAchiev_2");
			Social.ReportProgress(GoogleCon.achievement_sell_your_first_cow, 100.0f, (bool result) => {
				Debug.Log("Achievement unlocked?: " + result);
			});
		}

		public static void CheckAchiev_3(int cowsBought)
		{
			Debug.Log("Checking CheckAchiev_3 - Cows Bought: " + cowsBought);

			if(cowsBought < 10)
				return;

			Social.ReportProgress(GoogleCon.achievement_buy_10_cows, 100.0f, (bool result) => {
				Debug.Log("Achievement unlocked?: " + result);
			});
		}

		public static void CheckAchiev_4(double cash)
		{
			Debug.Log("Checking CheckAchiev_4 - Cash: " + cash);

			if(cash < 100000)
				return;

			Social.ReportProgress(GoogleCon.achievement_gain_100000_in_cash, 100.0f, (bool result) => {
				Debug.Log("Achievement unlocked?: " + result);
			});
		}

		public static void CheckAchiev_5(double cash)
		{
			Debug.Log("Checking CheckAchiev_5 - Cash: " + cash);

			if(cash < 50000)
				return;

			Social.ReportProgress(GoogleCon.achievement_gain_50000_in_cash, 100.0f, (bool result) => {
				Debug.Log("Achievement unlocked?: " + result);
			});
		}

		public static void CheckAchiev_6(int herdSize)
		{
			Debug.Log("Checking CheckAchiev_6 - Herd Size: " + herdSize);

			if(herdSize < 10)
				return;

			int step = 0;

			if (herdSize == 10) 
			{
				step = 1;
			}
			else if(herdSize >= 20)
			{
				step = 2;
			}

			PlayGamesPlatform.Instance.IncrementAchievement(
				GoogleCon.achievement_have_20_cows_in_your_herd, step, (bool result) => {
				Debug.Log("Achievement unlocked?: " + result);
			});
		}
	}
}