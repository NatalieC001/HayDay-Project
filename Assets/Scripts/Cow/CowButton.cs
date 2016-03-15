using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace IrishFarmSim
{
	public class CowButton : MonoBehaviour 
	{
		public Button button;
		public Text buttonText;
		public Image imageIcon;

		public void WhatIsMyName()
		{
			CreateScrollList.GetCowDetails(this.gameObject.name);
		}
	}
}