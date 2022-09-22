// DecompilerFi decompiler from Assembly-CSharp.dll class: PetUnlockedNotification
using Common.Managers;
using States;
using UnityEngine;
using UnityEngine.UI;

public class PetUnlockedNotification : TopNotification
{
	public delegate void OnClick();

	public class PetUnlockedInformation : ShowInformation
	{
		public Sprite icon;
	}

	public Button button;

	public Image image;

	public Text text;

	public OnClick onNotificationButtonClicked;

	private void Awake()
	{
		button.onClick.AddListener(delegate
		{
			Manager.Get<StateMachineManager>().PushState<PauseState>(new PauseStateStartParameter
			{
				canSave = true,
				allowTimeChange = true,
				categoryToOpen = "Customization"
			});
		});
	}

	public override void SetElement(ShowInformation information)
	{
		PetUnlockedInformation petUnlockedInformation = information as PetUnlockedInformation;
		image.sprite = petUnlockedInformation.icon;
		text.text = petUnlockedInformation.information;
	}
}
