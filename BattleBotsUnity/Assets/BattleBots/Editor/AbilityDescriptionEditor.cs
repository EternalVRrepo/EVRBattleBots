/////////////////////////////////////////////////////////////////////////////////
//
//	AbilityDescriptionEditor.cs
//	© EternalVR, All Rights Reserved
//
//	description:	Custom editor to let you make abilities
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 

using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(AbilityDescription))]
public class AbilityDescriptionEditor : Editor {

	AbilityDescription abilityEditor;

	public override void OnInspectorGUI()
	{
		serializedObject.Update ();
		abilityEditor = (AbilityDescription)target;
		
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Ability Name");
		abilityEditor.DisplayName = abilityEditor.name;
		abilityEditor.DisplayName = EditorGUILayout.TextField(abilityEditor.DisplayName);
		if (abilityEditor.name != abilityEditor.DisplayName)
			AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath (abilityEditor).ToString(), abilityEditor.DisplayName);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		abilityEditor.AbilityDamageType = (AbilityDescription.DamageType)EditorGUILayout.EnumPopup("Ability Target Type", abilityEditor.AbilityDamageType);
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal();
		abilityEditor.AbilityTargetType = (AbilityDescription.TargetType)EditorGUILayout.EnumPopup("Ability Damage Type", abilityEditor.AbilityTargetType);
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal();
		abilityEditor.AbilityAbilityType = (AbilityDescription.AbilityType)EditorGUILayout.EnumPopup("Ability Damage Type", abilityEditor.AbilityAbilityType);
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField ((abilityEditor.AbilityDamageType == AbilityDescription.DamageType.Heal 
		                             || abilityEditor.AbilityDamageType == AbilityDescription.DamageType.Absorb) ? "Healing" : "Damage");
		abilityEditor.damage = EditorGUILayout.IntField (abilityEditor.damage);
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal();
		if (abilityEditor.AbilityAbilityType == AbilityDescription.AbilityType.AreaOverTime || abilityEditor.AbilityAbilityType == AbilityDescription.AbilityType.SingleTargetOverTime) {
			EditorGUILayout.LabelField ("Duration");
			abilityEditor.Duration = EditorGUILayout.IntField (abilityEditor.Duration);
		}
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Cast Range");
		abilityEditor.castRange = EditorGUILayout.IntField (abilityEditor.castRange);
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal ();
		if (abilityEditor.AbilityAbilityType == AbilityDescription.AbilityType.Area || abilityEditor.AbilityAbilityType == AbilityDescription.AbilityType.AreaOverTime) {
			EditorGUILayout.LabelField ("AOE Range");
			abilityEditor.AreaOfEffectDistance = EditorGUILayout.IntField (abilityEditor.AreaOfEffectDistance);
		}
		EditorGUILayout.EndHorizontal ();
		
		if (GUILayout.Button ("Create New Status Effect")) {
			abilityEditor.effects.Add(new StatusEffect());
		}
		if (abilityEditor.effects.Count > 0)
			ShowEffectList ();
		serializedObject.Update ();
		EditorUtility.SetDirty (abilityEditor); //Have to set dirty or it wont update
	}

	/// <summary>
	/// Used to display an array
	/// </summary>
	public void ShowEffectList () {
		for (int i = 0; i < abilityEditor.effects.Count; i++) {
			StatusEffect eff = abilityEditor.effects[i];
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Status Effect Name:", GUILayout.MaxWidth (155));
			eff.StatusEffectName = EditorGUILayout.TextField (eff.StatusEffectName);
			if (GUILayout.Button ("Delete Effect", GUILayout.MaxWidth (100))) {
				abilityEditor.effects.RemoveAt (i);
				i--;
			}
			else {
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.BeginHorizontal();
				eff.statusType = (StatusEffect.StatusType)EditorGUILayout.EnumPopup("Status Effect Type", eff.statusType);
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.BeginHorizontal();
				eff.damageDurationType = (StatusEffect.DamageDurationType)EditorGUILayout.EnumPopup("Status Effect Duration", eff.damageDurationType);
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.BeginHorizontal ();
				if (eff.damageDurationType == StatusEffect.DamageDurationType.OverTime) {
					EditorGUILayout.LabelField("Status Effect Duration (In Turns)");
					eff.effectDuration = Mathf.Clamp (EditorGUILayout.IntField (eff.effectDuration), 1, 10);
				}
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.BeginHorizontal ();
				if (eff.statusType == StatusEffect.StatusType.Damage) {
					EditorGUILayout.LabelField ("Total Damage Amount");
					eff.damage = EditorGUILayout.FloatField (eff.damage);
				}
				else if (eff.statusType == StatusEffect.StatusType.Heal) {
					EditorGUILayout.LabelField ("Total Healing Amount");
					eff.damage = EditorGUILayout.FloatField (eff.damage);
				}
				else if (eff.statusType == StatusEffect.StatusType.Absorb) {
					EditorGUILayout.LabelField ("Total Absorb Amount");
					eff.damage = EditorGUILayout.FloatField (eff.damage);
				}
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.BeginHorizontal ();
				if (eff.statusType == StatusEffect.StatusType.Slow) {
					EditorGUILayout.LabelField ("Move Speed Reduction (Percent)", GUILayout.MaxWidth (200));
					eff.slowPercent = Mathf.Clamp(EditorGUILayout.FloatField(eff.slowPercent, GUILayout.MaxWidth (50)),0,1);
					eff.slowPercent = GUILayout.HorizontalSlider(eff.slowPercent, 0f, 1f);
				}
				EditorGUILayout.EndHorizontal ();
			}
			EditorGUILayout.LabelField ("___________________________________________________________________________");
		}
	}
}
