using UnityEngine;
using System.Collections;

public class GameController: MonoBehaviour {

	public GameObject blockPlaceholder;
	public Material holoYellow;
	private GameObject go;
	private Ray ray;
	private Vector3 placementPos;
	private float gridSize = 1f;

	// Use this for initialization
	void Start() {
		go = (GameObject)Instantiate(blockPlaceholder, transform.position, transform.rotation);
		go.GetComponent<Renderer>().material = holoYellow;
	}

	// Update is called once per frame
	void Update() {

		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		placementPos = new Vector3(Mathf.Round(ray.origin.x) * gridSize, 0, Mathf.Round(ray.origin.z) * gridSize);

		go.transform.position = new Vector3(Mathf.Round(ray.origin.x) * gridSize, -5, Mathf.Round(ray.origin.z) * gridSize);

		if (Input.GetMouseButtonDown(0)) { //Place block
			Instantiate(blockPlaceholder, placementPos, transform.rotation);
		}
		else if (Input.GetMouseButtonDown(1)) { //Delete block
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 10f)) {
				//GameObject.Find("Nameofyourobject") search your gameobject on the hierarchy with the desired name and allows you to use it
				Destroy(hit.collider.gameObject);
			}
		}
	}
}