using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour {
	
	public HighScoreController highScoreController;
	
	//Using the enum to check the state of the menu
	public enum menuScreen{
		None,
		MainMenu,
		ScoreScreen
	}
	
	public menuScreen menu=menuScreen.None;
	public string nextScene="GameScene";
	
	// Use this for initialization
	void Start () {
		InitialiseScripts();
		menu=menuScreen.MainMenu;
	}
	
	void OnGUI(){
		switch(menu){
		case (menuScreen.None):
			break;
		case (menuScreen.MainMenu):
			MainMenuGUI();
			break;
		case (menuScreen.ScoreScreen):
			ScoreScreenGUI();
			break;
		default:break;
		}
	}
	
	void InitialiseScripts(){
		if(highScoreController==null){
			highScoreController=GameObject.Find ("HighScoreObject").GetComponent<HighScoreController>();
		}
	}
	
	void MainMenuGUI(){
		if(GUI.Button (new Rect(Screen.width/9*4,Screen.height/2,120,50),"PLAY")){
			Application.LoadLevel(nextScene);
		}
		if(GUI.Button (new Rect(Screen.width/9*4,Screen.height/2+60,120,50),"HIGH SCORES")){
			menu=menuScreen.ScoreScreen;
		}
	}
	
	void ScoreScreenGUI(){
		highScoreController.showScores=true;
		if(GUI.Button (new Rect(Screen.width/9*4,Screen.height/5*4,120,50),"BACK")){
			menu=menuScreen.MainMenu;	
			highScoreController.showScores=false;
		}
	}
	
	void ActivateState(){
		
	}
}