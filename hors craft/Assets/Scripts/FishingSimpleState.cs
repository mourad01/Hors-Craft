// DecompilerFi decompiler from Assembly-CSharp.dll class: FishingSimpleState
using com.ootii.Cameras;
using Common.Managers;
using Common.Managers.States;
using Common.Utils;
using Gameplay;
using States;
using UnityEngine;


public class FishingSimpleState : FishingState
{
	public override void StartState(StartParameter parameter)
	{
		base.StartState(parameter);
	}

	protected override void PrepeareFishing(StartParameter parameter)
	{
		TimeScaleHelper.value = 1f;
		fishingSettings = Manager.Get<ModelManager>().fishingSettings;
		base.connector.miniGamePanel.updateMode = AnimatorUpdateMode.UnscaledTime;
		base.connector.resourceGatherSpot.GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
		GetFishingManager();
		SetButtonsActions();
		fishingManager.fishingConnector = base.connector;
		Vector3 eulerAngles = CameraController.instance.MainCamera.transform.parent.rotation.eulerAngles;
		eulerAngles.x = 10f;
		CameraController.instance.MainCamera.transform.parent.rotation = Quaternion.Euler(eulerAngles);
		fishingManager.GetPlayerFishingController.StartFishing();
		OnFishing();
	}

	private void SetButtonsActions()
	{
		base.connector.fightFishingButton.gameObject.SetActive(value: false);
		base.connector.onReturnButtonClicked = ((FishingState)this).OnReturn;
		base.connector.onFishingButtonClicked = ((FishingState)this).OnFishing;
		base.connector.onPrevChangeFishClicked = base.OnPrevFish;
		base.connector.onNextChangeFishClicked = base.OnNextFish;
		base.connector.onPrevChangeRodClicked = base.OnPrevRod;
		base.connector.onNextChangeRodClicked = base.OnNextRod;
	}

	private void GetFishingManager()
	{
		fishingManager = Manager.Get<FishingManager>();
	}

	public override void UpdateState()
	{
		if (fishingManager.fishFighting)
		{
			base.connector.miniGamePanel.SetBool("PanelOn", value: true);
		}
		else
		{
			base.connector.miniGamePanel.SetBool("PanelOn", value: false);
		}
		if (base.connector.fightFishingButton.pressed)
		{
			OnFishing();
		}
		base.connector.SetRodWidth(fishingManager.rodConfigCurrent.baseWidth, fishingManager.fishConfigOnHook.rarity);
		UpdateFishingStatus();
		if (fishingManager.fishFighting)
		{
			UpdateFishFighting();
		}
	}

	protected void UpdateFishFighting()
	{
		base.connector.fightFishingButton.gameObject.SetActive(value: true);
		if (fishCurValue == fishTargetValue)
		{
			fishTargetValue = fishCurValue + Random.Range(-0.3f, 0.3f);
			fishTargetValue = Mathf.Clamp01(fishTargetValue);
		}
		fishCurValue = Mathf.MoveTowards(fishCurValue, fishTargetValue, Time.unscaledDeltaTime * (0.1f * (float)fishingManager.fishConfigOnHook.rarity / (float)fishingManager.rodConfigCurrent.quality));
		rodTargetValue -= 0.05f;
		rodTargetValue = Mathf.Clamp01(rodTargetValue);
		rodCurValue = Mathf.MoveTowards(rodCurValue, rodTargetValue, Time.unscaledDeltaTime * 0.45f);
		base.connector.miniGameSlider.value = rodCurValue;
		base.connector.SetFishPivotPosition(fishCurValue);
		base.connector.SetRodAreaColor();
	}

	public override void FinishState()
	{
		base.FinishState();
	}

	public override void UnfreezeState()
	{
		base.UnfreezeState();
	}

	public override void OnFishing()
	{
		fishingManager.SetRodConfig(0);
		base.OnFishing();
	}

	public override void OnReturn()
	{
		fishingManager.EndFishing();
		if (Manager.Get<StateMachineManager>().IsCurrentStateA<FishingState>())
		{
			Manager.Get<StateMachineManager>().PopState();
		}
	}
}
