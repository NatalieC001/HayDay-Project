using System;

namespace HayDay
{
	[System.Serializable]
	public class Farmer
	{
		public string name { get; set; }
		public double cash { get; set; }
		public int grain { get; set; }
		public int hay { get; set; }
		public int pellet { get; set; }

		public Farmer(string name, double cash, int grain, int hay, int pellet)
		{
			this.name = name;
			this.cash = cash;
			this.grain = grain;
			this.hay = hay;
			this.pellet = pellet;
		}
	}
}