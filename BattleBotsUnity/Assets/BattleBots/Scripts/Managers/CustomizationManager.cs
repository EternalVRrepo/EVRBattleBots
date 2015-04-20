using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomizationManager : MonoBehaviour {

	List<PartyUnit> Party;
	PartyUnit CurrentlySelectedPartyUnit;

	void Update() {
		if (Input.GetKeyDown(KeyCode.U)) {
			OpenTalentEditor();
		}
		if (Input.GetKeyDown (KeyCode.Y)) {
			GameManager.instance.FinishCustomizationMenu();
		}
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			CyclePartyBack();
		}
		else if (Input.GetKeyDown (KeyCode.Alpha2)) {
			CyclePartyForward();
		}
	}

	/// <summary>
	/// Opens the talent editor.
	/// </summary>
	void OpenTalentEditor() {
		//Select main dude
		//Check his class/level/stats/etc
		//Check the party units talent tree = Resources.Load("TalentTrees/currentPartyUnit.ClassName + "Tree") as TalentTree;
		//User presses row 6 column 2
		//CurrentPartyUnit.TalentTree.TalentsChosen[5] = PressedButton.Talent;
	}

	/// <summary>
	/// Initialize the customization manager with the current party
	/// </summary>
	public void Initialize(List<PartyUnit> party) {
		Party = party;
		CurrentlySelectedPartyUnit = Party[0];
	}

	/// <summary>
	/// Cycles the selected party member back
	/// </summary>
	protected void CyclePartyBack() {
		int i = Party.IndexOf (CurrentlySelectedPartyUnit);
		i--;
		if (i < 0)
			i = Party.Count-1;
		CurrentlySelectedPartyUnit = Party[i];
	}

	/// <summary>
	/// Cycles the selected party member forward
	/// </summary>
	protected void CyclePartyForward() {
		int i = Party.IndexOf (CurrentlySelectedPartyUnit);
		i++;
		if (i >= Party.Count)
			i = 0;
		CurrentlySelectedPartyUnit = Party[i];
	}

}
