using UnityEngine;
using System.Collections;
//using System.Diagnostics;

public enum blockType {
	Structure, Interior, Thruster, Gyroscope, Reactor, Bridge
}

public class Block : MonoBehaviour {

	public blockType blockType;
    public bool mountPosN = true, mountPosE = true, mountPosS = true, mountPosW = true;
    public bool isPlaceable = false;
	public float hull, mass, energyGen, energyCon, maneuver, thrust;

}
