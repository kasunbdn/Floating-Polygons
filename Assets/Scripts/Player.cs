using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	// Use this for initialization
	public Sprite  [] circle,triangle,sq; 	
	public GameObject player_hit;
	AudioSource sound;
	Collider2D [] col; 
	int score = 0;
	SpriteRenderer spriteRenderer;
	int polyType=0;
	int color=1;
	int timer=30;
	void Start () {
		spriteRenderer = this.GetComponent<SpriteRenderer> ();
		sound= GetComponent<AudioSource>();
		col = new Collider2D[3];
		col[1]=GetComponent<BoxCollider2D>();
		col[2]=GetComponent<PolygonCollider2D>();
		col[0]=GetComponent<CircleCollider2D>();
		
		initialize ();
	
	}

	void initialize(){
		polyType = Random.Range (0,3);
		color = Random.Range (0,4);
		if(polyType==0)
			setSprite (circle[color],polyType,color);
		else if(polyType==1)
			setSprite (sq[color],polyType,color);
		
		else if(polyType==2)
			setSprite (triangle[color],polyType,color);
	}
	
	public int getPolyType(){
		return polyType;
	}
	public int getColor(){
		return color;
	}

	public bool validHit(Polygon aPolygon,bool mute=false){
		if (polyType == aPolygon.getPolyType () || color == aPolygon.getColor ()) {
			setSprite(aPolygon.getSprite(),aPolygon.getPolyType(),aPolygon.getColor());
			return true;
		}
		if(!mute)
		sound.Play();
		return false;
	}

	public void setSprite(Sprite sp,int nm,int cl){
		col [0].enabled = false;
		col [1].enabled = false;
		col [2].enabled = false;
		spriteRenderer.sprite = sp;
		polyType = nm;
		col [nm].enabled = true;
		color = cl;
	}

	public void touch(){
		transform.rigidbody2D.isKinematic = true;
		//transform.rigidbody2D.velocity = new Vector2 (0,0);
		transform.rigidbody2D.isKinematic = false;
	}
	public void applyForce(Vector2 force){
		transform.rigidbody2D.AddForce (force);
	}

	public void hitOther(Polygon aPolygon){
		transform.rigidbody2D.isKinematic = true;
		spriteRenderer.enabled = false;
		GameObject aPartical= Instantiate (player_hit, transform.position, transform.rotation) as GameObject;
		Destroy(aPartical.gameObject,2);
		StartCoroutine ("resetPlayer");
		setSprite(aPolygon.getSprite(),aPolygon.getPolyType(),aPolygon.getColor());
	}

	IEnumerator resetPlayer(){
			yield return new WaitForSeconds (0.7f);
		spriteRenderer.enabled = true;
		yield return new WaitForSeconds (0.3f);
		spriteRenderer.enabled = false;
		yield return new WaitForSeconds (0.3f);
		spriteRenderer.enabled = true;
		yield return new WaitForSeconds (0.1f);
		spriteRenderer.enabled = false;
		yield return new WaitForSeconds (0.1f);
		spriteRenderer.enabled = true;
		yield return new WaitForSeconds (0.1f);
		spriteRenderer.enabled = false;
		yield return new WaitForSeconds (0.1f);
		spriteRenderer.enabled = true;
		transform.rigidbody2D.isKinematic = false;

		
	}
}
