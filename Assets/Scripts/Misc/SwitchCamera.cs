using UnityEngine;
using System.Collections;

namespace HayDay
{
	public class SwitchCamera : MonoBehaviour {

		private static Camera mainCamera;
		private static Camera outsideCamera;

		void Start () {
			//mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
			//outsideCamera = GameObject.FindGameObjectWithTag("OutsideCamera").GetComponent<Camera>();
			//mainCamera.enabled = false;
		}

		/*void OnTriggerEnter(Collider other) 
		{
			if (other.gameObject.name.Equals ("Player"))
			{
				if(mainCamera.isActiveAndEnabled)
				{
					//outsideCamera.enabled = true;
					//mainCamera.enabled = false;
				}
				else
				{
					//mainCamera.enabled = true;
					//outsideCamera.enabled = false;
				}
			}
		}*/
	}
}