using UnityEngine;
using System.Collections;
using UnityEditor;

public class AbilityWindow : EditorWindow {

	[MenuItem("Window/Create New Ability")]
	static void Init () {
//		AbilityWindow window = (AbilityWindow)EditorWindow.GetWindow (typeof (AbilityWindow));
//		window.Show();
		Debug.Log ("Created new ability to Path: \"MorganMerge/Resources/Abilities/NewAbility.asset\"");
		AbilityDescription ab = ScriptableObject.CreateInstance<AbilityDescription>();
		AssetDatabase.CreateAsset(ab, @"Assets/MorganMerge/Resources/Abilities/NewAbility.asset");
	}
}
