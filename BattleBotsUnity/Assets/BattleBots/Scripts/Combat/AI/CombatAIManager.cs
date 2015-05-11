/////////////////////////////////////////////////////////////////////////////////
//
//	CombatAIManager.cs
//	© EternalVR, All Rights Reserved
//
//	description:	Handles the AIs turn during combat, started and stopped by 
//					combat manager
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatAIManager : MonoBehaviour {

	public bool TurnInProgress;
	public int enemyIndex;
	NonPlayerControlledBoardUnit currentUnit;
	List<NonPlayerControlledBoardUnit> enemies {
		get { return combatManager.CurrentEnemies; }
	}
	List<PlayerControlledBoardUnit> currentParty {
		get { return combatManager.CurrentParty; }
	}
	public CombatManager combatManager;
	public enum AIType {
		Melee,
		Ranged,
		Support
	}

	/// <summary>
	/// Starts the enemy turn.
	/// </summary>
	public void StartEnemyTurn() {
		TurnInProgress = true;
		enemyIndex = 0;
		HandleNextUnit();
	}

	/// <summary>
	/// Handles the next unit.
	/// </summary>
	protected void HandleNextUnit() {
		if (enemyIndex == enemies.Count) { //If we reach the last unit end the turn
			TurnInProgress = false;
			return;
		}

		currentUnit = enemies[enemyIndex];
		currentUnit.animator.SetBool("Idle", true);
		BoardManager.instance.StartTurn();
		currentUnit.StartTurn();
		enemyIndex++;

		if (!currentUnit.CanTakeTurn()) { //If this unit can take its turn (no stuns)
			HandleNextUnit();
			return;
		}
		StartCoroutine ("MoveUnit");
	}

	/// <summary>
	/// Decides where to move a unit
	/// </summary>
	IEnumerator MoveUnit() {
		yield return new WaitForSeconds(.5f);

		if (currentUnit.CanMove()) {
			List<Hexagon> path = GetBestPath();
			if (path != null) {
				int i = 0;
				Hexagon curr = null;
				currentUnit.animator.SetBool("Walking", true);
				while (currentUnit.remainingMoveDistance >= 0 && i < path.Count-1) { //Count-1 because the path leads ontop of another unit, so stop 1 short
					curr = path[i];
					currentUnit.IssueMovement (curr);
					currentUnit.transform.LookAt(new Vector3(curr.transform.position.x, currentUnit.transform.position.y, curr.transform.position.z));
					while (currentUnit.transform.position != curr.transform.position) {
						currentUnit.transform.position = Vector3.MoveTowards(currentUnit.transform.position, curr.transform.position, 1.35f*Time.deltaTime);
						yield return null;
					}
					currentUnit.remainingMoveDistance--;
					i++;
				}
				currentUnit.animator.SetBool("Walking", false);
				yield return new WaitForSeconds(.5f);
			}
		}
		StartCoroutine ("SelectAbility");
		yield return null;
	}

	/// <summary>
	/// Decides what ability to use
	/// </summary>
	AbilityDescription AbilitySelected = null;
	PlayerControlledBoardUnit AbilityTarget = null;
	IEnumerator SelectAbility() {
		int potentialDamage = 0;
		AbilitySelected = null;
		AbilityTarget = null;
		PlayerControlledBoardUnit newTarget = null;

		foreach (AbilityDescription ability in currentUnit.AbilityActivator.ListOfAbilities) { //Find the most damage we can do
			if (ability.AbilityTargetType == AbilityDescription.TargetType.TargetEnemy) {
				int newPotentialDamage = CheckPotentialDamage(ability, ref newTarget);
				if (newPotentialDamage > potentialDamage) {
					AbilityTarget = newTarget;
					AbilitySelected = ability;
				}
			}
		}

		if (AbilitySelected == null || AbilityTarget == null) { //No ability to use
			EndTurn ();
		}
		else StartCoroutine ("TargetAbility");
		yield return null;
	}

	/// <summary>
	/// Check how much damage an ability can do
	/// </summary>
	public int CheckPotentialDamage(AbilityDescription ability, ref PlayerControlledBoardUnit a) {
		List<PlayerControlledBoardUnit> units = BoardManager.instance.GetEnemiesInRange(currentUnit.CurrentlyOccupiedHexagon, 
		                                                                                ability.castRange, ability.AbilityTargetType);

		if (units.Count == 0)
			return 0;

		PlayerControlledBoardUnit target = units[0];
		if (units.Count > 1) {
			target = FindBestTarget(ability, units);
		}
		a = target;

		StartCoroutine("LookAtTarget", target.gameObject);

		return 1;
	}

	IEnumerator LookAtTarget(GameObject target) {
		Vector3 dir = (target.transform.position - currentUnit.transform.position).normalized;
		dir.y = currentUnit.transform.position.y;
		Quaternion rot = Quaternion.LookRotation(dir);

		while (currentUnit.transform.rotation != rot) {
			currentUnit.transform.rotation = Quaternion.Slerp(currentUnit.transform.rotation, rot, Time.deltaTime * 2f);
			yield return null;
		}
	}

	/// <summary>
	/// Finds the best target for an ability given a number of targets
	/// </summary>
	public PlayerControlledBoardUnit FindBestTarget(AbilityDescription ability, List<PlayerControlledBoardUnit> targets) {
		PlayerControlledBoardUnit currTarget = targets[0];

		float potentialDamage = 0;
		foreach (DebuffEffect debuff in ability.debuffs) {
			if (debuff.DebuffType == DebuffEffect.Debuff.Damage) {
				potentialDamage += debuff.Damage;
			}
		}

		foreach (PlayerControlledBoardUnit target in targets) {
			if (target.CurrentHealth < potentialDamage) //If we have lethal damage, its a no brainer
				return target;

			if (target.CurrentHealth/target.MaxHealth //If we find someone with lower health go for them
			    < currTarget.CurrentHealth/currTarget.MaxHealth)
				currTarget = target;

			if (currTarget.UnitClass == PlayerControlledBoardUnit.PlayerClass.Warrior) {
				if (target.UnitClass == PlayerControlledBoardUnit.PlayerClass.Wizard 
				    || target.UnitClass == PlayerControlledBoardUnit.PlayerClass.Support)
					currTarget = target;
			}
		}
		return currTarget;
	}

	/// <summary>
	/// Decides what to target with the ability
	/// </summary>
	IEnumerator TargetAbility() {
		currentUnit.animator.SetTrigger(currentUnit.Name + AbilitySelected.DisplayName);
		
		if (AbilitySelected != null) { //If we have an ability to use
			AbilityTarget.ReceiveAbilityHit(AbilitySelected);
//			Debug.Log ("Hit " + AbilityTarget.CurrentlyOccupiedHexagon + " with " + AbilitySelected.name);
		}

		EndTurn ();
		yield return null;
	}

	void EndTurn () {
		currentUnit.EndTurn ();
		BoardManager.instance.EndTurn ();
		HandleNextUnit ();
	}

	/// <summary>
	/// Gets the best movement hexagon.
	/// </summary>
	protected List<Hexagon> GetBestPath() {
		if (currentUnit.AIType == AIType.Melee) {
			return EvaluateMeleeMovement();
		}
		else if (currentUnit.AIType == AIType.Ranged) {
			return EvaluateRangedMovement();
		}
		else if (currentUnit.AIType == AIType.Support) {
			return EvaluateSupportMovement();
		}
		else {
			Debug.LogError("No suitable enemy found for AI");
			return null;
		}
	}

	/// <summary>
	/// Evaluates the movement value for a melee based unit
	/// </summary>
	protected List<Hexagon> EvaluateMeleeMovement() {
		Hexagon target = FindMeleeTarget(); //Find a target we want to get to
		List<Hexagon> path = BoardManager.instance.GetPath(currentUnit.CurrentlyOccupiedHexagon, target, target);


		return path;
	}

	/// <summary>
	/// All conditions for finding the optimal target to attack go here through modification of the 
	/// newPlayerPriorityMod, based on statis numbers assigned to each class. I.E. we want to target a ranged or 
	/// support character, but ultimately if someone is low health already next to us, it might be a better target
	/// </summary>
	protected Hexagon FindMeleeTarget() {
		float playerPriority = int.MinValue;
		float newPlayerPriority = playerPriority;
//		float playerDistance = int.MinValue;
		Hexagon targetHex = null;
		foreach (PlayerControlledBoardUnit pc in currentParty) {
			float newPlayerPriorityMod = 1;
			newPlayerPriority = int.MinValue;

			if (pc.UnitClass == PlayerControlledBoardUnit.PlayerClass.Warrior) {
				newPlayerPriority = 10;
			}
			else if (pc.UnitClass == PlayerControlledBoardUnit.PlayerClass.Wizard) {
				newPlayerPriority = 20;
			}
			else if (pc.UnitClass == PlayerControlledBoardUnit.PlayerClass.Support) {
				newPlayerPriority = 25;
			}

			if (pc.CurrentHealth < pc.MaxHealth*2/3)
				newPlayerPriorityMod += .21f;
			else if (pc.CurrentHealth < pc.MaxHealth/2)
				newPlayerPriorityMod += .6f;
			else if (pc.CurrentHealth < pc.MaxHealth/3)
				newPlayerPriorityMod += .9f;

			float d = BoardManager.instance.DistanceBetweenHexagons(pc.CurrentlyOccupiedHexagon, currentUnit.CurrentlyOccupiedHexagon);
			if (d <= currentUnit.remainingMoveDistance) {
				newPlayerPriorityMod += .8f;
			}

			newPlayerPriority *= newPlayerPriorityMod;
			if (newPlayerPriority > playerPriority) { 
				playerPriority = newPlayerPriority;
//				playerDistance = d;
				targetHex = pc.CurrentlyOccupiedHexagon;
			}
//			Debug.Log (newPlayerPriority + " " + pc.UnitClass);
		}

		return targetHex;
	}

	/// <summary>
	/// Evaluates the movement value for a ranged based unit
	/// </summary>
	protected List<Hexagon> EvaluateRangedMovement() {

		return null;
	}

	/// <summary>
	/// Evaluates the movement value for a support based unit
	/// </summary>
	protected List<Hexagon> EvaluateSupportMovement() {

		return null;
	}
}
