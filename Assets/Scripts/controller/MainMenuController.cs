using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

	public void ChangeScene(string scene) {
		if (GameObject.FindWithTag("Spaceship/Main"))
			DontDestroyOnLoad(GameObject.FindWithTag("Spaceship/Main"));
		SceneManager.LoadScene(scene);
	}

	public void ExitGame() {
		Application.Quit();
	}

}
