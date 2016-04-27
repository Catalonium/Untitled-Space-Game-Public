using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {

	void Start() {
		GameObject.Find("GameRevision_Text").GetComponent<Text>().text = "Alpha Stage - revision 26";
		GameObject.Find("GameVersion_Text").GetComponent<Text>().text = "Prototype Tech Demo v0.01";
		GameObject.Find("GameInfo_Text").GetComponent<Text>().text = "2016 - Adnan Bulut Çatıkoğlu";
	}

	public void ExitGame() {
		Application.Quit();
	}

}
