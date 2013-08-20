using UnityEngine;
using System.Collections;

public class HighScoreScript : MonoBehaviour {
	
	internal static bool created=false;
	public static HighScoreScript instance;
	
	public int score=0;
	
	public GUIStyle labelStyle;
	
	private HighScore[] topScores;
	private HighScore roundScore;
	
	public bool askForName;
	public bool submitted=false;
	public bool checkedTopTen=false;
	
	public bool showScores=false;
	
	// # SINGLETON
	
	public static HighScoreScript getInstance(){
		return instance;
	}
	
	void Awake(){
		if(!created){
			DontDestroyOnLoad(gameObject);
			created=true;
			instance=this;
		}
		else{
			Destroy(gameObject);
		}
	}

	void Start () {
		InitScores ();
		if(PlayerPrefs.GetString("HighScoreName1")!=""){
			LoadScoresFromPrefs();
		}
	}
	
	void OnGUI(){
		if(askForName){
			GUI.Box (new Rect(Screen.width/3,Screen.height/3,250,180),"");
			roundScore.name = GUI.TextField(new Rect(Screen.width/3+30, Screen.height/3+20, 200, 20), roundScore.name , 25);
			if(GUI.Button (new Rect(Screen.width/3+140,Screen.height/3+70,80,50),"Submit")){
				roundScore.val=score;
				NewScore(roundScore);
				submitted=true;			
			}
		}
		if(showScores){
			ShowScores();
		}
	}
	
	// What's in the update methos is used to avoid a bug when exiting the menu while submitting a name to the highscore list
	void Update(){
		if(Application.loadedLevelName.Equals("MenuScene")){
			askForName=false;
		}
	}
	
	void InitScores(){
		topScores=new HighScore[10];
		for(int it1=0;it1<10;it1++){
			topScores[it1].name="";
			topScores[it1].val=0;
		}
		roundScore = new HighScore();
		roundScore.name="";
		roundScore.val=0;
	}
	
	void PrintScores(){
		for(int it1=0;it1<10;it1++){
			Debug.Log (it1.ToString()+": "+topScores[it1].name+" "+topScores[it1].val.ToString());
		}
	}
	
	public void CheckNewScore(){
		if(topScores[9].val<score){
			checkedTopTen=true;
		}
	}
	
	void LoadScoresFromPrefs(){
		for(int it1=0;it1<10;it1++){
			topScores[it1].name=PlayerPrefs.GetString("HighScoreName"+it1.ToString());
			topScores[it1].val=PlayerPrefs.GetInt("HighScoreValue"+it1.ToString());
		}
	}
	
	void SaveScoresToPrefs(){
		for(int it1=0;it1<10;it1++){
			PlayerPrefs.SetString("HighScoreName"+it1.ToString(),topScores[it1].name);
			PlayerPrefs.SetInt("HighScoreValue"+it1.ToString(),topScores[it1].val);
		}
	}
	
	public void NewScore(HighScore newScore){
		topScores[9]=newScore;
		SortScores();
		SaveScoresToPrefs();
	}
	
	void SortScores(){
		HighScore tempScore;
		for(int it1=0;it1<10;it1++){
			for(int it2=0;it2<9;it2++){
				if(topScores[it1].val>topScores[it2].val){
					tempScore=topScores[it1];
					topScores[it1]=topScores[it2];
					topScores[it2]=tempScore;
				}
			}
		}
	}
	
	void ShowScores(){
		for(int it1=0;it1<10;it1++){
			GUI.Label(new Rect(Screen.width/4,Screen.height/20*(it1+1)+15*it1,Screen.width / 3, Screen.height / 20),topScores[it1].name,labelStyle);
			GUI.Label(new Rect(Screen.width/4*2,Screen.height/20*(it1+1)+15*it1,Screen.width / 3, Screen.height / 20),topScores[it1].val.ToString(),labelStyle);
		}
	}	

}


public struct HighScore{
	public string name;
	public int val;
}