using UnityEngine;
using System.Collections;
using UnityEditor;

public class PowerUpWindow : EditorWindow {
	
	[MenuItem("Window/Create New PowerUp")]
	static void Init () {
		//		AbilityWindow window = (AbilityWindow)EditorWindow.GetWindow (typeof (AbilityWindow));
		//		window.Show();
		Debug.Log ("Created new PowerUp to Path: \"BattleBots/Resources/PowerUps/NewPowerUp.asset\"");
		PowerUp ab = ScriptableObject.CreateInstance<PowerUp>();
		AssetDatabase.CreateAsset(ab, @"Assets/BattleBots/Resources/PowerUps/NewPowerUp.asset");
		AssetDatabase.SaveAssets ();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = ab;
	}
}