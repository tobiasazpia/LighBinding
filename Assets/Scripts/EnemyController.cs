using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

	private GameObject player;
	private Vector3 toPlayer;
	private Rigidbody2D rb2d;

	public float fatalThereshold;
	public float awarenessDistance;
	public float speed;

	// Use this for initialization
	void Start () {
		player = FindObjectOfType<PlayerController>().gameObject;
		rb2d = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		toPlayer = player.transform.position - gameObject.transform.position;
		if (toPlayer.magnitude < awarenessDistance) {
			toPlayer.Normalize ();
			rb2d.velocity = toPlayer*speed;
		}
	}

	void OnTriggerEnter2D (Collider2D otherObj)
	{
		if (otherObj.gameObject.tag == "Dynamic Obj") {
			if((otherObj.attachedRigidbody.velocity - rb2d.velocity).magnitude > fatalThereshold){
				Destroy(gameObject);
			}
		}else if(otherObj.gameObject.tag == "Player"){
			Application.LoadLevel("Level1");
		}
	}
}
