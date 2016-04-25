using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsMenuController : MonoBehaviour {

	private Dropdown qualityLevel;
	private Slider masterVolume;

	void Start() {
		// GUI elements init
		// Graphics
		qualityLevel = GameObject.Find("QualityLevel").GetComponent<Dropdown>();
		// Sound
		masterVolume = GameObject.Find("MasterVolume").GetComponent<Slider>();

		// Set current settings on GUI
		// Graphics
		qualityLevel.value = QualitySettings.GetQualityLevel();
		// Sound
		masterVolume.value = AudioListener.volume;
	}

	public void ChangeQualityLevel() {
		// Change setting
		QualitySettings.SetQualityLevel(qualityLevel.value);
	}
	public void ChangeMasterVolume() {
		// Change setting
		AudioListener.volume = masterVolume.value;
	}

}
