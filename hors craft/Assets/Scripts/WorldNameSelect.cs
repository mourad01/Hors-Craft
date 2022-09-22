// DecompilerFi decompiler from Assembly-CSharp.dll class: WorldNameSelect
using Common.Managers;
using Common.Managers.States.UI;
using System;
using UnityEngine.UI;

public class WorldNameSelect : UIConnector
{
	public InputField inputField;

	public Button returnButton;

	public Button createButton;

	public Text buttonLabel;

	private Action onReturnPressed;

	public void InitForCreation(Action onReturnPressed, Action<string> onCreate)
	{
		this.onReturnPressed = onReturnPressed;
		inputField.text = Manager.Get<SavedWorldManager>().GetNameOfNewWorld();
		buttonLabel.text = Manager.Get<TranslationsManager>().GetText("choose.world.button.name.create", "Create");
		InitButtons(onCreate);
	}

	public void InitForRename(string id, Action onReturnPressed, Action<string> onRename)
	{
		this.onReturnPressed = onReturnPressed;
		inputField.text = string.Empty;
		buttonLabel.text = Manager.Get<TranslationsManager>().GetText("choose.world.button.name.rename", "Rename");
		InitButtons(onRename);
	}

	private void InitButtons(Action<string> mainAction)
	{
		returnButton.onClick.AddListener(delegate
		{
			if (onReturnPressed != null)
			{
				onReturnPressed();
			}
		});
		createButton.onClick.AddListener(delegate
		{
			if (mainAction != null)
			{
				string text = inputField.textComponent.text;
				if (string.IsNullOrEmpty(text))
				{
					text = "My World";
				}
				mainAction(inputField.textComponent.text);
			}
		});
	}
}
