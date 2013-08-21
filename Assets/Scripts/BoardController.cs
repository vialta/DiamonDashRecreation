using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// I should mention that I prefer using public variables in order to check the value of the variable in the editor

public class BoardController: MonoBehaviour {
	
	// This may appear odd but I added the extra array of BrickClass to prevent calling .GetComponent<>() which is slow for mobile devices
	// The class is also used more than once so it is encouraged to use a variable instead of the method mentioned above.
	public GameObject[,] bricks;
	public BrickClass[,] brickClass;
	public GameObject brickPrefab;
	public HighScoreController highScoreController;
	public Vector3 StartVector= new Vector3(-4.5F,-7F,10F);
	public int matchingBrickCounter;
	public ParticleSystem hintParticles;
	
	private int scoreMultiplier=120;
	public int boardSize=10;
	
	public bool toScramble=false;
	public bool hintsUpdated=false;
	public bool timeUp=false;
	public bool menuOn=true;
	
	public GUIText scoreGUIText;
	public GUIText timeGUIText;
	public GUIText gameOverGUIText;
	
	public float hintTimer;
	public float scrambleTimer;
	public float roundSeconds=60f;
	public float roundTimer;
	public float standByTimer=0f;
	
	public List<GameObject> hints = new List<GameObject>();
	
	//runs first
	void Awake(){
		hintTimer = Time.time;
	}
	
	void Start () {
		Time.timeScale=1;
		UpdateRoundTimer();
		InitialiseScripts();
		bricks = new GameObject[boardSize,boardSize];
		brickClass = new BrickClass[boardSize,boardSize];
		BuildBoard ();		
	}
	
	void OnGUI(){
		if(GUI.Button(new Rect(50,50,50,50),"Menu")){
			Application.LoadLevel("MenuScene");
		}
		if(GUI.Button(new Rect(50,100,50,50),"Restart")){
			RestartLevel();
		}
	}	
	
