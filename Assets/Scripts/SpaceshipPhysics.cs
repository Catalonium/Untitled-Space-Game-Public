using UnityEngine;
using UnityEngine.UI;
using System;

public class SpaceshipPhysics : MonoBehaviour {

	private GameObject s;
	private SpaceshipStats sStats;
	private float vel, man;

	// Use this for initialization
	void Start () {
		s = gameObject;
		sStats = s.GetComponent<SpaceshipStats>();
	}
	
	// Update is called once per frame
	void Update () {
		GameObject.Find("GUIText1").GetComponent<Text>().text = Math.Round(vel, 4).ToString();
	}

	void FixedUpdate() {

		//Control
		if (Input.GetAxisRaw("Horizontal") != 0) {
			s.transform.Rotate(0, 10f / sStats.Mass * sStats.Maneuver * Input.GetAxis("Horizontal"), 0);
		}

		// Thrust
		if (Input.GetAxisRaw("Vertical") > 0) {
			vel += 0.01f / sStats.Mass * sStats.Thrust * Input.GetAxisRaw("Vertical");
		}
		else if (Input.GetAxisRaw("Vertical") < 0) {
			vel -= 0.01f / sStats.Mass * sStats.Thrust * -Input.GetAxisRaw("Vertical");
		}
		else if (Input.GetAxisRaw("Vertical") == 0) {
			if (Mathf.Abs(vel) < 0.002f) {
				vel = 0;
			}
		}

		s.transform.position += new Vector3(0, 0, (float)Math.Round(vel, 3));

	}
}
