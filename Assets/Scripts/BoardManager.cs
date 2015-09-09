using UnityEngine;
using System.Collections;

public class BoardManager : MonoBehaviour {

	public GameObject particals,border;
	public GameObject [] prototype;
	public Sprite  [] circle,sq,triangle; 



	private GameObject Board,clone;
	int boardWidth=40;

	public void createBoard (int width) {
		boardWidth = width;
		Board = new GameObject ("Board");
		Board.transform.parent = transform;
		border.GetComponent<BoxCollider2D> ().size = new Vector2 (width*2,0.5f);
		particals.GetComponent<ParticleSystem>().startLifetime =  width*0.09135f;

		clone = Instantiate (particals, new Vector3 (width, width, 0), Quaternion.Euler(90,90,0)) as GameObject;
		clone.transform.parent=Board.transform;
		clone = Instantiate (particals, new Vector3 (width, -width, 0), Quaternion.Euler(180,90,0)) as GameObject;
		clone.transform.parent=Board.transform;
		clone = Instantiate (particals, new Vector3 (-width, -width, 0), Quaternion.Euler(270,90,0)) as GameObject;
		clone.transform.parent=Board.transform;
		clone = Instantiate (particals, new Vector3 (-width, width, 0), Quaternion.Euler(0,90,0)) as GameObject;
		clone.transform.parent=Board.transform;

		clone = Instantiate (border, new Vector3 (width, 0, 0), Quaternion.Euler(0,0,90)) as GameObject;
		clone.transform.parent=Board.transform;
		clone = Instantiate (border, new Vector3 (0, -width, 0), Quaternion.Euler(0,0,180)) as GameObject;
		clone.transform.parent=Board.transform;
		clone = Instantiate (border, new Vector3 (-width, 0, 0), Quaternion.Euler(0,0,270)) as GameObject;
		clone.transform.parent=Board.transform;
		clone = Instantiate (border, new Vector3 (0, width, 0), Quaternion.Euler(0,0,0)) as GameObject;
		clone.transform.parent=Board.transform;
	}

	public void InstatiatePolygons(int numberOfPolygons,int playerPadding,int force){
		int poyCount = (int)Mathf.Sqrt (numberOfPolygons);
		int forLoopGap = (2 *10* boardWidth-20) / poyCount;

		playerPadding *= 10;
		int forLoopLength = (poyCount) * (int)(forLoopGap / 2);
		if (poyCount%2 == 1)
		forLoopLength = (poyCount-1) * (int)(forLoopGap / 2);

		int count = 0;
		for (int j=-forLoopLength; j<=0; j+=forLoopGap) {
			for (int i=-forLoopLength; i<=0; i+=forLoopGap) {
				if (!(i >- playerPadding && j >- playerPadding )) {
					
					if (count >= numberOfPolygons)
						break;
					clone = Instantiate (prototype [Random.Range (0, prototype.Length)], new Vector3 (transform.position.x + j / 10, transform.position.y + i / 10, 0), transform.rotation) as GameObject;
					clone.transform.SetParent (Board.transform);
					clone.transform.Rotate (Vector3.forward * Random.Range (0, 360));
					clone.rigidbody2D.AddForce (new Vector2 (Random.Range (-200, 200) / 10, Random.Range (-200, 200) / 10) * force);
					count++;
					if (count >= numberOfPolygons)
						break;

					if(i<0)
					{clone = Instantiate (prototype [Random.Range (0, prototype.Length)], new Vector3 (transform.position.x + j / 10, transform.position.y - i / 10, 0), transform.rotation) as GameObject;
					clone.transform.SetParent (Board.transform);
					clone.transform.Rotate (Vector3.forward * Random.Range (0, 360));
					clone.rigidbody2D.AddForce (new Vector2 (Random.Range (-200, 200) / 10, Random.Range (-200, 200) / 10) * force);
					count++;
					}
					if (count >= numberOfPolygons)
						break;
				}
			}
			if(j<0)
			for (int i=-forLoopLength; i<=0; i+=forLoopGap) {
				if (!(i >- playerPadding && j >- playerPadding )) {
					
					if (count >= numberOfPolygons)
						break;
					clone = Instantiate (prototype [Random.Range (0, prototype.Length)], new Vector3 (transform.position.x - j / 10, transform.position.y + i / 10, 0), transform.rotation) as GameObject;
					clone.transform.SetParent (Board.transform);
					clone.transform.Rotate (Vector3.forward * Random.Range (0, 360));
					clone.rigidbody2D.AddForce (new Vector2 (Random.Range (-200, 200) / 10, Random.Range (-200, 200) / 10) * force);
					count++;
					if (count >= numberOfPolygons)
						break;
					
					if(i<0)
					{
					clone = Instantiate (prototype [Random.Range (0, prototype.Length)], new Vector3 (transform.position.x - j / 10, transform.position.y - i / 10, 0), transform.rotation) as GameObject;
					clone.transform.SetParent (Board.transform);
					clone.transform.Rotate (Vector3.forward * Random.Range (0, 360));
					clone.rigidbody2D.AddForce (new Vector2 (Random.Range (-200, 200) / 10, Random.Range (-200, 200) / 10) * force);
					count++;
					}
					if (count >= numberOfPolygons)
						break;
				}
			}


		}

		for (int i=count; i<numberOfPolygons; i++) {
			int x=Random.Range(playerPadding,forLoopLength)*((int)Random.Range(0,2)==0?1:-1);
			int y=Random.Range(playerPadding,forLoopLength)*((int)Random.Range(0,2)==0?1:-1);
			clone = Instantiate (prototype [Random.Range (0, prototype.Length)], new Vector3(transform.position.x+x/10,transform.position.y+y/10,0), transform.rotation) as GameObject;
			clone.transform.SetParent (Board.transform);
			clone.transform.Rotate(Vector3.forward*Random.Range(0,360));
			clone.rigidbody2D.AddForce(new Vector2(Random.Range (-200, 200)/10,Random.Range (-200, 200)/10)*force);
		}



	}

	public void initBonusLevel( int polyType,int color){

		int rand = Random.Range (0, 10);
		foreach (Transform child in Board.transform) {
			if(child.tag=="poly"){

				if(rand>4){
					if(polyType==0)
					child.gameObject.GetComponent<Polygon>().setPolyType(circle,polyType);
					else if(polyType==1)
						child.gameObject.GetComponent<Polygon>().setPolyType(sq,polyType);
					else 
						child.gameObject.GetComponent<Polygon>().setPolyType(triangle,polyType);
				}
				else
					child.gameObject.GetComponent<Polygon>().setColor(color);

			}
		}
	}

}
