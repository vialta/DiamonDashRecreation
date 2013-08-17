using UnityEngine;
using System.Collections;

public class BrickClass : MonoBehaviour {
	
	public int brickValue;
	public Material[] brickMaterials;
	public bool toBeDestroyed;
	public bool hasBeenChecked;
	public bool toMove;
		
	void Start () {
		brickValue = Random.Range(0,5);
		this.renderer.material=brickMaterials[brickValue];
	}
	
	void Update () {
		if(toMove){
			transform.Translate(0F,-0.25F,0F);
		}
	}
	
	public void RandomBrickValue(){
		this.brickValue=Random.Range (0,5);
		this.renderer.material=brickMaterials[brickValue];
	}
	
}
