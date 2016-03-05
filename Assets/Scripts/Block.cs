using UnityEngine;

public enum BlockType {
	Structure, Component
}
public enum StructureType {
	None, Hull, Interior, Viewport
}
public enum ComponentType {
	None, Thruster, Gyroscope, Reactor, Bridge
}

[System.Serializable]	// This actually allows this class to hold its values that varies between prefabs (building blocks) caused by custom inspector/editor
public class Block : MonoBehaviour {

	public BlockType blockType;
	public StructureType structureType;
	public ComponentType componentType;

//	public bool[] mountPos = {true, true, true, true};
	public float hull, mass, energyGen, energyCon, maneuver, thrust;

}
