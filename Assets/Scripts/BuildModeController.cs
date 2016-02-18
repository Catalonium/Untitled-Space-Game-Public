using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

//using System.Collections;
//using JetBrains.Annotations;
//using UnityEngine.UI;


//	TODO: Clean-up the code
//	Author: Adnan Bulut Catikoglu - 2016

public class BuildModeController: MonoBehaviour {

	public GameObject cursorBlock, playerSpaceship; // Initialization prefabs
	private GameObject _cursorBlockRef, _movedBlockRef, _selectedBlock;
	private Ray ray;
	private RaycastHit rayHit;
	private Vector3 placementPos, oldPos;
	private int gridSize = 1;
	private bool moveBlock;

	void Start() {
		// Selected block initialization
		_selectedBlock = (GameObject)Resources.Load("Prefabs/Building Blocks/Block-Hull", typeof(GameObject));

		// Holocursor initialization
		_cursorBlockRef = (GameObject)Instantiate(cursorBlock, transform.position, transform.rotation);
		_cursorBlockRef.SetActive(false);
	}

	void Update() {
		// Ray position
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		// Mouse pointer position (block placement)
		placementPos = new Vector3(Mathf.Round(ray.origin.x) * gridSize, _selectedBlock.transform.position.y, Mathf.Round(ray.origin.z) * gridSize);
		// Mouse cursor position offset
		_cursorBlockRef.transform.position = placementPos + new Vector3(0, 5, 0);

		/* GITHUB ISSUE SOLUTION #10: Movement Disabled

		// MOVEMENT MODE
		if (moveBlock) {

			// Constant movement position update
			if (!Physics.Raycast(ray, out rayHit, 10f))
				_movedBlockRef.transform.position = new Vector3(placementPos.x, -5, placementPos.z);
			// Confirm block movement
			if (Input.GetMouseButtonDown(0)) {
				_movedBlockRef.transform.position = new Vector3(_movedBlockRef.transform.position.x, 0, _movedBlockRef.transform.position.z);
				_movedBlockRef = null; // Clear memory
				moveBlock = false;
			}
			// Cancel block movement
			else if (Input.GetMouseButtonDown(1)) {
				_movedBlockRef.transform.position = oldPos;
				_movedBlockRef = null; // Clear memory
				moveBlock = false;
			}
		}
		GITHUB ISSUE SOLUTION #10: Movement Disabled */

		// BUILD MODE


		// Disable cursor if it's activated
		//			if (_cursorBlockRef.activeSelf)
		//				_cursorBlockRef.SetActive(false);
		// Move block toggle
		//			if (Input.GetMouseButtonDown(0)) {
		//				_movedBlockRef = rayHit.collider.gameObject;
		//				oldPos = _movedBlockRef.transform.position;
		//				moveBlock = true;
		//			}



		// Holographic cursor logic
		if (Physics.Raycast(ray.origin + new Vector3(0, 0, -1), ray.direction, out rayHit, 10f) ||
			Physics.Raycast(ray.origin + new Vector3(0, 0, 1), ray.direction, out rayHit, 10f) ||
			Physics.Raycast(ray.origin + new Vector3(-1, 0, 0), ray.direction, out rayHit, 10f) ||
			Physics.Raycast(ray.origin + new Vector3(1, 0, 0), ray.direction, out rayHit, 10f) ||
			Physics.Raycast(ray.origin + new Vector3(0, 0, 0), ray.direction, out rayHit, 10f) ||
			playerSpaceship.GetComponentsInChildren<Block>().Length == 0) {

			// Activate cursor if it's disabled
			if (!_cursorBlockRef.activeSelf) {
				_cursorBlockRef.SetActive(true);
			}

			// Place block
			if (Input.GetMouseButtonDown(0)) {
				// Block placement function call
				PlaceBlock(_selectedBlock, placementPos, transform.rotation, ray);
			}
			// Delete block
			if (Input.GetMouseButtonDown(1)) {
				// Block deletion function call
				DeleteBlock(ray);
			}

		}
		else
			_cursorBlockRef.SetActive(false);



		// Hotkeys for block selection
		if (Input.GetKeyDown("1"))
			BlockSelection(1);
		if (Input.GetKeyDown("2"))
			BlockSelection(2);
		if (Input.GetKeyDown("3"))
			BlockSelection(3);
		if (Input.GetKeyDown("4"))
			BlockSelection(4);
		if (Input.GetKeyDown("5"))
			BlockSelection(5);
		if (Input.GetKeyDown("6"))
			BlockSelection(6);
	}

