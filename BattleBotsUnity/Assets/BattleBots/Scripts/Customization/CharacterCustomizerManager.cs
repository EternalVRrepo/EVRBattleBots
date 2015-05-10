using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterCustomizerManager : MonoBehaviour
{
	#region Variables
	public bool isDebug;
	public string tooltipTextName;
	public string tooltipTextDescription;
	public bool displayTooltip;
	public bool init;

	public PlatformManager currentlySelectedCharacter;
	public PlatformManager lastSelectedCharacter;
	public List<PartyUnit> currentParty;
	public GameObject PlatformPrefab;
	public Transform characterTray;

	protected List<PlatformManager> Platforms = new List<PlatformManager> ();

	//UX
	public float selectionSpeed = 1;
	public float platformOffset;
	#endregion

	//============================================================================
	// Initilization
	//============================================================================
	
	/// Initialize the specified party for the customization manager
	public void Initialize (List<PartyUnit> party)
	{
		isDebug = false;
		init = true;
		currentParty = party;
		GenerateCustomizationPlatforms ();
		SetFirstSelected ();
	}

	void Start ()
	{
		if (GameObject.Find ("GameManager") != null)
			return;

		if (isDebug) { 
			GenerateCustomizationPlatforms ();
			SetFirstSelected ();
			SetupParty ();
			init = true;
		}

	}
	
	//============================================================================
	// Update
	//============================================================================

	void Update ()
	{
		if (!init)
			return;

		moveCharacterToCamera (); 

		CheckForCurrentAndLastSelectedCharacter ();

		if (Input.GetButtonDown ("L1")) {
			CyclePartyBack ();
		} else if (Input.GetButtonDown ("R1")) {
			CyclePartyForward ();
		}
	}

	void CheckForCurrentAndLastSelectedCharacter ()
	{
		if (currentlySelectedCharacter != lastSelectedCharacter) {
			lastSelectedCharacter.isActive = false;
			lastSelectedCharacter.SetWindowsActive (false);
		}

		currentlySelectedCharacter.isActive = true;

		lastSelectedCharacter = currentlySelectedCharacter;
	}
	                                      
	void moveCharacterToCamera ()
	{
		characterTray.position = new Vector3 (Mathf.MoveTowards (characterTray.position.x, -currentlySelectedCharacter.startPosition.x, Time.deltaTime * selectionSpeed), characterTray.position.y, characterTray.position.z);
	}

	//============================================================================
	// Events
	//============================================================================

	void SetFirstSelected ()
	{
		currentlySelectedCharacter = Platforms [0];
		currentlySelectedCharacter.SetWindowsActive (true);
		currentlySelectedCharacter.TalentWindow.SetActive (false);
		lastSelectedCharacter = currentlySelectedCharacter;
	}
	
	/// Generates/Initializes the customization platforms.
	void GenerateCustomizationPlatforms ()
	{
		for (int i = 0; i < 4; i++) {
			GameObject platform = Instantiate (PlatformPrefab) as GameObject;
			platform.name = "Platform: " + i;
			platform.transform.parent = characterTray.transform;
			platform.transform.position = transform.position + Vector3.right * i * platformOffset;

			Platforms.Add (platform.GetComponent<PlatformManager> ());
			Platforms[i].debug = isDebug;
			if (currentParty.Count > i)
				Platforms [i].GetComponent<PlatformManager> ().Initialize (currentParty [i]);
			else
				Platforms [i].GetComponent<PlatformManager> ().Initialize (null);
		}
	}

	/// Cycles the selected party member back
	protected void CyclePartyBack ()
	{
		int i = Platforms.IndexOf (currentlySelectedCharacter);
		i--;
		if (i < 0)
			i = currentParty.Count - 1;
		currentlySelectedCharacter.SetWindowsActive (false);
		currentlySelectedCharacter = Platforms [i];
	}
	
	/// Cycles the selected party member forward
	protected void CyclePartyForward ()
	{
		int i = Platforms.IndexOf (currentlySelectedCharacter);
		i++;
		if (i >= currentParty.Count)
			i = 0;
		currentlySelectedCharacter.SetWindowsActive (false);
		currentlySelectedCharacter = Platforms [i];
	}

	/// Setup party for debugging so you dont have to run it from the open world every time
	void SetupParty ()
	{
		PartyUnit newUnit = ScriptableObject.CreateInstance<PartyUnit> ();
		newUnit.UnitPrefab = Resources.Load ("Characters/Hero") as GameObject;
		newUnit.MovementDistance = 3;
		newUnit.Health = 60;
		newUnit.UnitClass = PlayerControlledBoardUnit.PlayerClass.Warrior;
		newUnit.ListOfAbilities.Add (Instantiate (Resources.Load<AbilityDescription> ("Abilities/TestDamageSlow")) as AbilityDescription);
		newUnit.ListOfAbilities.Add (Instantiate (Resources.Load<AbilityDescription> ("Abilities/TestHeal")) as AbilityDescription);
		newUnit.ListOfAbilities.Add (Instantiate (Resources.Load<AbilityDescription> ("Abilities/TestHeal")) as AbilityDescription);
		newUnit.ListOfAbilities.Add (Instantiate (Resources.Load<AbilityDescription> ("Abilities/TestStun")) as AbilityDescription);
		newUnit.currentLevel = 1;
		newUnit.UnitTalentTree = Instantiate (Resources.Load<TalentTree> ("TalentTrees/WarriorTree")) as TalentTree;
		currentParty.Add (newUnit);
		newUnit = ScriptableObject.CreateInstance<PartyUnit> ();
		newUnit.UnitPrefab = Resources.Load ("Characters/Hero") as GameObject;
		newUnit.MovementDistance = 4;
		newUnit.ListOfAbilities.Add (Instantiate (Resources.Load<AbilityDescription> ("Abilities/TestDamageSlow")) as AbilityDescription);
		newUnit.ListOfAbilities.Add (Instantiate (Resources.Load<AbilityDescription> ("Abilities/TestHeal")) as AbilityDescription);
		newUnit.ListOfAbilities.Add (Instantiate (Resources.Load<AbilityDescription> ("Abilities/TestStun")) as AbilityDescription);
		newUnit.Health = 65;
		newUnit.UnitClass = PlayerControlledBoardUnit.PlayerClass.Wizard;
		newUnit.currentLevel = 1;
		newUnit.UnitTalentTree = Instantiate (Resources.Load<TalentTree> ("TalentTrees/WizardTree")) as TalentTree;
		currentParty.Add (newUnit);
	}

	//============================================================================
}
