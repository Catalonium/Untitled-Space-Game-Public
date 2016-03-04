using UnityEngine;
using UnityEngine.UI;
using System;

public class SpaceshipPhysics : MonoBehaviour {

	private Rigidbody rb;
	private SpaceshipStats sStats;
	private float vel, man;

	// Use this for initialization
	void Start() {
		rb = GetComponent<Rigidbody>();
		sStats = rb.GetComponent<SpaceshipStats>();
	}
	
	// Update is called once per frame
	void Update() {
		GameObject.Find("GUIText1").GetComponent<Text>().text = "Thrust: " + vel;
		GameObject.Find("GUIText2").GetComponent<Text>().text = "Velocity: " + rb.velocity.magnitude;
		GameObject.Find("GUIText3").GetComponent<Text>().text = rb.angularDrag.ToString();
	}

	void FixedUpdate() {

		// Maneuver
		{
			rb.AddRelativeTorque(0, 100f * sStats.Maneuver * Input.GetAxis("Horizontal"), 0);
		}
		

		// Thrust
		{
			if (Input.GetAxisRaw("Vertical") > 0) {
				vel += sStats.Thrust * Input.GetAxis("Vertical");
			}
			else if (Input.GetAxisRaw("Vertical") < 0) {
				vel -= sStats.Thrust * -Input.GetAxis("Vertical");
			}
			else if (Input.GetAxisRaw("Vertical") == 0) {
				if (Mathf.Abs(vel) < 100f) {
					vel = 0;
				}
			}
			rb.AddRelativeForce(new Vector3(0, 0, vel), ForceMode.Force);
		}

	}
}
