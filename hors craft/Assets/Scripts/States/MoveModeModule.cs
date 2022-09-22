// DecompilerFi decompiler from Assembly-CSharp.dll class: States.MoveModeModule
using Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class MoveModeModule : GameplayModule
	{
		public GameObject go;

		public GameObject walkIcon;

		public GameObject flyIcon;

		public Slider timeLeftSlider;

		public Text timeLeftText;

		public GameObject lockObject;

		public float maxFill = 0.9f;

		public float minFill = 0.4f;

		public bool useFlyText;

		protected override Fact[] listenedFacts => new Fact[3]
		{
			Fact.MOVE_MODE,
			Fact.SURVIVAL_MODE_ENABLED,
			Fact.DEV_ENABLED
		};

		public override void Init()
		{
			base.Init();
		}

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			if ((MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.SURVIVAL_MODE_ENABLED) || MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.ADVENTURE_MODE_ENABLED)) && !MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.DEV_ENABLED))
			{
				go.SetActive(value: false);
				return;
			}
			go.SetActive(value: true);
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: true);
			}
			MoveModeContext context = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<MoveModeContext>(Fact.MOVE_MODE).FirstOrDefault();
			if (context != null)
			{
				go.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
				go.GetComponentInChildren<Button>().onClick.AddListener(delegate
				{
					context.onMoveModeChange();
				});
				if (context.mode == GlobalSettings.MovingMode.FLYING)
				{
					flyIcon.gameObject.SetActive(value: false);
					walkIcon.gameObject.SetActive(value: true);
				}
				else if (context.mode == GlobalSettings.MovingMode.WALKING)
				{
					flyIcon.gameObject.SetActive(value: true);
					walkIcon.gameObject.SetActive(value: false);
				}
			}
		}

		protected override void Update()
		{
			base.Update();
			if (GlobalSettings.CanFlyForAd())
			{
				if (lockObject.activeSelf)
				{
					lockObject.SetActive(value: false);
				}
				timeLeftSlider.value = maxFill - GlobalSettings.GetFlyingModeTimeLeft() / GlobalSettings.GetMaxFlyingModeTimeLeft() * (maxFill - minFill);
				TimeSpan timeSpan = TimeSpan.FromSeconds(GlobalSettings.GetFlyingModeTimeLeft());
				string text = string.Format("{0}:{1}", timeSpan.Minutes.ToString("00"), timeSpan.Seconds.ToString("00"));
				timeLeftText.text = text;
				return;
			}
			timeLeftSlider.value = 0f;
			if (GlobalSettings.IsFlyingForAdEnabled())
			{
				if (!lockObject.activeSelf)
				{
					lockObject.SetActive(value: true);
				}
			}
			else if (lockObject.activeSelf)
			{
				lockObject.SetActive(value: false);
			}
			if (useFlyText)
			{
				timeLeftText.text = "FLY";
			}
			else
			{
				timeLeftText.text = string.Empty;
			}
		}
	}
}
