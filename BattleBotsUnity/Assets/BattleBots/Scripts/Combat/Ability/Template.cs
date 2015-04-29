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

	public void Disable() {
		foreach (Hexagon h in CurrentHighlight)
			h.StopHighlight ();

		transform.localScale = new Vector3(1,1,1);
		renderer.enabled = false;
		transform.parent.position = new Vector3(100,100,100);
		CurrentHighlight.Clear ();
	}

	public void Enable() {
		renderer.enabled = true;
	}

	public void SetScale(int range) {
		if (thisTemplate == TemplateManager.TargetTemplate.Cone)
			transform.localScale = new Vector3((2.93f/3f)*(float)range,1,(9.5f/3f)*(float)range);		
	}

	void OnTriggerEnter(Collider other) {
		Hexagon h = other.GetComponent<Hexagon>();

		if (h != null) {
			CurrentHighlight.Add (h);
			h.Highlight();
		}
	}

	void OnTriggerExit(Collider other) {
		Hexagon h = other.GetComponent<Hexagon>();

		if (h != null) {
			if (CurrentHighlight.Contains (h)) { 
				h.StopHighlight ();
				CurrentHighlight.Remove (h);
			}
		}
	}

}
