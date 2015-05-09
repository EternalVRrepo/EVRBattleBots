/////////////////////////////////////////////////////////////////////////////////
//
//	PartyUnitEditor.cs
//	© EternalVR, All Rights Reserved
//
//	description:	Custom editor to let you make party units
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 

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

[CustomEditor(typeof(PartyUnit))]
public class PartyUnitEditor : Editor {
	
	PartyUnit partyUnit;
	
	public override void OnInspectorGUI() {
		serializedObject.Update ();
		partyUnit = (PartyUnit)target;
		
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Name:");
		partyUnit.Name = EditorGUILayout.TextField (partyUnit.Name);
		if (partyUnit.name != partyUnit.Name)
			AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath (partyUnit).ToString(), partyUnit.Name);
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal ();
		partyUnit.UnitClass = (PlayerControlledBoardUnit.PlayerClass)EditorGUILayout.EnumPopup("Class: ", partyUnit.UnitClass);
		partyUnit.currentLevel = EditorGUILayout.IntField("Starting Level: ", partyUnit.currentLevel);
		EditorGUILayout.EndHorizontal ();
		
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField("Drag New Model Prefab Here", GUILayout.MaxWidth (200));
		EditorGUILayout.PropertyField (serializedObject.FindProperty ("newModel"), GUIContent.none); 
		if (Event.current.type == EventType.Repaint && partyUnit.newModel != null)  //repaint only to avoid layout errors
			partyUnit.UpdatePrefab ();
		EditorGUILayout.EndHorizontal ();
		
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Health:");
		partyUnit.Health = EditorGUILayout.IntField (partyUnit.Health);
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("MovementDistance:");
		partyUnit.MovementDistance = EditorGUILayout.IntField (partyUnit.MovementDistance);
		EditorGUILayout.EndHorizontal ();

//		EditorGUILayout.LabelField("Talent Tree: ");
		EditorGUILayout.PropertyField(serializedObject.FindProperty("UnitTalentTree"));

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Starting Abilities:");
		EditorGUILayout.EndHorizontal ();
		DisplayAbilities();
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField("Drag New Abilities Here", GUILayout.MaxWidth (200));
		EditorGUILayout.PropertyField (serializedObject.FindProperty ("newAbility"), GUIContent.none); 
		if (Event.current.type == EventType.Repaint) { //repaint only to avoid layout errors
			if (partyUnit.newAbility != null)
				partyUnit.AddNewAbility ();
		}
		EditorGUILayout.EndHorizontal ();
		
		serializedObject.ApplyModifiedProperties();
		EditorUtility.SetDirty (partyUnit); //Have to set dirty or it wont update
	}
	
	/// <summary>
	/// Used to display an array
	/// </summary>
	public void DisplayAbilities () {
		EditorGUI.indentLevel += 1;
		for (int i = 0; i < partyUnit.ListOfAbilities.Count; i++) {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField (partyUnit.ListOfAbilities[i].DisplayName, GUILayout.MaxWidth (200));
			if (GUILayout.Button("Select")) {
				AbilityDescription ab = partyUnit.ListOfAbilities[i];
				EditorUtility.FocusProjectWindow ();
				Selection.activeObject = ab;
			}
			if (GUILayout.Button("Delete")) {
				partyUnit.ListOfAbilities.RemoveAt(i);
				i--;
			}
			EditorGUILayout.EndHorizontal();
		}
		EditorGUI.indentLevel -= 1;
	}
	
	
	
	
	
}
