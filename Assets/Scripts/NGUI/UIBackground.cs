using UnityEngine;
using System.Collections;

namespace IrishFarmSim
{
	public class UIBackground : MonoBehaviour 
	{
		private static UITexture backgroundDark;
		private static UITexture loadingTexture;
		private static UITexture nogameTexture;
		private static UITexture logo;
		private static UITexture background;
		private static UITexture buttons;
		private static UITexture nameLabel;
		private static UISprite inputField;
		private static UILabel inputFieldLabel;

		void Start()
		{
			backgroundDark = GameObject.Find("Background Dark").GetComponent<UITexture>();
			loadingTexture = GameObject.Find("Loading").GetComponent<UITexture>();
			nogameTexture = GameObject.Find("No Game").GetComponent<UITexture>();
			background = GameObject.Find("Background").GetComponent<UITexture>();
			buttons = GameObject.Find("Buttons").GetComponent<UITexture>();

			if (Application.loadedLevelName.Equals ("Main Menu")) 
			{
				logo = GameObject.Find("Logo").GetComponent<UITexture>();
			}

			if (Application.loadedLevelName.Equals ("Player Name")) 
			{
				nameLabel = GameObject.Find ("Name Label").GetComponent<UITexture>();
				inputField = GameObject.Find ("Player Name Input").GetComponent<UISprite>();
				inputFieldLabel = GameObject.Find ("Player Name Input").GetComponentInChildren<UILabel>();
			}

			backgroundDark.enabled = false;
			loadingTexture.enabled = false;
			nogameTexture.enabled = false;
		}

		public static void BackgroundDark(bool value)
		{
			backgroundDark.enabled = value;
		}

		public static void LoadingTexture(bool value)
		{
			loadingTexture.enabled = value;
		}

		public static void NogameTexture(bool value)
		{
			nogameTexture.enabled = value;
		}

		public static void Logo(bool value)
		{
			logo.enabled = value;
		}

		public static void Background(bool value)
		{
			background.enabled = value;
		}

		public static void Buttons(bool value)
		{
			buttons.enabled = value;
		}

		public static void NameLabel(bool value)
		{
			nameLabel.enabled = value;
		}

		public static void InputField(bool value)
		{
			inputField.enabled = value;
			inputFieldLabel.enabled = value;
		}
	}
}