/////////////////////////////////////////////////////////////////////////////////
//
//	TemplateManager.cs
//	© EternalVR, All Rights Reserved
//
//	description:	This class handles the creation and use of ability targetting
//					templates
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TemplateManager : MonoBehaviour {

	public static TemplateManager instance;
	public bool TemplateInUse;
	public Target TemplateTargetType;
	public BoardUnit CurrentUnit;
	public List<Hexagon> CurrentTargets;

	protected Vector3 MouseCoords;
	protected List<GameObject> templates = new List<GameObject>();
	protected Template template;

	private int boardLayer;

	void Awake() {
		instance = this;
	}

	void Start() {
		CreateLayerMask();
		CreateTemplates();
	}

	public List<GameObject> TemplatePrefabs = new List<GameObject>();

	public enum TargetTemplate {
		Circle = 0,
		Line = 1,
		Cone = 2,
	}

	public enum Target {
		RadialTied, //Tied to character, choose direction
		RadialFree, //Follow mouse, choose direction
		StaticTied, //Static size tied to character
		StaticFree, //Follow mouse, static shape
		NULL,
	}

	void Update() {
		if (TemplateInUse) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast (ray, out hit, Mathf.Infinity, boardLayer))	{
				if (TemplateTargetType == Target.StaticTied || TemplateTargetType == Target.RadialTied) {
					template.transform.parent.position = new Vector3(CurrentUnit.transform.position.x, 0, CurrentUnit.transform.position.z);
					template.transform.parent.LookAt (new Vector3(hit.point.x, 0, hit.point.z));
				}
				else {
					template.transform.parent.position = new Vector3(hit.point.x, 0, hit.point.z);
					template.transform.parent.LookAt (new Vector3(CurrentUnit.transform.position.x, 0, CurrentUnit.transform.position.z));
				}
			}
		}
	}

	/// <summary>
	/// Creates the templates on start for use later
	/// </summary>
	void CreateTemplates() {
		foreach (GameObject o in TemplatePrefabs) {
			GameObject t = Instantiate(o) as GameObject;
			templates.Add(t);
			t.GetComponentInChildren<Template>().Disable();
		}
	}

	/// <summary>
	/// Starts highlighting and using a template for an ability
	/// </summary>
	public void StartHighlighting(BoardUnit u, AbilityDescription a) {
		CurrentTargets.Clear ();
//		GameObject go = Instantiate(TemplatePrefabs[(int)a.Template]) as GameObject;
		template = templates[(int)a.Template].GetComponentInChildren<Template>(); 
		TemplateInUse = true;
		TemplateTargetType = a.TemplateType;
		CurrentUnit = u;
		template.Enable();
		template.SetScale(a.castRange);
		CurrentTargets = template.CurrentHighlight;
	}

	/// <summary>
	/// Finishes using the template for the ability, returns what units the ability found
	/// </summary>
	public List<BoardUnit> FinishAbility() {
		if (TemplateInUse) {
			List<BoardUnit> units = new List<BoardUnit>();
			foreach (Hexagon h in CurrentTargets) {
				if (h.OccupiedUnit != null)
					units.Add(h.OccupiedUnit);
			}
			TemplateInUse = false;
			TemplateTargetType = Target.NULL;
			CurrentUnit = null;
			template.Disable ();
			template = null;
			return units;
		}
		return new List<BoardUnit>();
	}
	
	private void CreateLayerMask() {
		int Layer1 = 10; //Hexagon
		int LayerMask1 = 1 << Layer1;
		boardLayer = LayerMask1; //... | LayerMask2 | LayerMask3;
	}
}
