// DecompilerFi decompiler from Assembly-CSharp.dll class: UpgradeBar
using UnityEngine;
using UnityEngine.UI;

public class UpgradeBar : MonoBehaviour
{
	public Image fillImage;

	public Text upgradeName;

	public void Init(string name, string value, float progress)
	{
		upgradeName.text = name + ": " + value;
		fillImage.fillAmount = progress;
	}
}
