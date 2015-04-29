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
		powerUp.PowerUpTargetType = (PowerUp.PowerUpTarget)EditorGUILayout.EnumPopup("PowerUp Type", powerUp.PowerUpTargetType);
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal();
		powerUp.PowerUpEffect = (BuffEffect.Buff)EditorGUILayout.EnumPopup("PowerUp Bonus Type", powerUp.PowerUpEffect);
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal();
		if (powerUp.PowerUpEffect == BuffEffect.Buff.AllAbilityRangeIncrease || powerUp.PowerUpEffect == BuffEffect.Buff.MovementIncrease) {
			EditorGUILayout.LabelField ("Flat Bonus Amount");
			powerUp.PowerUpBonusValue = (float)EditorGUILayout.IntField ((int)powerUp.PowerUpBonusValue);
		}
		else if (powerUp.PowerUpEffect.ToString().Contains ("Percent")) {
			EditorGUILayout.LabelField("Percent Increase");
			powerUp.PowerUpBonusValue = EditorGUILayout.Slider(powerUp.PowerUpBonusValue, 0, 1);
		}
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal();
		if (powerUp.PowerUpEffect != BuffEffect.Buff.RemoveDebuffs) {
			EditorGUILayout.LabelField("Buff Duration (Turns)");
			powerUp.PowerUpBonusDuration = EditorGUILayout.IntField (powerUp.PowerUpBonusDuration);
		}
		EditorGUILayout.EndHorizontal ();

		serializedObject.Update ();
		EditorUtility.SetDirty (powerUp); //Have to set dirty or it wont update
	}
}
