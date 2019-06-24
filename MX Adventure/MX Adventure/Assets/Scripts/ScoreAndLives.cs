using UnityEngine;
using System.Collections;

public class ScoreAndLives : MonoBehaviour {

	//generate generateScript;

	// Use this for initialization
	void Start () {
		//generateScript = (generate)FindObjectOfType(typeof(generate));
	}
	
	// Update is called once per frame
	void Update () {
//		if(generateScript != null)
//		{
//			if(!generateScript.generating)
//			{
//				Invoke("endLevel",7f);
//			}
//		}
	}

	void endLevel()
	{
		//the level is over
		GameObject thePlayer = GameObject.Find("player");

		if(thePlayer != null)
		{
			//send them flying off the right of the screen
			thePlayer.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			thePlayer.GetComponent<Rigidbody2D>().AddForce(new Vector2(300, 0));
		}
	}

	//display score and lives
	void OnGUI () 
	{
		GUIStyle labelStyle = new GUIStyle();
		labelStyle.fontSize = 28;

		//display score
		labelStyle.normal.textColor = Color.green;
		GUI.Label(new Rect (5, 10 + gamestate.Instance.GetBannerHeight(), 100, 50), "Score: "+ gamestate.Instance.GetScore(), labelStyle);

		//display lives
		labelStyle.normal.textColor = Color.green;
		labelStyle.alignment = TextAnchor.UpperRight;
		GUI.Label(new Rect (Screen.width-110, 10 + gamestate.Instance.GetBannerHeight(), 100, 50), "Lives: "+ gamestate.Instance.getLives() +" ", labelStyle);

		//display high score
		labelStyle.normal.textColor = Color.black;
		GUI.Label(new Rect ((Screen.width-100)/2, 10 + gamestate.Instance.GetBannerHeight(), 100, 50), "Hi Score: "+ gamestate.Instance.getHighScore(), labelStyle);
	}
}
