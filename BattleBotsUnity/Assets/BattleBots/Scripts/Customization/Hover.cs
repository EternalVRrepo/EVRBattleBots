using UnityEngine;
using System.Collections;

public class Hover : MonoBehaviour
{
	#region Variables
	bool hit;
	float timer;

	[HideInInspector]
	public bool
		isActive;

	[HideInInspector]
	public Vector3
		startPosition;

	public MeshRenderer glowObject;

	public PlatformManager platformManager;

	CharacterCustomizerManager CCManager;

	public enum hoverType
	{
		characterSelection,
		talents,
		abilities,
		stats,
		talentsWindow,
		abiltiesWindow
	}

	public hoverType hType;
	Color startColor;
	TalentIcon talentIcon;
	bool talentWindowToggle;
	#endregion
	//============================================================================
	// Initilization
	//============================================================================

	void Start ()
	{
		startPosition = transform.position;
		CCManager = FindObjectOfType<CharacterCustomizerManager> ();

		if (hType == hoverType.characterSelection) {
			if (glowObject)
				startColor = glowObject.material.GetColor ("_TintColor");
			else 
				startColor = renderer.material.color;
		}

		if (hType == hoverType.talents) {
			talentIcon = GetComponent<TalentIcon> ();
		}
	}
	//============================================================================
	// Update
	//============================================================================

	void Update ()
	{
		timer += Time.deltaTime;

		HoverHandler (hit);
		GlowObjectAnimation (isActive);
	}

	void HoverHandler (bool target)
	{
		if (target) {
			if (hType == hoverType.characterSelection && !platformManager.isActive) {
				//Trigger Events
				if (timer > 2f || Input.GetMouseButtonDown (0) || Input.GetButtonDown ("Confirm")) {
					SetCharacterSelection ();
				}
			}

			if (hType == hoverType.talentsWindow) {
				if (timer > 2f || Input.GetMouseButtonDown (0) || Input.GetButtonDown ("Confirm")) {
					ToggleTalentWindow ();
				}
			}

			if (hType == hoverType.talents) {
				if (timer > 2f || Input.GetMouseButtonDown (0) || Input.GetButtonDown ("Confirm")) {
					talentIcon.Select (true);
				}
			}
		}
		
		if (!target) {
//			if (timer > 2f) {
//				//"ExitDelayFinished";
//			}
		}
	}

	//============================================================================
	// Events
	//============================================================================

	public void HoverHit (bool b)
	{
		timer = 0;
		hit = b;

		if (hType == hoverType.talents) {
			if (b) {
				CCManager.tooltipTextName = talentIcon.name;
				CCManager.tooltipTextDescription = talentIcon.description;
			}
			CCManager.displayTooltip = b;
		}
	}

	void GlowObjectAnimation (bool on)
	{
		if (glowObject == null) 
			return;

		switch (hType) {
		case hoverType.characterSelection: 

			if (hit && platformManager.isActive) {
				glowObject.material.SetColor ("_TintColor", Color.Lerp (glowObject.material.GetColor ("_TintColor"), startColor, Time.deltaTime * 1.5f));
			}

			if (hit && !platformManager.isActive) {
				glowObject.material.SetColor ("_TintColor", Color.Lerp (glowObject.material.GetColor ("_TintColor"), Color.white, Time.deltaTime * 0.5f));
			}

			if (!hit) {
				glowObject.material.SetColor ("_TintColor", Color.Lerp (glowObject.material.GetColor ("_TintColor"), startColor, Time.deltaTime * 1.5f));
			}
			return;

		case hoverType.talentsWindow:

			if (platformManager.TalentWindow.activeInHierarchy || hit)
				glowObject.material.SetColor ("_TintColor", Color.Lerp (glowObject.material.GetColor ("_TintColor"), Color.white, Time.deltaTime * 0.5f));
			else
				glowObject.material.SetColor ("_TintColor", Color.Lerp (glowObject.material.GetColor ("_TintColor"), startColor, Time.deltaTime * 1.5f));
			return;
		
		case hoverType.talents:
			return;

		default: 			

			if (on) {
				glowObject.material.SetColor ("_TintColor", Color.Lerp (glowObject.material.GetColor ("_TintColor"), Color.white, Time.deltaTime * 0.5f));
			} else {
				glowObject.material.SetColor ("_TintColor", Color.Lerp (glowObject.material.GetColor ("_TintColor"), startColor, Time.deltaTime * 1.5f));
			}
			return;
		}
	}

	void SetCharacterSelection ()
	{
		//Set Manager To Be Aware This Character Is Selected And Just Incase Sets Last Character To Inactive
		if (CCManager.currentlySelectedCharacter != platformManager && platformManager.CurrentlyDisplayedPartyUnit != null) {
			if (CCManager.currentlySelectedCharacter != null) 
				CCManager.currentlySelectedCharacter.SetWindowsActive (false);
			CCManager.currentlySelectedCharacter = platformManager;
			isActive = true;
		}
	}

	void ToggleTalentWindow ()
	{
		isActive = !isActive;
		if (platformManager.isActive)
			platformManager.TalentWindow.SetActive (isActive);
		timer = 0;
	}
}