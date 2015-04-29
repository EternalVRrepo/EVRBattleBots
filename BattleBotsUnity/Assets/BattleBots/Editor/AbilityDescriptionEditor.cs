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

		EditorGUILayout.LabelField ("Tooltip");
		abilityEditor.TooltipText = EditorGUILayout.TextArea(abilityEditor.TooltipText);

//		EditorGUILayout.BeginHorizontal();
//		abilityEditor.AbilityDamageType = (AbilityDescription.DamageType)EditorGUILayout.EnumPopup("Ability Target Type", abilityEditor.AbilityDamageType);
//		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal();
		abilityEditor.AbilityTargetType = (AbilityDescription.TargetType)EditorGUILayout.EnumPopup("Ability Target Type", abilityEditor.AbilityTargetType);
		EditorGUILayout.EndHorizontal ();

		if (abilityEditor.AbilityTargetType == AbilityDescription.TargetType.CustomTemplate) {
			EditorGUILayout.BeginHorizontal();
			abilityEditor.TemplateType = (TemplateManager.Target)EditorGUILayout.EnumPopup ("Template Type", abilityEditor.TemplateType);
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.BeginHorizontal();
			abilityEditor.Template = (TemplateManager.TargetTemplate)EditorGUILayout.EnumPopup ("Template", abilityEditor.Template);
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField ("TemplateLength", GUILayout.MaxWidth (100));
			abilityEditor.TemplateLength = EditorGUILayout.IntField (abilityEditor.TemplateLength);
			EditorGUILayout.LabelField ("TemplateWidth", GUILayout.MaxWidth (100));
			abilityEditor.TemplateWidth = EditorGUILayout.IntField (abilityEditor.TemplateWidth);
			EditorGUILayout.EndHorizontal ();
		}

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField ("Friendly Fire Enabled?");
		abilityEditor.FriendlyFireEnabled = EditorGUILayout.Toggle(abilityEditor.FriendlyFireEnabled);
		EditorGUILayout.EndHorizontal ();
		
		EditorGUILayout.BeginHorizontal();
//		abilityEditor.AbilityAbilityType = (AbilityDescription.AbilityType)EditorGUILayout.EnumPopup("Ability Ability Type", abilityEditor.AbilityAbilityType);
		EditorGUILayout.EndHorizontal ();

