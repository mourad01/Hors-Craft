// DecompilerFi decompiler from Assembly-CSharp.dll class: States.GameplayState
using com.ootii.Cameras;
using Common.Behaviours;
using Common.Crosspromo;
using Common.Managers;
using Common.Managers.States;
using Common.Utils;
using Gameplay;
using GameUI;
using System;
using System.Collections.Generic;
using System.Linq;
using Uniblocks;
using UnityEngine;
using UnityEngine.UI;
using UnityToolbag;

namespace States
{
	public class GameplayState : XCraftUIState<GameplayStateConnector>
	{
		public enum Substates
		{
			WALKING,
			VEHICLE,
			FLYING_VEHICLE,
			MINIGAME,
			SURVIVAL_COMBAT,
			SURVIVAL_VEHICLE_COMBAT,
			SURVIVAL_FLYING_VEHICLE_COMBAT
		}

		[Space]
		public TopNotification petNotificationPrefab;

		public GameObject questNotification;

		public bool hasToShowBody = true;

		public Vector2 crosspromoLocation = new Vector2(1f, 0.5f);

		public bool useAlphaButtons;

		public PatternRepeater alphaCondition;

		public float alphaValue = 0.6f;

		[Space]
		[Header("Initial popups")]
		[Reorderable]
		public List<GameplayPopupsController.ActivationTimePopup> activationTimePopups;

		[Reorderable]
		public List<GameplayPopupsController.QueuePopup> queuePopups;

		[Reorderable]
		public List<GameplayPopupsController.NotDisposablePopup> notDisposablePopups;

		private List<GameplaySubstate> substates = new List<GameplaySubstate>();

		private GameplayPopupsController popupsController;

		private TopNotification _notificationInstance;

		private TopNotification _petNotificationInstance;

		public Action<int, ResourceSprite> onResourceGathered;

		public GameplaySubstate currentSubstate
		{
			get;
			private set;
		}

		protected override bool hasBackground => false;

		protected override bool hasBackgroundOverlay => false;

		private TopNotification notificationInstance
		{
			get
			{
				if (_notificationInstance == null && questNotification != null)
				{
					_notificationInstance = UnityEngine.Object.Instantiate(questNotification.gameObject).GetComponentInChildren<TopNotification>();
				}
				return _notificationInstance;
			}
		}

		private TopNotification petNotificationInstance
		{
			get
			{
				if (_petNotificationInstance == null && petNotificationPrefab != null)
				{
					_petNotificationInstance = UnityEngine.Object.Instantiate(petNotificationPrefab.gameObject).GetComponent<TopNotification>();
				}
				return _petNotificationInstance;
			}
		}

		public List<GameplayModule> GetModules()
		{
			return base.connector.GetComponentsInChildren<GameplayModule>(includeInactive: true).ToList();
		}

		public T GetModule<T>() where T : GameplayModule
		{
			return base.connector.GetComponentsInChildren<GameplayModule>(includeInactive: true).FirstOrDefault((GameplayModule module) => module is T) as T;
		}

		public void SetSubstate(Substates substate, bool force = false)
		{
			if (!force && currentSubstate != null && substate == currentSubstate.substate)
			{
				UnityEngine.Debug.LogWarning("Tried to load the same substate (" + substate.ToString() + "). Aborting...");
				return;
			}
			GameplaySubstate gameplaySubstate = substates.FirstOrDefault((GameplaySubstate f) => f.substate == substate);
			if (gameplaySubstate == null)
			{
				UnityEngine.Debug.LogWarning("Tried to load not-existing substate (" + substate.ToString() + "). Aborting...");
				return;
			}
			gameplaySubstate.Activate(base.connector.transform.GetComponentsInChildren<ModuleLoader>().ToList());
			currentSubstate = gameplaySubstate;
		}

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			TimeScaleHelper.value = 1f;
			MonoBehaviourSingleton<GameplayFacts>.get.SetFact(Fact.LOADING, enabled: true);
			ShowCrosspromo();
			InitDevMode();
			InitSubstates(force: true);
			Manager.Get<GameCallbacksManager>().GameplayStarted();
			CheckSaveParameter(parameter);
			CheckScreenMaking();
			MonoBehaviourSingleton<GameplayFacts>.get.SetFact(Fact.LOADING, enabled: false);
			InitGameplay();
			InitPopupController();
			ShowCutScene();
		}

		private void InitDevMode()
		{
			UpdateDevMode();
			MonoBehaviourSingleton<DeveloperModeBehaviour>.get.AddOnActivatedCallback(delegate
			{
				if (GlobalSettings.mode != GlobalSettings.MovingMode.FLYING)
				{
					GlobalSettings.SwitchMoveMode();
				}
				MonoBehaviourSingleton<GameplayFacts>.get.AddFactPersistent(Fact.DEV_ENABLED);
				PlayerPrefs.SetInt("debugFpsCounter", 1);
				MonoBehaviourSingleton<GameplayFacts>.get.AddFactPersistent(Fact.FPS_ENABLED);
			});
		}

