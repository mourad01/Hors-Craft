// DecompilerFi decompiler from Assembly-CSharp.dll class: MaxWorldsConnector
using Common.Managers.States.UI;
using System;
using UnityEngine.UI;

public class MaxWorldsConnector : UIConnector
{
	public Button returnButton;

	public Button watchAdButton;

	private Action onReturnPressed;

	private Action onWatchAdPressed;

	public void Init(Action onReturnPressed, Action onWatchAdPressed)
	{
		this.onReturnPressed = onReturnPressed;
		this.onWatchAdPressed = onWatchAdPressed;
		InitButtons();
	}

	private void InitButtons()
	{
		returnButton.onClick.AddListener(delegate
		{
			if (onReturnPressed != null)
			{
				onReturnPressed();
			}
		});
		watchAdButton.onClick.AddListener(delegate
		{
			if (onWatchAdPressed != null)
			{
				onWatchAdPressed();
			}
		});
	}
}
