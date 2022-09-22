// DecompilerFi decompiler from Assembly-CSharp.dll class: ClaimAchievementButton
using GameUI;
using UnityEngine;
using UnityEngine.UI;

public class ClaimAchievementButton : MonoBehaviour
{
	[SerializeField]
	private string inProgressKey;

	[SerializeField]
	private string inProgressDefault;

	[SerializeField]
	private string claimKey;

	[SerializeField]
	private string claimDefault;

	[SerializeField]
	private string doneKey;

	[SerializeField]
	private string doneDefault;

	private Button claimButton;

	private ColorController backgroundColorController;

	private TranslateText buttonText;

	[SerializeField]
	private ColorManager.ColorCategory claimColor;

	[SerializeField]
	private ColorManager.ColorCategory inProgressColor;

	[SerializeField]
	private ColorManager.ColorCategory doneColor;

	private void Awake()
	{
		claimButton = GetComponent<Button>();
		backgroundColorController = GetComponent<ColorController>();
		buttonText = GetComponentInChildren<TranslateText>();
	}

	public void Init(AchievementElement element)
	{
		claimButton.onClick.AddListener(delegate
		{
			element.ClaimAchievement();
		});
	}

	public void UpdateButton(bool canClaim, bool isDone)
	{
		ColorManager.ColorCategory category = isDone ? doneColor : ((!canClaim) ? inProgressColor : claimColor);
		backgroundColorController.category = category;
		backgroundColorController.UpdateColor();
		string translationKey = isDone ? doneKey : ((!canClaim) ? inProgressKey : claimKey);
		buttonText.translationKey = translationKey;
		string defaultText = isDone ? doneDefault : ((!canClaim) ? inProgressDefault : claimDefault);
		buttonText.defaultText = defaultText;
		buttonText.ForceRefresh();
		claimButton.interactable = (!isDone && (canClaim ? true : false));
	}
}
