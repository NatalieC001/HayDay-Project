using UnityEngine;

namespace HayDay
{
	public class GameDifficulty : MonoBehaviour 
	{
		public void Start()
		{
			gameObject.GetComponent<UIPopupList>().value = GameController.Instance().gameDifficulty;
		}

		public void SetDifficulty()
		{
			GameController.Instance().gameDifficulty = gameObject.GetComponent<UIPopupList>().value;
		}
	}
}