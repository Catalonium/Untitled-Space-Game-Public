using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//	TODO: Clean-up the code
//	Author: Adnan Bulut Catikoglu - 2016

public class BuildModeController : MonoBehaviour {

	private GameObject playerSpaceship, playerSpaceship_Grids;
	private GameObject _cursorBlock, _cursorRefBlock, _cursorRefBlockMesh, _selectedBlock;
	private GameObject _infoPanel;

	private Material _cursorMatBlue, _cursorMatRed;

	private GridCollisionDetector[] blockOrientationGrids;

	private Ray ray;
	private RaycastHit rayHit;
	private Vector3 placementPos, rayVectorRnd;
	private Quaternion placementRot;
	private float degreesOfRot;

	private int gridSize = 1;

	//	private bool moveBlock;

	private ModalPopupController mpc;

	void Start() {

		// SPACESHIP PROCESS
		//
		// Get Spaceship to cache if it exists
		if (GameObject.FindWithTag("Spaceship/Main")) {
			playerSpaceship = GameObject.FindWithTag("Spaceship/Main");
			playerSpaceship_Grids = playerSpaceship.transform.Find("Grids").gameObject;
//			playerSpaceship_BuildableGrids = playerSpaceship.transform.Find("BuildableGrids").gameObject;
		}

		// Check for existing Spaceship
		if (playerSpaceship != null) {
			// disables physics script
			playerSpaceship.GetComponent<SpaceshipPhysics>().PhysicsToggle(false);
			// reset movement velocities
			playerSpaceship.GetComponent<Rigidbody>().velocity = Vector3.zero;
			playerSpaceship.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
			// spaceship position reset
			playerSpaceship.transform.position = Vector3.zero;
			playerSpaceship.transform.rotation = Quaternion.Euler(Vector3.zero);
			// grids position reset
			playerSpaceship_Grids.transform.position = Vector3.zero;
			playerSpaceship_Grids.transform.rotation = Quaternion.Euler(Vector3.zero);
			// rename the ship again (required for renaming field, otherwise field left empty)
			RenameShip(playerSpaceship.name);
		}
		// If playerSpaceship is null (meaning there isn't any existing ships in the scene), create a new Spaceship
		else {
			NewShip();
		}
		
		// INITIALIZATION PROCESS
		//
		// Collider trigger switch
		BoxCollider[] blockColliders = playerSpaceship_Grids.GetComponentsInChildren<BoxCollider>();
		foreach (BoxCollider bc in blockColliders) {
			bc.isTrigger = true;
		}
		
		// Holocursor initialization
		var holoBlock = (GameObject)Resources.Load("Prefabs/Holo/HolocursorBlock", typeof(GameObject));
		_cursorBlock = (GameObject)Instantiate(holoBlock, transform.position, transform.rotation);
		_cursorBlock.GetComponent<Renderer>().enabled = false;
//		// Holocursor reference block procedure
//		RefreshHolocursorRef();

		// Holocursor Materials initialization
		_cursorMatBlue = (Material)Resources.Load("Materials/Blocks/Holo/Holo-Blue", typeof(Material));
		_cursorMatRed = (Material)Resources.Load("Materials/Blocks/Holo/Holo-Red", typeof(Material));

		// Selected block initialization
//		_selectedBlock = (GameObject)Resources.Load("Prefabs/Building Blocks/Block-Hull_Bulk", typeof(GameObject));
		BlockSelection(1);

//		// Get BuildableGridBlock prefab
//		_buildableGridBlock = (GameObject)Resources.Load("Prefabs/Holo/BuildableGridBlock", typeof(GameObject));

		// Get ModalPanel to cache
		mpc = GetComponent<ModalPopupController>();
		// Get InfoPanel to cache
		_infoPanel = GameObject.FindGameObjectWithTag("GUI/InfoPanel/Main");
		// Clear InfoPanel
		ClearInfoPanel();

	}

