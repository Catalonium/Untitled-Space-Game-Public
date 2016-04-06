using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayModeController : MonoBehaviour {
	
	private GameObject playerSpaceship; // Initialization prefabs
	private Transform mainCam, backCam; // Initialization for cameras

	// Use this for initialization
	void Start() {
		var construction = GameObject.FindWithTag("Spaceship/Construction");
		var grids = GameObject.FindWithTag("Spaceship/Grids");

		var spaceshipPrefab = (GameObject)Resources.Load("Prefabs/Player/Spaceship", typeof(GameObject));
		playerSpaceship = (GameObject)Instantiate(spaceshipPrefab, construction.GetComponent<Rigidbody>().centerOfMass, Quaternion.Euler(Vector3.zero));
		playerSpaceship.name = "Spaceship";
		grids.transform.parent = playerSpaceship.transform;

		Destroy(construction);
		
		playerSpaceship.transform.position = Vector3.zero;
		playerSpaceship.GetComponent<SpaceshipPhysics>().enabled = true;
//		playerSpaceship.transform.position = -playerSpaceship.GetComponent<Rigidbody>().centerOfMass;

		mainCam = Camera.main.transform;
		backCam = GameObject.FindGameObjectWithTag("BackgroundCamera").transform;
	}

	// Update is called once per frame
	void Update() {
		// Player cam mover
		mainCam.position = new Vector3(playerSpaceship.transform.position.x, 10, playerSpaceship.transform.position.z);
		// Background cam mover
		backCam.position = new Vector3(playerSpaceship.transform.position.x * 0.005f, 10, playerSpaceship.transform.position.z * 0.005f);
	}

	public void ChangeScene(string scene) {
		DontDestroyOnLoad(playerSpaceship);
		SceneManager.LoadScene(scene);
	}

}
