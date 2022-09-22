// DecompilerFi decompiler from Assembly-CSharp.dll class: DeviceListItem
using Common.Managers;
using GameUI;
using UnityEngine;
using UnityEngine.UI;

public class DeviceListItem : MonoBehaviour
{
	public Button button;

	public Text deviceName;

	public Text deviceLevel;

	public GameObject lockGO;

	private ColorManager.ColorCategory previousColorCategory;

	public void SetHighlight(bool highlight)
	{
		if (!highlight)
		{
			Color color = Manager.Get<ColorManager>().GetColorForCategory(previousColorCategory) * 0.8f;
			color.a = 1f;
			button.GetComponent<Image>().color = color;
		}
		else
		{
			button.GetComponent<Image>().color = Manager.Get<ColorManager>().GetColorForCategory(previousColorCategory);
		}
	}

	public void Init(string n, string lvl, bool locked)
	{
		deviceName.text = n;
		deviceLevel.text = "LVL: " + lvl;
		deviceLevel.gameObject.SetActive(!locked);
		lockGO.SetActive(locked);
		ColorController component = button.GetComponent<ColorController>();
		component.category = ((!locked) ? ColorManager.ColorCategory.HIGHLIGHT_COLOR : ColorManager.ColorCategory.DISABLED_COLOR);
		previousColorCategory = component.category;
		component.UpdateColor();
	}
}
