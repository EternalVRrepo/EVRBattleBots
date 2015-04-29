using UnityEngine;
using System.Collections;
using UnityEditor;

public class EnemyUnitWindow : EditorWindow {
	
	[MenuItem("Window/Create New EnemyUnitInfo")]
	static void Init () {
		//		AbilityWindow window = (AbilityWindow)EditorWindow.GetWindow (typeof (AbilityWindow));
		//		window.Show();
		Debug.Log ("Created new EnemyUnitInfo to Path: \"BattleBots/Resources/Enemies/NewEnemy.asset\"");
		EnemyUnitInfo ab = ScriptableObject.CreateInstance<EnemyUnitInfo>();
		AssetDatabase.CreateAsset(ab, @"Assets/BattleBots/Resources/Enemies/NewEnemy.asset");
		AssetDatabase.SaveAssets ();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = ab;
	}
}
