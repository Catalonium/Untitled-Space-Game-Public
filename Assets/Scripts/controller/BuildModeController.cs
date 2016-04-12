using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

//	TODO: Clean-up the code
//	Author: Adnan Bulut Catikoglu - 2016

public class BuildModeController : MonoBehaviour {

	private GameObject playerSpaceship, playerSpaceship_Grid; // spaceship
	private GameObject _cursorBlock, _selectedBlock;
	private Ray ray;
	private RaycastHit rayHit;
	private Vector3 placementPos, oldPos;
	private int gridSize = 1;
	private bool moveBlock;

	void Start() {
		// Get Spaceship/Player to cache if it exists
		if (GameObject.FindWithTag("Spaceship/Player"))
			playerSpaceship = GameObject.FindWithTag("Spaceship/Player");

		// Check for existing Spaceship/Player
		if (playerSpaceship != null) {
			playerSpaceship.name = "Construction";
			playerSpaceship.tag = "Spaceship/Construction";

			// disables physics script
			playerSpaceship.GetComponent<SpaceshipPhysics>().enabled = false;
			// zeroes movement velocities
			playerSpaceship.GetComponent<Rigidbody>().velocity = Vector3.zero;
			playerSpaceship.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
			// spaceship position reset
			playerSpaceship.transform.position = Vector3.zero;
			playerSpaceship.transform.rotation = Quaternion.Euler(Vector3.zero);
			// grids position reset
			playerSpaceship.transform.Find("Grids").transform.position = Vector3.zero;
			playerSpaceship.transform.Find("Grids").transform.rotation = Quaternion.Euler(Vector3.zero);
		}

//		// Check for existing Spaceship/Construction
//		else if (GameObject.FindWithTag("Spaceship/Construction")) {
//			// gets the ship in construction if it exists
//			playerSpaceship = GameObject.FindWithTag("Spaceship/Construction");
//		}

		// If Spaceship/Player or Spaceship/Construction is not found in the scene, create Spaceship/Construction
		else {
			// creates a new construction
			var constructionPrefab = (GameObject)Resources.Load("Prefabs/Player/Construction", typeof(GameObject));
			playerSpaceship = (GameObject)Instantiate(constructionPrefab, transform.position, transform.rotation);
			// required because name is set to "... (Clone)" due to instantiation
			playerSpaceship.name = "Construction";
		}

		// Get Spaceship/Grid to cache
		playerSpaceship_Grid = playerSpaceship.transform.Find("Grids").gameObject;

		// COLLIDER TRIGGER SWITCH
		BoxCollider[] blockColliders = playerSpaceship_Grid.GetComponentsInChildren<BoxCollider>();
		foreach (BoxCollider bc in blockColliders) {
			bc.isTrigger = true;
		}

		// Selected block initialization
		_selectedBlock = (GameObject)Resources.Load("Prefabs/Building Blocks/Block-Hull", typeof(GameObject));

		// Holocursor initialization
		var holoBlock = (GameObject)Resources.Load("Prefabs/Holo/Placeholder-Holo", typeof(GameObject));
		_cursorBlock = (GameObject)Instantiate(holoBlock, transform.position, transform.rotation);
		_cursorBlock.SetActive(false);
	}

