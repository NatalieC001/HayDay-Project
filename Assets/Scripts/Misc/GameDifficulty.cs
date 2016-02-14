using UnityEngine;

namespace HayDay
{
	public class GameDifficulty : MonoBehaviour 
	{
		public void SetDifficulty()
		{
			GameController.Instance().gameDifficulty = gameObject.GetComponent<UIPopupList>().value;
		}
	}
}