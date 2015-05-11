/////////////////////////////////////////////////////////////////////////////////
//
//	AbilityDescription.cs
//	© EternalVR, All Rights Reserved
//
//	description:	This class encapsulates what an ability is, what it does, how it
//					targets, etc
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AbilityDescription : ScriptableObject {

	public string DisplayName = ""; //Name of this ability 
	public string TooltipText = ""; 
	public Sprite AbilityIcon;
	public Sprite newTexture;
	public GameObject EffectPrefab;
	public GameObject newPrefab;
	public TargetType AbilityTargetType;
	public enum TargetType {
		TargetSelf,
		TargetUnit,
		TargetEnemy,
		TargetAlly,
		TargetHexagon,
		CustomTemplate
	}
	public TemplateManager.Target TemplateType;
	public TemplateManager.TargetTemplate Template;
	public int TemplateSize;
	public bool FriendlyFireEnabled;
	public bool SelfFireEnabled;
	public bool RequireSourceHexagon;
	public Hexagon SourceHexagon;
	public int Cooldown;
	public int currentCooldown;
	public int castRange; //How far from the player this ability can be cast
	public int AreaOfEffectDistance; //How far from the target is affected by this ability
	public int duration;
	public int HexDuration; //Duration it stays on a hex
	public List<DebuffEffect> debuffs = new List<DebuffEffect>();
	public List<BuffEffect> buffs = new List<BuffEffect>();
	public BoardUnit sourceUnit;
	public Hexagon targetHexagon;

	public void Initialize(AbilityDescription a) {
		DisplayName = a.DisplayName;
		TooltipText = a.TooltipText;
		AbilityIcon = a.AbilityIcon;
		newTexture = a.newTexture;
		EffectPrefab = a.EffectPrefab;
		newPrefab = a.newPrefab;
		AbilityTargetType = a.AbilityTargetType;
		TemplateType = a.TemplateType;
		Template = a.Template;
		TemplateSize = a.TemplateSize;
		FriendlyFireEnabled = a.FriendlyFireEnabled;
		SelfFireEnabled = a.SelfFireEnabled;
		RequireSourceHexagon = a.RequireSourceHexagon;
		SourceHexagon = a.SourceHexagon;
		Cooldown = a.Cooldown;
		currentCooldown = a.currentCooldown;
		castRange = a.castRange;
		AreaOfEffectDistance = a.AreaOfEffectDistance;
		duration = a.duration;
		HexDuration = a.HexDuration;

		foreach (DebuffEffect d in a.debuffs) {
			DebuffEffect de = new DebuffEffect(d);
			debuffs.Add(de);
		}
		
		foreach (BuffEffect b in a.buffs) {
			BuffEffect be = new BuffEffect(b);
			buffs.Add(be);
		}
	}

	public void UpdateIcon() {
		if (newTexture == null || newTexture == AbilityIcon)
			return;
		AbilityIcon = newTexture;
	}

	public void UpdatePrefab() {
		if (newPrefab == null || newPrefab == EffectPrefab)
			return;
		EffectPrefab = newPrefab;
	}
}
