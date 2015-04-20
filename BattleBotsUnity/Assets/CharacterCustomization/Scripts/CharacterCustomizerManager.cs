using UnityEngine;
using System.Collections;

public class CharacterCustomizerManager : MonoBehaviour
{
	Hover[] hovers;

	public Hover defaultSelectedCharacter;
	public Hover currentlySelectedCharacter;
	public Hover currentlySelectedGUI;
	Hover lastSelectedCharacter;
	Hover lastSelectedGUI;

	[SerializeField]
	Transform
		characterTray;

	[SerializeField]
	float
		selectionSpeed = 1;
	
	void Start ()
	{
		hovers = GetComponentsInChildren<Hover> ();
		defaultSelectedCharacter.isActive = true;
		currentlySelectedCharacter = defaultSelectedCharacter;
		lastSelectedCharacter = defaultSelectedCharacter;
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
	}
}
