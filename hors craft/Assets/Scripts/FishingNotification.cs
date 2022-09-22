// DecompilerFi decompiler from Assembly-CSharp.dll class: FishingNotification
using Common.Managers;
using States;
using UnityEngine;
using UnityEngine.UI;

public class FishingNotification : TopNotification
{
	public class CollectionUnlockedInformation : ShowInformation
	{
		public Sprite icon;
	}

	public Button button;

	public Image image;

	public Text text;

	private void Awake()
	{
		button.onClick.AddListener(delegate
		{
			Manager.Get<StateMachineManager>().PushState<PauseState>(new PauseStateStartParameter
			{
				canSave = true,
				allowTimeChange = true,
				categoryToOpen = "Collections"
			});
		});
	}

	public override void SetElement(ShowInformation information)
	{
		CollectionUnlockedInformation collectionUnlockedInformation = information as CollectionUnlockedInformation;
		image.sprite = collectionUnlockedInformation.icon;
		text.text = collectionUnlockedInformation.information;
	}
}
