using UnityEngine;
using System.Collections;

public class BoardScript : MonoBehaviour {

	public GameObject[,] Bricks;
	public GameObject brickPrefab;
	public Vector3 StartVector= new Vector3(-4.5F,-7F,10F);

	void Start () {
		Bricks = new GameObject[10,10];
		
		for (int it1=0;it1<10;it1++){
			for(int it2=0;it2<10;it2++){
				Bricks[it1,it2]=Instantiate (brickPrefab,new Vector3(-4.5F+it2,-7F+it1,10F),Quaternion.identity) as GameObject;
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
					MatchAlgorithm(Mathf.FloorToInt(hitObject.x),Mathf.FloorToInt(hitObject.y));
				}
			}
			
		}
		
	}
	
	void CheckForNull(){
	}
	
	void MatchAlgorithm(int brickPositionX,int brickPositionY){
	}
	
}
