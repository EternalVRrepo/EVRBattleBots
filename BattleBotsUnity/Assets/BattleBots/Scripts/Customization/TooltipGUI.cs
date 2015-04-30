using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TooltipGUI : MonoBehaviour
{
	public CharacterCustomizerManager ccm;
	public Text tooltip;
	Canvas canvas;

	void Start ()
	{
		canvas = GetComponent<Canvas> ();
	}

	void Update ()
	{
		tooltip.text = ccm.tooltipText;
		canvas.enabled = ccm.displayTooltip;
	}
}
