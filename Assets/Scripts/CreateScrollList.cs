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

    public static List<GameObject> cowButtons = new List<GameObject>();

	public static int cowIndex = 0;
	public static CreateScrollList access;

	void Start () 
	{
        if (!init)
        {
            game = this;
            game.player.name = playerName;
            game.player.cash = 50000;
            game.player.grain = 0;
            game.player.hay = 0;
            game.player.pellet = 0;
            init = true;
        }

		access = this;
		PopulateList();
	}

	public void PopulateList()
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
            cowButtons.Add(newButton);
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
        Destroy(cowButtons[index]);
        cowButtons.RemoveAt(index);
    }

	public static void RemoveAllButtons()
	{
		for(int i = 0; i < cowButtons.Count; i++)
			Destroy(cowButtons[i]);
		cowButtons.Clear();
	}
}