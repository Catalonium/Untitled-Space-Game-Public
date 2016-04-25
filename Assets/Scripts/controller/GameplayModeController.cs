using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameplayModeController : MonoBehaviour {
	
	private GameObject playerSpaceship; // spaceship
	private Camera mainCam, backCam; // cameras & starfield
	
	void Start() {

//		var construction = GameObject.FindWithTag("Spaceship/Main");
		playerSpaceship = GameObject.FindWithTag("Spaceship/Main");
		var grids = GameObject.FindWithTag("Spaceship/Grids");

		// COLLIDER TRIGGER SWITCH
		BoxCollider[] blockColliders = grids.GetComponentsInChildren<BoxCollider>();
		foreach (BoxCollider bc in blockColliders) {
			bc.isTrigger = false;
		}

//		var spaceshipPrefab = (GameObject)Resources.Load("Prefabs/Player/Spaceship", typeof(GameObject));
//		playerSpaceship = (GameObject)Instantiate(spaceshipPrefab, construction.GetComponent<Rigidbody>().centerOfMass, Quaternion.Euler(Vector3.zero));
//		playerSpaceship.name = construction.name;
//		grids.transform.parent = playerSpaceship.transform;
//
//		Destroy(construction);
//
//		playerSpaceship.transform.position = Vector3.zero;
//		playerSpaceship.GetComponent<SpaceshipPhysics>().enabled = true;

		grids.transform.position = -playerSpaceship.GetComponent<Rigidbody>().centerOfMass;
		playerSpaceship.GetComponent<SpaceshipPhysics>().PhysicsToggle(true);

		// Camera & Starfield initialization
		mainCam = Camera.main;
		backCam = GameObject.FindGameObjectWithTag("BackgroundCamera").GetComponent<Camera>();

		// Spaceship name init
		GameObject.Find("GUIShipName").GetComponent<Text>().text = playerSpaceship.name;

	}
	
	void Update() {
		// Player cam mover
		mainCam.transform.position = new Vector3(playerSpaceship.transform.position.x, 10, playerSpaceship.transform.position.z);
		// Background cam mover
		backCam.transform.position = new Vector3(playerSpaceship.transform.position.x * 0.005f, 10, playerSpaceship.transform.position.z * 0.005f);
	}

	void LateUpdate() {
		// Camera zoom
		if (Input.GetAxis("Mouse ScrollWheel") > 0 && Camera.main.orthographicSize > 3) {
			Camera.main.orthographicSize--;
		}
		else if (Input.GetAxis("Mouse ScrollWheel") < 0 && Camera.main.orthographicSize < 25) {
			Camera.main.orthographicSize++;
		}
		// Camera zoom reset
		else if (Input.GetMouseButton(2)) {
			Camera.main.orthographicSize = 15;
		}
	}

}
