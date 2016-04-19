using UnityEngine;
using System.Collections;

public class VFX_Starfield : MonoBehaviour {

	private SpaceshipPhysics sp;

	private Material particleMat;

	// Maximum number of particles in the sphere (configure to your needs for look and performance)
	public int maxParticles = 1000;
	// Range of particle sphere (when particles are beyond this range from its
	// parent they will respawn (relocate) to within range at distanceSpawn of range.
	public float range = 100f;
	// Distance percentile of range to relocate/spawn particles to
	public float distanceSpawn = 0.95f;
	// Minimum size of particles
	public float minParticleSize = 1.0f;
	// Maximum size of particles
	public float maxParticleSize = 2.0f;

	// Private variables
	private float _distanceToSpawn;
	private ParticleSystem _cacheParticleSystem;
	private Transform _cacheTransform;

	void Start() {

		// Initialization for starfield visibility calculation
		if (GameObject.FindGameObjectWithTag("Spaceship/Main")) {
			sp = GameObject.FindGameObjectWithTag("Spaceship/Main").GetComponent<SpaceshipPhysics>();
			particleMat = (Material)Resources.Load("Materials/FX/Starfield", typeof(Material));
		}

		// Cache transform and particle system to improve performance
		_cacheTransform = transform;
		_cacheParticleSystem = GetComponent<ParticleSystem>();
		// Calculate the actual spawn distances
		_distanceToSpawn = range * distanceSpawn;

		// Spawn all new particles within a sphere in range of the object
		for (int i = 0; i < maxParticles; i++) {
			ParticleSystem.Particle _newParticle = new ParticleSystem.Particle();
			_newParticle.position = _cacheTransform.position + (Random.insideUnitSphere * _distanceToSpawn);
			_newParticle.lifetime = Mathf.Infinity;
			_newParticle.startSize = Random.Range(minParticleSize, maxParticleSize);
			_cacheParticleSystem.Emit(1);
		}

	}

	void Update() {

		int numParticles = _cacheParticleSystem.particleCount;
		// Get the particles from the particle system
		ParticleSystem.Particle[] _particles = new ParticleSystem.Particle[numParticles];
		_cacheParticleSystem.GetParticles(_particles);

		// Iterate through the particles and relocate (spawn)
		for (int i = 0; i < _particles.Length; i++) {
			// Calcualte distance to particle from transform position
			float _distance = Vector3.Distance(_particles[i].position, _cacheTransform.position);

			// If distance is greater than range...
			if (_distance > range) {
				// reposition (respawn) particle according to spawn distance
				_particles[i].position = Random.onUnitSphere * _distanceToSpawn + _cacheTransform.position;
				// Set a new size of the particle
				_particles[i].startSize = Random.Range(minParticleSize, maxParticleSize);
			}

			// Starfield visibility calculation
			if (sp != null) {
				particleMat.SetColor("_TintColor", new Color(1, 1, 1, Mathf.Clamp(sp.currentSpeed * 0.5f, 0, 0.65f)));
			}
		}

		// Set the particles according to above modifications
		_cacheParticleSystem.SetParticles(_particles, numParticles);

	}

}
