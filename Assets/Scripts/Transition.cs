using UnityEngine;
using System.Collections;

public class Transition : MonoBehaviour
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
		userInterface.playerUI = false;
		userInterface.cowUI = false;
		userInterface.loadSaveUI = false;
		userInterface.sceneTransitionUI = true;
	}
}