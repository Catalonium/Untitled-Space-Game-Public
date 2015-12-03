using UnityEngine;
using System.Collections;

public class SpaceshipStats : MonoBehaviour {

	public float Hull, Mass, EnergyGen, EnergyCon, Maneuver, Thrust;
	private Block[] blocks;

	void Start() {
		blocks = GetComponentsInChildren<Block>();
		StatCalc();
	}

	private void Update() {
		if (blocks.Length != GetComponentsInChildren<Block>().Length)
			StatCalc();
	}

	public void StatCalc() {

		// Reset every stat
		Hull = 0;		Mass = 0;		EnergyGen = 0;
		EnergyCon = 0;	Maneuver = 0;	Thrust = 0;

		blocks = GetComponentsInChildren<Block>();

		foreach (Block b in blocks) {
			Hull += b.hull;
			Mass += b.mass;
			EnergyGen += b.energyGen;
			EnergyCon += b.energyCon;
			Maneuver += b.maneuver;
			Thrust += b.thrust;
		}

	}
}
