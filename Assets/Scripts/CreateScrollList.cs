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
        game = this;
        if (!loadPlayer)
        {
            game.player.name = playerName;
            game.player.cash = 50000;
            game.player.grain = 0;
            game.player.hay = 0;
            game.player.pellet = 0;
        }
        else
        {
            Load();
            loadPlayer = false;
        }

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