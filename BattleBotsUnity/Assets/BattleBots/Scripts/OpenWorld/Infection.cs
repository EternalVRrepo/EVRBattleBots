/////////////////////////////////////////////////////////////////////////////////
//
//	Infection.cs
//	© EternalVR, All Rights Reserved
//
//	description:	Gives an object Infection which allows the player to interact with it
//					and try to stop the infection
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Infection : MonoBehaviour
{
	public bool collisionTriggersBattle = true;

	public List<EnemyUnitInfo> enemies = new List<EnemyUnitInfo> ();

	/// <summary>
	/// Interact with this instance, intiate combat
	/// </summary>
	public void Interact ()
	{
		GameManager.instance.StartCombat (enemies);
	}
}
