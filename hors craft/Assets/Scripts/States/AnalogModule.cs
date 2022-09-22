// DecompilerFi decompiler from Assembly-CSharp.dll class: States.AnalogModule
using GameUI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace States
{
	public class AnalogModule : GameplayModule
	{
		public AnalogController analog;

		public SimpleRepeatButton analogButton;

		public SimpleRepeatButton jumpButton;

		private MovementContext movement;

		private float jumpPressedTime;

		private float lastJumpTime;

		protected override Fact[] listenedFacts => new Fact[3]
		{
			Fact.MAIN_ANALOG,
			Fact.MCPE_STEERING,
			Fact.MOVEMENT
		};

		public override void Init()
		{
			base.Init();
		}

		public override void OnFactsChanged(HashSet<Fact> facts)
		{
			bool flag = true;
			McpeContext factContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<McpeContext>(Fact.MCPE_STEERING);
			flag = (factContext?.flyInCameraDirection ?? true);
			if (base.gameObject.activeSelf != flag)
			{
				base.gameObject.SetActive(flag);
			}
			if (factContext != null && factContext.flyInCameraDirection && movement == null && jumpButton != null)
			{
				movement = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<MovementContext>(Fact.MOVEMENT);
				if (movement != null)
				{
					movement.setFallButton(IsFallPressed);
					movement.setJumpButton(IsJumpPressed);
					analog.gaugeTransform.sizeDelta = new Vector2(120f, 120f);
				}
			}
			if (flag)
			{
				SetAnalogController();
			}
		}

		private void SetAnalogController()
		{
			MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<AnalogInputContext>(Fact.MAIN_ANALOG).FirstOrDefault()?.setAnalogController(analog, analogButton);
		}

		protected override void Update()
		{
			if (jumpButton != null && jumpButton.pressed)
			{
				jumpPressedTime += Time.deltaTime;
			}
		}

		private bool IsFallPressed()
		{
			return false;
		}

		private bool IsJumpPressed()
		{
			if (Time.realtimeSinceStartup - lastJumpTime < 0.1f)
			{
				return true;
			}
			if (!jumpButton.pressed && jumpPressedTime > 0f)
			{
				if (jumpPressedTime < 0.35f)
				{
					jumpPressedTime = 0f;
					lastJumpTime = Time.realtimeSinceStartup;
					return true;
				}
				jumpPressedTime = 0f;
				return false;
			}
			return false;
		}
	}
}
