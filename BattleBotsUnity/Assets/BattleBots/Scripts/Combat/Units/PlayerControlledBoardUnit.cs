/////////////////////////////////////////////////////////////////////////////////
//
//	PlayedControlledBoardUnit.cs
//	© EternalVR, All Rights Reserved
//
//	description:	This is a class extending BoardUnit that encompasses units that
//					are controlled by the player, so it will function and respond
//					to player input and anything else a player needs
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 
using UnityEngine;
using System.Collections;

public class PlayerControlledBoardUnit : BoardUnit
{

	public PlayerClass UnitClass;
	public enum PlayerClass
	{ 
		Warrior,
		Assassin,
		Wizard,
		Support
	}

	public override void IssueMovement (Hexagon h)
	{
		MoveToHexagon (h);
	}

	public override void Spawn (Hexagon h)
	{
		AddToHexagon (h);
	}

	/// <summary>
	/// Initialize the unit based  on a PartyUnit
	/// </summary>
	public void Initialize (PartyUnit u)
	{
		MoveDistance = u.MovementDistance;
		AbilityActivator.ListOfAbilities = u.ListOfAbilities;
		MaxHealth = u.Health;
		CurrentHealth = u.Health;
		alive = true;
		UnitClass = u.UnitClass;
	}

	/// <summary>
	/// Applies the power up.
	/// </summary>
	public void ApplyPowerUp (PowerUp p)
	{
		if (!alive)
			return;

		if (p.PowerUpEffect == BuffEffect.Buff.RemoveDebuffs) {
			debuffs.Clear ();
			RemoveStuns ();
			RemoveSlows ();
			RemoveRoots ();
			RemoveSilences ();
			RemoveUnstableStatic ();
		} else if (p.PowerUpEffect == BuffEffect.Buff.MovementIncrease) {
			ApplyMovementIncrease ((int)p.PowerUpBonusValue);
		} else if (p.PowerUpEffect == BuffEffect.Buff.FullHeal) {
			ApplyHeal (int.MaxValue);
		} else if (p.PowerUpEffect == BuffEffect.Buff.Absorb) {
			ApplyAbsorb ((int)p.PowerUpBonusValue);
		}

		if (p.PowerUpBonusDuration > 0) {
			buffs.Add (new BuffEffect (p));
		}
	}
}
