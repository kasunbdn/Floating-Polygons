using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

	BoardManager boardScript;
	private Button bSoundOn,bSoundOff;
	public Color32 blackTransparent,white;
	public Canvas hiscoreCanvas;
	public Text highScoreText;
	public RectTransform title;
	AudioSource sound;
	bool mute=false;
	void Awake () {


		sound= GetComponent<AudioSource>();
		bSoundOn= GameObject.Find ("sound_on").GetComponent<Button>();
		bSoundOff= GameObject.Find ("sound_off").GetComponent<Button>();

		if (PlayerPrefs.GetInt ("mute", 0) == 0) {
			bSoundOff.gameObject.SetActive (false);
		} else {			
			bSoundOn.gameObject.SetActive (false);
		}
		mute = PlayerPrefs.GetInt ("mute")==1;
		if (mute)
			sound.mute = true;

		blackTransparent = new Color32 (40,40,40,0);
		white= new Color32 (255,255,255,255);

		boardScript=GetComponent<BoardManager>();
		boardScript.createBoard (8);
		boardScript.InstatiatePolygons (15,1,4);

		//StartCoroutine("initialize"); 
		title.position = new Vector3 (title.position.x,title.position.y-(Screen.height/4)+50);
	
	}

	
	// Update is called once per frame
	public void play(){
		PlayerPrefs.SetInt ("level",1);
		PlayerPrefs.SetInt ("score",0);
		PlayerPrefs.SetInt ("bonus",0);
		Application.LoadLevel (1);
	}
	public void quitGame(){
		Application.Quit();
	}

	public void muteOn(){
		bSoundOff.gameObject.SetActive (true);
		bSoundOn.gameObject.SetActive (false);	
		PlayerPrefs.SetInt ("mute", 1);
		sound.mute = true;
	}

	public void muteOff(){
		bSoundOff.gameObject.SetActive (false);
		PlayerPrefs.SetInt ("mute", 0);
		bSoundOn.gameObject.SetActive (true);
		sound.mute = false;
	}

	public void highScore(){
		highScoreText.text = "High Score\n" + PlayerPrefs.GetInt ("high_score");
		hiscoreCanvas.enabled = true;
		hiscoreCanvas.gameObject.GetComponent<Animator> ().SetTrigger ("open");
	}

	public void closeHighScoreCanvas(){
		hiscoreCanvas.gameObject.GetComponent<Animator> ().SetTrigger ("close");
		StartCoroutine ("closeCanvas");

	}

	IEnumerator closeCanvas(){
		yield return new WaitForSeconds (0.1f);
		hiscoreCanvas.enabled = false;
	}


}
