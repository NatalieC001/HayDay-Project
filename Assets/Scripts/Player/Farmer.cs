using System;

namespace IrishFarmSim
{
	[System.Serializable]
	public class Farmer
	{
		public string name { get; set; }
		public double cash { get; set; }
		public int grain { get; set; }
		public int hay { get; set; }
		public int pellet { get; set; }
		public int cowsBought { get; set; }
		public string gameDifficulty { get; set; }
		public float fxLevel { get; set; }

		public Farmer(string name, double cash, int grain, int hay, int pellet, int cowsBought, string gameDifficulty, float fxLevel)
		{
			this.name = name;
			this.cash = cash;
			this.grain = grain;
			this.hay = hay;
			this.pellet = pellet;
			this.cowsBought = cowsBought;
			this.gameDifficulty = gameDifficulty;
			this.fxLevel = fxLevel;
		}
	}
}