using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HappinessBar : MonoBehaviour 
{
	public float happiness = 0f;

	public static Image happinessBar;
	
	void Start () 
	{
		happinessBar = GetComponent<Image>();
		happinessBar.transform.position = new Vector3((Screen.width / 2) + 225f, Screen.height  * .078f, 0);
		//happinessBar.transform.position = new Vector3(Screen.width * .29f , Screen.height * .75f, 0);
	}

	public static void SetHappiness(float happiness)
	{
		happinessBar.fillAmount = happiness;
	}
}