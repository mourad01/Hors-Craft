// DecompilerFi decompiler from Assembly-CSharp.dll class: SaveWorldCheckConnector
using Common.Managers;
using Common.Managers.States.UI;
using System;
using UnityEngine.UI;

public class SaveWorldCheckConnector : UIConnector
{
	public WorldSelectElement worldPrefabElement;

	public Text doYouWantToSaveText;

	public Text youWillLose;

	public Button returnButton;

	public Button acceptButton;

	public Button refuseButton;

	private Action onReturnPressed;

	private Action onAccept;

	private Action onRefuse;

	private WorldData data;

	public void InitForDelete(WorldData worldData, Action onReturnPressed, Action onRefuse, Action onAccept)
	{
		worldPrefabElement.gameObject.SetActive(value: true);
		TranslateText component = doYouWantToSaveText.GetComponent<TranslateText>();
		component.defaultText = "Do you want to delete?";
		component.translationKey = "choose.world.question.delete";
		component.ForceRefresh();
		TranslateText component2 = youWillLose.GetComponent<TranslateText>();
		component2.ClearVisitors();
		component2.defaultText = $"You will lose {worldData.name} pernamently!";
		component2.translationKey = "choose.world.question.delete.lose";
		component2.AddVisitor(delegate
		{
			string text2 = Manager.Get<TranslationsManager>().GetText("choose.world.question.delete.lose", "You will lose {0} pernamently!");
			return string.Format(text2, worldData.name);
		});
		InitCommon(worldData, onReturnPressed, onRefuse, onAccept);
	}

	public void InitForSurvivalNight(WorldData worldData, Action onReturnPressed, Action onRefuse, Action onAccept)
	{
		worldPrefabElement.gameObject.SetActive(value: false);
		TranslateText component = doYouWantToSaveText.GetComponent<TranslateText>();
		component.defaultText = "Do you want to exit?";
		component.translationKey = "choose.world.question.survival";
		component.ForceRefresh();
		TranslateText component2 = youWillLose.GetComponent<TranslateText>();
		component2.ClearVisitors();
		component2.defaultText = $"This world cannot be saved at night.{Environment.NewLine}You will lose your progress!";
		component2.translationKey = "choose.world.result.survival";
		InitCommon(worldData, onReturnPressed, onRefuse, onAccept);
	}

	private void InitCommon(WorldData worldData, Action onReturnPressed, Action onRefuse, Action onAccept)
	{
		this.onReturnPressed = onReturnPressed;
		this.onAccept = onAccept;
		this.onRefuse = onRefuse;
		InitButtons();
		SetData(worldData);
	}

	public void InitForSaving(WorldData worldData, Action onReturnPressed, Action onRefuse, Action onAccept)
	{
		worldPrefabElement.gameObject.SetActive(value: true);
		TranslateText component = doYouWantToSaveText.GetComponent<TranslateText>();
		component.defaultText = "Do you want to save?";
		component.translationKey = "choose.world.question";
		component.ForceRefresh();
		TranslateText component2 = youWillLose.GetComponent<TranslateText>();
		component2.ClearVisitors();
		component2.defaultText = $"If you won't save {worldData.name}, your changes will be lost!";
		component2.translationKey = "choose.world.result";
		component2.AddVisitor(delegate
		{
			string text2 = Manager.Get<TranslationsManager>().GetText("choose.world.result", "If you won't save {0}, your changes will be lost!!");
			return string.Format(text2, worldData.name);
		});
		InitCommon(worldData, onReturnPressed, onRefuse, onAccept);
	}

	private void SetData(WorldData worldData)
	{
		data = worldData;
		worldPrefabElement.Init(data, null, null);
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
		acceptButton.onClick.AddListener(delegate
		{
			if (onAccept != null)
			{
				onAccept();
			}
		});
		refuseButton.onClick.AddListener(delegate
		{
			if (onRefuse != null)
			{
				onRefuse();
			}
		});
	}
}
