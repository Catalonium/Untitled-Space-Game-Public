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

	public bool thrustIsActive; // public read-only variable for thrusters
	public float speed; // public read-only variable for speed

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
		GameObject.Find("GUIText2").GetComponent<Text>().text = "Velocity: " + Math.Round(rb.velocity.magnitude, 2, MidpointRounding.AwayFromZero).ToString("F2") + " ᴧ";
		GameObject.Find("GUIText3").GetComponent<Text>().text = "Raw velocity: " + rb.velocity.magnitude;
		GameObject.Find("GUIText4").GetComponent<Text>().text = "Inputs: " + thrustInput + " / " + maneuverInput;
		GameObject.Find("GUIText5").GetComponent<Text>().text = "EFF ratio: " + eff.ToString("F2") + " >> " + (eff*100) + "%";
		GameObject.Find("GUIText6").GetComponent<Text>().text = "THR output: " + thrOut + " / " + spaceshipStats.Thrust + " (" + Mathf.Round(thrOut) + " ᴪ)";
		GameObject.Find("GUIText7").GetComponent<Text>().text = "MNR output: " + manOut + " / " + spaceshipStats.Maneuver + " (" + Mathf.Round(manOut) + " ᴪ)";
		GameObject.Find("GUIText8").GetComponent<Text>().text = "PWR surplus: " + (spaceshipStats.EnergyGen - spaceshipStats.EnergyCon) + " ᴦ";
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
					eff = (float) Math.Round(eff, 2, MidpointRounding.AwayFromZero); // rounded with 2 decimals
				}
				else {
					if (spaceshipStats.EnergyGen.Equals(0)) eff = 0;
					else eff = 1;
				}
			}

			// Thrust limit calculation
			{
				lim = (spaceshipStats.Thrust * eff) * 100;
				lim = Mathf.Round(lim); // protection for not rounded limit value, otherwise redundant
			}

			// Potential maneuver amt calculation
			{
				man = (spaceshipStats.Maneuver * eff) * 100f * maneuverInput;
				man = Mathf.Round(man); // multiplied by degrees per sec, then rounded
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

				// Full thrust
				if (Input.GetKey(KeyCode.R)) {
					vel = lim;
				}
				// Full stop
				if (Input.GetKey(KeyCode.F)) {
					vel = 0;
				}

				thrustIsActive = !vel.Equals(0); // read-only thrust on-off
				speed = rb.velocity.magnitude; // read-only speed
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
