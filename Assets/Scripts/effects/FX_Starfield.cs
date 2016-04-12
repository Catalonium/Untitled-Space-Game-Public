using UnityEngine;
using System.Collections;
using System.Net;

public class FX_Starfield : MonoBehaviour {

	private SpaceshipPhysics sp;

	private Transform tf;
	private ParticleSystem.Particle[] particles;

	private Material particleMat;

	public int starCount = 1000;
	public float starMinSize = 0.10f;
	public float starMaxSize = 0.15f;
	public float starDistance = 75;
	public float starClipDistance = 1;
	private float starDistanceSqr;
	private float starClipDistanceSqr;


	// Use this for initialization
	void Start() {
		if (GameObject.FindGameObjectWithTag("Spaceship/Player")) {
			sp = GameObject.FindGameObjectWithTag("Spaceship/Player").GetComponent<SpaceshipPhysics>();
			particleMat = (Material)Resources.Load("Materials/FX/Starfield", typeof(Material));
		}
		
		tf = transform;
		starDistanceSqr = starDistance * starDistance;
		starClipDistanceSqr = starClipDistance * starClipDistance;
	}


	private void CreateStars() {
		particles = new ParticleSystem.Particle[starCount];
		
		for (var i = 0; i < starCount; i++) {
			particles[i].position = Random.insideUnitSphere * starDistance + tf.position;
			particles[i].startColor = new Color(1, 1, 1, 1);
			particles[i].startSize = Random.Range(starMinSize, starMaxSize);
		}
	}


	// Update is called once per frame
	void Update() {
		if (particles == null)
			CreateStars();

		for (var i = 0; i < starCount; i++) {
			if ((particles[i].position - tf.position).sqrMagnitude > starDistanceSqr) {
				particles[i].position = Random.insideUnitSphere.normalized * starDistance + tf.position;
			}

			// Star distance clipping calculation
			if ((particles[i].position - tf.position).sqrMagnitude <= starClipDistanceSqr) {
				float percent = (particles[i].position - tf.position).sqrMagnitude / starClipDistanceSqr;
				particles[i].startColor = new Color(1, 1, 1, percent);
				particles[i].startSize = percent * particles[i].startSize;
			}

			// Starfield visibility calculation
			if (sp != null) {
				particleMat.SetColor("_TintColor", new Color(1, 1, 1, Mathf.Clamp(sp.speed * 0.5f, 0, 0.6f)));
			}
		}

		GetComponent<ParticleSystem>().SetParticles(particles, particles.Length);
	}

}
