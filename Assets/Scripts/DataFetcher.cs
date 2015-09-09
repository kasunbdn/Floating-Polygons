using UnityEngine;
using System.Collections;

public class DataFetcher : MonoBehaviour {
	//UnityEngine.UI.Text debug;

	float xFactor = 6f/Screen.width,yFactor = 4f/Screen.height;

	private int[,] potraitT = new int[6, 4],
	landscapeT = new int[4, 6],
	potraitZ = new int[6, 4],
	landscapeZ = new int[4, 6],
	potraitR = new int[6, 4],
	landscapeR = new int[4, 6];

	bool updated=false;
	bool landscape=true;

	int screenCat=2;
	float screenSize=12;

	void Start () {
		//debug=GameObject.Find("debug").GetComponent<UnityEngine.UI.Text>();
		screenSize = Mathf.Sqrt (Screen.width * Screen.width + Screen.height * Screen.height) / Screen.dpi;
		if (screenSize <= 5)
			screenCat = 0;
		else if(screenSize <= 10)
			screenCat = 1;
		//debug.text = "" + screenCat;
		//Debug.Log (screenCat);

		initialize ();
	}

	void calculateFactors(){
		if (landscape) {
			xFactor = 6f/Screen.width;
			yFactor = 4f/Screen.height;
		}
		else{
			xFactor = 4f/Screen.width;
			yFactor = 6f/Screen.height;
			}
	}

	void initialize(){
		for (int i=0; i<4; i++) {
			for (int j=0; j<6; j++) {
				landscapeT[i,j]=PlayerPrefs.GetInt("LT"+i+""+j);
				landscapeZ[i,j]=PlayerPrefs.GetInt("LZ"+i+""+j);
				landscapeR[i,j]=PlayerPrefs.GetInt("LR"+i+""+j);
			}
		}
		for (int i=0; i<6; i++) {
			for (int j=0; j<4; j++) {
				potraitT[i,j]=PlayerPrefs.GetInt("PT"+i+""+j);
				potraitZ[i,j]=PlayerPrefs.GetInt("PZ"+i+""+j);
				potraitR[i,j]=PlayerPrefs.GetInt("PR"+i+""+j);
			}
		}

		InvokeRepeating ("syncData", 5, 6);

	}

	public void inputTranslate(Vector2 position){
		
		if (Screen.width < Screen.height) {
			if(landscape){
				landscape=false;
				calculateFactors();
			}

			int x=(int)(position.x*xFactor);
			int y=(int)(position.y*yFactor);

			if(x<4&&y<6)
				++potraitT[y,x];




		} else {
			if(!landscape){
				landscape=true;
				calculateFactors();
			}
			int x=(int)(position.x*xFactor);
			int y=(int)(position.y*yFactor);
			if(x<6&&y<4)
				++landscapeT[y,x];

		}
		updated = true;
	}

	public void inputZoom(Vector2 position){
		
		if (Screen.width < Screen.height) {

			if(landscape){
				landscape=false;
				calculateFactors();
			}
			int x=(int)(position.x*xFactor);
			int y=(int)(position.y*yFactor);
			
			if(x<4&&y<6)
				++potraitZ[y,x];



			
		} else {

			if(!landscape){
				landscape=true;
				calculateFactors();
			}
			int x=(int)(position.x*xFactor);
			int y=(int)(position.y*yFactor);
			if(x<6&&y<4)
				++landscapeZ[y,x];

		}
		updated = true;
	}

	public void inputRotate(Vector2 position){
		
		if (Screen.width < Screen.height) {

			if(landscape){
				landscape=false;
				calculateFactors();
			}

			int x=(int)(position.x*xFactor);
			int y=(int)(position.y*yFactor);
			
			if(x<4&&y<6)
				++potraitR[y,x];


			
		} else {

			if(!landscape){
				landscape=true;
				calculateFactors();
			}
			int x=(int)(position.x*xFactor);
			int y=(int)(position.y*yFactor);
			if(x<6&&y<4)
				++landscapeR[y,x];

		}
		updated = true;
	}

	void syncData(){
		if (updated) {
			updated=false;
			for (int i=0; i<4; i++) {
				for (int j=0; j<6; j++) {
					PlayerPrefs.SetInt("LT"+i+""+j,landscapeT[i,j]);
					PlayerPrefs.SetInt("LZ"+i+""+j,landscapeZ[i,j]);
					PlayerPrefs.SetInt("LR"+i+""+j,landscapeR[i,j]);
				}
			}
			for (int i=0; i<6; i++) {
				for (int j=0; j<4; j++) {
					PlayerPrefs.SetInt("PT"+i+""+j,potraitT[i,j]);
					PlayerPrefs.SetInt("PZ"+i+""+j,potraitZ[i,j]);
					PlayerPrefs.SetInt("PR"+i+""+j,potraitR[i,j]);
				}
			}
		}
	}

	public void dispatchToTheServer(){
		StartCoroutine ("dispatchData");
	}

	IEnumerator dispatchData(){
		yield return new WaitForSeconds (4f);


		/* web server data has been removed for security reasons 



			yield return postRequest;
			//debug.text = "\n\n\n" + postRequest.text;
		//Debug.Log (postRequest.text);
			if (postRequest.text.Equals ("success")) {
				Debug.Log ("success");
			//yield return new WaitForSeconds (1f);
				resetData ();
			}*/

	}

	void resetData(){
		potraitT = new int[6, 4];
		landscapeT = new int[4, 6];
		potraitZ = new int[6, 4];
		landscapeZ = new int[4, 6];
		potraitR = new int[6, 4];
		landscapeR = new int[4, 6];

		updated = true;
		syncData ();
		Debug.Log ("data reset");
		//debug.text = "reset";

	}


}