	void Update() {

		// Only run building logic if a ModalPanel is NOT opened
		if (!mpc.isModalPopupActive && !EventSystem.current.IsPointerOverGameObject()) {

			// Ray position
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			// Mouse pointer position (block placement)
			placementPos = new Vector3(Mathf.Round(ray.origin.x) * gridSize, _selectedBlock.transform.position.y,
				Mathf.Round(ray.origin.z) * gridSize);

			// Mouse cursor position offset
			_cursorBlock.transform.position = placementPos + new Vector3(0, 10, 0);

			// Rounding x-z axis' of ray vector for catching non-placeable positions issue
			rayVectorRnd = new Vector3(Mathf.Round(ray.origin.x), ray.origin.y, Mathf.Round(ray.origin.z));

			// Holographic cursor block logic
			if (Physics.Raycast(rayVectorRnd, ray.direction, out rayHit, 30)) {

				// ---
				if (rayHit.collider.gameObject.tag.Equals("Spaceship/Block")) {

					// Call Info panel function if mouse ray collides with an actual block
					RefreshInfoPanel();

					// If interior block is hovered 
					if (rayHit.collider.gameObject.GetComponent<Block>().structureType == StructureType.Interior) {

						// If selected block is component
						if (_selectedBlock.GetComponent<Block>().blockType == BlockType.Component) {

							_cursorBlock.GetComponent<Renderer>().material = _cursorMatBlue;
							// Activate cursor if it's disabled
							if (!_cursorBlock.GetComponent<Renderer>().enabled)
								_cursorBlock.GetComponent<Renderer>().enabled = true;
							// Activate cursor reference block
							_cursorRefBlockMesh.SetActive(true);

							// PLACE block
							if (Input.GetMouseButtonDown(0)) {
								// Block placement function call
								PlaceBlock(_selectedBlock, placementPos, placementRot, rayHit);
							}

						}
						// In case of other than component blocks selected
						else {

							_cursorBlock.GetComponent<Renderer>().material = _cursorMatRed;
							// Activate cursor if it's disabled
							if (!_cursorBlock.GetComponent<Renderer>().enabled)
								_cursorBlock.GetComponent<Renderer>().enabled = true;
							// Deactivate cursor reference block
							_cursorRefBlockMesh.SetActive(false);

							// PLACE block
							if (Input.GetMouseButtonDown(0)) {
								// Play error sound & show error popup
								mpc.ShowNotificationPopup("Only Component blocks can be placed onto Interior blocks.");
								GetComponent<SFX_BuildMode>().errorBlockSFX();
							}

						}

					}
					// In case of other than interior blocks hovered
					else {

						_cursorBlock.GetComponent<Renderer>().material = _cursorMatRed;
						// Activate cursor if it's disabled
						if (!_cursorBlock.GetComponent<Renderer>().enabled)
							_cursorBlock.GetComponent<Renderer>().enabled = true;
						// Deactivate cursor reference block
						_cursorRefBlockMesh.SetActive(false);

						// PLACE block
						if (Input.GetMouseButtonDown(0)) {
							// Play error sound & show error popup
							mpc.ShowNotificationPopup("Cannot place - there's another block in the way.");
							GetComponent<SFX_BuildMode>().errorBlockSFX();
						}

					}

					// DELETE block
					if (Input.GetMouseButtonDown(1)) {
						// Block deletion function call
						DeleteBlock(rayHit);
					}

				}
				else if (rayHit.collider.gameObject.tag.Equals("Spaceship/OrientationGrid")) {

					// If selected block is a structure block, blue cursor 
					if (_selectedBlock.GetComponent<Block>().blockType == BlockType.Structure) {

						// Orientation grid check
						if (_selectedBlock.GetComponent<Block>().isRotateable) {

							if (BlockOrientationChecker()) {

								_cursorBlock.GetComponent<Renderer>().material = _cursorMatBlue;
								// Activate cursor if it's disabled
								if (!_cursorBlock.GetComponent<Renderer>().enabled)
									_cursorBlock.GetComponent<Renderer>().enabled = true;
								// Activate cursor reference block
								_cursorRefBlockMesh.SetActive(true);

								// PLACE block
								if (Input.GetMouseButtonDown(0)) {
									// Block placement function call
									PlaceBlock(_selectedBlock, placementPos, placementRot, rayHit);
								}

							}
							else {

								_cursorBlock.GetComponent<Renderer>().material = _cursorMatRed;
								// Activate cursor if it's disabled
								if (!_cursorBlock.GetComponent<Renderer>().enabled)
									_cursorBlock.GetComponent<Renderer>().enabled = true;
								// Activate cursor reference block
								_cursorRefBlockMesh.SetActive(true);

								// PLACE block
								if (Input.GetMouseButtonDown(0)) {
									// Play error sound & show error popup
									mpc.ShowNotificationPopup("Cannot place this block with current orientation.");
									GetComponent<SFX_BuildMode>().errorBlockSFX();
								}

							}

						}
						else {

							_cursorBlock.GetComponent<Renderer>().material = _cursorMatBlue;
							// Activate cursor if it's disabled
							if (!_cursorBlock.GetComponent<Renderer>().enabled)
								_cursorBlock.GetComponent<Renderer>().enabled = true;
							// Activate cursor reference block
							_cursorRefBlockMesh.SetActive(true);

							// PLACE block
							if (Input.GetMouseButtonDown(0)) {
								// Block placement function call
								PlaceBlock(_selectedBlock, placementPos, placementRot, rayHit);
							}

						}

					}
					// Else (other than structure block selected), red cursor
					else {

						_cursorBlock.GetComponent<Renderer>().material = _cursorMatRed;
						// Activate cursor if it's disabled
						if (!_cursorBlock.GetComponent<Renderer>().enabled)
							_cursorBlock.GetComponent<Renderer>().enabled = true;
						// Activate cursor reference block
						_cursorRefBlockMesh.SetActive(true);

						// PLACE block
						if (Input.GetMouseButtonDown(0)) {
							// Play error sound & show error popup
							mpc.ShowNotificationPopup("Component blocks can only be placed onto Interior blocks.");
							GetComponent<SFX_BuildMode>().errorBlockSFX();
						}

					}

				}
				// Red cursor w/ref block
				else {

					// Clear InfoPanel
					ClearInfoPanel();

					_cursorBlock.GetComponent<Renderer>().material = _cursorMatRed;
					// Activate cursor if it's disabled
					if (!_cursorBlock.GetComponent<Renderer>().enabled)
						_cursorBlock.GetComponent<Renderer>().enabled = true;
					// Activate cursor reference block
					_cursorRefBlockMesh.SetActive(true);

					// PLACE block
					if (Input.GetMouseButtonDown(0)) {
						// Play error sound & show error popup
						mpc.ShowNotificationPopup("Cannot place - invalid position.");
						GetComponent<SFX_BuildMode>().errorBlockSFX();
					}

				}

			}
			else if (playerSpaceship_Grids.GetComponentsInChildren<Block>().Length == 0) {

				if (_selectedBlock.GetComponent<Block>().blockType == BlockType.Structure) {

					_cursorBlock.GetComponent<Renderer>().material = _cursorMatBlue;
					// Activate cursor if it's disabled
					if (!_cursorBlock.GetComponent<Renderer>().enabled)
						_cursorBlock.GetComponent<Renderer>().enabled = true;
					// Activate cursor reference block
					_cursorRefBlockMesh.SetActive(true);

					// PLACE block
					if (Input.GetMouseButtonDown(0)) {
						// Block placement function call
						PlaceBlock(_selectedBlock, placementPos, placementRot, rayHit);
					}

				}
				else {

					// Clear InfoPanel
					ClearInfoPanel();

					_cursorBlock.GetComponent<Renderer>().material = _cursorMatRed;
					// Activate cursor if it's disabled
					if (!_cursorBlock.GetComponent<Renderer>().enabled)
						_cursorBlock.GetComponent<Renderer>().enabled = true;
					// Activate cursor reference block
					_cursorRefBlockMesh.SetActive(true);

					// PLACE block
					if (Input.GetMouseButtonDown(0)) {
						// Play error sound & show error popup
						mpc.ShowNotificationPopup("A component block should be placed onto existing Interior blocks.");
						GetComponent<SFX_BuildMode>().errorBlockSFX();
					}

				}

			}
			else {

				// Clear InfoPanel
				ClearInfoPanel();

				// Deactivate cursor if it's enabled
				if (_cursorBlock.GetComponent<Renderer>().enabled)
					_cursorBlock.GetComponent<Renderer>().enabled = false;
				// Deactivate cursor reference block
				_cursorRefBlockMesh.SetActive(false);

			}



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
			if (Input.GetKeyDown("7"))
				BlockSelection(7);
			if (Input.GetKeyDown("8"))
				BlockSelection(8);

			// Hotkeys for block rotation and logic
			if (_selectedBlock.GetComponent<Block>().isRotateable) {

				placementRot = Quaternion.Euler(transform.rotation.x, Mathf.Round(degreesOfRot), transform.rotation.z);
				_cursorBlock.transform.rotation = placementRot;

				if (Input.GetKeyDown(KeyCode.Q)) {

					if (degreesOfRot.Equals(0))
						degreesOfRot = 270;
					else
						degreesOfRot -= 90;

					placementRot = Quaternion.Euler(transform.rotation.x, Mathf.Round(degreesOfRot), transform.rotation.z);
					_cursorBlock.transform.rotation = placementRot;
					mpc.ShowNotificationPopup("Rotation: " + degreesOfRot);

				}
				if (Input.GetKeyDown(KeyCode.E)) {

					if (degreesOfRot.Equals(270))
						degreesOfRot = 0;
					else
						degreesOfRot += 90;

					placementRot = Quaternion.Euler(transform.rotation.x, Mathf.Round(degreesOfRot), transform.rotation.z);
					_cursorBlock.transform.rotation = placementRot;
					mpc.ShowNotificationPopup("Rotation: " + degreesOfRot);

				}

			}
			else {

				placementRot = _selectedBlock.transform.rotation;
				_cursorBlock.transform.rotation = placementRot;

			}

		}
		else {

			// Clear InfoPanel
			ClearInfoPanel();

			// Deactivate cursor if it's enabled
			if (_cursorBlock.GetComponent<Renderer>().enabled)
				_cursorBlock.GetComponent<Renderer>().enabled = false;
			// Deactivate cursor reference block
			_cursorRefBlockMesh.SetActive(false);

		}

	}

