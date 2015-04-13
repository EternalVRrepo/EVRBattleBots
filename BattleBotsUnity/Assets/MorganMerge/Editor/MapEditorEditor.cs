/////////////////////////////////////////////////////////////////////////////////
//
//	MapEditorEditor.cs
//	© EternalVR, All Rights Reserved
//
//	description:	Custom Editor class for the BoardManager to add functionality
//					to allow map editting while in editor instead of in game
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(MapEditor))]
public class MapEditorEditor : Editor {
	
	public string[] currentBrushStrings = new string[]{ "Normal", "Impassable", "Imp-Walled", "Null" };
	public string[] hexTypeModeStrings = new string[]{ "HexTypeMode", "HexTextureMode", "SpawnMode" };
	public string[] currentSpawnStrings = new string[]{ "None", "Player", "Enemy" };

	MapEditor mapEditor;

	public override void OnInspectorGUI()
	{
		serializedObject.Update ();
		EditorGUILayout.LabelField ("Refer to the Google Drive Documentation for instructions");
		
		mapEditor = (MapEditor)target;

		EditorGUILayout.BeginHorizontal();
		mapEditor.GridHexRows = EditorGUILayout.IntSlider("Number of Rows", (int)mapEditor.GridHexRows, 1, 20);
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.BeginHorizontal();
		mapEditor.GridHexColumns = EditorGUILayout.IntSlider("Number of Columns", (int)mapEditor.GridHexColumns, 1, 20);
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.Space ();
		if (mapEditor.transform.childCount == 0) //If there are no hexagons, it is not initialized
			mapEditor.initialized = false;
		else if(GUILayout.Button(mapEditor.initialized ? "Recreate Grid" : "Create Grid")) //Button to recreate grid
		{
			mapEditor.Recreate();
		}
		if (mapEditor.transform.childCount < mapEditor.GetArrayLength()) { //Error if the hexagons have been tampered with in a nonpermitted way
			Debug.LogError ("Do not manually delete Hexagons, flag them as impassable or null.\nUndo the deletion or recreate the grid to continue.");
			GUILayout.Label("A hexagon is missing from the grid, undo its deletion\nor reset the grid to continue");
			return;
		}

		EditorGUILayout.LabelField ("Brush and View Mode");
		EditorGUILayout.BeginHorizontal();
		mapEditor.HexTypeMode = GUILayout.SelectionGrid (mapEditor.HexTypeMode, hexTypeModeStrings, 3);
		EditorGUILayout.EndHorizontal();

		if (mapEditor.HexTypeMode == 0) {
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField("Hexagon Type Brush (TODO: Needs Textures)");
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal ();
			mapEditor.CurrentBrushType = (Hexagon.HexType)GUILayout.SelectionGrid ((int)mapEditor.CurrentBrushType, currentBrushStrings, 4);
			EditorGUILayout.EndHorizontal();
		}
		else if (mapEditor.HexTypeMode == 1) {
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Hexagon Texture Brush");
		EditorGUILayout.EndHorizontal();
//		if (boardManager.textures.Count == 0) {
//			EditorGUILayout.LabelField ("\tDrag materials below to start using them");
//		}
		ShowIcons (mapEditor.textures);
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Click to Remove selected texture", GUILayout.MaxWidth(200));
		if(GUILayout.Button ("Remove")) {
			if (mapEditor.textures.Count > 0)
				mapEditor.RemoveCurrentTexture();
		}
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField("Drag New Materials Here", GUILayout.MaxWidth (200));
		EditorGUILayout.PropertyField (serializedObject.FindProperty ("newTexture"), GUIContent.none); 
		if (mapEditor.newTexture != null)
			mapEditor.AddNewTexture ();
		EditorGUILayout.EndHorizontal ();
		}
		else if (mapEditor.HexTypeMode == 2) {
		EditorGUILayout.Space ();
		EditorGUILayout.LabelField("Hexagon Spawn Type Brush (TODO: Needs Textures)");
		mapEditor.CurrentBrushSpawn = (Hexagon.SpawnType)GUILayout.SelectionGrid ((int)mapEditor.CurrentBrushSpawn, currentSpawnStrings, 3);
		}

		EditorGUILayout.Space ();
		EditorGUILayout.Separator ();
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button ((mapEditor.boardManager.Maps[mapEditor.mapSliderIndex] != null) ? "Load Map" : "", GUILayout.Width (150))) {
			if (mapEditor.boardManager.Maps[mapEditor.mapSliderIndex] != null) {				
				if (mapEditor.mapSliderIndex < mapEditor.boardManager.Maps.Count)
					mapEditor.LoadMap();
				else Debug.Log ("No Map in slot: " + mapEditor.mapSliderIndex);
			}
		}
		if (GUILayout.Button (mapEditor.initialized ? "Save Map" : "")) {
			if (mapEditor.initialized)
				mapEditor.SaveMap();
		}
		if (GUILayout.Button (mapEditor.initialized ? "Save New Map" : "")) {
			if (mapEditor.initialized)
				mapEditor.SaveNewMap();
		}
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Current Level Loaded: ", GUILayout.MaxWidth (175));
//		if (GUILayout.Button ("Create New Map")) {
//			mapEditor.CreateNewMap();
//		}
		if (GUILayout.Button ("Delete Map")) {
			mapEditor.DeleteMap();
		}
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField("Level #", GUILayout.MaxWidth (175));
		mapEditor.mapSliderIndex = EditorGUILayout.IntSlider(mapEditor.mapSliderIndex, 0, mapEditor.boardManager.Maps.Count-1);
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.Space ();
		mapEditor.inDebug = EditorGUILayout.Toggle("Debug Mode", mapEditor.inDebug);
		if (mapEditor.inDebug) {
			serializedObject.Update ();
			mapEditor.collapse = EditorGUILayout.Foldout (mapEditor.collapse, "HexagonArray");
			ShowList (serializedObject.FindProperty("HexagonArray"), mapEditor.collapse);
			serializedObject.ApplyModifiedProperties();
		}
		
		serializedObject.ApplyModifiedProperties();
	}

	public void ShowIcons( List<Material> list ) {
		List<Texture> textures = new List<Texture>();
		foreach (Material m in list) {
			if (m.mainTexture != null)
				textures.Add (m.mainTexture);
		}
		float h = EditorGUIUtility.currentViewWidth/5 * Mathf.CeilToInt((float)mapEditor.textures.Count/5);
		mapEditor.textureBrushIndex = GUILayout.SelectionGrid (mapEditor.textureBrushIndex, textures.ToArray (), 5, GUILayout.Height(h));

	}

	/// <summary>
	/// Used to display an array
	/// </summary>
	public void ShowList (SerializedProperty list, bool collapsed, bool showSize = true) {
		EditorGUI.indentLevel += 1;
		if (collapsed) {
			if (showSize)
				EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));
			for (int i = 0; i < list.arraySize; i++) {
				EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
			}
		}
		EditorGUI.indentLevel -= 1;
	}
}



