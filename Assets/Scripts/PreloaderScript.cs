using UnityEngine;
using System.Collections;

public class PreloaderScript : MonoBehaviour {
	
	public GUIText loadingText;
	
	private AsyncOperation preloader=null;
	
	// Use this for initialization
	void Start () {
		loadingText=gameObject.GetComponent<GUIText>();
	}
	
	// Update is called once per frame
	void Update () {
		Preload();
	}
	
	void Preload(){
		if(Time.timeSinceLevelLoad<3.0f){
			switch(Mathf.FloorToInt(Time.timeSinceLevelLoad*5)%4){
				case 0:
					loadingText.text="Loading";
					break;
				case 1:
					loadingText.text="Loading.";
					break;
				case 2:
					loadingText.text="Loading..";
					break;
				case 3:
					loadingText.text="Loading...";
					break;
				default:
					break;
			}
		}
		else{
			Application.LoadLevel("MenuScene");
		}
	}
	
	/*
	Had I had Unity PRO, this is the solution I would have used. A coroutine that would load the background level and data.
	The Preload() method is simply used like smokes and mirrors to give the impression that something is going on. It could simply
	replaced with a OnGUI() method that has two textures, an empty one and a full one and keeps increasing the size of the full one by using 
	async.progress which returns a float variable, describing the status of loading.
	
	 
	IEnumerator Preload(string levelName){
		preloader = Application.LoadLevelAsync("MenuScene");
		yield return preloader;
	}
	
	
	void Preload(){
		if(preloader!=null){
			switch(Mathf.FloorToInt(Time.timeSinceLevelLoad*5)%4){
				case 0:
					loadingText.text="Loading";
					break;
				case 1:
					loadingText.text="Loading.";
					break;
				case 2:
					loadingText.text="Loading..";
					break;
				case 3:
					loadingText.text="Loading...";
					break;
				default:
					break;
			}
		}
	}
	*/
	
}
