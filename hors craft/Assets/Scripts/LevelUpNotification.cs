// DecompilerFi decompiler from Assembly-CSharp.dll class: LevelUpNotification
using Common.Managers;
using States;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpNotification : TopNotification
{
	[HideInInspector]
	public string categoryToOpen;

	private Button button;

	private void Awake()
	{
		button = GetComponentInChildren<Button>();
		button.onClick.AddListener(delegate
		{
			if (!Manager.Get<StateMachineManager>().IsCurrentStateA<PauseState>())
			{
				Manager.Get<StateMachineManager>().PushState<PauseState>(new PauseStateStartParameter
				{
					canSave = true,
					allowTimeChange = true,
					categoryToOpen = categoryToOpen
				});
			}
		});
	}

	public override void SetElement(ShowInformation information)
	{
	}
}