		private void InitGameplay()
		{
			if (!ChooseWorldReq.ShowChooseWorld())
			{
				Manager.Get<SavedWorldManager>().OnPlayWorld(Manager.Get<SavedWorldManager>().GetCurrentWorld().uniqueId);
			}
			GlobalSettings.UpdateMoveModeFact();
			MouseLock(value: true);
			SetFPSMode(GameMode.Gameplay);
			Shader.SetGlobalFloat("_SSAO", 1f);
		}

		private void InitSubstates(bool force = false)
		{
			substates = GetComponentsInChildren<GameplaySubstate>().ToList();
			Substates substate = substates[0].substate;
			SetSubstate(substate, force);
		}

		private void InitPopupController()
		{
			GameObject gameObject = new GameObject("Popup controller");
			gameObject.transform.SetParent(base.connector.transform, worldPositionStays: false);
			popupsController = gameObject.AddComponent<GameplayPopupsController>();
			activationTimePopups.ForEach(delegate(GameplayPopupsController.ActivationTimePopup p)
			{
				popupsController.AddPopup(p);
			});
			queuePopups.ForEach(delegate(GameplayPopupsController.QueuePopup p)
			{
				popupsController.AddPopup(p);
			});
			notDisposablePopups.ForEach(delegate(GameplayPopupsController.NotDisposablePopup p)
			{
				popupsController.AddPopup(p);
			});
		}

		private void CheckSaveParameter(StartParameter parameter)
		{
			ShouldSaveParameter shouldSaveParameter = parameter as ShouldSaveParameter;
			if (shouldSaveParameter != null && shouldSaveParameter.shouldSaveWorld)
			{
				Manager.Get<SavedWorldManager>().SaveWorldAndData();
			}
		}

		private void CheckScreenMaking()
		{
			if (Manager.Contains<SavedWorldManager>())
			{
				Manager.Get<SavedWorldManager>().CheckIfMakeScreenShoot();
			}
		}

		public override void UpdateState()
		{
			base.UpdateState();
			if (Engine.EditMode)
			{
				UpdateEditMode();
			}
			UpdateDidYouKnow();
		}

		private void UpdateDidYouKnow()
		{
			if (Manager.Get<ModelManager>().timeBasedDidYouKnow.HasToShowDidYouKnow(Time.timeSinceLevelLoad))
			{
				Manager.Get<StateMachineManager>().PushState<DidYouKnowPopupState>();
			}
		}

		public override void FreezeState()
		{
			TimeScaleHelper.value = 0f;
			HideNotificationImmediately();
			HideBlockAnimationImmiediately();
			if (Manager.Contains<ProgressManager>())
			{
				Manager.Get<ProgressManager>().HideLevelUpNotificationImmediately();
			}
			DisableUserMovement();
			MouseLock(value: false);
			Application.targetFrameRate = 20;
			base.FreezeState();
		}

		private void HideNotificationImmediately()
		{
			if (notificationInstance != null)
			{
				notificationInstance.HideImmediately();
			}
		}

		private void HideBlockAnimationImmiediately()
		{
			if (base.connector.resourceGatherSpot != null)
			{
				base.connector.resourceGatherSpot.HideImmediately();
			}
		}

		public void EnableUserMovement()
		{
			PlayerGraphic controlledPlayerInstance = PlayerGraphic.GetControlledPlayerInstance();
			if (!(controlledPlayerInstance == null))
			{
				PlayerMovement componentInParent = controlledPlayerInstance.GetComponentInParent<PlayerMovement>();
				componentInParent.EnableMovement(enable: true);
				controlledPlayerInstance.TryToEnableHeadThings(CameraController.instance.currentCameraPreset != CameraController.CameraPresets.FPP);
				if (hasToShowBody)
				{
					controlledPlayerInstance.ShowBodyAndLegs();
				}
				else
				{
					controlledPlayerInstance.HideBodyAndLegs();
				}
			}
		}

		public void DisableUserMovement()
		{
			PlayerGraphic controlledPlayerInstance = PlayerGraphic.GetControlledPlayerInstance();
			if (!(controlledPlayerInstance == null))
			{
				PlayerMovement componentInParent = controlledPlayerInstance.GetComponentInParent<PlayerMovement>();
				if (!(componentInParent == null))
				{
					componentInParent.EnableMovement(enable: false);
				}
			}
		}

