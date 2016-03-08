using UnityEngine;
using System.Collections;

namespace HayDay
{
	public class MartTransition : MonoBehaviour
	{
		void FixedUpdate () 
		{
			transform.Rotate (0, 2, 0);
		}

		void OnTriggerEnter(Collider other) 
		{
			if (other.gameObject.name.Equals ("Player"))
			{
				GameController.Instance().martSceneTransitionUI = true;
			}
		}
		
		void OnTriggerExit(Collider other)
		{
			if (other.gameObject.name.Equals ("Player"))
			{
				GameController.Instance().martSceneTransitionUI = false;
			}
		}
	}
}