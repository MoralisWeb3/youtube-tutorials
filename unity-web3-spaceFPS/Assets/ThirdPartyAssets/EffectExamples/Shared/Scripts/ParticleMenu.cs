using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParticleMenu : MonoBehaviour {

	// our ParticleExamples class being turned into an array of things that can be referenced
	public ParticleExamples[] particleSystems;

	// the gun GameObject
	public GameObject gunGameObject;

	// a private integer to store the current position in the array
	private int currentIndex;

	// the currently shown prefab game object
	private GameObject currentGO;

	// where to spawn prefabs 
	public Transform spawnLocation;

	// references to the UI Text components
	public Text title;
	public Text description;
	public Text navigationDetails;

	// setting up the first menu item and resetting the currentIndex to ensure it's at zero
	void Start()
	{
		Navigate (0);
		currentIndex = 0;
	}


	// our public function that gets called by our menu's buttons
	public void Navigate(int i){

		// set the current position in the array to the next or previous position depending on whether i is -1 or 1, defined in our button event
		currentIndex = (particleSystems.Length + currentIndex + i) % particleSystems.Length;

		// check if there is a currentGO, if there is (if its not null), then destroy it to make space for the new one..
		if(currentGO != null)
			Destroy (currentGO);

		// ..spawn the relevant game object based on the array of potential game objects, according to the current index (position in the array)
		currentGO = Instantiate (particleSystems[currentIndex].particleSystemGO, spawnLocation.position + particleSystems[currentIndex].particlePosition, Quaternion.Euler(particleSystems[currentIndex].particleRotation)) as GameObject;

		// only activate the gun GameObject if the current effect is a weapon effect
		gunGameObject.SetActive (particleSystems[currentIndex].isWeaponEffect);

		// setup the UI texts according to the strings in the array 
		title.text = particleSystems [currentIndex].title;
		description.text = particleSystems [currentIndex].description;
		navigationDetails.text = "" + (currentIndex+1) + " out of " + particleSystems.Length.ToString();

	}
}
