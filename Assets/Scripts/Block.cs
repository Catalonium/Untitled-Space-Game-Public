using UnityEngine;
using System.Collections;

public enum blockType {
	Structure, Component
}
public enum structureType {
	None, Hull, Interior, Viewport
}
public enum componentType {
	None, Thruster, Gyroscope, Reactor, Bridge
}

public class Block : MonoBehaviour {

	public blockType blockType;
	public structureType structureType;
	public componentType componentType;

    public bool[] mountPos = {true, true, true, true};
	public float hull, mass, energyGen, energyCon, maneuver, thrust;

}
