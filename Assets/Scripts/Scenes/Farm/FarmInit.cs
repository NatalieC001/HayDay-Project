using UnityEngine;

namespace IrishFarmSim
{
	public class FarmInit : MonoBehaviour
	{
		// Cow default spawn info & location area
		private Vector2 farmTopLeftPosA = new Vector2(102f, 261f);
		private Vector2 farmBottomRightPosA = new Vector2(57f, 242f);
		private Vector2 farmTopLeftPosB = new Vector2(102f, 333f);
		private Vector2 farmBottomRightPosB = new Vector2(73f, 309f);
		private Vector2 farmTopLeftPosC = new Vector2(246f, 267f);
		private Vector2 farmBottomRightPosC = new Vector2(230f, 244f);
		// Default values
		private Vector2 farmTopLeftPos = new Vector2(102f, 261f);
		private Vector2 farmBottomRightPos = new Vector2(57f, 242f);

		void Start()
		{
			try
			{
				if(GameController.Instance().newGame)
				{
					InitNewGame();
				}
				else 
				{
					LoadGame();
				}
			}
			catch (UnityException e)
			{
				Debug.Log("Error - " + e);
			}
		}

		// Initialization of new game
		private void InitNewGame()
		{
			GameController.Instance().cows.Clear();
			int cowAmount;

			// Check what difficulty the game is set to, set correct stats to player character
			switch(GameController.Instance().gameDifficulty)
			{
				case "Easy":
					cowAmount = 5;
					break;
				case "Normal":
					cowAmount = 3;
					break;
				case "Hard":
					cowAmount = 2;
					break;
				default:
					cowAmount = 3;
					break;
			}
			
			for(int i = 0;i < cowAmount; i++)
			{
				// Generate cow instance, spawn cow with location variables
				Cow cow = CowMaker.GenerateCow();
				GetRandomPos();
				// Using location variables to spawn cows
				if(CowMaker.SpawnCow(cow, Random.Range(farmTopLeftPos.x, farmBottomRightPos.x), Random.Range(farmTopLeftPos.y, farmBottomRightPos.y), Vector3.zero) == 1)
					GameController.Instance().cows.Add(cow);
			}
			
			GameController.Instance().newGame = false;
		}

		// Initialization of new game
		private void LoadGame()
		{
			// Else load player data from file
			if(GameController.Instance().loadPlayer)
			{
				// Loading from file & spawning cows by looping through loaded list of cows
				GameController.Instance().Load();
				
				foreach (Cow cow in GameController.Instance().cows)
				{
					GetRandomPos();
					// Using location variables to spawn cows & resetting current scene then spawn
					cow.currScene = GameController.GetSceneName();
					if(CowMaker.SpawnCow(cow, Random.Range(farmTopLeftPos.x, farmBottomRightPos.x), Random.Range(farmTopLeftPos.y, farmBottomRightPos.y), Vector3.zero) == 1)
						cow.cowController.Wait();
				}
			}
		}

		// Generate a random position for cows to spawn
		private void GetRandomPos()
		{
			int randomPos = Random.Range(0, 3);
			
			switch(randomPos)
			{
				case 0:
					farmTopLeftPos = farmTopLeftPosA;
					farmBottomRightPos = farmBottomRightPosA;
					break;
				case 1:
					farmTopLeftPos = farmTopLeftPosB;
					farmBottomRightPos = farmBottomRightPosB;
					break;
				case 2:
					farmTopLeftPos = farmTopLeftPosC;
					farmBottomRightPos = farmBottomRightPosC;
					break;
				default:
					farmTopLeftPos = farmTopLeftPosA;
					farmBottomRightPos = farmBottomRightPosA;
					break;
			}
		}
	}
}