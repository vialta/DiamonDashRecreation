using UnityEngine;
using System.Collections;

public class BrickClass : MonoBehaviour {
	
	public int brickValue;
	public Material[] brickMaterials;
	public bool toBeDestroyed;
	public bool hasBeenChecked;
		
	void Start () {
		brickValue = Random.Range(0,4);
		this.renderer.material=brickMaterials[brickValue];
	}
	
	void Update () {
	
	}
	
}
