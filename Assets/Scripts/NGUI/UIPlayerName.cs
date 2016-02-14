using UnityEngine;

namespace HayDay
{
	public class UIPlayerName : MonoBehaviour 
	{
		public void Start()
		{
			int startingMoney = 25000;
			int grain = 0;
			int hay = 0;
			int pellet = 0;

			switch(GameController.Instance().gameDifficulty)
			{
				case "Easy":
					startingMoney = 25000;
					grain = 10;
					hay = 10;
					pellet = 5;
				break;
				case "Normal":
					startingMoney = 25000;
					grain = 5;
					hay = 5;
					pellet = 5;
				break;
				case "Hard":
					startingMoney = 25000;
					grain = 5;
					hay = 1;
					pellet = 0;
				break;
				default:
					startingMoney = 25000;
					grain = 5;
					hay = 5;
					pellet = 5;
				break;
			}
			GameController.Instance().player = new Farmer ("Farmer", startingMoney, grain, hay, pellet);
		}

		public void OnChangeName()
		{
			GameController.Instance().player.name = gameObject.GetComponent<UIInput>().value;
		}
	}
}