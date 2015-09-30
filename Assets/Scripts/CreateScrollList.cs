using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class CreateScrollList : GameController
{
	public GameObject cowButton;
	public Transform contentPanel;
	public Sprite icon;

	public static int cowIndex = 0;

	void Start () 
	{
		PopulateList();
	}

	void PopulateList()
	{
		int count = 0;

		foreach(var cow in game.cows)
		{
			++count;
			GameObject newButton = Instantiate (cowButton) as GameObject;
			CowButton genButton = newButton.GetComponent <CowButton>();
			genButton.GetComponentInChildren<Text>().text = "Cow " + count;
			genButton.name = "" + count;
			genButton.imageIcon.sprite = icon;
			genButton.transform.SetParent(contentPanel);
		}
	}

	public static void GetCowDetails(string buttonName)
	{
		UIMart.cowSelected = true;
		UIMart.cowIndex = (int.Parse(buttonName) - 1);
		UIMart.cowList.SetActive (false);
	}
}