	void LateUpdate() {

		// Camera movement
		if (Input.GetMouseButton(2)) {
			float mX = Input.GetAxis("Mouse X"); // Smoothed X axis values
			float mY = Input.GetAxis("Mouse Y"); // Smoothed Y axis values
			Vector3 cam = Camera.main.transform.position;

			if (mX != 0) {
				cam += new Vector3(mX * -Camera.main.orthographicSize / 30, 0, 0);
			}
			if (mY != 0) {
				cam += new Vector3(0, 0, mY * -Camera.main.orthographicSize / 30);
			}
			cam = new Vector3(Mathf.Clamp(cam.x, -10, 10), cam.y, (Mathf.Clamp(cam.z, -10, 10)));
			Camera.main.transform.position = cam;
		}
		// Camera zoom
		else if (Input.GetAxis("Mouse ScrollWheel") > 0 && Camera.main.orthographicSize > 5) {
			Camera.main.orthographicSize--;
		}
		else if (Input.GetAxis("Mouse ScrollWheel") < 0 && Camera.main.orthographicSize < 25) {
			Camera.main.orthographicSize++;
		}
		// Camera position and zoom reset
		else if (Input.GetKeyDown(KeyCode.Space)) {
			Camera.main.orthographicSize = 15;
			Camera.main.transform.position = new Vector3(0, 20, 0);
		}

	}
	
