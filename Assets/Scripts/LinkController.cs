using UnityEngine;
using System.Collections;

public class LinkController : MonoBehaviour
{

	//TO DO? - Generalize Link so that it applies the force towards an 
	//expecific point within the object and relative to its transform
	//rather than to the center of the object

	public int forceStrength;
	public float powerChargeRate;
	private Camera cam;
	private PlayerController playerCont;
	private GameObject linkedA;
	private GameObject linkedB;
	private Rigidbody2D rbA;
	private Rigidbody2D rbB;
	private Vector2 posA;
	private Vector2 posB;
	private Vector2 forceA;
	private Vector2 forceB;
	private int objsLinked = 0;
	private bool isADynamic;
	private bool isBDynamic;
	private float power = 1;
	private bool active = false;
	private BoxCollider2D colA;
	private BoxCollider2D colB;
	private bool playerCanMagic;

	// Use this for initialization
	void Start ()
	{
		cam = FindObjectOfType<Camera> ();
		playerCont = FindObjectOfType<PlayerController> ();
		power = 1;
		active = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		playerCanMagic = playerCont.CanDoMagic ();
		if (objsLinked < 2) {
			if (Input.GetKeyDown (KeyCode.LeftShift)) {
				Destroy (gameObject);
			}
			if (playerCanMagic && Input.GetMouseButtonDown (0)) {
				RaycastHit2D hit = Physics2D.Raycast (new Vector2 (cam.ScreenToWorldPoint (Input.mousePosition).x, cam.ScreenToWorldPoint (Input.mousePosition).y), Vector2.zero, Mathf.Infinity, 1 << 9);
				if (hit.collider != null/* && hit.collider.gameObject.layer == 9*/) {
					if (objsLinked == 0) { 
						colA = gameObject.AddComponent<BoxCollider2D> ();
						colA.isTrigger = true;
						colA.size = new Vector2 (0.3f, 0.3f);
						if (hit.collider.gameObject.tag == "Fixed Obj") {
							isADynamic = false;
							posA = new Vector2 (cam.ScreenToWorldPoint (Input.mousePosition).x, cam.ScreenToWorldPoint (Input.mousePosition).y);
							transform.position = new Vector3 (posA.x, posA.y, 0);
							objsLinked++;
							//Debug.Log ("A fixed");
						} else {
							isADynamic = true;
							//Debug.Log ("A linked");
							linkedA = hit.collider.gameObject;
							rbA = hit.collider.attachedRigidbody;
							posA = new Vector2 (hit.collider.gameObject.transform.position.x, hit.collider.gameObject.transform.position.y);
							objsLinked++;
						}
					} else {
						//TO DO - Add code to prevent linking to self, and linking between two fixed objects
						colB = gameObject.AddComponent<BoxCollider2D> ();
						colB.isTrigger = true;
						colB.size = new Vector2 (0.3f, 0.3f);
						if (hit.collider.gameObject.tag == "Fixed Obj") {
							if(isADynamic){
							isBDynamic = false;
							posB = new Vector2 (cam.ScreenToWorldPoint (Input.mousePosition).x, cam.ScreenToWorldPoint (Input.mousePosition).y);
								colB.offset = posB - posA;
								objsLinked++;
							}//Debug.Log ("B fixed");
						} else {
							isBDynamic = true;
							//Debug.Log ("B linked");
							linkedB = hit.collider.gameObject;	
							rbB = hit.collider.attachedRigidbody;
							posB = new Vector2 (hit.collider.gameObject.transform.position.x, hit.collider.gameObject.transform.position.y);
							colB.offset = posB - posA;
							if (linkedA != linkedB) objsLinked++;
							else posB = Vector2.zero;
						}

					}
				
				}
			}
		} else {
			if (active) { 
				//Possibly change to aplly force to point? 
				//harder to control but ould have interesting effets.
				//bug where alredy linked objet cant link again
				if (isADynamic) {
					posA = new Vector2 (linkedA.transform.position.x, linkedA.transform.position.y);
					transform.position = new Vector3 (posA.x, posA.y, 0);
					forceA = (posB - posA).normalized;
					rbA.AddForce (forceA * forceStrength * power);

				}
				if (isBDynamic) {
					posB = new Vector2 (linkedB.transform.position.x, linkedB.transform.position.y);
					forceB = (posA - posB).normalized;
					rbB.AddForce (forceB * forceStrength * power);
				}
				colB.offset = posB - posA;
			} else {
				//For debugging
				if (isADynamic) {
					posA = new Vector2 (linkedA.transform.position.x, linkedA.transform.position.y);
					transform.position = new Vector3 (posA.x, posA.y, 0);
				}
				if (isBDynamic) {
					posB = new Vector2 (linkedB.transform.position.x, linkedB.transform.position.y);
				}
				//

				if (Input.GetMouseButton (0)) {
					if (playerCont.GetEnergy () <= power) {
						active = true;
						playerCont.SpendEnergy (power * playerCont.GetMagicCostMutiplyer ());
					}
					power += powerChargeRate;
					if (power >= 4) {
						active = true;
						playerCont.SpendEnergy (4 * playerCont.GetMagicCostMutiplyer ());
					}
				} else {
					active = true;
					playerCont.SpendEnergy (power * playerCont.GetMagicCostMutiplyer ());
				}
			}
			Debug.DrawLine (new Vector3 (posA.x, posA.y, 0), new Vector3 (posB.x, posB.y, 0), new Color ((power - 1) * 0.33f, (2 - power) * 0.33f, 0));
			//Debug.Log("Force Aplied");
		}
	}

	public float GetPower ()
	{
		return power;
	}
}
