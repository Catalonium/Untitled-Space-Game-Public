using UnityEditor;

[CustomEditor(typeof(Block))]
public class BlockEditor : Editor {

//	private bool showMountPositions = true;

	public override void OnInspectorGUI() {

		Block block = (Block) target;

		block.blockType = (BlockType) EditorGUILayout.EnumPopup("Block Type", block.blockType);

		switch (block.blockType) {
			case BlockType.Structure:
				block.structureType = (StructureType) EditorGUILayout.EnumPopup("Structure Type", block.structureType);
				block.componentType = ComponentType.None;

				if (block.structureType != StructureType.None) {
//					showMountPositions = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), showMountPositions, "Mount Points", true);
//					if (showMountPositions) {
//						EditorGUI.indentLevel++;
//						block.mountPos[0] = EditorGUILayout.Toggle("▲ - North", block.mountPos[0]);
//						block.mountPos[1] = EditorGUILayout.Toggle("► - East", block.mountPos[1]);
//						block.mountPos[2] = EditorGUILayout.Toggle("▼ - South", block.mountPos[2]);
//						block.mountPos[3] = EditorGUILayout.Toggle("◄ - West", block.mountPos[3]);
//						EditorGUI.indentLevel--;
//					}

					block.brandName = EditorGUILayout.TextField("Brand Name", block.brandName);
					block.modelName = EditorGUILayout.TextField("Model Name", block.modelName);
					block.hull = EditorGUILayout.FloatField("Hull", block.hull);
					block.mass = EditorGUILayout.FloatField("Mass", block.mass);
					block.energyGen = EditorGUILayout.FloatField("Energy Output", block.energyGen);
					block.energyCon = EditorGUILayout.FloatField("Energy Input", block.energyCon);
					block.maneuver = EditorGUILayout.FloatField("Maneuver", block.maneuver);
					block.thrust = EditorGUILayout.FloatField("Thrust", block.thrust);

				}
				break;

			case BlockType.Component:
				block.componentType = (ComponentType)EditorGUILayout.EnumPopup("Component Type", block.componentType);
				block.structureType = StructureType.None;

				if (block.componentType != ComponentType.None) {
//					showMountPositions = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), showMountPositions, "Mount Points", true);
//					if (showMountPositions) {
//						EditorGUI.indentLevel++;
//						block.mountPos[0] = EditorGUILayout.Toggle("▲ - North", block.mountPos[0]);
//						block.mountPos[1] = EditorGUILayout.Toggle("► - East", block.mountPos[1]);
//						block.mountPos[2] = EditorGUILayout.Toggle("▼ - South", block.mountPos[2]);
//						block.mountPos[3] = EditorGUILayout.Toggle("◄ - West", block.mountPos[3]);
//						EditorGUI.indentLevel--;
//					}

					block.brandName = EditorGUILayout.TextField("Brand Name", block.brandName);
					block.modelName = EditorGUILayout.TextField("Model Name", block.modelName);
					block.hull = EditorGUILayout.FloatField("Hull", block.hull);
					block.mass = EditorGUILayout.FloatField("Mass", block.mass);
					block.energyGen = EditorGUILayout.FloatField("Energy Output", block.energyGen);
					block.energyCon = EditorGUILayout.FloatField("Energy Input", block.energyCon);
					block.maneuver = EditorGUILayout.FloatField("Maneuver", block.maneuver);
					block.thrust = EditorGUILayout.FloatField("Thrust", block.thrust);

				}
				break;
			
		}

	}

}