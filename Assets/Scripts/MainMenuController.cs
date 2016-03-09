using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

	public void ChangeScene(string scene) {
		if (GameObject.FindWithTag("Spaceship/Construction"))
			DontDestroyOnLoad(GameObject.FindWithTag("Spaceship/Construction"));
		SceneManager.LoadScene(scene);
	}

	public void ExitGame() {
		Application.Quit();
	}

}
