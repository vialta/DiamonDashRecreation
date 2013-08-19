using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour {
	
	public static MenuScript instance {get; private set;}
	
	void Awake(){
		instance=this;
		DontDestroyOnLoad(gameObject);
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
