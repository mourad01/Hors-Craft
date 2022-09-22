// DecompilerFi decompiler from Assembly-CSharp.dll class: FishingSimpleManager
using com.ootii.Cameras;
using Common.Managers;
using UnityEngine;

public class FishingSimpleManager : FishingManager
{
	public override bool isTutorialFinished
	{
		get
		{
			return true;
		}
		set
		{
			base.isTutorialFinished = true;
		}
	}

	public override void Init()
	{
		base.Init();
	}

	public override void InitFishing()
	{
		MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.FISHING_ENABLED);
		SetBaseSettingsFromModel();
	}

	protected override void Update()
	{
		if (!isFishingEnabled)
		{
			return;
		}
		if (playerFishingController == null)
		{
			GetFishingController();
		}
		if (currentFishingStatus == FishingStatus.WAIT)
		{
			if (!waitingForFishToCatch)
			{
				waitingTimeLeft = Random.Range(3f, 4f);
				waitingForFishToCatch = true;
			}
			if (waitingTimeLeft > 0f)
			{
				waitingTimeLeft -= Time.deltaTime;
			}
			else
			{
				waitingTimeLeft = 0f;
				fishOnHook = true;
				catchingFishProgress = 50f;
			}
		}
		if (currentFishingStatus == FishingStatus.FIGHT)
		{
			FishFight();
			if (catchingFishProgress >= 100f)
			{
				catchingFishProgress = 100f;
				FishCatched();
			}
			else if (catchingFishProgress <= 0f)
			{
				if (!isTutorialFinished)
				{
					catchingFishProgress = 0f;
				}
				else
				{
					catchingFishProgress = 0f;
					FishBreaks();
				}
			}
		}
		if (currentFishingStatus == FishingStatus.SUCCESS)
		{
			Vector3 position = fishCatchedTransform.position;
			Vector3 vector = CameraController.instance.MainCamera.ScreenToWorldPoint(Vector3.zero);
			float y = vector.y;
			Vector3 size = hookTransform.GetChild(0).GetComponentInChildren<Renderer>().bounds.size;
			position.y = y + size.y / 2f - 0.2f;
			hookTransform.localRotation = Quaternion.Lerp(hookTransform.localRotation, Quaternion.Euler(FishingManager.fishCatchedRotation), Time.deltaTime * 3f);
			hookTransform.position = Vector3.Lerp(hookTransform.position, position, Time.deltaTime * 3f);
			spawnedFishAnimator.SetTrigger("catched");
		}
	}

	private void GetFishingController()
	{
		if (!(CameraController.instance == null) && !(CameraController.instance.Anchor == null))
		{
			playerFishingController = CameraController.instance.Anchor.GetComponentInChildren<PlayerFishingController>();
			if ((bool)playerFishingController)
			{
				rodLineStartTransform = playerFishingController.rodLinePivot;
				hookTransform = playerFishingController.fishHookPivot;
				fishCatchedTransform = playerFishingController.fishCatchedPivot;
			}
		}
	}

	protected override void ThrowRod()
	{
		playerFishingController.ThrowRod();
		currentFishingStatus = FishingStatus.WAIT;
		if (hookTransform == null)
		{
			hookTransform = playerFishingController.fishHookPivot;
		}
		if (rodLineStartTransform == null)
		{
			rodLineStartTransform = playerFishingController.rodLinePivot;
		}
		hookTransform.localPosition = new Vector3(0f, 0f, distanceToOtherSide) / 2f + rodLineStartTransform.localPosition + rodLineStartOffset;
		Vector3 position = CameraController.instance.MainCamera.transform.position;
		position.y -= 2f;
		hookTransform.LookAt(position);
	}

	protected override void StartFighting()
	{
		fishConfigOnHook = fishesConfig[0];
		SpawnFish();
		playerFishingController.FishFight();
		currentFishingStatus = FishingStatus.FIGHT;
		fishFighting = true;
	}

	protected override void FishCatched()
	{
		fishOnHook = false;
		fishFighting = false;
		fishBreaks = false;
		fishCatched = true;
		playerFishingController.FishCatched();
		StartCoroutine(EnableFishingButton());
		fishingConnector.fightFishingButton.gameObject.SetActive(value: false);
		currentFishingStatus = FishingStatus.SUCCESS;
		Manager.Get<StatsManager>().MinigameFinished(StatsManager.MinigameType.FISH, success: true);
		Manager.Get<StatsManager>().FishingSuccess();
		GivePlayerTheFish();
	}

	protected void GivePlayerTheFish()
	{
		int num = 58;
		Singleton<PlayerData>.get.playerItems.AddToResources(num, 1);
		string text = Manager.Get<TranslationsManager>().GetText("item.received", string.Empty);
		string key = string.Format("{0}.{1}", "item.quest.name", num);
		string text2 = Manager.Get<TranslationsManager>().GetText(key, string.Empty);
		Manager.Get<ToastManager>().ShowToast(string.Format("{2} {0} (x{1})", text2, 1, text), 4f);
	}

	public override void EndFishing()
	{
		base.EndFishing();
	}

	protected override void EndShowing()
	{
		playerFishingController.EndShowing();
		waitingForFishToCatch = false;
		fishBreaks = false;
		fishCatched = false;
		currentFishingStatus = FishingStatus.WAIT;
		UnityEngine.Object.Destroy(spawnedFish.gameObject);
		hookTransform.localPosition = fishSpawnPosition / 2f + rodLineStartTransform.localPosition + rodLineStartOffset;
		hookTransform.localRotation = Quaternion.Euler(FishingManager.fishFightingRotation);
	}
}
