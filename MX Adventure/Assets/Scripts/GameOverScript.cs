using UnityEngine;
using System.Collections;

public class GameOverScript : MonoBehaviour {
	public GUIStyle customGuiStyle;
	public Sprite[] Backgrounds;
	public GUISkin MenuSkin;

	int logoX;
	int logoY;
	private string fbMessage = "";
	private System.Exception fbException;
	private bool sharedToFB = false;

	void Awake()
	{
		
	}

	private void SetInit()
	{
		enabled = true;
		
	}

	private void OnHideUnity(bool isGameShown)
	{
		if(!isGameShown)
		{
			Time.timeScale = 0;
		}
		else
			Time.timeScale = 1;
	}

	void OnLoggedIn()
	{

	}


	void Start()
	{
		gamestate.Instance.SetGameRunning(false);
		
		try {
			//save gamestate to player prefs
			gamestate.Instance.Save("GameState");
		} catch (System.Exception ex) {
			string message = ex.Message;
		}

		//set background
		GameObject.FindGameObjectWithTag("background").GetComponent<SpriteRenderer>().sprite = Backgrounds[gamestate.Instance.getActiveLevel() - 1];
	}

	void OnStart()
	{
//		gamestate.Instance.SetGameRunning(false);
//		
//		try {
//			//save gamestate to player prefs
//			gamestate.Instance.Save("GameState");
//		} catch (System.Exception ex) {
//			string message = ex.Message;
//		}

		customGuiStyle = new GUIStyle();
		
		customGuiStyle.font = (Font)Resources.Load("Fonts/advlit");
		customGuiStyle.active.textColor = Color.red; // not working
		customGuiStyle.hover.textColor = Color.blue; // not working
		customGuiStyle.normal.textColor = Color.green;
		customGuiStyle.fontSize = 50;
		
		customGuiStyle.stretchWidth = true; // ---
		customGuiStyle.stretchHeight = true; // not working, since backgrounds aren't showing
	}

	// Update is called once per frame
	void Update () {


	}

	void OnGUI()
	{
		logoX = (Screen.width - 300 ) / 2;
		logoY = (Screen.height - 450) / 2;

		//logo
		//drop shadow
		customGuiStyle.normal.textColor = Color.black;
		GUI.Box(new Rect( logoX+3, logoY+3, 450, 30 ), "Game Over!" ,customGuiStyle);
		
		customGuiStyle.normal.textColor = Color.green;
		GUI.Box(new Rect( logoX, logoY, 450, 30 ), "Game Over!" ,customGuiStyle);

		GUI.skin = MenuSkin;
		
		//if (!FB.IsLoggedIn)
		//{			
		//	if (GUI.Button(new Rect(
		//		(Screen.width-200)/2,
		//		Screen.height - 65,
		//		200,
		//		60
		//		), "", MenuSkin.GetStyle("button_login")))
		//	{
		//		try {
		//			FB.Login("publish_actions", LoginCallback);
		//		} catch (System.Exception ex) {
		//			fbException = ex;
		//			fbMessage = ex.Message;
					
		//		}
		//	}
		//}
		//else if(!sharedToFB) //display share dialog
		//{
		//	try {
		//		//check for a new highscore and modify message
		//		string newHighScoreMessage = "";
		//		string newHigherLevelMessage = "";
				
		//		if(gamestate.Instance.NewHighScore())
		//		{
		//			newHighScoreMessage = "I just got a new High Score on MX Adventure: "+ gamestate.Instance.GetScore() +"! Can you beat it?";
		//		}
		//		else if(gamestate.Instance.NewHigherLevel())
		//		{
		//			newHigherLevelMessage = "I just reached level "+ gamestate.Instance.getLevelProgress() +"! Can you beat it?";
		//		}
		//		else
		//			newHighScoreMessage = "I got a score of "+ gamestate.Instance.GetScore() +" on MX Adventure!";

				
				
				
		//		FB.Feed(
		//			linkCaption: newHighScoreMessage + newHigherLevelMessage,
		//			//picture: "http://www.promotocross.com/sites/default/files/WPS_FLYracing_logos_640x120_v3.jpg",
		//			linkName: "Checkout my MX Skills!",
		//			link: "https://play.google.com/store/apps/details?id=com.SnickleFritz.MXAdventure"
		//			);
				
		//		sharedToFB = true;
		//	} catch (System.Exception ex) {
		//		print(ex.Message);
		//	}
		//}
	}
}
