using UnityEngine;
using System.Collections;

public class BrickClass : MonoBehaviour {
	
	public int brickValue { get; set; }
	public MaterialController materialController;
	public bool toBeDestroyed { get; set; }
	public bool hasBeenChecked { get; set; }
	public bool toMove { get; set; }
		
	void Start () {
		InitScripts();
		brickValue = Random.Range(0,5);
		this.renderer.material=materialController.brickMaterials[brickValue];	
	}
	
	void Update () {
		if(toMove){
			transform.Translate(0F,-0.25F,0F);
		}
	}
	
	public void RandomBrickValue(){
		this.brickValue=Random.Range (0,5);
		this.renderer.material=materialController.brickMaterials[brickValue];
	}
	
	void InitScripts(){
		if(materialController==null){
			materialController=GameObject.Find ("MaterialObject").GetComponent<MaterialController>();
		}
	}
	
}