	void LateUpdate() {
		// Camera movement
		if (Input.GetMouseButton(2)) {
			float mX = Input.GetAxis("Mouse X"); // Smoothed X axis values
			float mY = Input.GetAxis("Mouse Y"); // Smoothed Y axis values
			Vector3 cam = Camera.main.transform.position;

			if (mX != 0) {
				cam += new Vector3(mX * -0.5f, 0, 0);
			}
			if (mY != 0) {
				cam += new Vector3(0, 0, mY * -0.5f);
			}
			cam = new Vector3(Mathf.Clamp(cam.x, -10, 10), cam.y, (Mathf.Clamp(cam.z, -10, 10)));
			Camera.main.transform.position = cam;
		}
		// Camera reset
		else if (Input.GetKeyDown(KeyCode.Space)) {
			Camera.main.transform.position = new Vector3(0, 10, 0);
		}

	}

	public void PlaceBlock(GameObject selBlock, Vector3 placePos, Quaternion placeRot, Ray ray) {
		// If mouse ray collides with a block
		if (Physics.Raycast(ray.origin + new Vector3(0, -2, 0), ray.direction, out rayHit, 10f)) {
			if (rayHit.collider.gameObject.GetComponent<Block>().structureType == StructureType.Interior) {
				if (_selectedBlock.GetComponent<Block>().blockType == BlockType.Component) {
					var newBlock = (GameObject)Instantiate(selBlock, placePos, placeRot);
					newBlock.transform.parent = playerSpaceship.transform;
				}
			}
		}
		// If mouse ray doesn't collide with a block
		else {
			if (_selectedBlock.GetComponent<Block>().blockType != BlockType.Component) {
				var newBlock = (GameObject)Instantiate(selBlock, placePos, placeRot);
				newBlock.transform.parent = playerSpaceship.transform;
			}
		}
	}

	public void DeleteBlock(Ray ray) {
		// If mouse ray collides with a block
		if (Physics.Raycast(ray.origin + new Vector3(0, -2, 0), ray.direction, out rayHit, 10f)) {
			// Delete block
			Destroy(rayHit.collider.gameObject);
		}
	}

	public void BlockSelection(int i) {
		switch (i) {
			case 1:
				_selectedBlock = (GameObject)Resources.Load("Prefabs/Building Blocks/Block-Hull", typeof(GameObject));
				break;
			case 2:
				_selectedBlock = (GameObject)Resources.Load("Prefabs/Building Blocks/Block-Interior", typeof(GameObject));
				break;
			case 3:
				_selectedBlock = (GameObject)Resources.Load("Prefabs/Building Blocks/Block-Bridge", typeof(GameObject));
				break;
			case 4:
				_selectedBlock = (GameObject)Resources.Load("Prefabs/Building Blocks/Block-Thruster", typeof(GameObject));
				break;
			case 5:
				_selectedBlock = (GameObject)Resources.Load("Prefabs/Building Blocks/Block-Gyroscope", typeof(GameObject));
				break;
			case 6:
				_selectedBlock = (GameObject)Resources.Load("Prefabs/Building Blocks/Block-Reactor_mk1", typeof(GameObject));
				break;
		}
	}

	public void ChangeScene(string level) {
		SceneManager.LoadScene(level);
	}

}