using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalVars : MonoBehaviour {

	public static bool playerUI;
	public static bool buySuppliesUI;
	public static bool cowUI;
	public static bool cowMoreInfoUI;
	public static bool cowFeedUI;
	public static bool loadSaveUI;
	public static bool sceneTransitionUI;
	public static bool cowSelected;
	public static bool newGame;
	public static bool loadPlayer;
	public static bool init;
	public static string playerName = "Joe";
	public static GameController game;
	public static List<GameObject> cowButtons = new List<GameObject>();
	public static int cowIndex = 0;
	public static CreateScrollList access;
}