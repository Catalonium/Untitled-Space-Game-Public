using System;
using UnityEngine;
using UnityEngine.UI;

public class SpaceshipPhysics : MonoBehaviour {

	private Rigidbody rb;
	private SpaceshipStats spaceshipStats;
	private float vel, man, eff, lim;
	private float thrustInput, maneuverInput;

	float thrustDelay = 0.1f;
	float thrustDelayTimer = 0;

	public bool thrust_IsActive; // public variable for reading velocity

	// Use this for initialization
	void Start() {
		rb = GetComponent<Rigidbody>();
		spaceshipStats = rb.GetComponent<SpaceshipStats>();
	}
	
	// Update is called once per frame
	void Update() {
		var thrOut = spaceshipStats.Thrust * eff;
		var manOut = spaceshipStats.Maneuver * eff;
		GameObject.Find("GUIText1").GetComponent<Text>().text = "Thrust/Limit: " + vel + " / " + lim;
		GameObject.Find("GUIText2").GetComponent<Text>().text = "Velocity: " + Math.Round(rb.velocity.magnitude, 2, MidpointRounding.AwayFromZero).ToString("F2");
		GameObject.Find("GUIText3").GetComponent<Text>().text = "Raw velocity: " + rb.velocity.magnitude;
		GameObject.Find("GUIText4").GetComponent<Text>().text = "Inputs: " + thrustInput + " / " + maneuverInput;
		GameObject.Find("GUIText5").GetComponent<Text>().text = "Eff. ratio: " + eff.ToString("F2") + " >> " + (eff*100) + "%";
		GameObject.Find("GUIText6").GetComponent<Text>().text = "Thr. output: " + thrOut + " / " + spaceshipStats.Thrust + " (" + Mathf.Round(thrOut) + ")";
		GameObject.Find("GUIText7").GetComponent<Text>().text = "Man. output: " + manOut + " / " + spaceshipStats.Maneuver + " (" + Mathf.Round(manOut) + ")";
		GameObject.Find("GUIText8").GetComponent<Text>().text = "";
	}

	void FixedUpdate() {

		// Get axis' here for caching
		thrustInput = Input.GetAxisRaw("Vertical");
		maneuverInput = Input.GetAxisRaw("Horizontal");


		// Calculations
		{
			// Energy efficiency calculation
			{
				if (spaceshipStats.EnergyCon > spaceshipStats.EnergyGen) {
					eff = (spaceshipStats.EnergyGen / spaceshipStats.EnergyCon) * 0.4f;
					eff = (float)Math.Round(eff, 2, MidpointRounding.AwayFromZero); // rounded with 2 decimals
				}
				else eff = 1;
			}

			// Thrust limit calculation
			{
				lim = (spaceshipStats.Thrust * eff) * 100;
				lim = Mathf.Round(lim); // protection for not rounded limit value, otherwise redundant
			}

			// Potential maneuver amt calculation
			{
				man = (spaceshipStats.Maneuver * eff) * maneuverInput;
				man = Mathf.Round(man * 100f); // multiplied by degrees per sec, then rounded
			}

			// Potential thrust amt calculation
			{
				thrustDelayTimer -= Time.deltaTime;

				if (thrustInput > 0 && thrustDelayTimer <= 0 && vel < lim) {
					thrustDelayTimer = thrustDelay;  // delay reset
					if (Mathf.Round(vel + (spaceshipStats.Thrust * eff) * thrustInput) > lim)
						vel = lim;
					else
						vel += Mathf.Round((spaceshipStats.Thrust * eff) * thrustInput);
				}
				else if (thrustInput < 0 && thrustDelayTimer <= 0 && vel > 0) {
					thrustDelayTimer = thrustDelay;  // delay reset
					if (Mathf.Round(vel - (spaceshipStats.Thrust * eff) * -thrustInput) < 0)
						vel = 0;
					else
						vel -= Mathf.Round((spaceshipStats.Thrust * eff) * -thrustInput);
				}
				if (Input.GetKeyDown(KeyCode.F)) {
					vel = 0;
				}

				thrust_IsActive = !vel.Equals(0); // thruster fx
			}
		}


		// Maneuver
		{
			// Rotation formula
			rb.AddTorque(0, man, 0);
			// Rotation constraint freeze
			transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
		}


		// Thrust
		{
			// Movement formula
			rb.AddRelativeForce(new Vector3(0, 0, vel), ForceMode.Force);
			// Position constraint freeze
			transform.position = new Vector3(transform.position.x, 0, transform.position.z);
		}


		// Stopping formula
		{
			if (thrustInput.Equals(0) && vel.Equals(0) && rb.velocity.magnitude < 0.005f)
				rb.velocity = Vector3.zero;
		}

	}
}
