/////////////////////////////////////////////////////////////////////////////////
//
//	Template.cs
//	© EternalVR, All Rights Reserved
//
//	description:	Template used by the template manager for seeing what hexagons
//					are selected
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Template : MonoBehaviour {

	public List<Hexagon> CurrentHighlight = new List<Hexagon>();
	public TemplateManager.TargetTemplate thisTemplate;
	bool VisualTemplate;
	public bool TargetHexagon;

	public void Disable() {
		foreach (Hexagon h in CurrentHighlight)
			h.StopHighlight ();

		transform.localScale = new Vector3(1,1,1);
		renderer.enabled = false;
		transform.parent.position = new Vector3(100,100,100);
		CurrentHighlight.Clear ();
		VisualTemplate = false;
		TargetHexagon = false;
	}

	public void Enable() {
		renderer.enabled = true;
		VisualTemplate = true;
	}

	public void SetScale(int range, int width = 0) {
		if (thisTemplate == TemplateManager.TargetTemplate.Line) {
			Vector3 scale = new Vector3(1,1,1);
			if (width == 6)
				scale.z = 9;

			if (range == 3)
				scale.x = 3;
			transform.localScale = scale;
		}
		if (thisTemplate == TemplateManager.TargetTemplate.Cone)
			transform.localScale = new Vector3((2.93f/3f)*(float)range,1,(9.5f/3f)*(float)range);	
		else if (thisTemplate == TemplateManager.TargetTemplate.Circle) {
			if (range == 0)
				transform.localScale = new Vector3(1.6f, 1, 1.6f);
			else if (range == 1)
				transform.localScale = new Vector3(3.5f, 1, 3.5f);
			else if (range == 2)
				transform.localScale = new Vector3(6f, 1, 6f);
			else if (range == 3)
				transform.localScale = new Vector3(9f, 1, 9f);
			else if (range == 4)
				transform.localScale = new Vector3(12.75f, 1, 12.75f);
		}
	}

	public void GetHits (AbilityActivator ab, int range, Hexagon source, Hexagon destination) {
		VisualTemplate = false;
		SetScale (range);
		transform.parent.position = destination.transform.position;
		transform.parent.LookAt (source.transform.position);
		transform.Rotate (new Vector3(0, 180, 0));
		StartCoroutine ("WaitForCollision", ab);
	}

	IEnumerator WaitForCollision(AbilityActivator ab) {
		yield return new WaitForFixedUpdate();
		ab.targets = CurrentHighlight;
		ab.waiting = false;
		yield return new WaitForEndOfFrame();
		Disable();
	}


	void OnTriggerEnter(Collider other) {
		Hexagon h = other.GetComponent<Hexagon>();
	
		if (h != null) {

			if (!Targettable(h)) 
				return;

			CurrentHighlight.Add (h);
			if (VisualTemplate)
				h.Highlight();
		}
	}

	public bool Targettable(Hexagon h) {
		if (h.CurrentHexType == Hexagon.HexType.WalledImpassable) 
			return false;

		if (h.CurrentHexType == Hexagon.HexType.Null)
			return false;

		if (h.CurrentHexType == Hexagon.HexType.Impassable)
			return false;

		return true;
	}

	void OnTriggerExit(Collider other) {
		Hexagon h = other.GetComponent<Hexagon>();

		if (h != null) {
			if (CurrentHighlight.Contains (h)) { 
				if (!TargetHexagon)
					h.StopHighlight ();
				CurrentHighlight.Remove (h);
			}
		}
	}

}