//		EditorGUILayout.BeginHorizontal();
//		EditorGUILayout.LabelField ((abilityEditor.AbilityDamageType == AbilityDescription.DamageType.Heal 
//		                             || abilityEditor.AbilityDamageType == AbilityDescription.DamageType.Absorb) ? "Healing" : "Damage");
//		abilityEditor.damage = EditorGUILayout.IntField (abilityEditor.damage);
//		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal();
//		if (abilityEditor.AbilityAbilityType == AbilityDescription.AbilityType.AreaOverTime) {
//			EditorGUILayout.LabelField ("Duration");
//			abilityEditor.Duration = EditorGUILayout.IntField (abilityEditor.Duration);
//		}
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Cast Range");
		abilityEditor.castRange = EditorGUILayout.IntField (abilityEditor.castRange);
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal ();
//		if (abilityEditor.AbilityAbilityType == AbilityDescription.AbilityType.Area || abilityEditor.AbilityAbilityType == AbilityDescription.AbilityType.AreaOverTime) {
//			EditorGUILayout.LabelField ("AOE Range");
//			abilityEditor.AreaOfEffectDistance = EditorGUILayout.IntField (abilityEditor.AreaOfEffectDistance);
//		}
		EditorGUILayout.EndHorizontal ();
		
		if (GUILayout.Button ("Create New Debuff")) {
			abilityEditor.debuffs.Add(new DebuffEffect());
		}
		if (abilityEditor.debuffs.Count > 0)
			ShowDebuffList ();
		if (GUILayout.Button ("Create New Buff")) {
			abilityEditor.buffs.Add(new BuffEffect());
		}
		if (abilityEditor.buffs.Count > 0)
			ShowBuffList ();
		serializedObject.Update ();
		EditorUtility.SetDirty (abilityEditor); //Have to set dirty or it wont update
	}

	/// <summary>
	/// Shows the buff list.
	/// </summary>
	public void ShowBuffList() {
		for (int i = 0; i < abilityEditor.buffs.Count; i++) {
			BuffEffect eff = abilityEditor.buffs[i];
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField("Buff Name:", GUILayout.MaxWidth (155));
			eff.EffectName = EditorGUILayout.TextField (eff.EffectName);
			if (GUILayout.Button("Delete Buff", GUILayout.MaxWidth(100))) {
				abilityEditor.buffs.RemoveAt (i);
				i--;
			}
			else {
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.BeginHorizontal ();
				eff.BuffType = (BuffEffect.Buff)EditorGUILayout.EnumPopup("Buff Type", eff.BuffType);
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.BeginHorizontal ();
				eff.EffectDurationType = (StatusEffect.EffectDuration)EditorGUILayout.EnumPopup ("Buff Duration Type", eff.EffectDurationType);
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.BeginHorizontal ();
				if (eff.EffectDurationType == StatusEffect.EffectDuration.OverTime) {
					EditorGUILayout.LabelField("Buff Duration (In Turns)");
					eff.Duration = Mathf.Clamp (EditorGUILayout.IntField (eff.Duration), 1, 10);
				}
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.BeginHorizontal ();
				if (eff.BuffType == BuffEffect.Buff.Heal) {
					EditorGUILayout.LabelField ("Total Heal Amount");
					eff.amount = (float)EditorGUILayout.IntField ((int)eff.amount);
				}
				else if (eff.BuffType == BuffEffect.Buff.Absorb) {
					EditorGUILayout.LabelField ("Total Absorb Amount");
					eff.amount = (float)EditorGUILayout.IntField ((int)eff.amount);
				}
				else if (eff.BuffType == BuffEffect.Buff.MovementIncrease) {
					EditorGUILayout.LabelField ("Total MS Increase Amount");
					eff.amount = (float)EditorGUILayout.IntField ((int)eff.amount);
				}
				EditorGUILayout.EndHorizontal ();
			}
			EditorGUILayout.LabelField ("___________________________________________________________________________");
		}
	}

	/// <summary>
	/// Used to display an array
	/// </summary>
	public void ShowDebuffList () {
		for (int i = 0; i < abilityEditor.debuffs.Count; i++) {
			DebuffEffect eff = abilityEditor.debuffs[i];
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Debuff Name:", GUILayout.MaxWidth (155));
			eff.EffectName = EditorGUILayout.TextField (eff.EffectName);
			if (GUILayout.Button ("Delete Debuff", GUILayout.MaxWidth (100))) {
				abilityEditor.debuffs.RemoveAt (i);
				i--;
			}
			else {
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.BeginHorizontal();
				eff.DebuffType = (DebuffEffect.Debuff)EditorGUILayout.EnumPopup("Debuff Type", eff.DebuffType);
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.BeginHorizontal();
				eff.EffectDurationType = (StatusEffect.EffectDuration)EditorGUILayout.EnumPopup("Debuff Duration Type", eff.EffectDurationType);
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.BeginHorizontal ();
				if (eff.EffectDurationType == StatusEffect.EffectDuration.OverTime) {
					EditorGUILayout.LabelField("Debuff Duration (In Turns)");
					eff.Duration = Mathf.Clamp (EditorGUILayout.IntField (eff.Duration), 1, 10);
				}
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.BeginHorizontal ();
				if (eff.DebuffType == DebuffEffect.Debuff.Damage) {
					EditorGUILayout.LabelField ("Total Damage Amount");
					eff.Damage = EditorGUILayout.FloatField (eff.Damage);
				}
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.BeginHorizontal ();
				if (eff.DebuffType == DebuffEffect.Debuff.Slow) {
					EditorGUILayout.LabelField ("Move Speed Reduction (Percent)", GUILayout.MaxWidth (200));
					eff.SlowPercent = Mathf.Clamp(EditorGUILayout.FloatField(eff.SlowPercent, GUILayout.MaxWidth (50)),0,1);
					eff.SlowPercent = GUILayout.HorizontalSlider(eff.SlowPercent, 0f, 1f);
				}
				EditorGUILayout.EndHorizontal ();
			}
			EditorGUILayout.LabelField ("___________________________________________________________________________");
		}
	}
}