	public void PlaceBlock(GameObject selBlock, Vector3 placePos, Quaternion placeRot, RaycastHit rayHit, bool hideGrid = false) {

//		if (hideGrid) {
//			rayHit.collider.GetComponent<Collider>().enabled = false
//		}

		// Play placement sound
		GetComponent<SFX_BuildMode>().placeBlockSFX();
		// Place block (with x-z axis position and rotation rounding)
		placePos = new Vector3(Mathf.Round(placePos.x), placePos.y, Mathf.Round(placePos.z));
		var newBlock = (GameObject) Instantiate(selBlock, placePos, placeRot);
		newBlock.transform.parent = playerSpaceship_Grids.transform;

	}

	public void DeleteBlock(RaycastHit rayHit) {
		
		var blockToDel = rayHit.collider.gameObject;

		// If it's a structure block
		if (blockToDel.GetComponent<Block>().blockType == BlockType.Structure) {

			// Grid seperation check
			if (GridIntegrityCheck(blockToDel)) {
				// Play remove sound
				GetComponent<SFX_BuildMode>().removeBlockSFX();
				// Delete block
				Destroy(blockToDel);
			}
			else {
				// Play error sound & show error popup
				mpc.ShowNotificationPopup("Cannot delete block - grid seperation detected.");
				GetComponent<SFX_BuildMode>().errorBlockSFX();
			}

		}
		// If it's a component block
		else if (blockToDel.GetComponent<Block>().blockType == BlockType.Component) {

			// Play remove sound
			GetComponent<SFX_BuildMode>().removeBlockSFX();
			// Delete block
			Destroy(blockToDel);

		}
		else {

			// Play error sound & show error popup
			mpc.ShowNotificationPopup("There's no block to delete.");
			GetComponent<SFX_BuildMode>().errorBlockSFX();

		}

	}
	
