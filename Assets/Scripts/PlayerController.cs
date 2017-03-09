using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	////Link Constants
	///
	public int startingEnergy;
	public int maxEnergy;
	public GameObject link;
	public float maxDistanceForMagic;

	//Not Currently in use:
	public float minDistanceForMagic;

	public bool magicCostIsActive;
	///

	////Movement Constants
	public float acceleration;
	public float dragFactor;
	public float jumpForce;


	private float magicCostDistanceMultiplyer;
	private float energy;
	private Rigidbody2D rb2d;
	private bool grounded = true;
	private Camera cam;
	private float distanceFromPlayerToCursor;
	private Vector2 cursorPos;
	private bool canDoMagic;
	private Vector2 groundVel = Vector2.zero;
	private float ownSpeed;
	private float groundSpeed;

	// Use this for initialization
	void Start ()
	{
		cam = FindObjectOfType<Camera> ();
		energy = startingEnergy;
		rb2d = gameObject.GetComponent<Rigidbody2D> ();
	}

	// Update is called once per frame
	void Update ()
	{
		float hAxis = Input.GetAxis ("Horizontal");	
		rb2d.velocity = new Vector2 (acceleration * hAxis + groundVel.x, rb2d.velocity.y);
		if (rb2d.velocity.x < 0.1 && rb2d.velocity.x > -0.1) {
			groundVel.x = 0;
		}
		if (grounded && Input.GetKeyDown (KeyCode.W)) {
			rb2d.velocity = new Vector2 (rb2d.velocity.x, jumpForce);
		}

		cursorPos = new Vector2 (cam.ScreenToWorldPoint (Input.mousePosition).x, cam.ScreenToWorldPoint (Input.mousePosition).y);
		distanceFromPlayerToCursor = (cursorPos - new Vector2 (transform.position.x, transform.position.y)).magnitude;
		if (magicCostIsActive)
			magicCostDistanceMultiplyer = Mathf.Max (minDistanceForMagic, Mathf.Min (distanceFromPlayerToCursor, maxDistanceForMagic));
		else {
			magicCostDistanceMultiplyer = 1;
			maxDistanceForMagic = 20;
		}
		if (distanceFromPlayerToCursor < maxDistanceForMagic)
			canDoMagic = true;
		else
			canDoMagic = false;

		if (canDoMagic) {
			if (Input.GetMouseButtonDown (1)) {
				RaycastHit2D hit = Physics2D.Raycast (new Vector2 (cam.ScreenToWorldPoint (Input.mousePosition).x, cam.ScreenToWorldPoint (Input.mousePosition).y), Vector2.zero, 0, 1 << LayerMask.NameToLayer("Links") );
				if (hit.collider != null && hit.collider.gameObject.tag == "Link") {
					LinkController lC = hit.collider.gameObject.GetComponent<LinkController> ();
					if (lC != null)
						GainEnergy (lC.GetPower ());
					Destroy (hit.collider.gameObject);
				}
			}
			if (Input.GetMouseButton (1)) {
				RaycastHit2D hit = Physics2D.Raycast (new Vector2 (cam.ScreenToWorldPoint (Input.mousePosition).x, cam.ScreenToWorldPoint (Input.mousePosition).y), Vector2.zero, 0, 1 << LayerMask.NameToLayer("Lights") );
				if (hit.collider != null && hit.collider.gameObject.tag == "Light") {
					LightController lightC = hit.collider.gameObject.GetComponent<LightController> ();
					if (lightC.GetEnergy () >= lightC.eMultiplyer) {
						lightC.GiveAwayEnergy (lightC.eMultiplyer);
						GainEnergy (lightC.eMultiplyer);
					}
				}
			}
		}
		if (Input.GetKeyDown (KeyCode.LeftShift)) {
			if (energy >= 1) {
				Instantiate (link);
			}
		}
		Debug.DrawLine (new Vector2 (transform.position.x - energy * 0.025f, transform.position.y - 1), new Vector2 (transform.position.x + energy * 0.025f, transform.position.y - 1), Color.cyan);
	}

	void OnTriggerEnter2D (Collider2D otherObj)
	{
		grounded = true;
	}

	void OnTriggerStay2D (Collider2D otherObj)
	{
		grounded = true;
		if (otherObj.gameObject.tag == "Dynamic Obj") {
			groundVel = otherObj.attachedRigidbody.velocity;
		} else {
			if (Mathf.Abs (rb2d.velocity.x) < dragFactor)
				groundVel.x = 0;
			if(groundVel.x > dragFactor){
				groundVel.x -= dragFactor;
				groundVel.x = Mathf.Max(groundVel.x, 0);
			}else if (groundVel.x < -dragFactor){
				groundVel.x += dragFactor;
				groundVel.x = Mathf.Min(groundVel.x, 0);
			}else{
				groundVel.x = 0;
			}
		}
	}

	void OnTriggerExit2D (Collider2D otherObj)
	{
		grounded = false;
	}

	public void SpendEnergy (float n)
	{
		Debug.Log ("Magic lost: " + n / magicCostDistanceMultiplyer);
		energy -= n * magicCostDistanceMultiplyer;
		energy = Mathf.Max (energy, 0);
	}

	public void GainEnergy (float n)
	{
		Debug.Log ("Magic gained: " + n / magicCostDistanceMultiplyer);
		energy += n / magicCostDistanceMultiplyer;
		energy = Mathf.Min (energy, maxEnergy);
	}


	//// Accesors

	public float GetEnergy ()
	{
		return energy;
	}

	public bool CanDoMagic ()
	{
		return canDoMagic;
	}

	public float GetMagicCostMutiplyer ()
	{
		return magicCostDistanceMultiplyer;
	}
}