	void Update() {
		// Ray position
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		// Mouse pointer position (block placement)
		placementPos = new Vector3(Mathf.Round(ray.origin.x) * gridSize, _selectedBlock.transform.position.y, Mathf.Round(ray.origin.z) * gridSize);
		// Mouse cursor position offset
		_cursorBlock.transform.position = placementPos + new Vector3(0, 5, 0);

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
		//			if (_cursorBlock.activeSelf)
		//				_cursorBlock.SetActive(false);
		// Move block toggle
		//			if (Input.GetMouseButtonDown(0)) {
		//				_movedBlockRef = rayHit.collider.gameObject;
		//				oldPos = _movedBlockRef.transform.position;
		//				moveBlock = true;
		//			}



		// Holographic cursor block logic
		if (Physics.Raycast(ray.origin + new Vector3(0, 0, -1), ray.direction, out rayHit, 15) ||
			Physics.Raycast(ray.origin + new Vector3(0, 0, 1), ray.direction, out rayHit, 15) ||
			Physics.Raycast(ray.origin + new Vector3(-1, 0, 0), ray.direction, out rayHit, 15) ||
			Physics.Raycast(ray.origin + new Vector3(1, 0, 0), ray.direction, out rayHit, 15) ||
			Physics.Raycast(ray.origin + new Vector3(0, 0, 0), ray.direction, out rayHit, 15) ||
			playerSpaceship_Grid.GetComponentsInChildren<Block>().Length == 0) {

			// Activate cursor if it's disabled
			if (!_cursorBlock.activeSelf) {
				_cursorBlock.SetActive(true);
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
		else _cursorBlock.SetActive(false);



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
		if (Physics.Raycast(ray.origin, ray.direction, out rayHit, 15)) {
			if (rayHit.collider.gameObject.GetComponent<Block>().structureType == StructureType.Interior) {
				if (_selectedBlock.GetComponent<Block>().blockType == BlockType.Component) {
					var newBlock = (GameObject)Instantiate(selBlock, placePos, placeRot);
					newBlock.transform.parent = playerSpaceship_Grid.transform;
				}
			}
		}
		// If mouse ray doesn't collide with a block
		else {
			if (_selectedBlock.GetComponent<Block>().blockType != BlockType.Component) {
				var newBlock = (GameObject)Instantiate(selBlock, placePos, placeRot);
				newBlock.transform.parent = playerSpaceship_Grid.transform;
			}
		}
	}

	public void DeleteBlock(Ray ray) {
		// If mouse ray collides with a block
		if (Physics.Raycast(ray.origin, ray.direction, out rayHit, 15)) {

			var blockToDel = rayHit.collider.gameObject;

			if (blockToDel.GetComponent<Block>().blockType == BlockType.Structure) {
				// Grid seperation check
				if (GridIntegrityCheck(blockToDel)) {
					// Delete block
					Destroy(blockToDel);
				}
				else {
					// Can't delete block
					Debug.LogError("Can't delete block - grid seperation detected.");
				}
			}
			else if (blockToDel.GetComponent<Block>().blockType == BlockType.Component) {
				Destroy(blockToDel);
			}
		}
	}
	
	public bool GridIntegrityCheck(GameObject block) {

		bool result = true;
		
		List<GameObject> adjacentGrids = new List<GameObject>();
		Collider[] mainArray = playerSpaceship_Grid.GetComponentsInChildren<Collider>();

//		tempArray.Add(block.transform.gameObject);

		// Starting block adjacent grids check
		if (Physics.Raycast(block.transform.position + new Vector3(0, 10, -1), Vector3.down, out rayHit, 15)) {
			adjacentGrids.Add(rayHit.collider.gameObject);
		}
		if (Physics.Raycast(block.transform.position + new Vector3(0, 10, 1), Vector3.down, out rayHit, 15)) {
			adjacentGrids.Add(rayHit.collider.gameObject);
		}
		if (Physics.Raycast(block.transform.position + new Vector3(-1, 10, 0), Vector3.down, out rayHit, 15)) {
			adjacentGrids.Add(rayHit.collider.gameObject);
		}
		if (Physics.Raycast(block.transform.position + new Vector3(1, 10, 0), Vector3.down, out rayHit, 15)) {
			adjacentGrids.Add(rayHit.collider.gameObject);
		}

		// If there is at least 1 adjacent grid
		if (adjacentGrids.Count > 0) {

			// Check adjacent grids one by one, to detect whether the action will seperate grids in any way or not
			for (int a = 0; a < adjacentGrids.Count; a++) {

				// create temp neighbour grid list which will reset at every adjacent block iteration
				List<GameObject> tempGrids = new List<GameObject>();
				// add first adjacent block as starting point for grid check
				tempGrids.Add(adjacentGrids[a]);
				// disable block-to-be-deleted's collider
				block.GetComponent<Collider>().enabled = false;

				// Check neighbour grids for this adjacent grid
				for (int i = 0; i < tempGrids.Count; i++) {

					// only check this block's neighbour grids if it hasn't been checked before (via Collider.enabled)
					if (tempGrids[i].GetComponent<Collider>().enabled) {

						if (Physics.Raycast(tempGrids[i].transform.position + new Vector3(0, 10, -1), Vector3.down, out rayHit, 15)) {
							tempGrids.Add(rayHit.collider.gameObject);
						}
						if (Physics.Raycast(tempGrids[i].transform.position + new Vector3(0, 10, 1), Vector3.down, out rayHit, 15)) {
							tempGrids.Add(rayHit.collider.gameObject);
						}
						if (Physics.Raycast(tempGrids[i].transform.position + new Vector3(-1, 10, 0), Vector3.down, out rayHit, 15)) {
							tempGrids.Add(rayHit.collider.gameObject);
						}
						if (Physics.Raycast(tempGrids[i].transform.position + new Vector3(1, 10, 0), Vector3.down, out rayHit, 15)) {
							tempGrids.Add(rayHit.collider.gameObject);
						}

						tempGrids[i].GetComponent<Collider>().enabled = false;

					}

				}

				foreach (Collider c in mainArray) {
					if (!c.enabled) {
						c.enabled = true;
					}
					else if (c.enabled) {
						result = false;
					}
				}

				if (!result) return false;

			}

		}

		return true;

	}

	public void SaveShip() {

		GameObject grids = GameObject.FindWithTag("Spaceship/Grids");
		Block[] blocks = grids.GetComponentsInChildren<Block>();

		List<string> lines = new List<string>();
		
		foreach (Block b in blocks) {
			// Example #1: Write an array of strings to a file.
			// Create a string array that consists of three lines.
			lines.Add("<" + b.blockType.ToString().ToLower() + ">");
			lines.Add(b.brandName);
			lines.Add(b.modelName);
			lines.Add(Mathf.Round(b.gameObject.transform.position.x).ToString());
			lines.Add(Mathf.Round(b.gameObject.transform.position.y).ToString());
			lines.Add(Mathf.Round(b.gameObject.transform.position.z).ToString());
			lines.Add("");	// for blocks seperation
		}
		// WriteAllLines creates a file, writes a collection of strings to the file,
		// and then closes the file.  You do NOT need to call Flush() or Close().
		File.WriteAllLines(Application.dataPath + "/spaceship.save", lines.ToArray());
//		File.WriteAllLines("C:/Users/Cloud/Desktop/ship1.save", lines.ToArray());
	}

	public void LoadShip() {

		if (File.Exists(Application.dataPath + "/spaceship.save")) {
//		if (File.Exists(@"C:\Users\Cloud\Desktop\ship1.save")) {

			var file = File.OpenRead(Application.dataPath + "/spaceship.save");
//			var file = File.OpenRead(@"C:\Users\Cloud\Desktop\ship1.save");
			StreamReader reader = new StreamReader(file);

			GameObject[] buildingBlocks = Resources.LoadAll<GameObject>("Prefabs/Building Blocks");

			// delete existing blocks-grids
			GameObject[] blocksToDelete = GameObject.FindGameObjectsWithTag("Spaceship/Block");
			foreach (GameObject target in blocksToDelete) {
				Destroy(target);
			}

			while (!reader.EndOfStream) {

				string line_Type = reader.ReadLine(); // type of block (tag in xml file)
				string line_Brand = reader.ReadLine(); // brand of block
				string line_Model = reader.ReadLine(); // model of block
				float line_xPos = float.Parse(reader.ReadLine()); // transform.x of block
				float line_yPos = float.Parse(reader.ReadLine()); // transform.y of block
				float line_zPos = float.Parse(reader.ReadLine()); // transform.z of block
				reader.ReadLine(); // empty line (for visual seperation in save file)

				line_Type = line_Type.Substring(1, 9); // tag chars removal

				foreach (GameObject go in buildingBlocks) {
					if (go.GetComponent<Block>().brandName == line_Brand && go.GetComponent<Block>().modelName == line_Model) {
						if (line_Type == go.GetComponent<Block>().blockType.ToString().ToLower()) {

							Vector3 line_Position = new Vector3(line_xPos, line_yPos, line_zPos);

							var newBlock = (GameObject)Instantiate(go, line_Position, transform.rotation);
							newBlock.transform.parent = GameObject.FindWithTag("Spaceship/Grids").transform;

							break;
						}
					}
				}

			}

			reader.Close();

		}
		else Debug.LogError("No save file exists.");

	}

	public void BlockSelection(int i) {
		switch(i) {
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
				_selectedBlock = (GameObject)Resources.Load("Prefabs/Building Blocks/Block-Thruster_01", typeof(GameObject));
				break;
			case 5:
				_selectedBlock = (GameObject)Resources.Load("Prefabs/Building Blocks/Block-Gyroscope", typeof(GameObject));
				break;
			case 6:
				_selectedBlock = (GameObject)Resources.Load("Prefabs/Building Blocks/Block-Reactor_01", typeof(GameObject));
				break;
		}
	}

	public void ChangeScene(string scene) {
		DontDestroyOnLoad(playerSpaceship);
		SceneManager.LoadScene(scene);
	}

}