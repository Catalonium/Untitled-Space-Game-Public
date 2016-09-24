using UnityEngine;
using System.Collections;

public class GridCollisionDetector : MonoBehaviour {

	public bool CollisionState;
	
	// Use this for initialization
	private void Start() {
		CollisionState = false;
	}

	private void OnTriggerEnter(Collider other) {
		if (other.tag == "Spaceship/Block") {
			CollisionState = true;
		}
	}

	private void OnTriggerExit(Collider other) {
		if (other.tag == "Spaceship/Block") {
			CollisionState = false;
		}
	}

//	public bool CollisionState { get; private set; }

}
