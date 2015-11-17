using System;
using UnityEngine;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine.UI;

public class BuildModeController: MonoBehaviour {

	public GameObject cursorBlock, playerSpaceship, selectedBlock; // Initialization prefabs
	private GameObject _cursorBlockRef, _movedBlockRef; // 
	private Ray ray;
	private RaycastHit rayHit;
	private Vector3 placementPos, oldPos;
	private int gridSize = 1;
	private bool moveBlock = false;

	// Use this for initialization
	void Start() {
		_cursorBlockRef = (GameObject)Instantiate(cursorBlock, transform.position, transform.rotation);
		_cursorBlockRef.SetActive(false);
	}

	// Update is called once per frame
	void Update() {

		// Ray position
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		//Debug.Log(ray);
		// Mouse pointer position (block placement)
		placementPos = new Vector3(Mathf.Round(ray.origin.x) * gridSize, 0, Mathf.Round(ray.origin.z) * gridSize);
		Debug.Log(placementPos);
		// Mouse cursor position offset
		_cursorBlockRef.transform.position = placementPos + new Vector3(0, -5, 0);



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
		// STANDARD MODE
		else {

			// If mouse ray collides with a block
			if (Physics.Raycast(ray, out rayHit, 10f)) {

				// Disable cursor if it's activated
				if (_cursorBlockRef.activeSelf)
					_cursorBlockRef.SetActive(false);
				// Move block toggle
				if (Input.GetMouseButtonDown(0)) {
					_movedBlockRef = rayHit.collider.gameObject;
					oldPos = _movedBlockRef.transform.position;
					moveBlock = true;
				}
				// Delete block
				else if (Input.GetMouseButtonDown(1)) {
					// Delete block
					Destroy(rayHit.collider.gameObject);
				}

			}
			// If mouse ray doesn't collides with a block
			else {

				if (Physics.Raycast(ray.origin + new Vector3(0, 0, -1), ray.direction, out rayHit, 10f) || Physics.Raycast(ray.origin + new Vector3(0, 0, 1), ray.direction, out rayHit, 10f) ||
					Physics.Raycast(ray.origin + new Vector3(-1, 0, 0), ray.direction, out rayHit, 10f) || Physics.Raycast(ray.origin + new Vector3(1, 0, 0), ray.direction, out rayHit, 10f)) {

					// Activate cursor if it's disabled
					if (!_cursorBlockRef.activeSelf) {
						_cursorBlockRef.SetActive(true);
					}

					// Place new block
					if (Input.GetMouseButtonDown(0)) {

						var newBlock = (GameObject) Instantiate(selectedBlock, placementPos, transform.rotation);
						newBlock.transform.parent = playerSpaceship.transform;
						_cursorBlockRef.SetActive(false);

					}

				}
				else _cursorBlockRef.SetActive(false);
				
			}

		}

	}

	public void BlockSelection(int a) {
		switch (a) {
			case 1:
				selectedBlock = (GameObject)Resources.Load("Prefabs/Building Blocks/Block-Hull", typeof(GameObject));
				break;
			case 2:
				selectedBlock = (GameObject)Resources.Load("Prefabs/Building Blocks/Block-Interior", typeof(GameObject));
				break;
			case 3:
				selectedBlock = (GameObject)Resources.Load("Prefabs/Building Blocks/Block-Bridge", typeof(GameObject));
				break;
			case 4:
				selectedBlock = (GameObject)Resources.Load("Prefabs/Building Blocks/Block-Thruster", typeof(GameObject));
				break;
			case 5:
				selectedBlock = (GameObject)Resources.Load("Prefabs/Building Blocks/Block-Gyroscope", typeof(GameObject));
				break;
			case 6:
				selectedBlock = (GameObject)Resources.Load("Prefabs/Building Blocks/Block-Reactor", typeof(GameObject));
				break;
		}
	}

}