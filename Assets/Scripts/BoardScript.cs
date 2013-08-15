using UnityEngine;
using System.Collections;

public class BoardScript : MonoBehaviour {

	public GameObject[,] Bricks;
	public GameObject brickPrefab;
	
	// Use this for initialization
	void Start () {
		Bricks = new GameObject[10,10];
		
		for (int it1=0;it1<10;it1++)
			for(int it2=0;it2<10;it2++){
				Bricks[it1,it2]=Instantiate (brickPrefab,new Vector3(-4.5F+it2,-7F+it1,10F),Quaternion.identity) as GameObject;
			}
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void checkForNull(){
	}
	
	void OnMouseOver(){
		
	}
}
