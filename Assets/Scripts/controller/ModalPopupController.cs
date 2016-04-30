using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ModalPopupController : MonoBehaviour {

	private BuildModeController bmc;
	private SceneController sc;
	private SpaceshipStats ss;
	private GameObject spaceshipGrid;

	private CanvasGroup modalPopupCanvas, notificationPopupCanvas;
	private Text question, notification;

	private string commandType;
	private float notificationDuration;

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
		notificationPopupCanvas = GameObject.FindGameObjectWithTag("GUI/ErrorPanel/Main").GetComponent<CanvasGroup>();
		notificationPopupCanvas.alpha = 0;
		notification = GameObject.FindGameObjectWithTag("GUI/ErrorPanel/Text").GetComponent<Text>();
		notification.text = "";

		// Other initializations
		commandType = "";
		notificationDuration = 1f;
		isErrorFadeOutActive = false;
	}

	void Update() {
		// Error popup fade-out effect
		if (isErrorFadeOutActive) {
			if (notificationDuration > 0) {
				notificationDuration -= Time.deltaTime;
			}
			else {
				if (notificationPopupCanvas.alpha > 0.01f)
					notificationPopupCanvas.alpha -= Time.deltaTime;
				else {
					notificationPopupCanvas.alpha = 0f;
					isErrorFadeOutActive = false;
					notification.text = "";
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
			ShowNotificationPopup("Error - no save file found.");
		}
	}

	public void ResponseYes() {
		if (commandType.Equals("overwrite") || commandType.Equals("save")) {
			bmc.SaveShip();
			ShowNotificationPopup("Spaceship saved.");
		}
		else if (commandType.Equals("load")) {
			bmc.LoadShip();
			ShowNotificationPopup("Spaceship loaded.");
		}
		else if (commandType.Equals("gameplay")) {
			sc.ChangeScene("GameplayMode");
		}
		else if (commandType.Equals("resetship")) {
			bmc.ResetShip();
			ShowNotificationPopup("Spaceship reset.");
		}
		else {
			ShowNotificationPopup("An unknown error occured.\nNo known commandType assigned in ModalPopupController.cs");
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

	public void ShowNotificationPopup(string nText, float nDuration = 1f) {
		notification.text = nText;
		notificationPopupCanvas.alpha = 1;
		notificationDuration= nDuration;
		isErrorFadeOutActive = true;
	}
}
