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

	void Start () 
	{
		if (!GlobalVars.init)
        {
            GlobalVars.game = this;
			GlobalVars.game.player.name = GlobalVars.playerName;
            GlobalVars.game.player.cash = 50000;
            GlobalVars.game.player.grain = 0;
            GlobalVars.game.player.hay = 0;
            GlobalVars.game.player.pellet = 0;
			GlobalVars.init = true;
        }

		GlobalVars.access = this;
		PopulateList();
	}

	public void PopulateList()
	{
		int count = 0;

		foreach(var cow in GlobalVars.game.cows)
		{
			++count;
			GameObject newButton = Instantiate (cowButton) as GameObject;
			CowButton genButton = newButton.GetComponent <CowButton>();
			genButton.GetComponentInChildren<Text>().text = "Cow " + count;
			genButton.name = "" + count;
			genButton.imageIcon.sprite = icon;
			genButton.transform.SetParent(contentPanel);
			GlobalVars.cowButtons.Add(newButton);
		}
	}

	public static void GetCowDetails(string buttonName)
	{
		UIMart.cowSelected = true;
		UIMart.cowIndex = (int.Parse(buttonName) - 1);
		UIMart.cowList.SetActive (false);
	}

    public static void RemoveCowButton(int index)
    {
		Destroy(GlobalVars.cowButtons[index]);
		GlobalVars.cowButtons.RemoveAt(index);
    }

	public static void RemoveAllButtons()
	{
		for(int i = 0; i < GlobalVars.cowButtons.Count; i++)
			Destroy(GlobalVars.cowButtons[i]);
		GlobalVars.cowButtons.Clear();
	}
}