using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterCustomizerManager : MonoBehaviour
{
	Hover[] hovers;

	public PlatformManager defaultSelectedCharacter;
	public PlatformManager currentlySelectedCharacter {
		get {
			return selectedChar;
		}
		set {
			UpdateActive(value);
			lastSelectedCharacter = selectedChar;
			selectedChar = value;
		}
	}
	public PlatformManager lastSelectedCharacter;
	public PartyUnit CurrentlyDisplayedUnit;
	public List<PartyUnit> currentParty;
	public GameObject PlatformPrefab;
	public float PlatformDistance;
	public Transform characterTray;
	public float selectionSpeed = 1;
	public bool isDebug;

	protected List<PlatformManager> Platforms = new List<PlatformManager>();
	protected PlatformManager selectedChar;

	void Start () {
		if (isDebug) 
			SetupParty();
		GenerateCustomizationPlatforms();
		hovers = GetComponentsInChildren<Hover> ();
		defaultSelectedCharacter = Platforms[0];
		defaultSelectedCharacter.isActive = true;
		currentlySelectedCharacter = defaultSelectedCharacter;
		CurrentlyDisplayedUnit = currentParty[0];
		lastSelectedCharacter = defaultSelectedCharacter;
	}

	/// <summary>
	/// Setup party for debugging so you dont have to run it from the open world every time
	/// </summary>
	void SetupParty() {
		PartyUnit newUnit = ScriptableObject.CreateInstance<PartyUnit>();
		newUnit.UnitPrefab = Resources.Load ("Characters/Hero") as GameObject;
		newUnit.MovementDistance = 3;
		newUnit.Health = 60;
		newUnit.UnitClass = PlayerControlledBoardUnit.PlayerClass.Melee;
		newUnit.ListOfAbilities.Add (Instantiate (Resources.Load<AbilityDescription>("Abilities/TestDamageSlow")) as AbilityDescription);
		newUnit.ListOfAbilities.Add (Instantiate (Resources.Load<AbilityDescription>("Abilities/TestHeal")) as AbilityDescription);
		newUnit.ListOfAbilities.Add (Instantiate (Resources.Load<AbilityDescription>("Abilities/TestHeal")) as AbilityDescription);
		newUnit.ListOfAbilities.Add (Instantiate (Resources.Load<AbilityDescription>("Abilities/TestStun")) as AbilityDescription);
		newUnit.currentLevel = 1;
		newUnit.UnitTalentTree = Instantiate (Resources.Load<TalentTree>("TalentTrees/MeleeTree")) as TalentTree;
		currentParty.Add (newUnit);
		newUnit = ScriptableObject.CreateInstance<PartyUnit>();
		newUnit.UnitPrefab = Resources.Load ("Characters/Hero") as GameObject;
		newUnit.MovementDistance = 4;
		newUnit.ListOfAbilities.Add (Instantiate (Resources.Load<AbilityDescription>("Abilities/TestDamageSlow")) as AbilityDescription);
		newUnit.ListOfAbilities.Add (Instantiate (Resources.Load<AbilityDescription>("Abilities/TestHeal")) as AbilityDescription);
		newUnit.ListOfAbilities.Add (Instantiate (Resources.Load<AbilityDescription>("Abilities/TestStun")) as AbilityDescription);
		newUnit.Health = 65;
		newUnit.UnitClass = PlayerControlledBoardUnit.PlayerClass.Ranged;
		newUnit.currentLevel = 1;
		newUnit.UnitTalentTree = Instantiate (Resources.Load<TalentTree>("TalentTrees/RangedTree")) as TalentTree;
		currentParty.Add (newUnit);
	}

	void UpdateActive(PlatformManager pm) {
		foreach (PlatformManager p in Platforms) {
			if (p != pm) {
				p.SetFocused(false);
			}
		}
		pm.SetFocused(true);
	}

	/// <summary>
	/// Initialize the specified party for the customization manager
	/// </summary>
	public void Initialize(List<PartyUnit> party) {
		isDebug = false; //Dont want to add stuff if we are actually initializing
		currentParty = party;
	}

	/// <summary>
	/// Generates/Initializes the customization platforms.
	/// </summary>
	void GenerateCustomizationPlatforms() {
		for (int i = 0; i < 4; i++) {
			GameObject platform = Instantiate (PlatformPrefab) as GameObject;
			platform.name = "Platform: " + i;
			platform.transform.parent = characterTray.transform;
			platform.transform.position = transform.position + Vector3.right*i*PlatformDistance;
			Platforms.Add(platform.GetComponent<PlatformManager>());
			if (currentParty.Count > i)
				Platforms[i].GetComponent<PlatformManager>().Initialize(currentParty[i]);
			else Platforms[i].GetComponent<PlatformManager>().Initialize(null);
		}
	}
	
	void Update ()
	{
		if (currentlySelectedCharacter != lastSelectedCharacter) {
			lastSelectedCharacter.isActive = false;
		}
		//Move Camera To Character
//		if (currentlySelectedCharacter != null)
//			cam.transform.position = new Vector3 (Mathf.MoveTowards (cam.transform.position.x, currentlySelectedCharacter.gameObject.transform.position.x, Time.deltaTime * camSpeed), cam.transform.position.y, cam.transform.position.z);

		//Move Character To Camera
		if (currentlySelectedCharacter != null)
			characterTray.position = new Vector3 (Mathf.MoveTowards (characterTray.position.x, -currentlySelectedCharacter.startPosition.x, Time.deltaTime * selectionSpeed), characterTray.position.y, characterTray.position.z);

		lastSelectedCharacter = currentlySelectedCharacter;

		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			CyclePartyBack();
		}
		else if (Input.GetKeyDown (KeyCode.Alpha2)) {
			CyclePartyForward();
		}
	}

	/// <summary>
	/// Cycles the selected party member back
	/// </summary>
	protected void CyclePartyBack() {
		int i = Platforms.IndexOf (currentlySelectedCharacter);
		i--;
		if (i < 0)
			i = currentParty.Count-1;
		currentlySelectedCharacter.isActive = false;
		currentlySelectedCharacter = Platforms[i];
	}
	
	/// <summary>
	/// Cycles the selected party member forward
	/// </summary>
	protected void CyclePartyForward() {
		int i = Platforms.IndexOf (currentlySelectedCharacter);
		i++;
		if (i >= currentParty.Count)
			i = 0;
		currentlySelectedCharacter.isActive = false;
		currentlySelectedCharacter = Platforms[i];
	}
}
