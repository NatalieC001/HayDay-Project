using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

namespace IrishFarmSim
{
	public class CheckGC : MonoBehaviour {

		void Start()
		{
			CheckGCExists("GameController");
		}
		
		void Awake()
		{
			CheckGCExists("GameController");
		}

		private void CheckGCExists(string gcName)
		{
			// Checks if the game game controller exists
			if(GameObject.Find(gcName) == null)
			{
				GameObject cowGameObject = Instantiate(Resources.Load(gcName) as GameObject);
				cowGameObject.name = gcName;
				CallGPG();
			}

			// Asking user to login into google play services
			GPGController.LoginIntoGPG();
		}

		private void CallGPG()
		{
			PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
			PlayGamesPlatform.InitializeInstance(config);
			// recommended for debugging:
			PlayGamesPlatform.DebugLogEnabled = true;
			// Activate the Google Play Games platform
			PlayGamesPlatform.Activate();
		}
	}
}