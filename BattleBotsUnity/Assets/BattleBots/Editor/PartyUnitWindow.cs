using UnityEngine;
using System.Collections;
using UnityEditor;

public class PartyUnitWindow : EditorWindow {
	
	[MenuItem("Window/Create New PartyUnit")]
	static void Init () {
		//		AbilityWindow window = (AbilityWindow)EditorWindow.GetWindow (typeof (AbilityWindow));
		//		window.Show();
		Debug.Log ("Created new PartyUnit to Path: \"BattleBots/Resources/PartyUnits/NewPartyUnit.asset\"");
		PartyUnit ab = ScriptableObject.CreateInstance<PartyUnit>();
		AssetDatabase.CreateAsset(ab, @"Assets/BattleBots/Resources/PartyUnits/NewPartyUnit.asset");
		AssetDatabase.SaveAssets ();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = ab;
	}
}
