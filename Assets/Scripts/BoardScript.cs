using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardScript : MonoBehaviour {
	
	public GameObject[,] bricks;
	public BrickClass[,] brickClass;
	public GameObject brickPrefab;
	public HighScoreScript highScoreScript;
	public Vector3 StartVector= new Vector3(-4.5F,-7F,10F);
	public int matchingBrickCounter;
	public ParticleSystem hintParticles;
	
	public int scoreMultiplier=120;
	
	public bool toScramble=false;
	public bool hintsUpdated=false;
	public bool timeUp=false;
	public bool menuOn=true;
	
	public float respawnPosition;
	public float hintTimer;
	public float scrambleTimer;
	public float roundSeconds=10f;
	public float roundTimer;
	public float standByTimer=0f;
	
	public List<GameObject> hints = new List<GameObject>();
	
	void Awake(){
		hintTimer = Time.time;
		
	}
	
	void Start () {
		Time.timeScale=1;
		UpdateRoundTimer();
		InitialiseScripts();
		bricks = new GameObject[10,10];
		brickClass = new BrickClass[10,10];
		BuildBoard ();		
	}
	
	void OnGUI(){
		if(GUI.Button(new Rect(50,50,50,50),"Print")){
			PrintBlocks();
		}
		if(GUI.Button(new Rect(50,100,50,50),"Restart")){
			RestartLevel();
		}
		GUI.Label(new Rect(700,50,50,50),(roundTimer).ToString("#"));
		GUI.Label (new Rect(700,100,50,50),highScoreScript.score.ToString());
		if(timeUp){
			GUI.Label(new Rect(400,350,100,100),"GAME OVER");
		}
	}
	
	void Update () {
		CheckForMovement();
		if(roundTimer>0){
			UpdateHintArray();
			if(Input.GetMouseButtonDown(0)){
				ClickMouseAction();
			}
			CheckShowHint();
			CheckForSolution();
			UpdateRoundTimer();
		}
		else{
			timeUp=true;
			if(highScoreScript.checkedTopTen){
				if(highScoreScript.submitted){
					highScoreScript.score=0;
					highScoreScript.askForName=false;
					highScoreScript.submitted=false;
					RestartLevel();
				}
				else{
					highScoreScript.askForName=true;
					Time.timeScale=0;
				}
			}
			else{
				highScoreScript.CheckNewScore();
			}
			
		}
	}
	
	void InitialiseScripts(){
		if(highScoreScript==null){
			highScoreScript=GameObject.Find ("HighScoreObject").GetComponent<HighScoreScript>();
		}
	}
	
	void RestartLevel(){
		timeUp=false;
		standByTimer=0f;
		Application.LoadLevel(Application.loadedLevel);
	}
	
	void UpdateRoundTimer(){
		roundTimer=roundSeconds-Time.timeSinceLevelLoad+standByTimer;
		
	}
	
	bool CheckForNull(){
		for (int it1=0;it1<10;it1++){
			for(int it2=0;it2<10;it2++){
				if(bricks[it1,it2]==null){
					return true;
				}
			}
		}
		return false;
	}
	
	void CheckShowHint(){
		if(Time.time-hintTimer>=3.0f){
			if(!hintParticles.isPlaying){
				GameObject r = hints[Random.Range (0,hints.Count)];
				hintParticles.transform.position=r.transform.position;
				hintParticles.Play();
			}
		}
	}
	
	void CheckForSolution(){
		if(hints.Count==0&&!toScramble){
			scrambleTimer=Time.time;
			toScramble=true;
		}
		if(toScramble){
			if(Time.time-scrambleTimer<1.0f){
				ScrambleBoard();
			}
			else{
				toScramble=false;
				hintsUpdated=false;
				standByTimer+=1.2f;
			}
		}
	}
	
	void ClearBoard(){
		for (int it1=0;it1<10;it1++){
			for(int it2=0;it2<10;it2++){
				Destroy(bricks[it1,it2]);
				Destroy(brickClass[it1,it2]);
			}
		}
	}
	
	void BuildBoard(){
		for (int it1=0;it1<10;it1++){
			for(int it2=0;it2<10;it2++){
				bricks[it1,it2]=Instantiate (brickPrefab,new Vector3(-4.5F+it1,-7F+it2,10F),Quaternion.identity) as GameObject;
				brickClass[it1,it2]=bricks[it1,it2].GetComponent<BrickClass>();
			}
		}
		hintsUpdated=false;
	}
	
	void ScrambleBoard(){
		for (int it1=0;it1<10;it1++){
			for(int it2=0;it2<10;it2++){
				brickClass[it1,it2].RandomBrickValue();
			}
		}
	}
	
	void UpdateHintArray(){
		if(!hintsUpdated){
			hints.Clear();
			for(int it1=0;it1<10;it1++){
				for(int it2=0;it2<10;it2++){
					matchingBrickCounter=0;
					MatchAlgorithm(it1,it2,false);	
					if(matchingBrickCounter>=3){
						hints.Add(bricks[it1,it2]);
					}
				}
			}
			hintsUpdated=true;
			Debug.Log ("Count: "+hints.Count);
		}
		matchingBrickCounter=0;
		for (int it1=0;it1<10;it1++){
			for(int it2=0;it2<10;it2++){
				brickClass[it1,it2].toBeDestroyed=false;
				brickClass[it1,it2].hasBeenChecked=false;
			}
		}
	}
	
	void ClickMouseAction(){		
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(ray,out hit)){
			if(hit.transform.gameObject.name=="Brick(Clone)"){
				Vector3 hitObject=hit.transform.position-StartVector;
				if(!brickClass[Mathf.FloorToInt(hitObject.x),Mathf.FloorToInt(hitObject.y)].toMove){
					MatchAlgorithm(Mathf.FloorToInt(hitObject.x),Mathf.FloorToInt(hitObject.y),true);
					if(matchingBrickCounter>=3){
						hintTimer=Time.time;
						hintParticles.Stop();
						for (int it1=0;it1<10;it1++){
							for(int it2=0;it2<10;it2++){
								if(brickClass[it1,it2]!=null){						
									if(brickClass[it1,it2].toBeDestroyed==true){
										Destroy (bricks[it1,it2]);
										bricks[it1,it2]=null;
									}
								}
							}
						}
						for(int it1=0;it1<10;it1++){
							MoveBlocks(it1);
						}
						Reinstantiate();
						ResetBlockClasses();
						hintsUpdated=false;
						highScoreScript.score+=matchingBrickCounter*scoreMultiplier;
					}
					else{
						Debug.Log (matchingBrickCounter);
						for (int it1=0;it1<10;it1++){
							for(int it2=0;it2<10;it2++){
								brickClass[it1,it2].toBeDestroyed=false;
								brickClass[it1,it2].hasBeenChecked=false;
							}
						}
					}			
				}
				matchingBrickCounter=0;
			}
		}
	}
	
	void ResetBlockClasses(){
		Debug.Log ("Resetting");
		for(int it1=0;it1<10;it1++){
			for(int it2=0;it2<10;it2++){
				brickClass[it1,it2]=bricks[it1,it2].GetComponent<BrickClass>();
			}
		}
	}
	
	void MoveBlocks(int brickPositionX){
		GameObject tempGameObject;
		for(int it1=0;it1<9;it1++){
			if(bricks[brickPositionX,it1]==null){
				for(int it2=it1;it2<10;it2++){
					if(bricks[brickPositionX,it2]!=null){
						tempGameObject=bricks[brickPositionX,it1];						
						bricks[brickPositionX,it1]=bricks[brickPositionX,it2];					
						bricks[brickPositionX,it2]=tempGameObject;
						it2=10;
					}
				}
			}
		}
	}
	
	void CheckForMovement(){
		for(int it1=0;it1<10;it1++){
			for (int it2=0;it2<10;it2++){
				if(bricks[it1,it2]!=null){
					if(bricks[it1,it2].transform.position.y!=it2-7.0F){
						brickClass[it1,it2].toMove=true;
					}
					else{
						brickClass[it1,it2].toMove=false;
					}
				}
			}
		}
	}
	
	void MatchAlgorithm(int brickPositionX,int brickPositionY,bool clicked){
		brickClass[brickPositionX,brickPositionY].hasBeenChecked=true;
		brickClass[brickPositionX,brickPositionY].toBeDestroyed=clicked;
		matchingBrickCounter+=1;	
		if(brickPositionX>0){
			if(brickClass[brickPositionX-1,brickPositionY].hasBeenChecked==false && brickClass[brickPositionX-1,brickPositionY].brickValue==brickClass[brickPositionX,brickPositionY].brickValue){
				MatchAlgorithm(brickPositionX-1,brickPositionY,clicked);
			}
			
		}
		if(brickPositionX<9){
			if(brickClass[brickPositionX+1,brickPositionY].hasBeenChecked==false && brickClass[brickPositionX+1,brickPositionY].brickValue==brickClass[brickPositionX,brickPositionY].brickValue){
				MatchAlgorithm(brickPositionX+1,brickPositionY,clicked);
			}
		}
		if(brickPositionY>0){
			if(brickClass[brickPositionX,brickPositionY-1].hasBeenChecked==false && brickClass[brickPositionX,brickPositionY-1].brickValue==brickClass[brickPositionX,brickPositionY].brickValue){
				MatchAlgorithm(brickPositionX,brickPositionY-1,clicked);
			}
		}
		if(brickPositionY<9){
			if(brickClass[brickPositionX,brickPositionY+1].hasBeenChecked==false && brickClass[brickPositionX,brickPositionY+1].brickValue==brickClass[brickPositionX,brickPositionY].brickValue){
				MatchAlgorithm(brickPositionX,brickPositionY+1,clicked);
			}
		}		
	}
	
	void PrintBlocks(){
		string printOut;
		for(int it1=0;it1<10;it1++){
			printOut="";
			for(int it2=0;it2<10;it2++){
				if(brickClass[it1,it2]==null){
					printOut+="- ";
				}
				else{ printOut+=brickClass[it1,it2].brickValue.ToString()+" ";
				}
			}
			Debug.Log(printOut);
		}
	}
	
	void Reinstantiate(){
		while(CheckForNull()){
			for(int it1=0;it1<10;it1++){
				for(int it2=0;it2<10;it2++){
					if(bricks[it1,it2]==null){
						bricks[it1,it2]=Instantiate (brickPrefab,new Vector3(-4.5F+it1,it2-5.0F,10F),Quaternion.identity) as GameObject;
					}
				}
			}
		}
	}
}