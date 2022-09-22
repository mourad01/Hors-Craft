// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.BlueprintController
using com.ootii.Cameras;
using Common.Managers;
using Gameplay;
using GameUI;
using States;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Uniblocks
{
	public class BlueprintController : MonoBehaviour
	{
		public GameObject highlightBlockPrefab;

		private const int RAYCAST_DISTANCE = 8;

		private BlueprintManager blueprintManager;

		private Renderer selectedBlockGraphic;

		private VoxelHoverAction voxelHoverAction;

		private Vector3 hitPosition;

		private bool useCutomHitPosition;

		private Vector3 customHitPosition;

		public GameObject highlightBlock;

		private bool blueprintActionsActivated;

		private BlueprintFillContext fillContext;

		private SurvivalPhaseContext survivalPhaseContext;

		private bool checkedTutorialShowing;

		public bool inBlueprintTutorial;

		private PlayerMovement _playerMovement;

		private PlayerController _playerController;

		private GameplayState _gameplayInstance;

		private Slider progressSlider;

		private Text progressText;

		private SimpleRepeatButton tapButton;

		private float tapmaxTime = 0.2f;

		private int pointerId;

		private float tapStartTime;

		private float blueprintLoadTime = 3f;

		private Transform cameraTransform => CameraController.instance.MainCamera.transform;

		private Vector3 forward => cameraTransform.transform.forward;

		private PlayerMovement playerMovement
		{
			get
			{
				if (_playerMovement == null)
				{
					_playerMovement = PlayerGraphic.GetControlledPlayerInstance().GetComponentInParent<PlayerMovement>();
				}
				return _playerMovement;
			}
		}

		private PlayerController playerController
		{
			get
			{
				if (_playerController == null)
				{
					_playerController = PlayerGraphic.GetControlledPlayerInstance().GetComponentInParent<PlayerController>();
				}
				return _playerController;
			}
		}

		private GameplayState gameplayInstance
		{
			get
			{
				if (_gameplayInstance == null)
				{
					_gameplayInstance = Manager.Get<StateMachineManager>().GetStateInstance<GameplayState>();
				}
				return _gameplayInstance;
			}
		}

		private void Awake()
		{
			blueprintManager = ((!Manager.Contains<BlueprintManager>()) ? null : Manager.Get<BlueprintManager>());
		}

		private void Init()
		{
			selectedBlockGraphic = GameObject.Find("SelectedBlock").GetComponent<Renderer>();
			if (Manager.Contains<SurvivalManager>())
			{
				voxelHoverAction = GetComponent<CameraEventsSender>().GetHoverAction<SurvivalVoxelHoverAction>();
				survivalPhaseContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<SurvivalPhaseContext>(Fact.SURVIVAL_PHASE);
			}
			else
			{
				voxelHoverAction = GetComponent<CameraEventsSender>().GetHoverAction<VoxelHoverAction>();
			}
		}

		private void Update()
		{
			if (blueprintManager == null)
			{
				return;
			}
			TryLoadingBlueprints();
			if (IsWithinBlueprintRange() && CanInteractWithBlueprints())
			{
				ActivateHighlightBlock();
				TryShowTutorial();
				if (!blueprintActionsActivated)
				{
					ActivateBlueprintNonAccurateActions();
					blueprintActionsActivated = true;
				}
			}
			else if (blueprintActionsActivated)
			{
				DeactivateBlueprintAccurateActions();
				DeactivateBlueprintNonAccurateActions();
				DeactivateHighlightBlock();
				blueprintActionsActivated = false;
			}
		}

		private bool CanInteractWithBlueprints()
		{
			return gameplayInstance.currentSubstate.substate == GameplayState.Substates.WALKING && !playerMovement.inCutscene && (survivalPhaseContext == null || !survivalPhaseContext.isCombat) && (!MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.IN_TUTORIAL) || inBlueprintTutorial);
		}

		private void TryShowTutorial()
		{
			if (!checkedTutorialShowing && PlayerPrefs.GetInt("blueprint.tutorial.showed", 0) == 0)
			{
				blueprintManager.ShowTutorial();
			}
			checkedTutorialShowing = true;
		}

		private void TryLoadingBlueprints()
		{
			blueprintLoadTime += Time.deltaTime;
			if (blueprintLoadTime > 5f)
			{
				StartCoroutine(blueprintManager.LoadBlueprints());
				blueprintLoadTime = 0f;
			}
		}

		private void ActivateBlueprintNonAccurateActions()
		{
			DeactivateBlueprintNonAccurateActions();
			Vector3 position = hitPosition;
			fillContext = new BlueprintFillContext
			{
				instantFill = OnFillInstantly,
				destroy = delegate
				{
					DeleteBlueprint(position);
				},
				setFillVoxelButton = delegate(SimpleRepeatButton srb)
				{
					tapButton = srb;
					srb.onFingerDown = (SimpleRepeatButton.OnFingerDown)Delegate.Combine(srb.onFingerDown, new SimpleRepeatButton.OnFingerDown(StartFillingVoxel));
					srb.onFingerUp = (SimpleRepeatButton.OnFingerDown)Delegate.Combine(srb.onFingerUp, new SimpleRepeatButton.OnFingerDown(CompleteFillingVoxel));
				},
				setProgressSlider = delegate(Slider slider, Text text)
				{
					progressSlider = slider;
					progressText = text;
				}
			};
			MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.IN_BLUEPRINT_RANGE, fillContext);
		}

		private void UpdateProgress(Vector3 position)
		{
			if (progressSlider != null)
			{
				PlacedBlueprint placedBlueprint = blueprintManager.GetPlacedBlueprint(position, accurate: true);
				if (placedBlueprint != null)
				{
					progressSlider.value = placedBlueprint.progress;
					progressText.text = $"{placedBlueprint.placedVoxels.Count.ToString()}/{placedBlueprint.placedBlueprintData.blocksToFillInBlueprint.ToString()}";
				}
			}
		}

		private void StartFillingVoxel(int id)
		{
			pointerId = id;
			tapStartTime = Time.unscaledTime;
		}

		private void CompleteFillingVoxel(int id)
		{
			fillContext.placedBlueprintVoxel = false;
			if (id == pointerId && Time.unscaledTime - tapStartTime <= tapmaxTime)
			{
				Vector2 position = UnityEngine.Input.GetTouch(id).position;
				Ray ray = CameraController.instance.MainCamera.ScreenPointToRay(position);
				Vector3 normalized = (highlightBlock.transform.position - ray.origin).normalized;
				float num = Vector3.Dot(ray.direction, normalized);
				if (num > 0.97f)
				{
					fillContext.placedBlueprintVoxel = true;
					blueprintManager.FillVoxelInPosition(hitPosition);
					return;
				}
				VoxelInfo voxelInfo = Engine.VoxelGridRaycastFor(ray.origin, ray.direction, 8f, Engine.usefulIDs.blueprintID);
				if (voxelInfo != null && voxelInfo.GetVoxel() == Engine.usefulIDs.blueprintID)
				{
					fillContext.placedBlueprintVoxel = true;
					blueprintManager.FillVoxelInPosition(Engine.VoxelInfoToPosition(voxelInfo));
					return;
				}
				VoxelInfo voxelInfo2 = Engine.VoxelGridRaycast(ray.origin, ray.direction, 8f);
				if (voxelInfo2 != null && blueprintManager.IsWithinBlueprintTriggerRange(Engine.VoxelInfoToPosition(voxelInfo2), 1))
				{
					fillContext.placedBlueprintVoxel = true;
				}
			}
			else
			{
				pointerId = -1;
			}
		}

		private void DeactivateBlueprintNonAccurateActions()
		{
			if (fillContext != null)
			{
				SimpleRepeatButton simpleRepeatButton = tapButton;
				simpleRepeatButton.onFingerDown = (SimpleRepeatButton.OnFingerDown)Delegate.Remove(simpleRepeatButton.onFingerDown, new SimpleRepeatButton.OnFingerDown(StartFillingVoxel));
				SimpleRepeatButton simpleRepeatButton2 = tapButton;
				simpleRepeatButton2.onFingerUp = (SimpleRepeatButton.OnFingerDown)Delegate.Remove(simpleRepeatButton2.onFingerUp, new SimpleRepeatButton.OnFingerDown(CompleteFillingVoxel));
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.IN_BLUEPRINT_RANGE, fillContext);
				fillContext = null;
			}
		}

		private void ActivateBlueprintAccurateActions()
		{
			if (!MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.MCPE_STEERING))
			{
				if (voxelHoverAction == null)
				{
					Init();
				}
				voxelHoverAction.SetCustomAddAction(OnAdd);
				voxelHoverAction.SetCustomDeleteAction(OnDelete);
				selectedBlockGraphic.enabled = false;
			}
		}

		private void DeactivateBlueprintAccurateActions()
		{
			if (voxelHoverAction != null)
			{
				voxelHoverAction.SetCustomAddAction(null);
				voxelHoverAction.SetCustomDeleteAction(null);
			}
		}

		public void ActivateHighlightBlock()
		{
			if (highlightBlock == null)
			{
				highlightBlock = UnityEngine.Object.Instantiate(highlightBlockPrefab);
			}
			VoxelInfo voxelInfo = Engine.VoxelGridRaycastFor(cameraTransform.transform.position, forward, 8f, Engine.usefulIDs.blueprintID);
			if (voxelInfo != null)
			{
				hitPosition = Engine.VoxelInfoToPosition(voxelInfo);
			}
			else
			{
				hitPosition = blueprintManager.GetDefaultBlockToBuild(playerController.transform.position);
			}
			highlightBlock.transform.position = hitPosition;
			UpdateProgress(hitPosition);
			TargetHighlightBlock();
		}

		private void DeactivateHighlightBlock()
		{
			if (highlightBlock != null)
			{
				UnityEngine.Object.Destroy(highlightBlock);
			}
		}

		private void OnDestroy()
		{
			DeactivateHighlightBlock();
		}

		private void TargetHighlightBlock()
		{
			Vector3 a = highlightBlock.transform.position - cameraTransform.transform.position;
			float magnitude = a.magnitude;
			if (magnitude > 8f)
			{
				DeactivateBlueprintAccurateActions();
				return;
			}
			a /= magnitude;
			float num = Vector3.Dot(forward, a);
			if (num > 0.93f)
			{
				ActivateBlueprintAccurateActions();
			}
			else
			{
				DeactivateBlueprintAccurateActions();
			}
		}

		private void OnAdd()
		{
			blueprintManager.FillVoxelInPosition(hitPosition);
		}

		public void DeleteBlueprint(Vector3 position)
		{
			if (CanInteractWithBlueprints())
			{
				useCutomHitPosition = true;
				customHitPosition = position;
				OnDelete();
			}
		}

		private void OnDelete()
		{
			if (CanInteractWithBlueprints())
			{
				Manager.Get<StateMachineManager>().PushState<GenericPopupState>(new GenericPopupStateStartParameter
				{
					configureMessage = delegate(TranslateText t)
					{
						t.translationKey = "blueprint.delete";
						t.defaultText = "Are you sure you want to delete this blueprint?";
					},
					configureLeftButton = delegate(Button b, TranslateText t)
					{
						t.translationKey = "dating.no";
						t.defaultText = "no";
						b.onClick.AddListener(delegate
						{
							useCutomHitPosition = false;
							Manager.Get<StateMachineManager>().PopState();
						});
					},
					configureRightButton = delegate(Button b, TranslateText t)
					{
						t.translationKey = "dating.yes";
						t.defaultText = "yes";
						b.onClick.AddListener(delegate
						{
							blueprintManager.DeleteBlueprint((!useCutomHitPosition) ? hitPosition : customHitPosition);
							useCutomHitPosition = false;
							Manager.Get<StateMachineManager>().PopState();
						});
					}
				});
			}
		}

		private void OnFillInstantly()
		{
			if (Manager.Contains<TicketsManager>())
			{
				FillForTickets();
				return;
			}
			PlacedBlueprint placedBlueprint = Manager.Get<BlueprintManager>().GetPlacedBlueprint(hitPosition, accurate: true);
			Manager.Get<StateMachineManager>().PushState<WatchXAdsPopUpState>(new WatchXAdsPopUpStateStartParameter
			{
				description = "Watch {0} ads to fill blueprint instantly",
				translationKey = "blueprint.fill.multi.ads",
				numberOfAdsNeeded = Mathf.CeilToInt((1f - placedBlueprint.progress) / (1f / (float)placedBlueprint.adsToFillBlueprint)),
				type = AdsCounters.None,
				configWatchButton = delegate(GameObject go)
				{
					TranslateText componentInChildren = go.GetComponentInChildren<TranslateText>();
					componentInChildren.defaultText = "watch";
					componentInChildren.translationKey = "menu.watch";
					componentInChildren.ForceRefresh();
				},
				onSuccess = delegate
				{
					InstantFillBlueprint();
				},
				reason = StatsManager.AdReason.XCRAFT_BLUEPRINT_FILL
			});
		}

		private void FillForTickets()
		{
			int amount = Manager.Get<ModelManager>().ticketsSettings.GetFillBlueprintPrice();
			if (Manager.Get<TicketsManager>().ownedTickets < amount)
			{
				Manager.Get<TicketsManager>().OnAddTickets();
			}
			else
			{
				Manager.Get<StateMachineManager>().PushState<GenericPopupState>(new GenericPopupStateStartParameter
				{
					configureMessage = delegate(TranslateText t)
					{
						t.translationKey = "fill.blueprint.for.resource";
						t.defaultText = "Do you want to fill blueprint for {0} tickets?";
						t.AddVisitor((string tt) => tt.Replace("{0}", amount.ToString()));
					},
					configureLeftButton = delegate(Button b, TranslateText t)
					{
						t.translationKey = "menu.cancel";
						t.defaultText = "cancel";
						b.onClick.AddListener(delegate
						{
							Manager.Get<StateMachineManager>().PopState();
						});
					},
					configureRightButton = delegate(Button b, TranslateText t)
					{
						t.translationKey = "menu.ok";
						t.defaultText = "ok";
						b.onClick.AddListener(delegate
						{
							OnBuy(amount);
							Manager.Get<StateMachineManager>().PopState();
						});
					}
				});
			}
		}

		private void OnBuy(int amount)
		{
			Manager.Get<TicketsManager>().ownedTickets -= amount;
			InstantFillBlueprint();
		}

		private void InstantFillBlueprint()
		{
			Manager.Get<BlueprintManager>().InstantFillAllBlueprintAccurate(hitPosition);
		}

		private bool IsWithinBlueprintRange()
		{
			return blueprintManager.IsWithinBlueprintTriggerRange(playerController.transform.position);
		}
	}
}
