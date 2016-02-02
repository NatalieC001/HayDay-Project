using System;

namespace HayDay
{
	[System.Serializable]
	public class Cow
	{	
		[System.NonSerialized]
		public CowController cowController;
		public string name { get; set; }    
		public bool ownedByPlayer { get; set; }
		public int age { get; set; }
		public string breed { get; set; }
		public int happiness { get; set; }
		public int health { get; set; }
		public bool pregnant { get; set; }
		public bool gender { get; set; }
		public int weight { get; set; }
		public int estimatedValue { get; set; }

		public Cow(string name, int age, string breed, int happiness, int health, bool pregnant, bool gender, int weight)
		{
			this.name = name;
			this.age = age;
			this.breed = breed;
			this.happiness = happiness;
			this.health = health;
			this.pregnant = pregnant;
			this.gender = gender;
			this.weight = weight;
		}
	}
}