using UnityEngine;
using System.Collections;

public class Polygon : MonoBehaviour {
	
	public int polyType=0;
	public Sprite[] colorSprites;
	public GameObject [] particles;
	SpriteRenderer spriteRenderer;
	bool isEnabled=true;
	int color=0;
	// Use this for initialization
	void Start () {
		spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
		color = Random.Range (0, colorSprites.Length);
		spriteRenderer.sprite=colorSprites[color];

		//Debug.Log (Screen.resolutions[0].height);

	}
	public int getPolyType(){
		return polyType;
	}
	public int getColor(){
		return color;
	}
	public Sprite getSprite(){
		return spriteRenderer.sprite;
	}



	public void setPolyType(Sprite [] sp,int poly){
		spriteRenderer.sprite = sp[color];
		this.polyType = poly;
		GameObject aPartical= Instantiate (particles[color], transform.position, transform.rotation) as GameObject;
		Destroy(aPartical.gameObject,2);
	}

	public void setColor(int col){

		this.color = col;
		spriteRenderer.sprite=colorSprites[col];
		GameObject aPartical= Instantiate (particles[col], transform.position, transform.rotation) as GameObject;
		Destroy(aPartical.gameObject,2);

	}


	void OnCollisionEnter2D(Collision2D coll){
		if (coll.collider.tag == "Player") {
			GameManager.instance.playerHit(this);
		}


	}

	public void destroyPolygon(bool mute=false){
		GameObject aPartical= Instantiate (particles[color], transform.position, transform.rotation) as GameObject;
		if(mute)
			aPartical.GetComponent<AudioSource>().mute=true;
		Destroy(aPartical.gameObject,2);
		Destroy(gameObject);

	}

	

	
}
