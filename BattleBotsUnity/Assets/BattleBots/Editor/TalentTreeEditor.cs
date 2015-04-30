/////////////////////////////////////////////////////////////////////////////////
//
//	TalentTreeEditor.cs
//	© EternalVR, All Rights Reserved
//
//	description:	Custom editor to create/change talent tree
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(TalentTree))]
public class TalentTreeEditor : Editor
{

	TalentTree tree;
	List<bool> showTalent = new List<bool> ();

	public override void OnInspectorGUI ()
	{
		serializedObject.Update ();
		tree = (TalentTree)target;

		if (string.Equals (tree.name, PlayerControlledBoardUnit.PlayerClass.Warrior.ToString () + "Tree"))
			EditorGUILayout.LabelField (PlayerControlledBoardUnit.PlayerClass.Warrior.ToString () + "Tree");
		else if (string.Equals (tree.name, PlayerControlledBoardUnit.PlayerClass.Wizard.ToString () + "Tree"))
			EditorGUILayout.LabelField (PlayerControlledBoardUnit.PlayerClass.Wizard.ToString () + "Tree");
		else if (string.Equals (tree.name, PlayerControlledBoardUnit.PlayerClass.Support.ToString () + "Tree"))
			EditorGUILayout.LabelField (PlayerControlledBoardUnit.PlayerClass.Support.ToString () + "Tree");
		else
			EditorGUILayout.LabelField ("Name the file \"ClassName\" (Warrior/Wizard/Support) + \"Tree\"");

		for (int i = 0; i < 5; i++) {
			while (i > showTalent.Count-1) {
				showTalent.Add (false);
			}
			showTalent [i] = EditorGUILayout.Foldout (showTalent [i], "  Row: " + i);
			if (showTalent [i])
				ShowRowTalents (i);
		}

		serializedObject.Update ();
		EditorUtility.SetDirty (tree); //Have to set dirty or it wont update
	}

	void ShowRowTalents (int row)
	{
		EditorGUI.indentLevel++;
		while (tree.Tree.Count < row*3+3) { //auto create more talents if they havent been yet
			tree.Tree.Add (new Talent ());
		}

		for (int i=0; i<3; i++) {
			Talent t = tree.Tree [row * 3 + i];

			EditorGUILayout.BeginHorizontal ();
			t.TalentName = EditorGUILayout.TextField ("Talent Name", t.TalentName, GUILayout.MinWidth (100));
			EditorGUILayout.EndHorizontal ();

			t.TalentDescription = EditorGUILayout.TextField ("Talent Description", t.TalentDescription);
			t.ThisTalentType = (Talent.TalentType)EditorGUILayout.EnumPopup ("Talent Type", t.ThisTalentType);
			if (t.ThisTalentType == Talent.TalentType.StatBoost) {
				t.StatType = (Talent.Stat)EditorGUILayout.EnumPopup ("Stat Type", t.StatType);
				if (t.StatType.ToString ().Contains ("Percent"))
					t.StatBoostAmount = EditorGUILayout.Slider ("Percent Increase", t.StatBoostAmount, 0, 1);
				else
					t.StatBoostAmount = EditorGUILayout.FloatField ("Stat Boost Amount", Mathf.RoundToInt (t.StatBoostAmount));
			} else if (t.ThisTalentType == Talent.TalentType.AbilityBoost) {
				t.AbilityModified = EditorGUILayout.TextField ("Ability Name", CheckAbilityExists (t.AbilityModified));
				if (t.AbilityModified.EndsWith (" Not Found"))
					EditorGUILayout.LabelField ("Ability Not Found, Please Use an Existing Ability Name!");
				t.AbilityModType = (Talent.AbilityModification)EditorGUILayout.EnumPopup ("Stat Type", t.AbilityModType);
				if (t.AbilityModType == Talent.AbilityModification.CooldownReduction)
					t.AbilityModificationAmount = EditorGUILayout.IntField ("CDR (Turns)", t.AbilityModificationAmount);
				else if (t.AbilityModType == Talent.AbilityModification.Damage)
					t.AbilityModificationAmount = EditorGUILayout.IntField ("Total Damage Increase", t.AbilityModificationAmount);
				else if (t.AbilityModType == Talent.AbilityModification.Range)
					t.AbilityModificationAmount = EditorGUILayout.IntField ("Total Range Increase", t.AbilityModificationAmount);
			} else if (t.ThisTalentType == Talent.TalentType.Special) {
				t.AbilityModified = EditorGUILayout.TextField ("Ability Name", CheckAbilityExists (t.AbilityModified));
				if (t.AbilityModified.EndsWith (" Not Found"))
					EditorGUILayout.LabelField ("Ability Not Found, Please Use an Existing Ability Name!");
				t.SpecialAbilityModIdentifer = EditorGUILayout.IntField ("Ability Special Index", t.SpecialAbilityModIdentifer);
			}
			EditorGUILayout.Space ();
			EditorGUILayout.Separator ();
			EditorGUILayout.Separator ();
			EditorGUILayout.Separator ();
			EditorGUILayout.Separator ();

		}

		EditorGUI.indentLevel--;
	}

	/// <summary>
	/// Checks the ability exists in the AssetDatabase
	/// </summary>
	string CheckAbilityExists (string name)
	{
		AbilityDescription _exists = (AbilityDescription)AssetDatabase.
			LoadAssetAtPath ("Assets/BattleBots/Resources/Abilities/" + name + ".asset", typeof(AbilityDescription));
		if (_exists != null)
			return _exists.name;
		else {
			if (name.Length > 0 && !name.EndsWith (" Not Found")) {
				return name + " Not Found";
			} else
				return name;
		}

	}
}


