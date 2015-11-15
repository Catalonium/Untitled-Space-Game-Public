using System;
using UnityEngine;
using System.Collections;

public enum blockType {
	Structure, Interior, Thruster, Gyroscope, Reactor, Bridge
}

public class BlockStruct : MonoBehaviour {

	private blockType blockType;
	private bool isBuildable;
	private float hull, mass, energyGen, energyCon, maneuver, thrust;

}
