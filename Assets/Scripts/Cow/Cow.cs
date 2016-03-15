using System;

namespace IrishFarmSim
{
	[System.Serializable]
	public class Cow
	{	
		[System.NonSerialized]
		public CowController cowController;
		public string name { get; set; }    
		public int age { get; set; }
		public string breed { get; set; }
		public int happiness { get; set; }
		public int health { get; set; }
		public bool pregnant { get; set; }
		public bool gender { get; set; }
		public int weight { get; set; }
		public string currScene { get; set; }

		public Cow()
		{
			cowController = new CowController();
			this.name = "";
			this.age = 0;
			this.breed = "";
			this.happiness = 0;
			this.health = 0;
			this.pregnant = false;
			this.gender = false;
			this.weight = 0;
			this.currScene = "Mart";
		}

		public Cow(string name, int age, string breed, int happiness, int health, bool pregnant, bool gender, int weight, string currScene)
		{
			cowController = new CowController();
			this.name = name;
			this.age = age;
			this.breed = breed;
			this.happiness = happiness;
			this.health = health;
			this.pregnant = pregnant;
			this.gender = gender;
			this.weight = weight;
			this.currScene = currScene;
		}
	}
}