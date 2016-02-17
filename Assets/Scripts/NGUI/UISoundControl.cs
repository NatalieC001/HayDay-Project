using UnityEngine;
using System.Collections;

namespace HayDay
{
	public class UISoundControl : MonoBehaviour 
	{
		public void Start()
		{
			if(gameObject.name.Equals("FX Slider"))
			{
				gameObject.GetComponent<UISlider>().value = GameController.Instance().fxLevel;
			}

			if(gameObject.name.Equals("Music Slider"))
			{
				gameObject.GetComponent<UISlider>().value = GameController.Instance().musicLevel;
			}
		}

		public void FXLevelChange()
		{
			GameController.Instance().fxLevel = gameObject.GetComponent<UISlider>().value;
			AudioListener.volume = GameController.Instance().fxLevel;

			if(GameController.Instance().fxLevel < 0.05f)
			{
				GameController.Instance().fxLevel = 0.05f;
				gameObject.GetComponent<UISlider>().value = GameController.Instance().fxLevel;
			}
		}

		public void MusicLevelChange()
		{
			GameController.Instance().musicLevel = gameObject.GetComponent<UISlider>().value;
			//AudioListener.volume = GameController.Instance().musicLevel;

			if(GameController.Instance().musicLevel < 0.05f)
			{
				GameController.Instance().musicLevel = 0.05f;
				gameObject.GetComponent<UISlider>().value = GameController.Instance().musicLevel;
			}
		}
	}
}