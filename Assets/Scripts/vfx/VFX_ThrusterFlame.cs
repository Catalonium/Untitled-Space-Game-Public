using UnityEngine;

public class VFX_ThrusterFlame : MonoBehaviour {

	private SpaceshipPhysics sp;
	private ParticleSystem.EmissionModule em;

	void Start() {
		sp = GameObject.FindGameObjectWithTag("Spaceship/Main").GetComponent<SpaceshipPhysics>();
	}
	
	void Update() {
		if (sp.enabled) {
			em = GetComponent<ParticleSystem>().emission;
			em.enabled = sp.thrustState;
		}
		else em.enabled = false;
	}

}
