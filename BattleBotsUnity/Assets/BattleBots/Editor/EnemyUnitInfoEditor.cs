/////////////////////////////////////////////////////////////////////////////////
//
//	EnemyUnitInfoEditor.cs
//	© EternalVR, All Rights Reserved
//
//	description:	Custom editor to let you make enemies
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 

using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(EnemyUnitInfo))]
public class EnemyUnitInfoEditor : Editor {

	EnemyUnitInfo enemyUnit;
	
	public override void OnInspectorGUI() {
		serializedObject.Update ();
		enemyUnit = (EnemyUnitInfo)target;

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Name:");
		enemyUnit.Name = EditorGUILayout.TextField (enemyUnit.Name);
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField("Drag New Model Prefab Here", GUILayout.MaxWidth (200));
		EditorGUILayout.PropertyField (serializedObject.FindProperty ("newModel"), GUIContent.none); 
		if (Event.current.type == EventType.Repaint && enemyUnit.newModel != null)  //repaint only to avoid layout errors
			enemyUnit.UpdatePrefab ();
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Health:");
		enemyUnit.Health = EditorGUILayout.IntField (enemyUnit.Health);
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("MovementDistance:");
		enemyUnit.MovementDistance = EditorGUILayout.IntField (enemyUnit.MovementDistance);
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.BeginHorizontal ();
		enemyUnit.AIType = (CombatAIManager.AIType)EditorGUILayout.EnumPopup("Enemy AI Type", enemyUnit.AIType);
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Abilities:");
		EditorGUILayout.EndHorizontal ();
		DisplayAbilities();
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField("Drag New Abilities Here", GUILayout.MaxWidth (200));
		EditorGUILayout.PropertyField (serializedObject.FindProperty ("newAbility"), GUIContent.none); 
		if (Event.current.type == EventType.Repaint) { //repaint only to avoid layout errors
			if (enemyUnit.newAbility != null)
				enemyUnit.AddNewAbility ();
		}
		EditorGUILayout.EndHorizontal ();
	
		serializedObject.ApplyModifiedProperties();
		EditorUtility.SetDirty (enemyUnit); //Have to set dirty or it wont update
	}

	/// <summary>
	/// Used to display an array
	/// </summary>
	public void DisplayAbilities () {
		EditorGUI.indentLevel += 1;
		for (int i = 0; i < enemyUnit.ListOfAbilities.Count; i++) {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField (enemyUnit.ListOfAbilities[i].DisplayName, GUILayout.MaxWidth (200));
			if (GUILayout.Button("Select")) {
				AbilityDescription ab = enemyUnit.ListOfAbilities[i];
				EditorUtility.FocusProjectWindow ();
				Selection.activeObject = ab;
			}
			if (GUILayout.Button("Delete")) {
				enemyUnit.ListOfAbilities.RemoveAt(i);
				i--;
			}
			EditorGUILayout.EndHorizontal();
		}
		EditorGUI.indentLevel -= 1;
	}





}
