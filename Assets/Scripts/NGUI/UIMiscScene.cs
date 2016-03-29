using UnityEngine;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;

namespace IrishFarmSim
{
	public class UIMiscScene : MonoBehaviour 
	{
		public bool loadAchievements;

		public void OnClick()
		{
			if(loadAchievements)
			{
				OpenA();
			}
			else
			{
				OpenL();
			}
		}

		public void OpenA()
		{
			Social.ShowAchievementsUI();
		}

		public void OpenL()
		{
			Social.ShowLeaderboardUI();
		}
	}
}