using UnityEngine;
using System.Collections;

namespace IrishFarmSim
{
	public class BackgroundMusic : MonoBehaviour 
	{
		private static BackgroundMusic instance = null;

		public static BackgroundMusic Instance 
		{
			get { return instance; }
		}

		void Awake() 
		{
			if (instance != null && instance != this) 
			{
				Destroy(this.gameObject);
				return;
			} 
			else 
			{
				instance = this;
			}

			DontDestroyOnLoad(this.gameObject);
		}
	}
}