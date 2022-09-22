// DecompilerFi decompiler from Assembly-CSharp.dll class: DailyRewardsItem
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardsItem : MonoBehaviour
{
	[SerializeField]
	private Image contentBorder;

	[SerializeField]
	private GameObject selectedOk;

	[SerializeField]
	private Text dayText;

	[SerializeField]
	private Text contentText;

	[SerializeField]
	private Image contentSprite;

	[SerializeField]
	private Image glow;

	[SerializeField]
	private Text countText;

	public void Init(string dayText, string contentText, Color defaultColor, Color textColor, Sprite rewardSprite, bool wasClaimed)
	{
		AssainText(this.dayText, dayText);
		AssainText(this.contentText, contentText);
		SetTextColor(this.dayText, textColor);
		SetTextColor(this.contentText, textColor);
		base.transform.localScale = Vector3.one;
		contentBorder.color = defaultColor;
		contentSprite.sprite = rewardSprite;
		if (selectedOk != null)
		{
			selectedOk.SetActive(wasClaimed);
		}
		if (glow != null)
		{
			glow.gameObject.SetActive(value: false);
		}
	}

	public void SetCounter(int count)
	{
		if (count > 1 && countText != null)
		{
			countText.transform.parent.gameObject.SetActive(value: true);
			countText.text = $"x{count}";
		}
	}

	public void SetActive(Color activeColor, Color textColor)
	{
		contentBorder.color = activeColor;
		dayText.color = textColor;
		contentText.color = textColor;
		if (glow != null)
		{
			glow.gameObject.SetActive(value: true);
			glow.color = activeColor;
		}
	}

	private void AssainText(Text textObject, string text)
	{
		if (textObject != null)
		{
			textObject.text = text;
		}
	}

	private void SetTextColor(Text textObject, Color color)
	{
		if (textObject != null)
		{
			textObject.color = color;
		}
	}
}