	public bool GridIntegrityCheck(GameObject block) {

		bool result = true;

		// This layermask allows raycasts to only detect blocks on Block/Structure layer
		int layerMask = 1 << 10;

		List<GameObject> adjacentGrids = new List<GameObject>();
		List<Collider> mainArray = new List<Collider>();

		// Get all grids
		var allGrids = playerSpaceship_Grids.GetComponentsInChildren<Collider>();
		// Only select structure blocks by their layer from allGrids
		foreach (Collider go in allGrids) {
			if (go.gameObject.layer.Equals(10))
				mainArray.Add(go);
		}

		//		tempArray.Add(block.transform.gameObject);

		// Starting block adjacent grids check
		if (Physics.Raycast(block.transform.position + new Vector3(0, 10, -1), Vector3.down, out rayHit, 30, layerMask)) {
			adjacentGrids.Add(rayHit.collider.gameObject);
		}
		if (Physics.Raycast(block.transform.position + new Vector3(0, 10, 1), Vector3.down, out rayHit, 30, layerMask)) {
			adjacentGrids.Add(rayHit.collider.gameObject);
		}
		if (Physics.Raycast(block.transform.position + new Vector3(-1, 10, 0), Vector3.down, out rayHit, 30, layerMask)) {
			adjacentGrids.Add(rayHit.collider.gameObject);
		}
		if (Physics.Raycast(block.transform.position + new Vector3(1, 10, 0), Vector3.down, out rayHit, 30, layerMask)) {
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

						if (Physics.Raycast(tempGrids[i].transform.position + new Vector3(0, 10, -1), Vector3.down, out rayHit, 30, layerMask)) {
							tempGrids.Add(rayHit.collider.gameObject);
						}
						if (Physics.Raycast(tempGrids[i].transform.position + new Vector3(0, 10, 1), Vector3.down, out rayHit, 30, layerMask)) {
							tempGrids.Add(rayHit.collider.gameObject);
						}
						if (Physics.Raycast(tempGrids[i].transform.position + new Vector3(-1, 10, 0), Vector3.down, out rayHit, 30, layerMask)) {
							tempGrids.Add(rayHit.collider.gameObject);
						}
						if (Physics.Raycast(tempGrids[i].transform.position + new Vector3(1, 10, 0), Vector3.down, out rayHit, 30, layerMask)) {
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

	public bool BlockOrientationChecker() {

		foreach (var grid in blockOrientationGrids) {

			if (grid.CollisionState) {
				return true;
			}

		}

		return false;

	}

	public void NewShip() {
		// Get spaceship prefab
		var constructionPrefab = (GameObject)Resources.Load("Prefabs/Player/Spaceship", typeof(GameObject));
		// Create, and cache a new spaceship
		playerSpaceship = (GameObject)Instantiate(constructionPrefab, transform.position, transform.rotation);
		// Cache spaceship's grid and buildablegrid
		playerSpaceship_Grids = playerSpaceship.transform.Find("Grids").gameObject;
//		playerSpaceship_BuildableGrids = playerSpaceship.transform.Find("BuildableGrids").gameObject;
		// Required because name is set to "... (Clone)" due to instantiation
		RenameShip("Untitled Ship");
//		// Place initial buildable grid
//		var initialGrid = (GameObject)Resources.Load("Prefabs/Holo/BuildableGridBlock", typeof(GameObject));
//		var newBlock = (GameObject)Instantiate(initialGrid, initialGrid.transform.position, initialGrid.transform.rotation);
//		newBlock.transform.parent = playerSpaceship_BuildableGrids.transform;
	}

	public void ResetShip() {
		// Get each block under Spaceship/Grids
		Block[] grids = playerSpaceship_Grids.GetComponentsInChildren<Block>();
//		// Get each block under Spaceship/BuildableGrids
//		Collider[] buildableGrids = playerSpaceship_BuildableGrids.GetComponentsInChildren<Collider>();
		
		// Delete each block one by one
		foreach (Block block in grids) {
			Destroy(block.gameObject);
		}
//		// Delete each buildable grid one by one
//		foreach (Collider grid in buildableGrids) {
//			Destroy(grid.gameObject);
//		}

//		// Place initial block
//		var initialBlock = (GameObject)Resources.Load("Prefabs/Holo/BuildableGridBlock", typeof(GameObject));
//		var newBlock = (GameObject)Instantiate(initialBlock, initialBlock.transform.position, initialBlock.transform.rotation);
//		newBlock.transform.parent = playerSpaceship_BuildableGrids.transform;
	}

	public void SaveShip() {

		GameObject grids = GameObject.FindWithTag("Spaceship/Grids");
		Block[] blocks = grids.GetComponentsInChildren<Block>();

		List<string> lines = new List<string>();

		lines.Add("<Spaceship Name>");
		lines.Add(playerSpaceship.name);
		lines.Add("");  // for line seperation

		foreach (Block b in blocks) {
			// Write block information
			lines.Add("<" + b.blockType.ToString().ToLower() + ">");
			lines.Add(b.blockName);
			lines.Add(Mathf.RoundToInt(b.gameObject.transform.position.x).ToString()); // Mathf.Round used for precise placement
			lines.Add(Mathf.RoundToInt(b.gameObject.transform.position.y).ToString()); // Raw placement not needed from now on, Mathf.Round used for precise placement
			lines.Add(Mathf.RoundToInt(b.gameObject.transform.position.z).ToString()); // Mathf.Round used for precise placement
			lines.Add(Mathf.RoundToInt(b.gameObject.transform.rotation.eulerAngles.x).ToString());
			lines.Add(Mathf.RoundToInt(b.gameObject.transform.rotation.eulerAngles.y).ToString());
			lines.Add(Mathf.RoundToInt(b.gameObject.transform.rotation.eulerAngles.z).ToString());
			lines.Add("");	// for line seperation
		}
		// WriteAllLines creates a file, writes a collection of strings to the file,
		// and then closes the file.  You do NOT need to call Flush() or Close().
		File.WriteAllLines(Application.dataPath + "/spaceship.save", lines.ToArray());
	}

	public void LoadShip() {
			
		var file = File.OpenRead(Application.dataPath + "/spaceship.save");
		StreamReader reader = new StreamReader(file);

		GameObject[] buildingBlocks = Resources.LoadAll<GameObject>("Prefabs/Building Blocks");

		// delete existing blocks-grids
		GameObject[] blocksToDelete = GameObject.FindGameObjectsWithTag("Spaceship/Block");
		foreach (GameObject target in blocksToDelete) {
			Destroy(target);
		}

		reader.ReadLine(); // spaceship tag
		string line_ShipName = reader.ReadLine(); // name of spaceship
		reader.ReadLine(); // empty line (for visual seperation in save file)

		while (!reader.EndOfStream) {

			string line_Type = reader.ReadLine(); // type of block (tag in xml file)
			string line_Name = reader.ReadLine(); // name of block
			float line_xPos = float.Parse(reader.ReadLine()); // position.x of block
			float line_yPos = float.Parse(reader.ReadLine()); // position.y of block
			float line_zPos = float.Parse(reader.ReadLine()); // position.z of block
			float line_xRot = float.Parse(reader.ReadLine()); // rotation.x of block
			float line_yRot = float.Parse(reader.ReadLine()); // rotation.y of block
			float line_zRot = float.Parse(reader.ReadLine()); // rotation.z of block

			reader.ReadLine(); // empty line (for visual seperation in save file)

			line_Type = line_Type.Substring(1, 9); // tag chars removal

			foreach (GameObject go in buildingBlocks) {
				if (go.GetComponent<Block>().blockName == line_Name) {
					if (line_Type == go.GetComponent<Block>().blockType.ToString().ToLower()) {

						Vector3 line_Position = new Vector3(line_xPos, line_yPos, line_zPos);
						Quaternion line_Rotation = Quaternion.Euler(line_xRot, line_yRot, line_zRot);

						var newBlock = (GameObject)Instantiate(go, line_Position, line_Rotation);
						newBlock.transform.parent = GameObject.FindWithTag("Spaceship/Grids").transform;

						break;
					}
				}
			}
		}

		RenameShip(line_ShipName); // name the ship
		reader.Close();

	}

	public void RenameShip(InputField inputField) {
		playerSpaceship.name = inputField.text; // change the name of spaceship with typing into renaming field
	}

	public void RenameShip(string shipName) {
		playerSpaceship.name = shipName; // assign name to spaceship
		GameObject.Find("ShipNameField").GetComponent<InputField>().text = shipName; // assign name to renaming field
	}

	public void BlockSelection(int i) {

		var selButtons = GameObject.FindGameObjectsWithTag("GUI/BlockSelection/Button");
		foreach (var btn in selButtons) {
			// if button is NOT interactable (disabled) AND button (by name) is NOT the same with the selection
			if (!btn.GetComponent<Button>().IsInteractable() && "BlockButton" + i != btn.name)
				btn.GetComponent<Button>().interactable = true;
			else if ("BlockButton" + i == btn.name)
				btn.GetComponent<Button>().interactable = false;
		}
		switch (i) {
			case 1:
				_selectedBlock = (GameObject)Resources.Load("Prefabs/Building Blocks/Block-Hull_Bulk", typeof(GameObject));
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
				_selectedBlock = (GameObject)Resources.Load("Prefabs/Building Blocks/Block-Gyroscope_01", typeof(GameObject));
				break;
			case 6:
				_selectedBlock = (GameObject)Resources.Load("Prefabs/Building Blocks/Block-Reactor_01", typeof(GameObject));
				break;
			case 7:
				_selectedBlock = (GameObject)Resources.Load("Prefabs/Building Blocks/Block-Hull_Wedge", typeof(GameObject));
				break;
			case 8:
				_selectedBlock = (GameObject)Resources.Load("Prefabs/Building Blocks/Block-Hull_PosLog", typeof(GameObject));
				break;
		}
		
		RefreshHolocursorRef();
		GetBlockOrientations();

	}

	public void RefreshHolocursorRef() {

//		_cursorBlock.SetActive(true);

		List<GameObject> refBlk = new List<GameObject>();

		if (GameObject.FindGameObjectWithTag("Game/HolocursorRef"))
			refBlk = GameObject.FindGameObjectsWithTag("Game/HolocursorRef").ToList();

		if (refBlk.Count > 0) {
			foreach (var rb in refBlk) {
				Destroy(rb);
			}
		}

		var tmpBlk = (GameObject)Instantiate(_selectedBlock, _cursorBlock.transform.position + new Vector3(0, -10, 0), _cursorBlock.transform.rotation);
		tmpBlk.tag = "Game/HolocursorRef";
		tmpBlk.GetComponent<Collider>().enabled = false;
		tmpBlk.transform.parent = _cursorBlock.transform;
		_cursorRefBlock = tmpBlk;
		_cursorRefBlockMesh = tmpBlk.transform.Find("Mesh").gameObject;
		//		_cursorBlock.SetActive(false);

	}

	public void GetBlockOrientations() {
		
		if (_cursorRefBlock.GetComponent<Block>().isRotateable)
			blockOrientationGrids = _cursorRefBlock.transform.FindChild("MountPoints").GetComponentsInChildren<GridCollisionDetector>();

//		foreach (var mountpoint in mountgrids) {
//			if (mountpoint.gameObject.CompareTag("Spaceship/OrientationGrid"))
//				blockOrientationGrids.Add(mountpoint);
//		}

	}

	public void RefreshInfoPanel(GameObject refBlock = null) {
		// Initialize
		Block block;
		// if a reference block is not given (null as default)
		if (refBlock == null) block = rayHit.collider.gameObject.GetComponent<Block>();
		// if a reference block is given
		else block = refBlock.GetComponent<Block>();
		var values = block.GetType().GetFields(BindingFlags.Instance |
					BindingFlags.Static |
					BindingFlags.Public);
		var fields = GameObject.FindGameObjectsWithTag("GUI/InfoPanel/Field");
		// Fill info panel
		foreach (FieldInfo value in values) {
			foreach (GameObject field in fields) {
				if (field.name.Equals(value.Name)) {
					if (field.name.Equals("blockName") || field.name.Equals("blockType")) {
						field.GetComponent<Text>().text = value.GetValue(block).ToString();
						break; // for breaking the 2nd foreach earlier (for performance reasons), otherwise redundant with "else" keywords
					}
					else if (field.name.Equals("structureType") || field.name.Equals("componentType")) {
						if (value.GetValue(block).ToString().Equals("None"))
							field.GetComponent<Text>().text = "";
						else
							field.GetComponent<Text>().text = value.GetValue(block).ToString();
						break; // for breaking the 2nd foreach earlier (for performance reasons), otherwise redundant with "else" keywords
					}
					else {
						field.GetComponent<Text>().text = value.Name.Substring(0, 1).ToUpper() + value.Name.Substring(1) + ": " + value.GetValue(block);
						break; // for breaking the 2nd foreach earlier (for performance reasons), otherwise redundant with "else" keywords
					}
				}
			}
		}
		_infoPanel.GetComponent<CanvasRenderer>().SetAlpha(1f);
	}

	public void ClearInfoPanel() {
		Text[] fields = _infoPanel.GetComponentsInChildren<Text>();
		foreach (Text field in fields) {
			field.text = "";
		}
		_infoPanel.GetComponent<CanvasRenderer>().SetAlpha(0f);
	}

}