	void Update () {
		CheckForMovement();
		UpdateTexts();
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
			if(highScoreController.checkedTopTen){
				if(highScoreController.submitted){
					highScoreController.score=0;
					highScoreController.askForName=false;
					highScoreController.submitted=false;
					RestartLevel();
				}
				else{
					highScoreController.askForName=true;
					Time.timeScale=0;
				}
			}
			else{
				highScoreController.CheckNewScore();
			}
			
		}
	}
	
	void InitialiseScripts(){
		if(highScoreController==null){
			highScoreController=GameObject.Find ("HighScoreObject").GetComponent<HighScoreController>();
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
	
	void UpdateTexts(){
		timeGUIText.text=roundTimer.ToString("#");
		scoreGUIText.text=highScoreController.score.ToString();
		if(timeUp){
			gameOverGUIText.text="GAME OVER";
		}
	}
	
	//Checks to see if there is a missing block in the whole board
	bool CheckForNull(){
		for (int it1=0;it1<boardSize;it1++){
			for(int it2=0;it2<boardSize;it2++){
				if(bricks[it1,it2]==null){
					return true;
				}
			}
		}
		return false;
	}
	
	//Checks the hint timer
	void CheckShowHint(){
		if(Time.time-hintTimer>=3.0f){
			if(!hintParticles.isPlaying){
				GameObject r = hints[Random.Range (0,hints.Count)];
				hintParticles.transform.position=r.transform.position;
				hintParticles.Play();
			}
		}
	}
	
	//Checks if there is a solution to the board. If not, it scrambles it for 1 second. Cool effect too.
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
	
	//Empties the board and destroys all objects on the board
	void ClearBoard(){
		for (int it1=0;it1<boardSize;it1++){
			for(int it2=0;it2<boardSize;it2++){
				Destroy(bricks[it1,it2]);
				Destroy(brickClass[it1,it2]);
			}
		}
	}
	
	//Instantiates the whole board
	void BuildBoard(){
		for (int it1=0;it1<boardSize;it1++){
			for(int it2=0;it2<boardSize;it2++){
				bricks[it1,it2]=Instantiate (brickPrefab,new Vector3(-4.5F+it1,-7F+it2,10F),Quaternion.identity) as GameObject;
				brickClass[it1,it2]=bricks[it1,it2].GetComponent<BrickClass>();
			}
		}
		hintsUpdated=false;
	}
	
	//Scrambles the values of the board
	void ScrambleBoard(){
		for (int it1=0;it1<boardSize;it1++){
			for(int it2=0;it2<boardSize;it2++){
				brickClass[it1,it2].RandomBrickValue();
			}
		}
	}
	
	void UpdateHintArray(){
		if(!hintsUpdated){
			hints.Clear();
			for(int it1=0;it1<boardSize;it1++){
				for(int it2=0;it2<boardSize;it2++){
					matchingBrickCounter=0;
					MatchAlgorithm(it1,it2,false);	
					if(matchingBrickCounter>=3){
						hints.Add(bricks[it1,it2]);
					}
				}
			}
			hintsUpdated=true;
		}
		matchingBrickCounter=0;
		for (int it1=0;it1<boardSize;it1++){
			for(int it2=0;it2<boardSize;it2++){
				brickClass[it1,it2].toBeDestroyed=false;
				brickClass[it1,it2].hasBeenChecked=false;
			}
		}
	}
	
	//What happens when the left mouse button is clicked. If it is on a brick and it is in a group of 3 or more bricks of the same type, it is destroyed
	// and replaced
	void ClickMouseAction(){		
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(ray,out hit)){
			if(hit.transform.gameObject.name.Equals("Brick(Clone)")){
				Vector3 hitObject=hit.transform.position-StartVector;
				if(!brickClass[Mathf.FloorToInt(hitObject.x),Mathf.FloorToInt(hitObject.y)].toMove){
					MatchAlgorithm(Mathf.FloorToInt(hitObject.x),Mathf.FloorToInt(hitObject.y),true);
					if(matchingBrickCounter>=3){
						hintTimer=Time.time;
						hintParticles.Stop();
						for (int it1=0;it1<boardSize;it1++){
							for(int it2=0;it2<boardSize;it2++){
								if(brickClass[it1,it2]!=null){						
									if(brickClass[it1,it2].toBeDestroyed){
										Destroy (bricks[it1,it2]);
										bricks[it1,it2]=null;
									}
								}
							}
						}
						for(int it1=0;it1<boardSize;it1++){
							MoveBlocks(it1);
						}
						Reinstantiate();
						ResetBlockClasses();
						hintsUpdated=false;
						highScoreController.score+=matchingBrickCounter*scoreMultiplier;
					}
					else{
						for (int it1=0;it1<boardSize;it1++){
							for(int it2=0;it2<boardSize;it2++){
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
		for(int it1=0;it1<boardSize;it1++){
			for(int it2=0;it2<boardSize;it2++){
				brickClass[it1,it2]=bricks[it1,it2].GetComponent<BrickClass>();
			}
		}
	}
	
	void MoveBlocks(int brickPositionX){
		GameObject tempGameObject;
		for(int it1=0;it1<9;it1++){
			if(bricks[brickPositionX,it1]==null){
				for(int it2=it1;it2<boardSize;it2++){
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
	
	//Checks to see if any blocks should "fall" 
	void CheckForMovement(){
		for(int it1=0;it1<boardSize;it1++){
			for (int it2=0;it2<boardSize;it2++){
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
			if(!brickClass[brickPositionX-1,brickPositionY].hasBeenChecked && brickClass[brickPositionX-1,brickPositionY].brickValue==brickClass[brickPositionX,brickPositionY].brickValue){
				MatchAlgorithm(brickPositionX-1,brickPositionY,clicked);
			}
			
		}
		if(brickPositionX<9){
			if(!brickClass[brickPositionX+1,brickPositionY].hasBeenChecked && brickClass[brickPositionX+1,brickPositionY].brickValue==brickClass[brickPositionX,brickPositionY].brickValue){
				MatchAlgorithm(brickPositionX+1,brickPositionY,clicked);
			}
		}
		if(brickPositionY>0){
			if(!brickClass[brickPositionX,brickPositionY-1].hasBeenChecked && brickClass[brickPositionX,brickPositionY-1].brickValue==brickClass[brickPositionX,brickPositionY].brickValue){
				MatchAlgorithm(brickPositionX,brickPositionY-1,clicked);
			}
		}
		if(brickPositionY<9){
			if(!brickClass[brickPositionX,brickPositionY+1].hasBeenChecked && brickClass[brickPositionX,brickPositionY+1].brickValue==brickClass[brickPositionX,brickPositionY].brickValue){
				MatchAlgorithm(brickPositionX,brickPositionY+1,clicked);
			}
		}		
	}
	
	//method used for debugging
	void PrintBlocks(){
		string printOut;
		for(int it1=0;it1<boardSize;it1++){
			printOut="";
			for(int it2=0;it2<boardSize;it2++){
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
			for(int it1=0;it1<boardSize;it1++){
				for(int it2=0;it2<boardSize;it2++){
					if(bricks[it1,it2]==null){
						bricks[it1,it2]=Instantiate (brickPrefab,new Vector3(-4.5F+it1,it2-5.0F,10F),Quaternion.identity) as GameObject;
					}
				}
			}
		}
	}
}