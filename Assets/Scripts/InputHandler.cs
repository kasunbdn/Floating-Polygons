using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour {

	// Use this for initialization
	Camera cam;
	private bool enabeld=false;
	Player player=null;
	DataFetcher dataFetcher;
	Text debug;

	int boardWidth=40;
	bool playerTouched=false;
	Vector2 playerTouchedPosition=new Vector2(0,0);
	public GameObject lineRenderer,touchPointer;
	GameObject spriteRn;
	LineRenderer linerenderer;
	SpriteRenderer spriteR;
	Vector3 zeroVector = new Vector3 (0, 0, 0);
	Vector3 prevPosition=new Vector3 (0, 0, 0);

	float zoomFactor =0.03f;
	float screenSize;
	void Start () {


		enabeld=false;
		//if (cam == null)
			cam = Camera.main;
		//if(player==null)
			player=GameObject.Find("Player").GetComponent<Player>();
		dataFetcher=GetComponent<DataFetcher>();
		//cam = Camera.main;
		debug=GameObject.Find("debug").GetComponent<Text>();
		//debug.text = Screen.dpi + " res " + Screen.width+" " + Screen.height;
		GameObject clone= Instantiate (lineRenderer, transform.position, transform.rotation) as GameObject;
		linerenderer=clone.GetComponent<LineRenderer>();
		spriteRn= Instantiate (touchPointer, transform.position, transform.rotation) as GameObject;
		spriteR=spriteRn.GetComponent<SpriteRenderer>();
		spriteR.enabled = false;
		linerenderer.enabled=false;
		 screenSize = Mathf.Sqrt (Screen.width * Screen.width + Screen.height * Screen.height) / Screen.dpi;


	}


	public void enableHandler(int boardWidth=0){
		enabeld=true;
		if(boardWidth>0)
		this.boardWidth = boardWidth;
	}

	public void disableHandler(){
		enabeld=false;
	}


	// Update is called once per frame
	void Update () {

		if (enabeld) {
			if (Input.touches.Length == 1) {

				if(playerTouched){

					if(Input.GetTouch (0).phase == TouchPhase.Moved){			//set positions of the line renderer
						linerenderer.SetPosition (1,new Vector3(cam.ScreenToWorldPoint(Input.GetTouch (0).position).x,cam.ScreenToWorldPoint(Input.GetTouch (0).position).y,-3) );
						dataFetcher.inputTranslate(Input.GetTouch (0).position);

					} else if(Input.GetTouch (0).phase == TouchPhase.Began){
						linerenderer.enabled=false;
						spriteR.enabled = false;
						playerTouched=false;
					}else if(Input.GetTouch (0).phase == TouchPhase.Ended) {						
						linerenderer.enabled=false;
						spriteR.enabled = false;
						playerTouched=false;
						inputPlayer (Input.GetTouch (0).position);
					}

				}
				else{

					if(CheckTouch (Input.GetTouch (0).position)&&Input.GetTouch (0).phase == TouchPhase.Began){						
						linerenderer.enabled=true;
						spriteR.enabled = true;
						spriteRn.transform.position= new Vector3(player.transform.position.x,player.transform.position.y,-3);
						linerenderer.SetPosition (0, new Vector3(player.transform.position.x,player.transform.position.y,-3));
						linerenderer.SetPosition (1, new Vector3(player.transform.position.x,player.transform.position.y,-3));
						playerTouched=true;
						playerTouchedPosition=cam.WorldToScreenPoint(player.transform.position);						
						player.touch ();
					}
					else if(Input.GetTouch (0).phase == TouchPhase.Moved){
						inputTranslate (Input.GetTouch (0));
						dataFetcher.inputTranslate(Input.GetTouch (0).position);
					}

				}

				prevPosition=cam.ScreenToWorldPoint (Input.GetTouch (0).position);

			} else if (Input.touches.Length > 1) {

				if(playerTouched){
					linerenderer.enabled=false;
					spriteR.enabled = false;
					playerTouched=false;
				}
				if (Input.GetTouch (0).phase == TouchPhase.Moved||Input.GetTouch (1).phase == TouchPhase.Moved){

					bool point1=CheckTouch (Input.GetTouch (0).position);
					bool point2=CheckTouch (Input.GetTouch (1).position);

					if(point1||point2){
							if(Input.GetTouch (0).position.magnitude<Input.GetTouch (1).position.magnitude)
								playerRotate(Input.GetTouch (0), Input.GetTouch (1));
							else
								playerRotate(Input.GetTouch (1), Input.GetTouch (0));

						dataFetcher.inputRotate(Input.GetTouch (0).position);
						dataFetcher.inputRotate(Input.GetTouch (1).position);


					}
					else{
						inputZoom (Input.GetTouch (0), Input.GetTouch (1));
						dataFetcher.inputZoom(Input.GetTouch (0).position);
						dataFetcher.inputZoom(Input.GetTouch (1).position);

					}
				}
					prevPosition=zeroVector;


			}


		}
	}

	bool CheckTouch(Vector2 pos)
	{
		Vector3 objP = cam.ScreenToWorldPoint (pos);
		Vector2 touchPos = new Vector2(objP.x, objP.y);
		Collider2D hit = Physics2D.OverlapPoint(touchPos);		//collider2d to check the point is overlappint another collider
		
		/* if button is touched... */
		if (hit != null) {
			if (hit.tag == "Player") 
				
				return true;									//if laser tag has been touched, return true
		}
		return false;
	}

	public void inputZoom(Touch position1, Touch position2){
		
		float size=cam.orthographicSize;
			Vector3 prev = (cam.ScreenToWorldPoint (position1.position) + cam.ScreenToWorldPoint (position2.position)) / 2;
			Vector2 t1=position1.position;
			Vector2 t2=position2.position;
			Vector2 tt1=new Vector2(t1.x+position1.deltaPosition.x,t1.y+position1.deltaPosition.y);
			Vector2 tt2=new Vector2(t2.x+position2.deltaPosition.x,t2.y+position2.deltaPosition.y);
			
			if(Vector2.Distance(t1,t2)>Vector2.Distance(tt1,tt2)){
				if(size<25)
					cam.orthographicSize+=size*zoomFactor;
			}else {
				if(size>2)
				cam.orthographicSize-=size*zoomFactor;
			}

		Vector3 curr=(cam.ScreenToWorldPoint (position1.position) + cam.ScreenToWorldPoint (position2.position)) / 2;
		cam.transform.Translate(prev-curr);


	}
	public void inputPlayer(Vector2 touchPosition){
			float distance=Vector2.Distance(playerTouchedPosition,touchPosition)/Screen.dpi;
		if (distance > 4/cam.orthographicSize) {
			float zoomScale = 1750/ (Screen.dpi*screenSize);
			player.applyForce (new Vector2 (touchPosition.x - playerTouchedPosition.x, touchPosition.y - playerTouchedPosition.y) * zoomScale);
		}else if (distance > 1/cam.orthographicSize) {
			float zoomScale = 750 / (Screen.dpi*screenSize);
			player.applyForce (new Vector2 (touchPosition.x - playerTouchedPosition.x, touchPosition.y - playerTouchedPosition.y) * zoomScale);
		}
		//debug.text = "" + distance;

		
	}
	
	public void inputTranslate(Touch position1){
		if(prevPosition!=zeroVector)
			cam.transform.Translate(prevPosition-cam.ScreenToWorldPoint(position1.position));

		if (cam.transform.position.x > boardWidth)
			cam.transform.position = new Vector3 (boardWidth,cam.transform.position.y,cam.transform.position.z);
		else if (cam.transform.position.x < -boardWidth)
			cam.transform.position = new Vector3 (-boardWidth,cam.transform.position.y,cam.transform.position.z);

		if (cam.transform.position.y > boardWidth)
			cam.transform.position = new Vector3 (cam.transform.position.x,boardWidth,cam.transform.position.z);
		else if (cam.transform.position.y < -boardWidth)
			cam.transform.position = new Vector3 (cam.transform.position.x,-boardWidth,cam.transform.position.z);

		prevPosition = zeroVector;
	}
	
	public void playerRotate(Touch touch1, Touch touch2){



		Vector2 prevPos1 = touch1.position - touch1.deltaPosition;  // Generate previous frame's finger positions
		Vector2 prevPos2 = touch2.position - touch2.deltaPosition;
		
		Vector2 prevDir = prevPos2 - prevPos1;      
		Vector2 currDir = touch2.position - touch1.position;
		float angle=Mathf.Atan2(currDir.y-prevDir.y, currDir.x-prevDir.x);

		if(angle>0.1)
			player.transform.Rotate(Vector3.forward,0.5f, Space.World);
		else if(angle<-0.1)
			player.transform.Rotate(Vector3.forward,-0.5f, Space.World);

	}


}
