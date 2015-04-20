using UnityEngine;
using System.Collections;
using UnityEditor;

public class TalentTreeWindow : EditorWindow {
	
	[MenuItem("Window/Create New Talent Tree")]
	static void Init () {
		//		AbilityWindow window = (AbilityWindow)EditorWindow.GetWindow (typeof (AbilityWindow));
		//		window.Show();
		Debug.Log ("Created new talent tree to Path: \"BattleBots/Resources/TalentTrees/NewTree.asset\"");
		TalentTree ab = ScriptableObject.CreateInstance<TalentTree>();
		AssetDatabase.CreateAsset(ab, @"Assets/BattleBots/Resources/TalentTrees/NewTree.asset");
		AssetDatabase.SaveAssets ();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = ab;
	}
}
