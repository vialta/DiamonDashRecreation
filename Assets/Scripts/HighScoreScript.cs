using UnityEngine;
using System.Collections;

public class HighScoreScript : MonoBehaviour {
	
	public static HighScoreScript instance {get; private set; }
	
	public HighScore[] topScores;
	public bool askForName;
	
	void Awake(){
		instance=this;
		DontDestroyOnLoad(gameObject);
	}

	void Start () {
		InitScores ();
		if(PlayerPrefs.GetString("HighScoreName1")!=""){
			LoadScoresFromPrefs();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI(){
		if(askForName){
			topScores[9].name = GUI.TextField(new Rect(400, 10, 200, 20), topScores[9].name , 25);
			if(GUI.Button (new Rect(450,50,80,50),"Submit")){
				SortScores();
				askForName=false;
			}
		}
	}
	
	void InitScores(){
		for(int it1=0;it1<10;it1++){
			topScores[it1].name="";
			topScores[it1].val=0;
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
	
	void NewScore(HighScore newScore){
		if(topScores[9].val<newScore.val){
			topScores[9]=newScore;
			askForName=true;
		}
	}
	
	void SortScores(){
		HighScore tempScore;
		for(int it1=0;it1<10;it1++){
			for(int it2=0;it2<9;it2++){
				if(topScores[it1].val<topScores[it2].val){
					tempScore=topScores[it1];
					topScores[it1]=topScores[it2];
					topScores[it2]=tempScore;
				}
			}
		}
	}
	
	
}


public struct HighScore{
	public string name;
	public int val;
}