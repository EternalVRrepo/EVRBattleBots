/////////////////////////////////////////////////////////////////////////////////
//
//	BoardManager.cs
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

[CustomEditor(typeof(BoardManager))]
public class BoardManagerEditor : Editor {
	
	public string[] currentBrushStrings = new string[]{ "Normal", "Impassable", "Imp-Walled", "Null" };
	public string[] hexTypeModeStrings = new string[]{ "HexTypeMode", "HexTextureMode" };

	BoardManager boardManager;

	public override void OnInspectorGUI()
	{
		serializedObject.Update ();
		
		boardManager = (BoardManager)target;

		EditorGUILayout.BeginHorizontal();
		boardManager.GridHexRows = EditorGUILayout.IntSlider("Number of Rows", (int)boardManager.GridHexRows, 1, 20);
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.BeginHorizontal();
		boardManager.GridHexColumns = EditorGUILayout.IntSlider("Number of Columns", (int)boardManager.GridHexColumns, 1, 20);
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.Space ();
		if (boardManager.transform.childCount == 0) //If there are no hexagons, it is not initialized
			boardManager.initialized = false;
		if (boardManager.recreateConfirmation) {
			if (GUILayout.Button("Are you sure, this will reset the board.")) {
				boardManager.recreateConfirmation = false;
				boardManager.Recreate();
			}
		}
		else if(GUILayout.Button(boardManager.initialized ? "Recreate Grid" : "Create Grid")) //Button to recreate grid
		{
			if (!boardManager.initialized)
				boardManager.Recreate();
			else boardManager.recreateConfirmation = true;
		}
		if (boardManager.transform.childCount < boardManager.ArrayLength()) { //Error if the hexagons have been tampered with in a nonpermitted way
			Debug.LogError ("Do not manually delete Hexagons, flag them as impassable or null.\nUndo the deletion or recreate the grid to continue.");
			GUILayout.Label("A hexagon is missing from the grid, undo its deletion\nor reset the grid to continue");
			return;
		}

		EditorGUILayout.LabelField ("Brush and View Mode");
		EditorGUILayout.BeginHorizontal();
		boardManager.HexTypeMode = GUILayout.SelectionGrid (boardManager.HexTypeMode, hexTypeModeStrings, 2);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space ();
		EditorGUILayout.LabelField("Hexagon Type Brush (TODO: Needs Textures)");
		boardManager.CurrentBrushType = (Hexagon.HexType)GUILayout.SelectionGrid ((int)boardManager.CurrentBrushType, currentBrushStrings, 4);

		EditorGUILayout.Space ();
		EditorGUILayout.LabelField ("Hexagon Texture Brush");
//		if (boardManager.textures.Count == 0) {
//			EditorGUILayout.LabelField ("\tDrag materials below to start using them");
//		}
		ShowIcons (boardManager.textures);
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Click to Remove selected texture", GUILayout.MaxWidth(200));
		if(GUILayout.Button ("Remove")) {
			boardManager.RemoveCurrentTexture();
		}
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField("Drag New Materials Here", GUILayout.MaxWidth (200));
		EditorGUILayout.PropertyField (serializedObject.FindProperty ("newTexture"), GUIContent.none); 
		if (boardManager.newTexture != null)
			boardManager.AddNewTexture ();
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.Space ();
		boardManager.inDebug = EditorGUILayout.Toggle("Debug Mode", boardManager.inDebug);
		if (boardManager.inDebug) {
			serializedObject.Update ();
			boardManager.collapse = EditorGUILayout.Foldout (boardManager.collapse, "HexagonArray");
			ShowList (serializedObject.FindProperty("HexagonArray"), boardManager.collapse);
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
		float h = EditorGUIUtility.currentViewWidth/5 * Mathf.CeilToInt((float)boardManager.textures.Count/5);
		boardManager.textureBrushIndex = GUILayout.SelectionGrid (boardManager.textureBrushIndex, textures.ToArray (), 5, GUILayout.Height(h));
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



