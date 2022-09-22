// DecompilerFi decompiler from Assembly-CSharp.dll class: LevelupRewardItem
using UnityEngine;
using UnityEngine.UI;

public class LevelupRewardItem : MonoBehaviour
{
	public Image icon;

	public Text itemName;

	public void SetStuff(LevelUpRewardItemData data)
	{
		icon.sprite = data.sprite;
		itemName.text = data.displayName;
	}
}
