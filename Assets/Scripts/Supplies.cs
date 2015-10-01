using UnityEngine;
using System.Collections;

public class Supplies : MonoBehaviour
{
	UIFarm userInterface;

	void FixedUpdate () 
	{
		transform.Rotate (0, 2, 0);
	}

	void Start()
	{
		userInterface = GameObject.FindGameObjectWithTag("UI").GetComponent<UIFarm>();
	}

	void OnTriggerEnter(Collider other) 
	{
		if (other.gameObject.name.Equals ("Player"))
		{
			userInterface.playerUI = true;
			userInterface.cowUI = false;
			userInterface.sceneTransitionUI = false;
			userInterface.buySuppliesUI = false;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.name.Equals ("Player"))
		{
			userInterface.playerUI = false;
			userInterface.cowUI = false;
			userInterface.sceneTransitionUI = false;
			userInterface.buySuppliesUI = false;
		}
	}
}