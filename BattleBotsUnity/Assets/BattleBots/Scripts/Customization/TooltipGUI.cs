using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TooltipGUI : MonoBehaviour
{
	public CharacterCustomizerManager ccm;
	public Text tooltipName;
	public Text tooltipDescription;
	public RawImage background;
	Canvas canvas;

	Color name, description, bg;

	void Start ()
	{
		canvas = GetComponent<Canvas> ();
		name = tooltipName.color;
		description = tooltipDescription.color;
		bg = background.color;
		tooltipName.color = Color.clear;
		tooltipDescription.color = Color.clear;
		background.color = Color.clear;
	}

	void Update ()
	{
		if (ccm) {
			tooltipName.text = ccm.tooltipTextName;
			tooltipDescription.text = ccm.tooltipTextDescription;

			if (ccm.displayTooltip) {
				tooltipName.color = name;
				tooltipDescription.color = description;
				background.color = bg;
			} else {
				tooltipName.color = Color.Lerp (tooltipName.color, Color.clear, Time.deltaTime * 2f);
				tooltipDescription.color = Color.Lerp (tooltipDescription.color, Color.clear, Time.deltaTime * 2f);
				background.color = Color.Lerp (background.color, Color.clear, Time.deltaTime * 2f);
			}
		}
	}
}
