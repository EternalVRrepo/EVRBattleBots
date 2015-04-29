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
	public bool MovementIsDirty; //Whether this units movement has been altered and needs to be recalculated (move speed buff/remove slow/etc)
	public bool AbilityUseIsDirty; //If this unit has been altered and can now cast abilities
	public bool alive;
	public List<DebuffEffect> debuffs = new List<DebuffEffect>();
	public List<BuffEffect> buffs = new List<BuffEffect>();
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
	protected bool rooted;
	protected bool silenced;
	protected bool hasUnstableStatic;
	protected int currentAmountSlowed;
	
	abstract public void Spawn(Hexagon hex);
	abstract public void IssueMovement(Hexagon hex);

	/// <summary>
	/// Receives an ability hit and applies status effects
	/// </summary>
	public void ReceiveAbilityHit(AbilityDescription ability) {
		foreach(DebuffEffect effect in ability.debuffs) {		
			if (effect.DebuffType == DebuffEffect.Debuff.Damage) {
				ApplyDamage((int)effect.Damage);
			}
			else if (effect.DebuffType == DebuffEffect.Debuff.Slow) {
				ApplySlow(effect.SlowPercent);
			}
			else if (effect.DebuffType == DebuffEffect.Debuff.Stun) {
				ApplyStun ();
			}
			else if (effect.DebuffType == DebuffEffect.Debuff.Silence) {
				ApplySilence();
			}
			else if (effect.DebuffType == DebuffEffect.Debuff.Root) {
				ApplyRoot();
			}
			else if (effect.DebuffType == DebuffEffect.Debuff.UnstableStatic) {
				ApplyUnstableStatic();
			}
		
			if (effect.EffectDurationType == StatusEffect.EffectDuration.OverTime) {
				debuffs.Add (new DebuffEffect(effect));
			}
		}

		foreach (BuffEffect effect in ability.buffs) {
			if (effect.BuffType == BuffEffect.Buff.RemoveDebuffs) {
				debuffs.Clear ();
				RemoveStuns();
				RemoveSlows();
				RemoveRoots();
				RemoveSilences ();
				RemoveUnstableStatic();
			}
			else if (effect.BuffType == BuffEffect.Buff.Absorb) {
				ApplyAbsorb((int)effect.amount);
			}
			else if (effect.BuffType == BuffEffect.Buff.FullHeal) {
				ApplyHeal(int.MaxValue);
			}
			else if (effect.BuffType == BuffEffect.Buff.Heal) {
				ApplyHeal ((int)effect.amount);
			}
			else if (effect.BuffType == (BuffEffect.Buff.MovementIncrease)) {
				ApplyMovementIncrease(Mathf.RoundToInt(effect.amount));
			}
			if (effect.EffectDurationType == StatusEffect.EffectDuration.OverTime) {
				buffs.Add (new BuffEffect(effect));
			}
		}
	}

	/// <summary>
	/// Applies the movement speed increase.
	/// </summary>
	protected void ApplyMovementIncrease(int amount) {
		remainingMoveDistance += amount;
		MovementIsDirty = true;
	}

	/// <summary>
	/// Applies the unstable static.
	/// </summary>
	protected void ApplyUnstableStatic() {
		hasUnstableStatic = true;
	}

	/// <summary>
	/// Applies the root.
	/// </summary>
	protected void ApplyRoot() {
		rooted = true;
	}

	/// <summary>
	/// Applies the silence.
	/// </summary>
	protected void ApplySilence() {
		silenced = true;
	}

	/// <summary>
	/// Applies the damage.
	/// </summary>
	protected void ApplyDamage(int amount) {

		AbsorbAmount -= amount; //Take damage from absorb first, its ok if it goes negative

		if (AbsorbAmount > 0) //If we still have absorb we know we arent dead and dont need to take health
			return;

		CurrentHealth += AbsorbAmount; //If we took more damage than we had absorb, add the remaining damage from absorb
		AbsorbAmount = 0; //Reset absorb amount to 0 
		if (CurrentHealth < 0)
			Die();
	}

	/// <summary>
	/// Applies the heal.
	/// </summary>
	protected void ApplyHeal(int amount) {
		CurrentHealth += amount;
		if (CurrentHealth > MaxHealth)
			CurrentHealth = MaxHealth;
	}

	/// <summary>
	/// Applies the absorb.
	/// </summary>
	protected void ApplyAbsorb(int amount) {
		AbsorbAmount += amount;
	}

	/// <summary>
	/// Applies the slow.
	/// </summary>
	protected void ApplySlow(float percent) {
		int newMoveDistance = Mathf.FloorToInt (MoveDistance*(1-percent)); //3 move speed at 30% slow gives 2
		if (MoveDistance - newMoveDistance > currentAmountSlowed) { //3 - 2 = slowed by 1, if > currentSlow of 0, apply slow
			currentAmountSlowed = MoveDistance - newMoveDistance; //Current slow amount = 3 - 2 = slowed by 1
		}
		if (newMoveDistance < remainingMoveDistance) //Only apply the slow if its slower than any previously applied slow or no slow
			remainingMoveDistance = newMoveDistance;
	}

	/// <summary>
	/// Applies stun.
	/// </summary>
	protected void ApplyStun() {
		stunned = true;
	}

	/// <summary>
	/// Removes slows.
	/// </summary>
	protected void RemoveSlows() {
		for (int i = 0; i < debuffs.Count; i++) {
			DebuffEffect e = debuffs [i];
			if (e.DebuffType == DebuffEffect.Debuff.Slow) {
				debuffs.Remove (e);
				i--;
			}
		}

		remainingMoveDistance += currentAmountSlowed;
		currentAmountSlowed = 0;
		MovementIsDirty = true;
	}

	/// <summary>
	/// Removes stuns.
	/// </summary>
	protected void RemoveStuns() {
		for (int i = 0; i < debuffs.Count; i++) {
			DebuffEffect e = debuffs [i];
			if (e.DebuffType == DebuffEffect.Debuff.Stun) {
				debuffs.Remove (e);
				i--;
			}
		}
		stunned = false;
	}

	/// <summary>
	/// Removes roots.
	/// </summary>
	protected void RemoveRoots() {
		for (int i = 0; i < debuffs.Count; i++) {
			DebuffEffect e = debuffs [i];
			if (e.DebuffType == DebuffEffect.Debuff.Root) {
				debuffs.Remove (e);
				i--;
			}
		}
		rooted = false;
		MovementIsDirty = true;
	}

	/// <summary>
	/// Removes silences.
	/// </summary>
	protected void RemoveSilences() {
		for (int i = 0; i < debuffs.Count; i++) {
			DebuffEffect e = debuffs [i];
			if (e.DebuffType == DebuffEffect.Debuff.Silence) {
				debuffs.Remove (e);
				i--;
			}
		}
		silenced = false;
	}

	/// <summary>
	/// Removes the unstable static.
	/// </summary>
	protected void RemoveUnstableStatic() {
		for (int i = 0; i < debuffs.Count; i++) {
			DebuffEffect e = debuffs [i];
			if (e.DebuffType == DebuffEffect.Debuff.UnstableStatic) {
				debuffs.Remove (e);
				i--;
			}
		}
		hasUnstableStatic = false;
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
		remainingMoveDistance = MoveDistance; //Reset move distance to max
		ApplyDebuffs ();
		ApplyBuffs ();
	}

	/// <summary>
	/// End of the turn for the player, can apply any status effects that work at end of turn 
	/// </summary>
	public void EndTurn() {
		DecrementDebuffs(); //First reduce duration on debuffs/remove timed out ones
		DecrementBuffs();
		TryRemoveStun();
		TryRemoveSilence();
		TryRemoveUnstableStatic();
		TryRemoveSlow();
		TryRemoveRoot();
	}

	/// <summary>
	/// Checks if we can stop being stunned
	/// </summary>
	protected void TryRemoveStun() {
		stunned = false;
		foreach (DebuffEffect e in debuffs) {
			if (e.DebuffType == DebuffEffect.Debuff.Stun) {
				stunned = true;
				return;
			}
		}
	}

	/// <summary>
	/// Checks if we can stop being silenced
	/// </summary>
	protected void TryRemoveSilence() {
		silenced = false;
		foreach (DebuffEffect e in debuffs) {
			if (e.DebuffType == DebuffEffect.Debuff.Silence) {
				silenced = true;
				return;
			}
		}
	}

	/// <summary>
	/// Check if can we stop having unstable static
	/// </summary>
	protected void TryRemoveUnstableStatic() {
		hasUnstableStatic = false;
		foreach (DebuffEffect e in debuffs) {
			if (e.DebuffType == DebuffEffect.Debuff.UnstableStatic) {
				hasUnstableStatic = true;
				return;
			}
		}
	}

	protected void TryRemoveSlow() {
		currentAmountSlowed = 0;
		foreach(DebuffEffect e in debuffs) {
			if (e.DebuffType == DebuffEffect.Debuff.Slow) {
				ApplySlow (e.SlowPercent);
			}
		}
	}

	/// <summary>
   	/// Check if we can stop being rooted
	/// </summary>
	protected void TryRemoveRoot() {
		rooted = false;
		foreach (DebuffEffect e in debuffs) {
			if (e.DebuffType == DebuffEffect.Debuff.Slow) {
				rooted = true;
				return;
			}
		}
	}

	/// <summary>
	/// Lower or remove duration of all existing debuffs
	/// </summary>
	protected void DecrementDebuffs() {
		for (int i = 0; i < debuffs.Count; i++) { //Reduce durations of all debuffs and remove if they hit 0
			DebuffEffect effect = debuffs [i];
			effect.Duration--;
			if (effect.Duration <= 0) {
				debuffs.Remove (effect);
				i--;
			}
		}
	}

	/// <summary>
	/// Lower or remove duration of all existing buffs
	/// </summary>
	protected void DecrementBuffs() {
		for (int i = 0; i < buffs.Count; i++) { //Reduce durations of all debuffs and remove if they hit 0
			BuffEffect effect = buffs [i];
			effect.Duration--;
			if (effect.Duration <= 0) {
				buffs.Remove (effect);
				i--;
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
	/// Determines whether this instance can move.
	/// </summary>
	public bool CanMove() {
		if (!CanTakeTurn ())
			return false;

		if (rooted) 
			return false;

		return true;
	}

	/// <summary>
	/// Determines whether this instance can cast ability.
	/// </summary>
	public bool CanCastAbility() {
		if (!CanTakeTurn())
			return false;

		if (silenced)
			return false;

		return true;
	}

	/// <summary>
	/// Applies the debuffs called at the start of each turn
	/// </summary>
	void ApplyDebuffs () {
		for (int i = 0; i < debuffs.Count; i++) { //Go through and apply all debuffs
			DebuffEffect effect = debuffs [i];
			
			if (effect.DebuffType == DebuffEffect.Debuff.Damage) {
				ApplyDamage((int)effect.Damage);
			}
			else if (effect.DebuffType == DebuffEffect.Debuff.Slow) {
				ApplySlow(effect.SlowPercent);
			}
			else if (effect.DebuffType == DebuffEffect.Debuff.Stun) {
				ApplyStun ();
			}
			else if (effect.DebuffType == DebuffEffect.Debuff.Silence) {
				ApplySilence();
			}
			else if (effect.DebuffType == DebuffEffect.Debuff.Root) {
				ApplyRoot();
			}
			else if (effect.DebuffType == DebuffEffect.Debuff.UnstableStatic) {
				ApplyUnstableStatic();
			}
		}	
	}

	/// <summary>
	/// Applies the debuffs called at the start of each turn
	/// </summary>
	void ApplyBuffs () {
		foreach (BuffEffect effect in buffs) {
			if (effect.BuffType == BuffEffect.Buff.RemoveDebuffs) {
				debuffs.Clear ();
				RemoveStuns();
				RemoveSlows();
				RemoveRoots();
				RemoveSilences ();
				RemoveUnstableStatic();
			}
			else if (effect.BuffType == BuffEffect.Buff.Absorb) {
				ApplyAbsorb((int)effect.amount);
			}
			else if (effect.BuffType == BuffEffect.Buff.FullHeal) {
				ApplyHeal(int.MaxValue);
			}
			else if (effect.BuffType == BuffEffect.Buff.Heal) {
				ApplyHeal ((int)effect.amount);
			}
			else if (effect.BuffType == (BuffEffect.Buff.MovementIncrease)) {
				ApplyMovementIncrease(Mathf.RoundToInt(effect.amount));
			}
		}
	}
	
	/// <summary>
	/// Resets the slow amount to 0 or whatever the highest slow is
	/// </summary>
	protected void ResetSlowAmount() {
		currentAmountSlowed = 0;

		foreach (DebuffEffect e in debuffs) {
			if (e.DebuffType == DebuffEffect.Debuff.Slow) {
				ApplySlow (e.SlowPercent);
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
