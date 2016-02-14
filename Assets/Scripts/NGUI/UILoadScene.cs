using UnityEngine;
using System.Collections;
using System.IO;

namespace HayDay
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
					Debug.Log ("Quitting!");
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
						bool fileTest3 = File.Exists(Application.persistentDataPath + "/farm.dat");
						
						if (fileTest1 || fileTest2 || fileTest3)
						{
							GameController.Instance().loadPlayer = true;
							Application.LoadLevel(levelName);
						}
					}
					else
					{
						if(newGame)
						{
							GameController.Instance().newGame = true;
							Application.LoadLevel(levelName);
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
	}
}