using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameplayModeController : MonoBehaviour {
	
	private GameObject playerSpaceship; // Initialization prefabs

	// Use this for initialization
	void Start () {
		playerSpaceship = GameObject.FindWithTag("Player");
		playerSpaceship.GetComponent<SpaceshipPhysics>().enabled = true;
	}

	// Update is called once per frame
	void Update () {

	}

	public void ChangeScene(string level) {
		DontDestroyOnLoad(playerSpaceship);
		SceneManager.LoadScene(level);
	}

}
