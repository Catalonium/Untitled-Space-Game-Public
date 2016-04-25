using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour {

	public float screenFadeSpeed = 0.1f;

	private Image screenFader;

	private bool fadeInActive;
	
	void Start() {
		// Cache ScreenFader object
		screenFader = GameObject.FindWithTag("GUI/ScreenFader").GetComponent<Image>();
		
		// Set initial starting alpha
		screenFader.color = Color.black;
	}

	void OnGUI() {
		// Fade-in effect until screenFader is completely "clear"
		if (screenFader.color != Color.clear)
			screenFader.color = Color.Lerp(screenFader.color, Color.clear, screenFadeSpeed);
	}

	public void ChangeScene(string scene) {
		DontDestroyOnLoad(GameObject.FindWithTag("Spaceship/Main"));
		SceneManager.LoadScene(scene);
	}

}
