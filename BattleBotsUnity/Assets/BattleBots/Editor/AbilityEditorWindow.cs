using UnityEngine;
using System.Collections;
using UnityEditor;

public class AbilityEditorWindow : EditorWindow {

	[MenuItem("Window/Create New Ability")]
	static void Init () {
//		AbilityWindow window = (AbilityWindow)EditorWindow.GetWindow (typeof (AbilityWindow));
//		window.Show();
		Debug.Log ("Created new ability to Path: \"BattleBots/Resources/Abilities/NewAbility.asset\"");
		AbilityDescription ab = ScriptableObject.CreateInstance<AbilityDescription>();
		AssetDatabase.CreateAsset(ab, @"Assets/BattleBots/Resources/Abilities/NewAbility.asset");
		AssetDatabase.SaveAssets ();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = ab;
	}
}
