using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ModalPopupController : MonoBehaviour {

	private BuildModeController bmc;
	private SceneController sc;
	private SpaceshipStats ss;
	private GameObject spaceshipGrid;

	private CanvasGroup modalPopupCanvas, errorPopupCanvas;
	private Text question, error;

	private string commandType;
	private float errorDuration;

	public Button gameplayModeButton;

	// Use this for initialization
	void Start() {
		// Get BuildModeController to cache
		bmc = GetComponent<BuildModeController>();
		sc = GetComponent<SceneController>();
		ss = GameObject.FindWithTag("Spaceship/Main").GetComponent<SpaceshipStats>();
		spaceshipGrid = GameObject.FindWithTag("Spaceship/Grids");

		// Modal popup initialization
		modalPopupCanvas = GameObject.FindGameObjectWithTag("GUI/ModalPanel/Main").GetComponent<CanvasGroup>();
		modalPopupCanvas.alpha = 0;
		question = GameObject.FindGameObjectWithTag("GUI/ModalPanel/Text").GetComponent<Text>();
		question.text = "";

		// Error popup initialization
		errorPopupCanvas = GameObject.FindGameObjectWithTag("GUI/ErrorPanel/Main").GetComponent<CanvasGroup>();
		errorPopupCanvas.alpha = 0;
		error = GameObject.FindGameObjectWithTag("GUI/ErrorPanel/Text").GetComponent<Text>();
		error.text = "";

		// Other initializations
		commandType = "";
		errorDuration = 1f;
		isErrorFadeOutActive = false;
	}

	void Update() {
		// Error popup fade-out effect
		if (isErrorFadeOutActive) {
			if (errorDuration > 0) {
				errorDuration -= Time.deltaTime;
			}
			else {
				if (errorPopupCanvas.alpha > 0.01f)
					errorPopupCanvas.alpha -= Time.deltaTime;
				else {
					errorPopupCanvas.alpha = 0f;
					isErrorFadeOutActive = false;
					error.text = "";
				}
			}
		}

		// Gameplay button disable if there's no block exists in spaceship grid
		if (spaceshipGrid.GetComponentsInChildren<Block>().Length < 1) {
			gameplayModeButton.interactable = false;
		}
		else if (!gameplayModeButton.interactable) {
			gameplayModeButton.interactable = true;
		}
	}

	public void ShowPopupGameplay() {
		if (!ss.isControllable) {
			commandType = "gameplay";
			question.text = "You will not be able to control the spaceship, because there's no Bridge block found on it. Are you sure you want to proceed?";
			ShowModalPopup();
		}
		else {
			sc.ChangeScene("GameplayMode");
		}
	}

	public void ShowPopupResetShip() {
		commandType = "resetship";
		question.text = "Are you sure you want to reset current spaceship?";
		ShowModalPopup();
	}

	public void ShowPopupSaveShip() {
		if (File.Exists(Application.dataPath + "/spaceship.save")) {
			commandType = "overwrite";
			question.text = "There's a saved spaceship exists. Do you want to overwrite it?";
		}
		else {
			commandType = "save";
			question.text = "Are you sure you want to save this spaceship?";
		}
		ShowModalPopup();
	}

	public void ShowPopupLoadShip() {
		if (File.Exists(Application.dataPath + "/spaceship.save")) {
			commandType = "load";
			question.text = "Are you sure you want to load a saved spaceship, replacing the existing one?";
			ShowModalPopup();
		}
		else {
			ShowErrorPopup("Error - no save file found.");
		}
	}

	public void ResponseYes() {
		if (commandType.Equals("overwrite") || commandType.Equals("save")) {
			bmc.SaveShip();
		}
		else if (commandType.Equals("load")) {
			bmc.LoadShip();
		}
		else if (commandType.Equals("gameplay")) {
			sc.ChangeScene("GameplayMode");
		}
		else if (commandType.Equals("resetship")) {
			bmc.ResetShip();
			ShowErrorPopup("Spaceship reset.");
		}
		else {
			ShowErrorPopup("An unknown error occured.\nNo known commandType assigned in ModalPopupController.cs");
		}
		// Clear the cache since the command is executed at this point
		ClearModalPopup();
	}

	public void ResponseNo() {
		// Technically do nothing other than clearing the cache
		ClearModalPopup();
	}
	
	private void ShowModalPopup() {
		// Set alpha to one (visible)
		modalPopupCanvas.alpha = 1;
		// Set isModalPopupActive to true
		isModalPopupActive = true;
		// Toggle "interactable" in CanvasGroup (for fixing stealth interaction when popup is not visible)
		modalPopupCanvas.interactable = isModalPopupActive;
		// Precaution for GUI being on top of the Camera Preview
		modalPopupCanvas.blocksRaycasts = isModalPopupActive;
	}

	private void ClearModalPopup() {
		// Set alpha to zero (invisible)
		modalPopupCanvas.alpha = 0;
		// Clear question text
		question.text = "";
		// Clear commandType variable
		commandType = "";
		// Set isModalPopupActive to false
		isModalPopupActive = false;
		// Toggle "interactable" in CanvasGroup (for fixing stealth interaction when popup is not visible)
		modalPopupCanvas.interactable = isModalPopupActive;
		// Precaution for GUI being on top of the Camera Preview
		modalPopupCanvas.blocksRaycasts = isModalPopupActive;
	}

	public bool isModalPopupActive { get; private set; }
	public bool isErrorFadeOutActive { get; private set; }

	public void ShowErrorPopup(string e) {
		error.text = e;
		errorPopupCanvas.alpha = 1;
		errorDuration = 1f;
		isErrorFadeOutActive = true;
	}
}
