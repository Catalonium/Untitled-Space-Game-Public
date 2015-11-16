﻿using System;
using UnityEngine;
using System.Collections;

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

		// Mouse pointer position
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		placementPos = new Vector3(Mathf.Round(ray.origin.x) * gridSize, 0, Mathf.Round(ray.origin.z) * gridSize);
		_cursorBlockRef.transform.position = new Vector3(Mathf.Round(ray.origin.x) * gridSize, -5, Mathf.Round(ray.origin.z) * gridSize);



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

				// Activate cursor if it's disabled
				if (!_cursorBlockRef.activeSelf) {
					_cursorBlockRef.SetActive(true);
				}

				// Place new block
				if (Input.GetMouseButtonDown(0)) {

						var newBlock = (GameObject)Instantiate(selectedBlock, placementPos, transform.rotation);
						newBlock.transform.parent = playerSpaceship.transform;
						_cursorBlockRef.SetActive(false);

				}

			}

		}

	}

}