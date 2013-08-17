using UnityEngine;
using System.Collections;

public class BoardScript : MonoBehaviour {
	
	public GameObject[,] bricks;
	public BrickClass[,] brickClass;
	public GameObject brickPrefab;
	public Vector3 StartVector= new Vector3(-4.5F,-7F,10F);
	public int matchingBrickCounter;
	public ParticleSystem hintParticles;
	
	public int score=0;
	
	public bool toScramble=false;
	public bool hintsUpdated=false;
	
	public float respawnPosition;
	public float hintTimer;
	public float scrambleTimer;
	public float startTimer=60f;
	
	public ArrayList hints = new ArrayList();
	
	void Awake(){
		hintTimer = Time.time;
	}
	
	void Start () {
		bricks = new GameObject[10,10];
		brickClass = new BrickClass[10,10];
		BuildBoard ();		
	}
	
	void OnGUI(){
		if(GUI.Button(new Rect(50,50,50,50),"Print")){
			PrintBlocks();
		}
		if(GUI.Button(new Rect(50,100,50,50),"Restart")){
			Application.LoadLevel(Application.loadedLevel);
		}
		GUI.Label(new Rect(700,50,50,50),(startTimer-Time.time).ToString("#"));
	}
	
	void Update () {
		CheckForMovement();
		UpdateHintArray();
		if(Input.GetMouseButtonDown(0)){
			ClickMouseAction();
		}
		CheckShowHint();
		CheckForSolution();
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
				GameObject r = hints[Random.Range (0,hints.Count)] as GameObject;
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
		GameObject auxGameObject;
		for(int it1=0;it1<9;it1++){
			if(bricks[brickPositionX,it1]==null){
				for(int it2=it1;it2<10;it2++){
					if(bricks[brickPositionX,it2]!=null){
						auxGameObject=bricks[brickPositionX,it1];						
						bricks[brickPositionX,it1]=bricks[brickPositionX,it2];					
						bricks[brickPositionX,it2]=auxGameObject;
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