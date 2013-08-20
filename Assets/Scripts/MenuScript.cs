using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour {
	
	public HighScoreScript highScoreScript;
	
	public enum menuScreen{
		MainMenu,
		ScoreScreen
	}
	
	public menuScreen menu=menuScreen.MainMenu;
	
	// Use this for initialization
	void Start () {
		InitialiseScripts();
	}
	
	void OnGUI(){
		switch(menu){
		case (menuScreen.MainMenu):
			MainMenuGUI();
			break;
		case (menuScreen.ScoreScreen):
			ScoreScreenGUI();
			break;
		default:break;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void InitialiseScripts(){
		if(highScoreScript==null){
			highScoreScript=GameObject.Find ("HighScoreObject").GetComponent<HighScoreScript>();
		}
	}
	
	void MainMenuGUI(){
		if(GUI.Button (new Rect(Screen.width/9*4,Screen.height/2,120,50),"PLAY")){
			Application.LoadLevel("GameScene");
		}
		if(GUI.Button (new Rect(Screen.width/9*4,Screen.height/2+60,120,50),"HIGH SCORES")){
			menu=menuScreen.ScoreScreen;
		}
	}
	
	void ScoreScreenGUI(){
		highScoreScript.showScores=true;
		if(GUI.Button (new Rect(Screen.width/9*4,Screen.height/5*4,120,50),"BACK")){
			menu=menuScreen.MainMenu;	
			highScoreScript.showScores=false;
		}
	}
}
