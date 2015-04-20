/////////////////////////////////////////////////////////////////////////////////
//
//	BoardUnit.cs
//	© EternalVR, All Rights Reserved
//
//	description:	This is the base class for any unit that will be placed on 
//					the hexagon board
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class BoardUnit : MonoBehaviour {

	public int MaxHealth;
	public int CurrentHealth;
	public int AbsorbAmount;
	public int MoveDistance; //How far we can move in 1 turn
	public int remainingMoveDistance; //How far left we can move this turn
	public bool alive;
	public List<StatusEffect> debuffs = new List<StatusEffect>();
	public Hexagon CurrentlyOccupiedHexagon {
		get { return currentlyOccupiedHexagon; }
		set { currentlyOccupiedHexagon = value; }
	}

	public int xPos {
		get {
			return CurrentlyOccupiedHexagon.HexRow;
		}
	}

	public int yPos {
		get {
			return CurrentlyOccupiedHexagon.HexColumn;
		}
	}
	private AbilityActivator abilityActivator;
	public AbilityActivator AbilityActivator {
		get {
			if (abilityActivator == null) 
				abilityActivator = GetComponent<AbilityActivator>();
			return abilityActivator;
		}
	}

	[SerializeField]
	private Hexagon currentlyOccupiedHexagon;
	protected bool stunned;
	
	abstract public void Spawn(Hexagon hex);
	abstract public void IssueMovement(Hexagon hex);

	/// <summary>
	/// Receives an ability hit and applies status effects
	/// </summary>
	public void ReceiveAbilityHit(AbilityHit effects) {
		foreach(StatusEffect effect in effects.effects) {
			if (effect.damageDurationType == StatusEffect.DamageDurationType.Instant) {
				if (effect.statusType == StatusEffect.StatusType.Damage) {
					ApplyDamage((int)effect.damage);
				}
				else if (effect.statusType == StatusEffect.StatusType.Heal) {
					ApplyHeal((int)effect.damage);
				}
				else if (effect.statusType == StatusEffect.StatusType.Absorb) {
					ApplyAbsorb((int)effect.damage);
				}
			}
			else if (effect.damageDurationType == StatusEffect.DamageDurationType.OverTime) {
				debuffs.Add (new StatusEffect(effect));
			}
		}
	}

	protected void ApplyDamage(int amount) {

		AbsorbAmount -= amount; //Take damage from absorb first, its ok if it goes negative

		if (AbsorbAmount > 0) //If we still have absorb we know we arent dead and dont need to take health
			return;

		CurrentHealth += AbsorbAmount; //If we took more damage than we had absorb, add the remaining damage from absorb
		if (CurrentHealth < 0)
			Die();
	}

	protected void ApplyHeal(int amount) {
		CurrentHealth += amount;
		if (CurrentHealth > MaxHealth)
			CurrentHealth = MaxHealth;
	}

	protected void ApplyAbsorb(int amount) {
		AbsorbAmount += amount;
	}

	/// <summary>
	/// Applies the slow.
	/// </summary>
	protected void ApplySlow(float percent) {
		int newMoveDistance = Mathf.CeilToInt (MoveDistance*percent);
		if (newMoveDistance < remainingMoveDistance) //Only apply the slow if its lower than any previously applied slow or no slow
			remainingMoveDistance = newMoveDistance;
	}

	protected void ApplyStun() {
		stunned = true;
	}

	/// <summary>
	/// This unit dies
	/// </summary>
	protected void Die() {
		alive = false;
		CombatManager.instance.KillUnit(this);
		currentlyOccupiedHexagon.RemoveUnit (this);
		gameObject.SetActive (false);
	}

	/// <summary>
	/// Initialize unit for start turn, reset movement and things like that
	/// </summary>
	public void StartTurn() {
		remainingMoveDistance = MoveDistance;

		ApplyDebuffs ();
	}

	/// <summary>
	/// End of the turn for the player, can apply any status effects that work at end of turn 
	/// </summary>
	public void EndTurn() {
		if (stunned) {
			stunned = false;
			foreach( StatusEffect debuff in debuffs) {
				if (debuff.statusType == StatusEffect.StatusType.Stun) {
					stunned = true;
				}
			}
		}
	}

	/// <summary>
	/// Determines whether this instance can take turn.
	/// </summary>
	public bool CanTakeTurn() {
		if (stunned)
			return false;

		if (!alive)
			return false;

		return true;
	}

	/// <summary>
	/// Applies the debuffs called at the start of each turn
	/// </summary>
	void ApplyDebuffs () {
		for (int i = 0; i < debuffs.Count; i++) { //Go through and apply all debuffs
			StatusEffect effect = debuffs [i];
			
			if (effect.statusType == StatusEffect.StatusType.Damage) {
				ApplyDamage ((int)effect.damage);
			}
			else if (effect.statusType == StatusEffect.StatusType.Heal) {
				ApplyHeal ((int)effect.damage);
			}
			else if (effect.statusType == StatusEffect.StatusType.Absorb) {
				ApplyAbsorb ((int)effect.damage);
			}
			else if (effect.statusType == StatusEffect.StatusType.Slow) {
				ApplySlow ((float)effect.slowPercent);
			}
			else if (effect.statusType == StatusEffect.StatusType.Stun) {
				ApplyStun ();
			}
		}	

		for (int i = 0; i < debuffs.Count; i++) { //Reduce durations of all debuffs and remove if they hit 0
			StatusEffect effect = debuffs [i];
			effect.effectDuration--;
			if (effect.effectDuration == 0) {
				debuffs.Remove (effect);
				i--;
			}
		}
	}

	/// <summary>
	/// Moves this Unit from the current hexagon to a new hexagon
	/// </summary>
	protected void MoveToHexagon(Hexagon hex) {
		if (RemoveFromHexagon ())
			AddToHexagon (hex);
	}

	/// <summary>
	/// Removes this Unit from the current hexagon 
	/// </summary>
	protected bool RemoveFromHexagon() {
		CurrentlyOccupiedHexagon.RemoveUnit(this);
		CurrentlyOccupiedHexagon = null;
		return true;
	}

	/// <summary>
	/// Places this unit on the new hexagon
	/// </summary>
	protected void AddToHexagon(Hexagon hex) {
		hex.AddUnit(this);
		CurrentlyOccupiedHexagon = hex;
		transform.position = hex.UnitAnchorPoint;
	}
}
