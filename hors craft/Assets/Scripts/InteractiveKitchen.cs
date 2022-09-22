// DecompilerFi decompiler from Assembly-CSharp.dll class: InteractiveKitchen
using Common.Managers;
using Gameplay;
using States;
using Uniblocks;
using UnityEngine;

public class InteractiveKitchen : InteractiveObject
{
	public string customKey = string.Empty;

	public string sceneToLoad = "Cooking";

	protected override void Awake()
	{
		base.Awake();
		isUsable = Manager.Get<ModelManager>().cookingSettings.IsCookingEnabled();
	}

	public override void OnUse()
	{
		base.OnUse();
		string key = (!string.IsNullOrEmpty(customKey)) ? customKey : SerializePosition(base.transform.position);
		GoToCooking(key, sceneToLoad);
	}

	public static void GoToCooking(string key, string sceneToLoad)
	{
		Engine.SaveWorldInstant();
		Manager.Get<GameCallbacksManager>().InFrequentSave();
		Manager.Get<GameCallbacksManager>().FrequentSave();
		Manager.Get<CookingManager>().AssingKitchenKey(key);
		SavedWorldManager.UnloadCraftGameplay();
		Manager.Get<StateMachineManager>().SetState<LoadLevelState>(new LoadLevelDefaultStartParameter
		{
			sceneToLoadName = sceneToLoad,
			stateType = typeof(CookingChooseLevelState)
		});
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	private string SerializePosition(Vector3 position)
	{
		int num = Mathf.FloorToInt(position.x);
		int num2 = Mathf.FloorToInt(position.y);
		int num3 = Mathf.FloorToInt(position.z);
		return num + "." + num2 + ".." + num3;
	}
}
