using UnityEngine;
using System.Collections;

public class LightController : MonoBehaviour {

	public float startingEnergy;
	public float eMultiplyer;

	private float energy;
	private Light light;

	// Use this for initialization
	void Start () {
		light = GetComponentInChildren<Light> ();
		energy = startingEnergy;
	}
	
	// Update is called once per frame
	void Update () {
		//Intensity goes from 0 to 8
		light.intensity = energy*eMultiplyer;
	}

	public void GiveAwayEnergy(float e){
		energy -= e;
	}

	public float GetEnergy(){
		return energy;
	}
}
