/////////////////////////////////////////////////////////////////////////////////
//
//	EnemyUnitInfo.cs
//	© EternalVR, All Rights Reserved
//
//	description:	This class holds the information for an enemy outside of combat,
//					once combat is started, it will load a list of these and create
//					the enemy units based on it
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyUnitInfo : ScriptableObject {

	public int Health;
	public GameObject UnitPrefab;
	public int MovementDistance;
	public List<AbilityDescription> ListOfAbilities = new List<AbilityDescription>();
	public CombatAIManager.AIType AIType;
}
