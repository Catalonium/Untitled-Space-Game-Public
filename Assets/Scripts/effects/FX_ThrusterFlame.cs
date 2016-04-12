using UnityEngine;

public class FX_ThrusterFlame : MonoBehaviour {

	private SpaceshipPhysics sp;
	private ParticleSystem.EmissionModule em;
	
	// Update is called once per frame
	void Update () {
		if (GameObject.FindGameObjectWithTag("Spaceship/Player")) {
			sp = GameObject.FindGameObjectWithTag("Spaceship/Player").GetComponent<SpaceshipPhysics>();
			em = GetComponent<ParticleSystem>().emission;

			em.enabled = sp.thrustIsActive;
		}
		else em.enabled = false;
	}

}
