using UnityEngine;
using System.Collections;

public class SFX_Block : MonoBehaviour {
	
	private SpaceshipPhysics sp; // spaceship physics component cache
	private GameObject gr; //  spaceship grid gameobject cache

	private Block block; // block that this script is attached to

	private AudioSource source; // audio source for this block
	private float originalVol; // original vol level for this block
	private float calculatedVol; // calculated vol level for this block
	private float fadingAmt; // vol level fading amount

	private bool volCalcState = false; // is vol level calculated?
	
	private bool triggerState; // trigger state of this sound

	[Range(1.0f, 5.0f)]
	public float volumeFadeSpeed = 2f; // volume level fading speed in secs (between 1f-5f, default=2f)


	void Awake() {
		source = GetComponent<AudioSource>();
		originalVol = source.volume;
	}

	void Start() {
		sp = GameObject.FindGameObjectWithTag("Spaceship/Main").GetComponent<SpaceshipPhysics>();
		gr = GameObject.FindGameObjectWithTag("Spaceship/Grids");
		block = GetComponentInParent<Block>();
	}

	void Update() {
		// If physics are enabled (will be true as long as game's in the GameplayMode scene)
		if (sp.enabled) {
			// Variable updates associated with their counterparts (continuous)
			switch (block.componentType) {
				case ComponentType.Thruster:
					triggerState = sp.thrustState;
					break;
				case ComponentType.Gyroscope:
					triggerState = sp.maneuverState;
					break;
				case ComponentType.Reactor:
					triggerState = true;
					break;
			}
			// Volume level calculation (discrete)
			if (!volCalcState) {
				var grids = gr.GetComponentsInChildren<Block>();
				var i = 0;
				// counts the number of blocks which are same type
				foreach (Block b in grids) {
					if (b.componentType == block.componentType)
						i++;
				}
				// calculates vol level with number of blocks
				calculatedVol = originalVol / i;
				source.volume = 0f; // for startup sound level
				volCalcState = true;
			}
		}
		// If physics are disabled (cases of the game not being in GameplayMode scene)
		else {
			if (volCalcState) volCalcState = false;
			if (triggerState) triggerState = false;
			soundController();
		}

		// Volume fading amount (continuous)
		fadingAmt = Time.deltaTime * volumeFadeSpeed;
		// Sound play/stop toggle (hybrid)
		soundController();
	}

	// Sound play/stop toggler function with volume fade in/out calculation
	void soundController() {
		// If the trigger is on
		if (triggerState) {
			// if there's no sound playing, play
			if (!source.isPlaying) {
				source.Play();
			}
			// if sound is playing, and volume is less than max, gradually increase it
			else if (source.volume + fadingAmt < calculatedVol)
				source.volume += fadingAmt;
			// if sound is playing, and volume is becoming greater than max, assign max
			else if (source.volume + fadingAmt > calculatedVol)
				source.volume = calculatedVol;
			// if everything above is done, simply enter and exit this statement without doing anything
		}
		// If the trigger is off
		else {
			// if sound is playing, and volume is more than min(0f), gradually decrease it
			if (source.volume - fadingAmt > 0f)
				source.volume -= fadingAmt;
			// if sound is playing, and volume is becoming lesser than min(0f), assign min(0f)
			else if (source.volume - fadingAmt < 0f)
				source.volume = 0f;
			// if there's sound playing, stop
			else if (source.isPlaying)
				source.Stop();
			// if everything above is done, simply enter and exit this statement without doing anything
		}
	}

}
