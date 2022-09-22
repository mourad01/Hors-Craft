// DecompilerFi decompiler from Assembly-CSharp.dll class: States.IsometricObjectPlacementState
using Common.Managers;
using Common.Managers.States;
using Common.Utils;
using GameUI;
using Uniblocks;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class IsometricObjectPlacementState : XCraftUIState<IsometricObjectPlacementStateConnector>
	{
		public GameObject cameraPrefab;

		public GameObject distanceBarrierPrefab;

		private IsometricObjectPlacementStateStartParameter startParameter;

		private IsometricPlaceableObject placedObject;

		private IsometricCamera cam;

		private PlayerGraphic player;

		private Vector3 playerPosition;

		private GameObject distanceBarrier;

		protected override bool hasBackground => false;

		protected override bool hasBackgroundOverlay => false;

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			Engine.doAnimate = false;
			startParameter = (parameter as IsometricObjectPlacementStateStartParameter);
			TimeScaleHelper.value = 0f;
			InitPlacedObject();
			InitCamera();
			DeactivatePlayer();
			InitDistanceBarrier();
			InitButtons();
			Engine.EngineInstance.GetComponent<GreedyMeshCreator>().useThreading = false;
		}

		public override void FinishState()
		{
			base.FinishState();
			Engine.doAnimate = true;
			Engine.EngineInstance.GetComponent<GreedyMeshCreator>().useThreading = true;
		}

		private void InitButtons()
		{
			base.connector.onAcceptButton = OnAcceptPlacement;
			base.connector.onBackButton = OnCancel;
			base.connector.onRotateButton = OnRotate;
			if (!startParameter.canRotate)
			{
				base.connector.rotateButton.gameObject.SetActive(value: false);
			}
		}

		private void InitPlacedObject()
		{
			placedObject = startParameter.obj;
			placedObject.SetAnalog(base.connector.analog, base.connector.analogButton);
		}

		private void DeactivatePlayer()
		{
			player = PlayerGraphic.GetControlledPlayerInstance();
			playerPosition = player.transform.position;
			player.gameObject.SetActive(value: false);
		}

		private void InitCamera()
		{
			GameObject gameObject = Object.Instantiate(cameraPrefab);
			cam = gameObject.GetComponent<IsometricCamera>();
			IsometricCamera isometricCamera = cam;
			SimpleRepeatButton dragButton = base.connector.dragButton;
			Transform transform = placedObject.transform;
			Vector3 size = placedObject.GetBounds().size;
			isometricCamera.Init(dragButton, transform, size.y);
			placedObject.SetCamera(cam);
		}

		private void InitDistanceBarrier()
		{
			distanceBarrier = Object.Instantiate(distanceBarrierPrefab);
			distanceBarrier.transform.localScale *= 32f;
			Vector3 position = playerPosition + new Vector3(1f, 0f, -1f);
			if (Physics.Raycast(playerPosition, Vector3.down, out RaycastHit hitInfo, 100f))
			{
				Vector3 point = hitInfo.point;
				position.y = point.y;
			}
			distanceBarrier.transform.position = position;
		}

		private void OnCancel()
		{
			Manager.Get<StateMachineManager>().PushState<GenericPopupState>(new GenericPopupStateStartParameter
			{
				configureMessage = delegate(TranslateText t)
				{
					if (placedObject is IsometricPlaceableCraft)
					{
						t.translationKey = "leave.isometric";
						t.defaultText = "Do you want to cancel object placement?";
					}
					else
					{
						t.translationKey = "leave.blueprint";
						t.defaultText = "Do you want to cancel blueprint placement?";
					}
				},
				configureLeftButton = delegate(Button b, TranslateText t)
				{
					t.translationKey = "dating.no";
					t.defaultText = "no";
					b.onClick.AddListener(delegate
					{
						Manager.Get<StateMachineManager>().PopState();
					});
				},
				configureRightButton = delegate(Button b, TranslateText t)
				{
					t.translationKey = "dating.yes";
					t.defaultText = "yes";
					b.onClick.AddListener(delegate
					{
						placedObject.OnDelete();
						GoToGameplay();
					});
				}
			});
		}

		private void OnRotate()
		{
			placedObject.OnRotate();
		}

		private void OnAcceptPlacement()
		{
			if (placedObject.OnPlace())
			{
				GoToGameplay();
			}
		}

		private void GoToGameplay()
		{
			UnityEngine.Object.Destroy(cam.gameObject);
			UnityEngine.Object.Destroy(distanceBarrier);
			TimeScaleHelper.value = 1f;
			player.gameObject.SetActive(value: true);
			Manager.Get<StateMachineManager>().PopStatesUntil<GameplayState>();
		}

		public void ShowAcceptButton(bool show)
		{
			base.connector.acceptButton.gameObject.SetActive(show);
		}

		public void ShowErrorMessage(bool show)
		{
			base.connector.errorMessage.SetActive(show);
		}

		public void SetupErrorMessage(string key, string defaultText)
		{
			TranslateText componentInChildren = base.connector.errorMessage.GetComponentInChildren<TranslateText>();
			componentInChildren.translationKey = key;
			componentInChildren.defaultText = defaultText;
			componentInChildren.ForceRefresh();
		}
	}
}
