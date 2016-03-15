using UnityEngine;
using System.Collections;

namespace IrishFarmSim
{
	public class Supplies : MonoBehaviour
	{
		void FixedUpdate () 
		{
			transform.Rotate (0, 2, 0);
		}

		void OnTriggerEnter(Collider other) 
		{
			if (other.gameObject.name.Equals ("Player"))
			{
				GameController.Instance().globalPlayerUI = true;
			}
		}

		void OnTriggerExit(Collider other)
		{
			if (other.gameObject.name.Equals ("Player"))
			{
				GameController.Instance().globalPlayerUI = false;
			}
		}
	}
}