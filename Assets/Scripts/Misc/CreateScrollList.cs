using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

namespace HayDay
{
	public class CreateScrollList : MonoBehaviour
	{
		public GameObject cowButton;
		public Transform contentPanel;
		public Sprite icon;

		void Start () 
		{
			GameController.Instance().scrollCowList = this;
			PopulateList();
		}

		public void PopulateList()
		{
			int count = 0;

			foreach(var cow in GameController.Instance().cows)
			{
				++count;
				GameObject newButton = Instantiate (cowButton) as GameObject;
				CowButton genButton = newButton.GetComponent <CowButton>();
				genButton.GetComponentInChildren<Text>().text = "Cow " + count;
				genButton.name = "" + count;
				genButton.imageIcon.sprite = icon;
				genButton.transform.SetParent(contentPanel);
				GameController.Instance().cowButtons.Add(newButton);
			}
		}

		public static void GetCowDetails(string buttonName)
		{
			GameController.Instance().cowSelected = true;
			GameController.Instance().cowIndex = (int.Parse(buttonName) - 1);
			UIMart.cowList.SetActive (false);
		}

	    public static void RemoveCowButton(int index)
	    {
			Destroy(GameController.Instance().cowButtons[index]);
			GameController.Instance().cowButtons.RemoveAt(index);
	    }

		public static void RemoveAllButtons()
		{
			for(int i = 0; i < GameController.Instance().cowButtons.Count; i++)
				Destroy(GameController.Instance().cowButtons[i]);

			GameController.Instance().cowButtons.Clear();
		}
	}
}