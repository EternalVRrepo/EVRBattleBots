using UnityEngine;
using System.Collections;

public class Hover : MonoBehaviour
{
	bool isHovered;
	float timer;

	[HideInInspector]
	public bool
		isActive;

	[HideInInspector]
	public Vector3
		startPosition;

	public PlatformManager platformManager;

	CharacterCustomizerManager CCManager;

	public enum hoverType
	{
		characterSelection,
		talents,
		abilities,
		stats
	}

	public hoverType hType;
	Color characterSelectionStartColor;
	    
	void Start ()
	{
		startPosition = transform.position;
		CCManager = GameObject.Find ("CharacterCustomizerManager").GetComponent<CharacterCustomizerManager> ();
		if (hType == hoverType.characterSelection)
			characterSelectionStartColor = gameObject.renderer.material.GetColor ("_TintColor");
	}

	void Update ()
	{
		timer += Time.deltaTime;

		HoverHandler (isHovered);
	}

	public void Enter ()
	{
		timer = 0;
		isHovered = true;
	}
	
	public void Exit ()
	{
		timer = 0;
		isHovered = false;
	}

	void HoverHandler (bool hover)
	{
		if (hover) {
			if (hType == hoverType.characterSelection) {
				//Trigger Events
				if (timer > 2f || Input.GetMouseButton (0)) {
					//Set Manager To Be Aware This Character Is Selected And Just Incase Sets Last Character To Inactive
					if (CCManager.currentlySelectedCharacter != platformManager && platformManager.CurrentlyDisplayedPartyUnit != null) {
						if (CCManager.currentlySelectedCharacter != null) 
							CCManager.currentlySelectedCharacter.isActive = false;
						CCManager.currentlySelectedCharacter = platformManager;
						isActive = true;
					}
				}
			}
		}

		if (!hover) {
			if (hType == hoverType.characterSelection) {
				//Animation Off
				gameObject.renderer.material.SetColor ("_TintColor", Color.Lerp (gameObject.renderer.material.GetColor ("_TintColor"), characterSelectionStartColor, Time.deltaTime * 1.5f));
				//Trigger Events
				if (timer > 2f) {
//					Debug.Log ("exitAnim");
				}
			}
		}
		//Active and looking at it
		if (isActive && hover) {
			if (hType == hoverType.characterSelection) {
				//Animation Off
				gameObject.renderer.material.SetColor ("_TintColor", Color.Lerp (gameObject.renderer.material.GetColor ("_TintColor"), characterSelectionStartColor, Time.deltaTime * 1.5f));
			}
		}

		//Active and not looking at it
		if (isActive && !hover) {
			if (hType == hoverType.characterSelection) {
				//Animation On
				Debug.Log ("AnimOn");
				gameObject.renderer.material.SetColor ("_TintColor", Color.Lerp (gameObject.renderer.material.GetColor ("_TintColor"), Color.white, Time.deltaTime * 0.3f));
			}
		}

		//Not Active and looking at it
		if (!isActive && hover) {
			if (hType == hoverType.characterSelection) {
				//Animation On
				gameObject.renderer.material.SetColor ("_TintColor", Color.Lerp (gameObject.renderer.material.GetColor ("_TintColor"), Color.white, Time.deltaTime * 0.3f));
			}
		}

		//Not Active Or Looking at it
		if (!isActive && !hover) {
			if (hType == hoverType.characterSelection) {
				//Animation Off
				gameObject.renderer.material.SetColor ("_TintColor", Color.Lerp (gameObject.renderer.material.GetColor ("_TintColor"), characterSelectionStartColor, Time.deltaTime * 1.5f));
			}
		}
	}
}
