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

public class TemplateManager : MonoBehaviour
{

	public static TemplateManager instance;
	public Transform CenterEyeAnchor;
	public bool TemplateInUse;
	public bool HexTargetting;
	public Target TemplateTargetType;
	public BoardUnit CurrentUnit;
	public List<Hexagon> CurrentTargets;
	public List<Hexagon> TargetHexagons = new List<Hexagon> ();

	protected Vector3 MouseCoords;
	protected List<GameObject> templates = new List<GameObject> ();
	protected Template template;

	private int boardLayer;

	void Awake ()
	{
		instance = this;
	}

	void Start ()
	{
		CreateLayerMask ();
		CreateTemplates ();
	}

	public List<GameObject> TemplatePrefabs = new List<GameObject> ();

	public enum TargetTemplate
	{
		Circle = 0,
		Line = 1,
		Cone = 2,
		none = 3,
	}

	public enum Target
	{
		RadialTied, //Tied to character, choose direction
		RadialFree, //Follow mouse, choose direction
		StaticTied, //Tied to character static size
		StaticFree, //Follow mouse, static shape
		NULL,
	}

	void Update ()
	{
		if (TemplateInUse || HexTargetting) {
			if (HexTargetting) {
				Ray ray = new Ray (CenterEyeAnchor.transform.position, CenterEyeAnchor.transform.forward);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit, Mathf.Infinity, boardLayer)) {
					if (TargetHexagons.Contains (hit.transform.GetComponent<Hexagon> ())) {
						template.transform.parent.position = hit.transform.position;
						if (hit.transform.GetComponent<Hexagon> () != CurrentUnit.CurrentlyOccupiedHexagon) {
							template.transform.parent.LookAt (CurrentUnit.transform.position);
							template.transform.parent.Rotate (new Vector3 (0, 180, 0));
						}
					}
				}
			} else {
				Ray ray = new Ray (CenterEyeAnchor.transform.position, CenterEyeAnchor.transform.forward);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit, Mathf.Infinity, boardLayer)) {
					if (TemplateTargetType == Target.StaticTied || TemplateTargetType == Target.RadialTied) {
						template.transform.parent.position = new Vector3 (CurrentUnit.transform.position.x, 0, CurrentUnit.transform.position.z);
						template.transform.parent.LookAt (new Vector3 (hit.point.x, 0, hit.point.z));
					} else {
						template.transform.parent.position = new Vector3 (hit.point.x, 0, hit.point.z);
						if (TemplateTargetType != Target.StaticFree) 
							template.transform.parent.LookAt (new Vector3 (CurrentUnit.transform.position.x, 0, CurrentUnit.transform.position.z));
					}
				}
			}
		}
	}

	/// <summary>
	/// Get hits for a template
	/// </summary>
	public void TemplateHit (AbilityActivator ab, TargetTemplate template, int range, Hexagon source, Hexagon destination)
	{
		Template t = templates [(int)template].GetComponentInChildren<Template> ();
		t.GetHits (ab, range, source, destination);
	}

	/// <summary>
	/// Creates the templates on start for use later
	/// </summary>
	void CreateTemplates ()
	{
		foreach (GameObject o in TemplatePrefabs) {
			GameObject t = Instantiate (o) as GameObject;
			templates.Add (t);
			t.GetComponentInChildren<Template> ().Disable ();
		}
	}

	/// <summary>
	/// Starts highlighting and using a template for an ability
	/// </summary>
	public void StartHighlighting (BoardUnit u, AbilityDescription a)
	{
		CurrentTargets.Clear ();
		template = templates [(int)a.Template].GetComponentInChildren<Template> (); 
		TemplateInUse = true;
		TemplateTargetType = a.TemplateType;
		CurrentUnit = u;
		template.Enable ();
		template.SetScale (a.TemplateSize, a.castRange);
		CurrentTargets = template.CurrentHighlight;
	}

	/// <summary>
	/// Starts highlighting for hex targetting
	/// </summary>
	public void StartHexHighlighting (BoardUnit u, AbilityDescription a)
	{
		CurrentTargets.Clear ();
		template = templates [(int)a.Template].GetComponentInChildren<Template> (); 
		HexTargetting = true;
		TemplateTargetType = a.TemplateType;
		CurrentUnit = u;
		if (a.AbilityTargetType == AbilityDescription.TargetType.TargetHexagon)
			template.TargetHexagon = true;
		template.Enable ();
		template.SetScale (a.TemplateSize, a.castRange);
		CurrentTargets = template.CurrentHighlight;
	}

	/// <summary>
	/// Finishes using the template for the ability, returns what units the ability found
	/// </summary>
	public List<Hexagon> FinishAbility ()
	{
		if (TemplateInUse || HexTargetting) {
			List<Hexagon> units = new List<Hexagon> ();
			foreach (Hexagon h in CurrentTargets) {
				units.Add (h);
			}
			TemplateInUse = false;
			HexTargetting = false;
			TemplateTargetType = Target.NULL;
			CurrentUnit = null;
			template.Disable ();
			template = null;
			return units;
		}
		return new List<Hexagon> ();
	}
	
	private void CreateLayerMask ()
	{
		int Layer1 = 10; //Hexagon
		int LayerMask1 = 1 << Layer1;
		boardLayer = LayerMask1; //... | LayerMask2 | LayerMask3;
	}
}
