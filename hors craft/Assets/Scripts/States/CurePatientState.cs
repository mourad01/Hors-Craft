// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CurePatientState
using com.ootii.Cameras;
using Common.Managers;
using Common.Managers.States;
using Common.Utils;
using Gameplay;
using System;
using Uniblocks;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class CurePatientState : XCraftUIState<CurePatientStateConnector>
	{
		public class CurePatientStartParameter : StartParameter
		{
			public int difficultyLevel;

			public float addingSpeed;

			public float losingSpeed;

			public float startValue;

			public Action onWin;

			public Action onFail;
		}

		private float virusTargetValue;

		private float medTargetValue;

		private float virusCurValue;

		private float medCurValue;

		public bool activeFight;

		private CurePatientStartParameter _parameter;

		protected override bool hasBackground => false;

		protected override bool hasBackgroundOverlay => false;

		protected override AutoAdsConfig autoAdsConfig
		{
			get
			{
				AutoAdsConfig autoAdsConfig = new AutoAdsConfig();
				autoAdsConfig.autoShowOnStart = false;
				autoAdsConfig.properAdReason = StatsManager.AdReason.XCRAFT_CURE;
				return autoAdsConfig;
			}
		}

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			TimeScaleHelper.value = 0f;
			_parameter = (CurePatientStartParameter)parameter;
			virusTargetValue = (virusCurValue = base.connector.miniGameSlider.maxValue * 0.8f);
			SetupConnector();
			SetupCamera();
		}

		public override void UpdateState()
		{
			if (base.connector.fightCureButton.pressed)
			{
				activeFight = true;
				base.connector.pullPulse.StopEffect();
				base.connector.actionCureButton.GetComponentInChildren<Button>().enabled = false;
				OnCure();
			}
			if (activeFight)
			{
				if (virusCurValue == virusTargetValue)
				{
					virusTargetValue = virusCurValue + UnityEngine.Random.Range(-0.3f, 0.3f);
					virusTargetValue = Mathf.Clamp01(virusTargetValue);
				}
				virusCurValue = Mathf.MoveTowards(virusCurValue, virusTargetValue, Time.unscaledDeltaTime * (0.1f * (float)_parameter.difficultyLevel / 2f));
				medTargetValue -= 0.05f;
				medTargetValue = Mathf.Clamp01(medTargetValue);
				medCurValue = Mathf.MoveTowards(medCurValue, medTargetValue, Time.unscaledDeltaTime * 0.45f);
				base.connector.miniGameSlider.value = medCurValue;
				base.connector.SetVirusPivotPosition(virusCurValue);
				base.connector.SetMedAreaColor();
				if (base.connector.progressSlider.value >= 100f && _parameter.onWin != null)
				{
					_parameter.onWin();
				}
				else if (base.connector.progressSlider.value <= 1E-06f && _parameter.onFail != null)
				{
					_parameter.onFail();
				}
			}
		}

		public override void FreezeState()
		{
			if (CameraController.instance.Anchor != null)
			{
				PlayerMovement component = CameraController.instance.Anchor.GetComponent<PlayerMovement>();
				if (component != null)
				{
					component.EnableMovement(enable: false);
				}
			}
			base.FreezeState();
		}

		public override void FinishState()
		{
			base.FinishState();
			activeFight = false;
		}

		private void OnReturn()
		{
			Manager.Get<StateMachineManager>().PopState();
		}

		private void OnCure()
		{
			medTargetValue += 0.08f;
		}

		public GameObject GetCureButton()
		{
			return base.connector.actionCureButton.gameObject;
		}

		private void SetupCamera()
		{
			Vector3 eulerAngles = CameraController.instance.MainCamera.transform.parent.rotation.eulerAngles;
			eulerAngles.x = 10f;
			CameraController.instance.MainCamera.transform.parent.rotation = Quaternion.Euler(eulerAngles);
		}

		private void SetupConnector()
		{
			base.connector.miniGamePanel.updateMode = AnimatorUpdateMode.UnscaledTime;
			base.connector.onReturnButtonClicked = OnReturn;
			base.connector.onCureButtonClicked = OnCure;
			base.connector.addingSpeed = _parameter.addingSpeed / (float)_parameter.difficultyLevel;
			base.connector.losingSpeed = _parameter.losingSpeed / (float)_parameter.difficultyLevel;
			base.connector.miniGamePanel.SetBool("PanelOn", value: true);
			base.connector.SetMedWidth(Manager.Get<ModelManager>().hospitalSettings.GetMedWith(), _parameter.difficultyLevel);
			base.connector.progressSlider.value = _parameter.startValue;
			base.connector.SetVirusPivotPosition(virusCurValue);
			base.connector.miniGameSlider.value = medCurValue;
			base.connector.actionCureButton.GetComponentInChildren<Button>().enabled = true;
		}
	}
}
