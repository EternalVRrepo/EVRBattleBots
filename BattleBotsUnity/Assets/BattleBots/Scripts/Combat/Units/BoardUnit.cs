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
	public bool isEnfeebled {
		get { return enfeebled; }
	}
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
	protected bool hasStaticGrip;
	protected bool enfeebled;
	protected bool hasStaticShell;
	protected Hexagon staticGripStart;
	protected int currentAmountSlowed;
	
	abstract public void Spawn(Hexagon hex);
	abstract public void IssueMovement(Hexagon hex);

	/// <summary>
	/// Receives an ability hit and applies status effects
	/// </summary>
	public void ReceiveAbilityHit(AbilityDescription ability, List<AbilityModifier> modifiers = null) {

		if (modifiers != null) {
			foreach (AbilityModifier mod in modifiers) {
				if (mod.ModifierType == AbilityModifier.Modifier.RemoveStunEffect) {
					for (int i = 0; i < ability.debuffs.Count; i++) {
						DebuffEffect e = ability.debuffs [i];
						if (e.DebuffType == DebuffEffect.Debuff.Stun) {
							ability.debuffs.RemoveAt (i);
							i--;
						}
					}
				}
			}
		}

		for (int i = 0; i < ability.debuffs.Count; i++) {
			DebuffEffect effect = ability.debuffs [i];
			if (effect.RequireUnstableStatic && !hasUnstableStatic) {
				ability.debuffs.RemoveAt (i);
				i--;
			}
			else if (effect.RequireUnstableStatic && hasUnstableStatic) {
				RemoveUnstableStatic();
			}
		}

		foreach(DebuffEffect effect in ability.debuffs) {		
			if (effect.DebuffType == DebuffEffect.Debuff.Damage) {
				ApplyDamage((int)effect.Damage, modifiers);
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
			else if(effect.DebuffType == DebuffEffect.Debuff.Enfeeble) {
				ApplyEnfeeble();
			}
			else if (effect.DebuffType == DebuffEffect.Debuff.StaticGrip) {
				HandleStaticGrip(effect);
			}
		
			if (effect.EffectDurationType == StatusEffect.EffectDuration.OverTime) {
				if (debuffs.Contains (effect)) {
					debuffs.Remove (effect);
				}
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
			else if (effect.BuffType == BuffEffect.Buff.StaticShell) {
				HandleStaticShell(effect);
			}
			if (effect.EffectDurationType == StatusEffect.EffectDuration.OverTime) {
				if (buffs.Contains (effect)) 
					buffs.Remove (effect);
				buffs.Add (new BuffEffect(effect));
			}
		}
	}

	public void ReceiveStaticShellHit(BuffEffect e) {
		ApplyDamage ((int)e.amount);
		debuffs.Add (new DebuffEffect("Unstable Static", DebuffEffect.Debuff.UnstableStatic, 25, 2));
		ApplyUnstableStatic();
	}

	protected void ApplyEnfeeble() {
		enfeebled = true;
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
	protected void ApplyDamage(int amount, List<AbilityModifier> modifiers = null) {

		if (modifiers != null) {
			foreach (AbilityModifier mod in modifiers) {
				if (mod.ModifierType == AbilityModifier.Modifier.Damage)
					amount = Mathf.RoundToInt (amount*mod.ModifierAmount);
			}
		}

		AbsorbAmount -= amount; //Take damage from absorb first, its ok if it goes negative

		if (AbsorbAmount > 0) //If we still have absorb we know we arent dead and dont need to take health
			return;

		CurrentHealth += AbsorbAmount; //If we took more damage than we had absorb, add the remaining damage from absorb
		AbsorbAmount = 0; //Reset absorb amount to 0 
		if (CurrentHealth <= 0)
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

	protected void RemoveEnfeeble() {
		for (int i = 0; i < debuffs.Count; i++) {
			DebuffEffect e = debuffs [i];
			if (e.DebuffType == DebuffEffect.Debuff.Enfeeble) {
				debuffs.Remove (e);
				i--;
			}
		}
		enfeebled = false;
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
		AbilityActivator.EndTurn();
		DecrementDebuffs(); //First reduce duration on debuffs/remove timed out ones
		DecrementBuffs();

		//Check if we can remove any debuffs now
		TryRemoveStun();
		TryRemoveSilence();
		TryRemoveUnstableStatic();
		TryRemoveSlow();
		TryRemoveRoot();
		TryRemoveEnfeeble();
	}

	/// <summary>
	/// Applies the hex status effects.
	/// </summary>
	public void ApplyHexStatusEffects() {
		foreach (StatusEffect e in CurrentlyOccupiedHexagon.HexEffects) {
			if (e.EffectDurationType == StatusEffect.EffectDuration.OverTime) {
				if (e is BuffEffect) {
					BuffEffect b = e as BuffEffect;

					if (b.BuffType == BuffEffect.Buff.RemoveDebuffs) {
						debuffs.Clear ();
						RemoveStuns();
						RemoveSlows();
						RemoveRoots();
						RemoveSilences ();
						RemoveUnstableStatic();
					}
					else if (b.BuffType == BuffEffect.Buff.Absorb) {
						ApplyAbsorb((int)b.amount);
					}
					else if (b.BuffType == BuffEffect.Buff.FullHeal) {
						ApplyHeal(int.MaxValue);
					}
					else if (b.BuffType == BuffEffect.Buff.Heal) {
						ApplyHeal ((int)b.amount);
					}
					else if (b.BuffType == (BuffEffect.Buff.MovementIncrease)) {
						ApplyMovementIncrease(Mathf.RoundToInt(b.amount));
					}
					if (b.EffectDurationType == StatusEffect.EffectDuration.OverTime) {
						if (buffs.Contains (b)) 
							buffs.Remove (b);
						buffs.Add (new BuffEffect(b));
					}
				}
				else if (e is DebuffEffect) {
					DebuffEffect d = e as DebuffEffect;

					if (d.DebuffType == DebuffEffect.Debuff.Damage) {
						ApplyDamage((int)d.Damage);
					}
					else if (d.DebuffType == DebuffEffect.Debuff.Slow) {
						ApplySlow(d.SlowPercent);
					}
					else if (d.DebuffType == DebuffEffect.Debuff.Stun) {
						ApplyStun ();
					}
					else if (d.DebuffType == DebuffEffect.Debuff.Silence) {
						ApplySilence();
					}
					else if (d.DebuffType == DebuffEffect.Debuff.Root) {
						ApplyRoot();
					}
					else if (d.DebuffType == DebuffEffect.Debuff.UnstableStatic) {
						ApplyUnstableStatic();
					}
					else if (d.DebuffType == DebuffEffect.Debuff.Enfeeble) {
						ApplyEnfeeble();
					}
					
					if (d.EffectDurationType == StatusEffect.EffectDuration.OverTime) {
						if (debuffs.Contains (d))
							debuffs.Remove (d);
						debuffs.Add(new DebuffEffect(d));
					}
				}
			}
		}
	}

	/// <summary>
	/// Tries the remove enfeeble.
	/// </summary>
	protected void TryRemoveEnfeeble() {
		enfeebled = false;
		foreach (DebuffEffect e in debuffs) {
			if (e.DebuffType == DebuffEffect.Debuff.Enfeeble) {
				enfeebled = true;
				return;
			}
		}
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
			else if (effect.DebuffType == DebuffEffect.Debuff.Enfeeble) {
				ApplyEnfeeble();
			}
			else if (effect.DebuffType == DebuffEffect.Debuff.StaticGrip) {
				HandleStaticGrip(effect);
			}
		}	
	}

	/// <summary>
	/// Handles the static grip effect
	/// </summary>
	void HandleStaticGrip(DebuffEffect e) {
		if (!hasStaticGrip) {
			staticGripStart = CurrentlyOccupiedHexagon;
			hasStaticGrip = true;
		}
		ApplySlow (.5f);
		if (e.Duration == 1) { //Proccing stage
			float d = BoardManager.instance.DistanceBetweenHexagons(staticGripStart, CurrentlyOccupiedHexagon);
			ApplyDamage ((int)d*(int)e.StaticGripDamagePerHex);
			Hexagon h = BoardManager.instance.StaticGripHex(CurrentlyOccupiedHexagon, staticGripStart);
			if (h != null) 
				IssueMovement (h);
			staticGripStart = null;
			hasStaticGrip = false;
		}
	}

	void HandleStaticShell(BuffEffect e) {
		if (!hasStaticShell) {
			hasStaticShell = true;
		}

		if (e.Duration == 1) {
			hasStaticShell = false;
			AbilityActivator.ActivateStaticShell(e);
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
				RemoveEnfeeble();
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
			else if (effect.BuffType == BuffEffect.Buff.StaticShell) {
				HandleStaticShell (effect);
			}
			else if (effect.BuffType == (BuffEffect.Buff.MovementIncrease)) {
				ApplyMovementIncrease(Mathf.RoundToInt(effect.amount));
			}
		}
	}

	/// <summary>
	/// KnockBack the unit from a source a certain distance
	/// </summary>
	public void KnockBack(Hexagon source, int distance) {
		Hexagon h = BoardManager.instance.GetKnockbackHex(source, CurrentlyOccupiedHexagon, distance);
		if (h != null)
			IssueMovement (h);
	}

	/// <summary>
	/// Pull unit towards something
	/// </summary>
	public void PullIn(Hexagon source, int distance) {
		Hexagon h = BoardManager.instance.GetPullinHex(source, CurrentlyOccupiedHexagon, distance);
		if (h != null)
			IssueMovement (h);
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
