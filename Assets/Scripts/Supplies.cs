using UnityEngine;
using System.Collections;

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
			GlobalVars.playerUI = true;
			GlobalVars.cowUI = false;
			GlobalVars.sceneTransitionUI = false;
			GlobalVars.buySuppliesUI = false;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.name.Equals ("Player"))
		{
			GlobalVars.playerUI = false;
			GlobalVars.cowUI = false;
			GlobalVars.sceneTransitionUI = false;
			GlobalVars.buySuppliesUI = false;
		}
	}
}