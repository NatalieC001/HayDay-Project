using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour 
{
	public float health = 0f;

	public static Image healthBar;
	
	void Start () 
	{
		healthBar = GetComponent<Image>();
		healthBar.transform.position = new Vector3((Screen.width / 2) + 225f, Screen.height * .166f, 0);
		//healthBar.transform.position = new Vector3(Screen.width * .29f , Screen.height * .75f, 0);
	}

	public static void SetHealth(float health)
	{
		healthBar.fillAmount = health;
	}
}