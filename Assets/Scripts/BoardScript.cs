using UnityEngine;
using System.Collections;

public class BoardScript : MonoBehaviour {
	
	public GameObject[,] bricks;
	public BrickClass[,] brickClass;
	public GameObject brickPrefab;
	public Vector3 StartVector= new Vector3(-4.5F,-7F,10F);
	public int matchingBrickCounter;

	void Start () {
		bricks = new GameObject[10,10];
		brickClass = new BrickClass[10,10];
		for (int it1=0;it1<10;it1++){
			for(int it2=0;it2<10;it2++){
				bricks[it1,it2]=Instantiate (brickPrefab,new Vector3(-4.5F+it1,-7F+it2,10F),Quaternion.identity) as GameObject;
				brickClass[it1,it2]=bricks[it1,it2].GetComponent<BrickClass>();
			}
		}
	}
	
	void Update () {
	
		if(Input.GetMouseButtonDown(0)){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray,out hit)){
				Debug.Log (hit.transform.position-StartVector);
				if(hit.transform.gameObject.name=="Brick(Clone)"){
					Vector3 hitObject=hit.transform.position-StartVector;
					Debug.Log (hitObject);
					MatchAlgorithm(Mathf.FloorToInt(hitObject.x),Mathf.FloorToInt(hitObject.y)+1);
					Debug.Log (matchingBrickCounter);
					if(matchingBrickCounter>=3){
						for (int it1=0;it1<10;it1++){
							for(int it2=0;it2<10;it2++){
								if(brickClass[it1,it2]!=null){						
									if(brickClass[it1,it2].toBeDestroyed==true){
										Destroy (bricks[it1,it2]);
										bricks[it1,it2]=null;
										Destroy (brickClass[it1,it2]);
										brickClass[it1,it2]=null;
									}
								}
							}
						}
						
						for(int it1=0;it1<10;it1++){
							MoveBlocks(it1);
						}
						Reinstantiate();
						PrintBlocks();
						//Move 
						//Reinstantiate
					}
					else{
						for (int it1=0;it1<10;it1++){
							for(int it2=0;it2<10;it2++){
								brickClass[it1,it2].toBeDestroyed=false;
								brickClass[it1,it2].hasBeenChecked=false;
							}
						}
					}
					matchingBrickCounter=0;
				}
			}		
		}
		
		
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
	
	void MoveBlocks(int brickPositionX){
		GameObject auxGameObject;
		BrickClass auxBrickClass;
		for(int it1=0;it1<9;it1++){
			if(bricks[brickPositionX,it1]==null){
				for(int it2=it1;it2<10;it2++){
					if(bricks[brickPositionX,it2]!=null){
						auxGameObject=bricks[brickPositionX,it1];
						auxBrickClass=brickClass[brickPositionX,it1];
						bricks[brickPositionX,it1]=bricks[brickPositionX,it2];
						brickClass[brickPositionX,it1]=brickClass[brickPositionX,it2];
						bricks[brickPositionX,it2]=auxGameObject;
						brickClass[brickPositionX,it2]=auxBrickClass;
						it2=10;
					}
				}
			}
		}
	}
	
	void MatchAlgorithm(int brickPositionX,int brickPositionY){
		Debug.Log (brickPositionX+" "+brickPositionY);
		brickClass[brickPositionX,brickPositionY].hasBeenChecked=true;
		brickClass[brickPositionX,brickPositionY].toBeDestroyed=true;
		matchingBrickCounter+=1;	
		if(brickPositionX>0){
			if(brickClass[brickPositionX-1,brickPositionY].hasBeenChecked==false && brickClass[brickPositionX-1,brickPositionY].brickValue==brickClass[brickPositionX,brickPositionY].brickValue){
				MatchAlgorithm(brickPositionX-1,brickPositionY);
			}
			
		}
		if(brickPositionX<9){
			if(brickClass[brickPositionX+1,brickPositionY].hasBeenChecked==false && brickClass[brickPositionX+1,brickPositionY].brickValue==brickClass[brickPositionX,brickPositionY].brickValue){
				MatchAlgorithm(brickPositionX+1,brickPositionY);
			}
		}
		if(brickPositionY>0){
			if(brickClass[brickPositionX,brickPositionY-1].hasBeenChecked==false && brickClass[brickPositionX,brickPositionY-1].brickValue==brickClass[brickPositionX,brickPositionY].brickValue){
				MatchAlgorithm(brickPositionX,brickPositionY-1);
			}
		}
		if(brickPositionY<9){
			if(brickClass[brickPositionX,brickPositionY+1].hasBeenChecked==false && brickClass[brickPositionX,brickPositionY+1].brickValue==brickClass[brickPositionX,brickPositionY].brickValue){
				MatchAlgorithm(brickPositionX,brickPositionY+1);
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
						bricks[it1,it2]=Instantiate (brickPrefab,new Vector3(-4.5F+it1,-5F+it2,10F),Quaternion.identity) as GameObject;
						brickClass[it1,it2]=bricks[it1,it2].GetComponent<BrickClass>();
					}
				}
			}
		}
	}
	
}
