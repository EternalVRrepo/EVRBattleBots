using UnityEngine;
using System.Collections;

public class Hover : MonoBehaviour
{
	#region Variables
	bool hit;
	float timer;

	[HideInInspector]
	public bool
		active;

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
	Color characterSelectionStartColor;
	#endregion
	//============================================================================
	// Initilization
	//============================================================================

	void Start ()
	{
		startPosition = transform.position;
		CCManager = GameObject.Find ("CharacterCustomizerManager").GetComponent<CharacterCustomizerManager> ();

		if (hType == hoverType.characterSelection) {
			if (glowObject)
				characterSelectionStartColor = glowObject.material.GetColor ("_TintColor");
		}
	}
	//============================================================================
	// Update
	//============================================================================
	void Update ()
	{
		timer += Time.deltaTime;

		HoverHandler (hit);
	}

	void HoverHandler (bool target)
	{
		if (target) {
			
			if (active)
				GlowObjectAnimation (false);
			else
				GlowObjectAnimation (true);
			
			
			if (hType == hoverType.characterSelection) {
				//Trigger Events
				if (timer > 2f || Input.GetMouseButton (0)) {
					SetCharacterSelection ();
				}
			}
		}
		
		if (!target) {
			
			if (active)
				GlowObjectAnimation (true);
			else
				GlowObjectAnimation (false);
			
			if (timer > 2f) {
				//"ExitDelayFinished";
			}
		}
	}

	//============================================================================
	// Events
	//============================================================================

	public void HoverHit (bool b)
	{
		timer = 0;
		hit = b;
	}
	
	public void SetActive (bool b)
	{
		active = b;
	}

	void GlowObjectAnimation (bool b)
	{
		if (glowObject == null) 
			return;

		if (b)  
			glowObject.material.SetColor ("_TintColor", Color.Lerp (gameObject.renderer.material.GetColor ("_TintColor"), Color.white, Time.deltaTime * 0.3f));
		else
			glowObject.material.SetColor ("_TintColor", Color.Lerp (gameObject.renderer.material.GetColor ("_TintColor"), characterSelectionStartColor, Time.deltaTime * 1.5f));
	}

	void SetCharacterSelection ()
	{
		//Set Manager To Be Aware This Character Is Selected And Just Incase Sets Last Character To Inactive
		if (CCManager.currentlySelectedCharacter != platformManager && platformManager.CurrentlyDisplayedPartyUnit != null) {
			if (CCManager.currentlySelectedCharacter != null) 
				CCManager.currentlySelectedCharacter.active = false;
			CCManager.currentlySelectedCharacter = platformManager;
			active = true;
		}
	}
}