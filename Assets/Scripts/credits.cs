using UnityEngine;
using System.Collections;

public class credits : MonoBehaviour {

	BoardManager boardScript;
	public RectTransform title;
	void Awake () {

		boardScript=GetComponent<BoardManager>();
		boardScript.createBoard (8);
		boardScript.InstatiatePolygons (15,1,4);

		title.position = new Vector3 (title.position.x,title.position.y-(Screen.height/4)+50);
	
	}

	public void close(){
		Application.LoadLevel (0);
	}
	

}
