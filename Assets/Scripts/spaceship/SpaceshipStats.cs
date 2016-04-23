using UnityEngine;

public class SpaceshipStats : MonoBehaviour {
	
	public float Hull, Mass, EnergyGen, EnergyCon, Maneuver, Thrust;
	private Rigidbody rb;
	private Block[] blocks;
	private GameObject grids;

	void Start() {
		rb = GetComponent<Rigidbody>();
		grids = GameObject.FindWithTag("Spaceship/Grids");
		StatCalc();
	}

	void Update() {
		if (blocks.Length != grids.GetComponentsInChildren<Block>().Length)
			StatCalc();
	}

	public void StatCalc() {
		// Reset every stat
		Hull = 0;		Mass = 0;		EnergyGen = 0;
		EnergyCon = 0;	Maneuver = 0;	Thrust = 0;
		isControllable = false;

		blocks = grids.GetComponentsInChildren<Block>();

		foreach (Block b in blocks) {
			Hull += b.hull;
			Mass += b.hull / 2; rb.mass = Mass;
			EnergyGen += b.energyGen;
			EnergyCon += b.energyCon;
			Maneuver += b.maneuver;
			Thrust += b.thrust;

			if (b.componentType.Equals(ComponentType.Bridge))
				isControllable = true;
		}
	}

	// read-only variable for ship control ability
	public bool isControllable { get; private set; }

}
