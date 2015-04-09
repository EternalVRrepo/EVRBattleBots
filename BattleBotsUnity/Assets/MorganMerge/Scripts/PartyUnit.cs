/////////////////////////////////////////////////////////////////////////////////
//
//	PartyUnit.cs
//	© EternalVR, All Rights Reserved
//
//	description:	This class embodies a party member the player is using, the
//					player will have 1-4 of these at any point, each one holds
//					the stats, ability selection, equipped gear, talents, etc of
//					that unit.
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PartyUnit : ScriptableObject {

	public GameObject UnitPrefab;
	public int MovementDistance;
	public List<AbilityDescription> ListOfAbilities = new List<AbilityDescription>();
}
