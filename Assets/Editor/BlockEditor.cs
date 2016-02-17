using System;
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Block))]
public class BlockEditor : Editor {

	private bool showMountPositions = true;

	public override void OnInspectorGUI() {

		Block block = (Block) target;

		block.blockType = (blockType) EditorGUILayout.EnumPopup("Block Type", block.blockType);

		switch (block.blockType) {
			case blockType.Structure:
				block.structureType = (structureType) EditorGUILayout.EnumPopup("Structure Type", block.structureType);
				block.componentType = componentType.None;

				if (block.structureType != structureType.None) {
					showMountPositions = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), showMountPositions, "Mount Points", true);
					if (showMountPositions) {
						EditorGUI.indentLevel++;
						block.mountPos[0] = EditorGUILayout.Toggle("▲ - North", block.mountPos[0]);
						block.mountPos[1] = EditorGUILayout.Toggle("► - East", block.mountPos[1]);
						block.mountPos[2] = EditorGUILayout.Toggle("▼ - South", block.mountPos[2]);
						block.mountPos[3] = EditorGUILayout.Toggle("◄ - West", block.mountPos[3]);
						EditorGUI.indentLevel--;
					}

					block.hull = EditorGUILayout.FloatField("Hull", block.hull);
					block.mass = EditorGUILayout.FloatField("Mass", block.mass);
					block.energyGen = 0;
					block.energyCon = 0;
					block.maneuver = 0;
					block.thrust = 0;

				}
				break;

			case blockType.Component:
				block.componentType = (componentType)EditorGUILayout.EnumPopup("Component Type", block.componentType);
				block.structureType = structureType.None;

				if (block.componentType != componentType.None) {
					showMountPositions = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), showMountPositions, "Mount Points", true);
					if (showMountPositions) {
						EditorGUI.indentLevel++;
						block.mountPos[0] = EditorGUILayout.Toggle("▲ - North", block.mountPos[0]);
						block.mountPos[1] = EditorGUILayout.Toggle("► - East", block.mountPos[1]);
						block.mountPos[2] = EditorGUILayout.Toggle("▼ - South", block.mountPos[2]);
						block.mountPos[3] = EditorGUILayout.Toggle("◄ - West", block.mountPos[3]);
						EditorGUI.indentLevel--;
					}

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