using UnityEngine;
using System.Collections;
using System.IO;

namespace IrishFarmSim
{
	public class UILoadScene : MonoBehaviour 
	{
		public string levelName;
		public bool loadPlayer;
		public bool newGame;

		void FixedUpdate() 
		{
			if (Input.GetKeyDown (KeyCode.Escape)) 
			{
				if(Application.loadedLevelName.Equals("Main Menu"))
				{
					StartCoroutine(Load("Quit"));
				}
				else
				{
					StartCoroutine(Load("Main Menu"));
				}
			}
		}

		public void OnClick()
		{
			StartCoroutine(Load(levelName));
		}

		private IEnumerator Load(string levelName) 
		{
			yield return new WaitForSeconds(0.7f);

			if (!string.IsNullOrEmpty(levelName))
			{
				if(levelName.Equals("Quit"))
				{
					Application.Quit();
				}
				else if(levelName.Equals("Farm"))
				{
					if(loadPlayer)
					{
						bool fileTest1 = File.Exists(Application.persistentDataPath + "/player.dat");
						bool fileTest2 = File.Exists(Application.persistentDataPath + "/cows.dat");
						
						if (fileTest1 || fileTest2)
						{
							GameController.Instance().loadPlayer = true;
							UIBackground.BackgroundDark(true);
							UIBackground.LoadingTexture(true);
							UIBackground.Background(false);
							UIBackground.Logo(false);
							UIBackground.LoadingTexture(true);
							StartCoroutine(DelayLoadScene(1, levelName));
						}
						else
						{
							StartCoroutine(DelayReset(2));
							UIBackground.Logo(false);
							UIBackground.BackgroundDark(true);
							UIBackground.NogameTexture(true);
							UIBackground.Background(false);
						}
					}
					else
					{
						if(newGame)
						{
							GameController.Instance().newGame = true;
							UIBackground.BackgroundDark(true);
							UIBackground.LoadingTexture(true);
							UIBackground.Background(false);
							UIBackground.NameLabel(false);
							UIBackground.InputField(false);
							StartCoroutine(DelayLoadScene(1, levelName));
						}
					}
				}
				else
				{
					NGUITools.Broadcast("End");
					Application.LoadLevel(levelName);
				}
			}
		}

		private IEnumerator DelayReset(int seconds) 
		{
			yield return new WaitForSeconds(seconds);
			UIBackground.BackgroundDark(false);
			UIBackground.NogameTexture(false);
			UIBackground.Logo(true);
			UIBackground.Background(true);
		}

		private IEnumerator DelayLoadScene(int seconds, string levelName) 
		{
			yield return new WaitForSeconds(seconds);
			Application.LoadLevel(levelName);
		}
	}
}