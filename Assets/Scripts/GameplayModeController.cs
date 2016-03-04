using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameplayModeController : MonoBehaviour {
	
	private GameObject playerSpaceship; // Initialization prefabs

	// Use this for initialization
	void Start () {
		playerSpaceship = GameObject.FindWithTag("Player");
		playerSpaceship.GetComponent<SpaceshipPhysics>().enabled = true;
		playerSpaceship.transform.position = -playerSpaceship.GetComponent<Rigidbody>().centerOfMass;
	}

	// Update is called once per frame
	void Update () {
		Camera.main.transform.position = new Vector3(playerSpaceship.transform.position.x, 10, playerSpaceship.transform.position.z);
	}

	public void ChangeScene(string level) {
		DontDestroyOnLoad(playerSpaceship);
		SceneManager.LoadScene(level);
	}

}
