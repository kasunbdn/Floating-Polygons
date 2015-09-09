using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public static GameManager instance=null;
	Camera cam;
	
	BoardManager boardScript;
	InputHandler inputHandler;
	DataFetcher dataFetcher;
	Player player;

	RectTransform scorePanel;
	Slider scoreSlider,livesSlider;
	Text scoreText;
	
	private Image sceneEnd;
	private Button bRestart,bResume,bPause,bExit,bSoundOn,bSoundOff;
	public Color backgroundColor;
	public Animator animatorScore,animatorTime;
	public Animator [] animatorButton;
	public Canvas decs;

	AudioSource sound;

	Text debug;

	Text levelText;
	Text levelDesc;
	int score=0,levelScore=0;
	int [] boardWidth= {15,20,35,40};
	int [] noOfPolygons={20,30,50,100};
	int [] speedOfPolygons={1,2,3,4};
	float camerazoom=14,endZoom=3;

	private int level=1;
	int levelPolygons=100;
	int time=10;
	bool bonus=false,paused=false,mute=false;
	int lives=3;
	int highS=0;

	void Awake () {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);

			cam = Camera.main;
			player=GameObject.Find("Player").GetComponent<Player>();
			scorePanel=GameObject.Find("Panel_Score").GetComponent<RectTransform>();

		boardScript=GetComponent<BoardManager>();
		inputHandler=GetComponent<InputHandler>();
		dataFetcher=GetComponent<DataFetcher>();

		scoreSlider = scorePanel.Find("Slider_Score").GetComponent<Slider>();
		livesSlider = scorePanel.Find("Slider_lives").GetComponent<Slider>();
		levelText=GameObject.Find("level").GetComponent<Text>();
		levelDesc=GameObject.Find("level_desc").GetComponent<Text>();
		
		scoreText = scorePanel.Find("Text_Score").GetComponent<Text>();
		sceneEnd = GameObject.Find ("scene_end").GetComponent<Image>();
		bRestart= GameObject.FindWithTag ("Respawn").GetComponent<Button>();
		bRestart.gameObject.SetActive (false);
		bResume= GameObject.FindWithTag ("Resume").GetComponent<Button>();
		bResume.gameObject.SetActive (false);
		bPause= GameObject.FindWithTag ("Pause").GetComponent<Button>();
		//bPause.gameObject.SetActive (false);
		bExit= GameObject.FindWithTag ("Finish").GetComponent<Button>();
		bExit.gameObject.SetActive (false);
		bSoundOn= GameObject.Find ("sound_on").GetComponent<Button>();
		bSoundOn.gameObject.SetActive (false);
		bSoundOff= GameObject.Find ("sound_off").GetComponent<Button>();
		bSoundOff.gameObject.SetActive (false);
		debug=GameObject.Find("debug").GetComponent<Text>();
		sound= GetComponent<AudioSource>();

		initGame ();
		if (Screen.width > Screen.height)
			camerazoom = 8;
		StartCoroutine("cameraZoomIn"); 
		highS = PlayerPrefs.GetInt ("high_score");
	}

	void initGame(){
		bonus=PlayerPrefs.GetInt ("bonus")==1;
		//bonus = true;
		level = PlayerPrefs.GetInt ("level",1);
		score = PlayerPrefs.GetInt ("score");
		mute = PlayerPrefs.GetInt ("mute")==1;
		if (mute)
			sound.mute = true;
		levelPolygons = noOfPolygons [level - 1];
		time = (int)(levelPolygons * (bonus?2:3));
		debug.text = "" + time;
		scoreText.text = score + "";
		boardScript.createBoard (boardWidth[level-1]);

	}




	public void playerHit(Polygon aPolygon){
		if (!paused) {
			noOfPolygons [level - 1]--;
			if (player.validHit (aPolygon,mute)) {
				animatorScore.SetTrigger ("scoreUp");
				levelScore++;
				score++;
				if (!bonus) {
					animatorTime.SetTrigger ("timeIncrease");
					time += 5;
					debug.text = "" + time;
				}
				scoreText.text = score + "";
				scoreSlider.value = (levelScore * 100f / levelPolygons);
				aPolygon.destroyPolygon (mute);
				if (noOfPolygons [level - 1] == 0)
					nextLevel ();
			} else {
				lives--;
				livesSlider.value--;
				if (lives <= 0) {
					aPolygon.rigidbody2D.isKinematic = true;
					player.rigidbody2D.isKinematic = true;
					StartCoroutine ("gameOver", (player.transform.position + aPolygon.transform.position) / 2); 
				} else {
					player.hitOther (aPolygon);
					aPolygon.destroyPolygon (true);
					if (noOfPolygons [level - 1] == 0)
						nextLevel ();
				}

			}
		}

	}

	IEnumerator cameraZoomIn(){
		
		if (bonus) {
			levelText.text = "bonus level";
			levelDesc.text="Can you get all\nthe polygons\nin "+time+" seconds";
		} else {
			levelText.text = "level " + level;
			if(level==1)
				dataFetcher.dispatchToTheServer();
		}
		float current=2;

		while(camerazoom-0.2>current) {
			yield return new WaitForSeconds (0.02f);
			current=cam.orthographicSize = Mathf.Lerp (cam.orthographicSize, camerazoom, Time.deltaTime);
		}

		boardScript.InstatiatePolygons (noOfPolygons[level-1],4+speedOfPolygons[level-1],speedOfPolygons[level-1]);
		yield return new WaitForSeconds (1.5f);
		if (bonus)
			boardScript.initBonusLevel(player.getPolyType(),player.getColor());
		decs.enabled = false;
		inputHandler.enableHandler (boardWidth[level-1]);
		InvokeRepeating ("decreaseTimeRemaining", 1, 1);
		
	}

	IEnumerator gameOver(Vector3 pos){
		if (highS <score)
			PlayerPrefs.SetInt ("high_score", score);
		paused=true;		
		inputHandler.disableHandler();
		bPause.gameObject.SetActive (false);
		showFinishScreen ();	

		for (int i=0; i<100; i++) {
			yield return new WaitForSeconds (0.001f);
			cam.orthographicSize = Mathf.Lerp (cam.orthographicSize, endZoom, Time.deltaTime);
			cam.transform.position=new Vector3(Mathf.Lerp (cam.transform.position.x, pos.x, Time.deltaTime),Mathf.Lerp (cam.transform.position.y, pos.y, Time.deltaTime),cam.transform.position.z);
			sceneEnd.color=Color.Lerp(sceneEnd.color,backgroundColor, Time.deltaTime);
		}

			
		
	}

	public void decreaseTimeRemaining(){

		if(!paused){
			if (time > 0) {
			time--;
			debug.text = "" + time;
			if(time<10)
				animatorTime.SetTrigger("hurry");

		}
		else {
			if(bonus){
				nextLevel();
			}
			else{
					player.rigidbody2D.isKinematic=true;
					StartCoroutine("gameOver",player.transform.position); 
			}
			}
		}else
			debug.text = "";
	}

	public void restart(){
		Time.timeScale = 1;
		PlayerPrefs.SetInt ("level",1);
		PlayerPrefs.SetInt ("score",0);
		PlayerPrefs.SetInt ("bonus", 0);
		Application.LoadLevel (Application.loadedLevel);
	}

	public void nextLevel(){
		PlayerPrefs.SetInt ("score",score);

		if (!bonus&&lives==3) {
			PlayerPrefs.SetInt ("bonus", 1);
			PlayerPrefs.SetInt ("level", level );
		} else {

			if(level==4){
				StartCoroutine("gameFinished",player.transform.position);
				return;
			}
			PlayerPrefs.SetInt ("bonus", 0);
			PlayerPrefs.SetInt ("level", level + 1);
		}

	Application.LoadLevel (Application.loadedLevel);
	}


	IEnumerator gameFinished(Vector3 pos){

		if (highS <score)
			PlayerPrefs.SetInt ("high_score", score);
		
		highS = PlayerPrefs.GetInt ("high_score");
		levelText.text="congratulations";
		levelDesc.text = "\nScore " + score+"\n"+"High Score "+highS+"";
		decs.enabled = true;

		for (int i=0; i<100; i++) {
			yield return new WaitForSeconds (0.001f);
			cam.orthographicSize = Mathf.Lerp (cam.orthographicSize, endZoom, Time.deltaTime);
			cam.transform.position=new Vector3(Mathf.Lerp (cam.transform.position.x, pos.x, Time.deltaTime),Mathf.Lerp (cam.transform.position.y, pos.y, Time.deltaTime),cam.transform.position.z);
			sceneEnd.color=Color.Lerp(sceneEnd.color,backgroundColor, Time.deltaTime);
		}

		yield return new WaitForSeconds (5f);
		Application.LoadLevel (2);
	}


	public void showFinishScreen(){
		if (highS <score)
			PlayerPrefs.SetInt ("high_score", score);

		highS = PlayerPrefs.GetInt ("high_score");
		levelText.text="game over";
		levelDesc.text = "\nScore " + score+"\n"+"High Score "+highS;
		decs.enabled = true;

		bExit.gameObject.SetActive (true);	
		bRestart.gameObject.SetActive (true);	
		if(mute)
			bSoundOff.gameObject.SetActive (true);	
		else
			bSoundOn.gameObject.SetActive (true);

		for(int i=0;i<animatorButton.Length;i++)
			animatorButton[i].SetTrigger ("show");
	}
	public void Pause(){
		if (highS <score)
			PlayerPrefs.SetInt ("high_score", score);
		inputHandler.disableHandler ();
		Time.timeScale = 0;
		bPause.gameObject.SetActive (false);	
		bResume.gameObject.SetActive (true);	
		bExit.gameObject.SetActive (true);	
		bRestart.gameObject.SetActive (true);	
		if(mute)
		bSoundOff.gameObject.SetActive (true);	
		else
			bSoundOn.gameObject.SetActive (true);	
	}

	public void Resume(){
		Time.timeScale = 1;
		inputHandler.enableHandler ();
		bPause.gameObject.SetActive (true);	
		bResume.gameObject.SetActive (false);	
		bExit.gameObject.SetActive (false);			
		bRestart.gameObject.SetActive (false);	
		bSoundOff.gameObject.SetActive (false);
		bSoundOn.gameObject.SetActive (false);
	}

	public void finish(){
		Time.timeScale = 1;
		Application.LoadLevel (0);
	}



	public void muteOn(){
		mute = true;
		bSoundOff.gameObject.SetActive (true);
		bSoundOn.gameObject.SetActive (false);	
		PlayerPrefs.SetInt ("mute", 1);
		sound.mute = true;
	}
	
	public void muteOff(){
		mute = false;
		bSoundOff.gameObject.SetActive (false);
		PlayerPrefs.SetInt ("mute", 0);
		bSoundOn.gameObject.SetActive (true);	
		sound.mute = false;
	}

}
