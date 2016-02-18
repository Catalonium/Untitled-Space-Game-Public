using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameplayModeController : MonoBehaviour {
	
	public GameObject playerSpaceship; // Initialization prefabs

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	public void ChangeScene(string level) {
		SceneManager.LoadScene(level);
	}

}
