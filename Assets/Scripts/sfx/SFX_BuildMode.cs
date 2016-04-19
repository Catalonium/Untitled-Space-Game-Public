using UnityEngine;
using System.Collections;

public class SFX_BuildMode : MonoBehaviour {

	public AudioClip placeBlock, removeBlock, errorBlock;
	private AudioSource source;

	void Awake() {
		source = GetComponent<AudioSource>();
	}

	public void placeBlockSFX() {
		source.PlayOneShot(placeBlock, Random.Range(0.8f, 1.0f));
	}
	public void removeBlockSFX() {
		source.PlayOneShot(removeBlock, Random.Range(0.8f, 1.0f));
	}
	public void errorBlockSFX() {
		source.PlayOneShot(errorBlock, Random.Range(0.8f, 1.0f));
	}

}
