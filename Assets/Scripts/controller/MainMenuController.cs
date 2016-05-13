using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {

	void Start() {
		GameObject.Find("GameVersion_Text").GetComponent<Text>().text = "Alpha Stage - Prototype Tech Demo v0.011";
		GameObject.Find("GameInfo_Text").GetComponent<Text>().text = "2016 - Adnan Bulut Çatıkoğlu";
	}

	public void ExitGame() {
		Application.Quit();
	}

}
