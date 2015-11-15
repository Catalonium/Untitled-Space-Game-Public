using System;
using UnityEngine;
using System.Collections;

public class BuildModeController: MonoBehaviour {

	public GameObject blockPlaceholder;
	public Material holoYellow;
	private GameObject cursorBlock, movedBlock;
	private Ray ray;
	private RaycastHit rayHit;
	private Vector3 placementPos, oldPos;
	private float gridSize = 1f;
	private bool moveBlock = false;

	// Use this for initialization
	void Start() {
		cursorBlock = (GameObject)Instantiate(blockPlaceholder, transform.position, transform.rotation);
		cursorBlock.GetComponent<Renderer>().material = holoYellow;
		cursorBlock.SetActive(false);
	}

	// Update is called once per frame
	void Update() {

		// Mouse pointer position
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		placementPos = new Vector3(Mathf.Round(ray.origin.x) * gridSize, 0, Mathf.Round(ray.origin.z) * gridSize);
		cursorBlock.transform.position = new Vector3(Mathf.Round(ray.origin.x) * gridSize, -5, Mathf.Round(ray.origin.z) * gridSize);



		// MOVEMENT MODE
		if (moveBlock) {

			// Constant movement position update
			if (!Physics.Raycast(ray, out rayHit, 10f))
				movedBlock.transform.position = new Vector3(placementPos.x, -5, placementPos.z);
			// Confirm block movement
			if (Input.GetMouseButtonDown(0)) {
				movedBlock.transform.position = new Vector3(movedBlock.transform.position.x, 0, movedBlock.transform.position.z);
				movedBlock = null; // Clear memory
				moveBlock = false;
			}
			// Cancel block movement
			else if (Input.GetMouseButtonDown(1)) {
				movedBlock.transform.position = oldPos;
				movedBlock = null; // Clear memory
				moveBlock = false;
			}

		}
		// STANDARD MODE
		else {

			// If mouse ray collides with a block
			if (Physics.Raycast(ray, out rayHit, 10f)) {

				// Disable cursor if it's activated
				if (cursorBlock.activeSelf)
					cursorBlock.SetActive(false);
				// Move block toggle
				if (Input.GetMouseButtonDown(0)) {
					movedBlock = rayHit.collider.gameObject;
					oldPos = movedBlock.transform.position;
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
				if (!cursorBlock.activeSelf)
					cursorBlock.SetActive(true);
				// Place new block
				if (Input.GetMouseButtonDown(0)) {
					Instantiate(blockPlaceholder, placementPos, transform.rotation);
					cursorBlock.SetActive(false);
				}

			}

		}

	}

}