/////////////////////////////////////////////////////////////////////////////////
//
//	PowerUpEditor.cs
//	© EternalVR, All Rights Reserved
//
//	description:	Custom editor to let you make power ups
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 

using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(PowerUp))]
public class PowerUpEditor : Editor {
	
	PowerUp powerUp;
	
	public override void OnInspectorGUI()
	{
		serializedObject.Update ();
		powerUp = (PowerUp)target;

		EditorGUILayout.BeginHorizontal();
		powerUp.PowerUpTargetType = (PowerUp.PowerUpTarget)EditorGUILayout.EnumPopup("Ability Target Type", powerUp.PowerUpTargetType);
		EditorGUILayout.EndHorizontal ();

		serializedObject.Update ();
		EditorUtility.SetDirty (powerUp); //Have to set dirty or it wont update
	}
}