		public override void UnfreezeState()
		{
			TimeScaleHelper.value = 1f;
			EnableUserMovement();
			UpdateDevMode();
			ShowCrosspromo();
			GlobalSettings.LoadSettings();
			MouseLock(value: true);
			base.UnfreezeState();
			Application.targetFrameRate = 30;
			MonoBehaviourSingleton<GameplayFacts>.get.SetFact(Fact.LOADING, enabled: false);
			SetFPSMode(GameMode.Gameplay);
			if (MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.WAS_IN_PAUSE))
			{
				MonoBehaviourSingleton<GameplayFacts>.get.SetFact(Fact.WAS_IN_PAUSE, enabled: false);
			}
		}

		public override void FinishState()
		{
			TimeScaleHelper.value = 0f;
			MouseLock(value: false);
			base.FinishState();
		}

		protected virtual void ShowCrosspromo()
		{
			Manager.Get<CrosspromoManager>().TryToShowIconAt(Manager.Get<CanvasManager>().canvas, this, crosspromoLocation);
		}

		private void UpdateEditMode()
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.Backspace))
			{
				Manager.Get<CraftingManager>().SpawnRandomResource(PlayerGraphic.GetControlledPlayerInstance().transform.position, spawnWithRandomForce: true);
			}
			if (GlobalSettings.mode != GlobalSettings.MovingMode.FLYING)
			{
				GlobalSettings.mode = GlobalSettings.MovingMode.FLYING;
			}
		}

		public void OnResourceGathered(int resourceId, ResourceSprite sprite = null)
		{
			if (base.connector.resourceGatherSpot != null || !Manager.Get<ModelManager>().craftingSettings.IsCraftingEnabled())
			{
				base.connector.resourceGatherSpot.Show(new ResourceGatheredAnimator.ResourceInformation(resourceId));
			}
			if (onResourceGathered != null)
			{
				onResourceGathered(resourceId, sprite);
			}
		}

		public void OnCraftableGathered(ushort blockId)
		{
			if (base.connector.resourceGatherSpot != null || !Manager.Get<ModelManager>().craftingSettings.IsCraftingEnabled())
			{
				base.connector.resourceGatherSpot.Show(new ResourceGatheredAnimator.CraftableInformation(blockId));
			}
		}

		public void ShowBlockNotification(string text, int craftableid)
		{
			if (!(notificationInstance == null) && Manager.Get<ModelManager>().craftingSettings.IsCraftingEnabled())
			{
				QuestNotification.QuestUnlockInfo questUnlockInfo = new QuestNotification.QuestUnlockInfo();
				questUnlockInfo.information = text;
				questUnlockInfo.id = craftableid;
				questUnlockInfo.setOnTop = true;
				questUnlockInfo.timeToHide = 1.5f;
				QuestNotification.QuestUnlockInfo information = questUnlockInfo;
				notificationInstance.Show(information);
				notificationInstance.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
				Vector3 position = GetPauseButton().transform.position;
				float y = position.y;
				notificationInstance.gameObject.transform.SetPositionY(y);
			}
		}

		public void ShowPetUnlockedNotification(string text, Sprite icon)
		{
			if (!(petNotificationInstance == null))
			{
				PetUnlockedNotification.PetUnlockedInformation petUnlockedInformation = new PetUnlockedNotification.PetUnlockedInformation();
				petUnlockedInformation.information = text;
				petUnlockedInformation.icon = icon;
				petUnlockedInformation.setOnTop = true;
				petUnlockedInformation.timeToHide = 1.5f;
				PetUnlockedNotification.PetUnlockedInformation information = petUnlockedInformation;
				petNotificationInstance.Show(information);
				petNotificationInstance.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
				Vector3 position = GetPauseButton().transform.position;
				float y = position.y;
				petNotificationInstance.gameObject.transform.SetPositionY(y);
			}
		}

		public void ShowQuestCompleteNotification(QuestType type)
		{
			if (!(notificationInstance == null))
			{
				QuestNotification.QuestUnlockInfo questUnlockInfo = new QuestNotification.QuestUnlockInfo();
				questUnlockInfo.information = Manager.Get<TranslationsManager>().GetText("quest.completed", "Quest completed!");
				questUnlockInfo.id = (int)type;
				questUnlockInfo.setOnTop = true;
				questUnlockInfo.timeToHide = 1.5f;
				QuestNotification.QuestUnlockInfo information = questUnlockInfo;
				notificationInstance.Show(information);
			}
		}

		private Button GetPauseButton()
		{
			return (GetModules().FirstOrDefault((GameplayModule m) => m is PauseModule) as PauseModule)?.pauseButton;
		}

		public void HideUI(bool useAction)
		{
			currentSubstate.Hide();
		}

		public void ShowUI()
		{
			currentSubstate.Show();
		}

		private void ShowCutScene()
		{
			if (Manager.Contains<CutScenesManager>())
			{
				CutScenesManager cutScenesManager = Manager.Get<CutScenesManager>();
				if (!(cutScenesManager == null))
				{
					cutScenesManager.StartIntroScene();
				}
			}
		}

		public virtual void OnPause()
		{
			string categoryToOpen = null;
			PauseState pauseState = Manager.Get<StateMachineManager>().GetStateInstance(typeof(PauseState)) as PauseState;
			if (pauseState.enableCollections)
			{
				categoryToOpen = "Collections";
			}
			else if (pauseState.enableHostpitalUpgrades)
			{
				categoryToOpen = "HospitalUpgrades";
			}
			Manager.Get<StateMachineManager>().PushState<PauseState>(new PauseStateStartParameter
			{
				canSave = true,
				allowTimeChange = true,
				categoryToOpen = categoryToOpen
			});
		}

		private void UpdateDevMode()
		{
			if (Engine.EditMode)
			{
				UpdateEditMode();
			}
		}

		protected virtual void DevHideUI()
		{
			currentSubstate.Hide();
			Manager.Get<CrosspromoManager>().DisposeRunningInstance();
		}

		private void MouseLock(bool value)
		{
		}
	}
}
