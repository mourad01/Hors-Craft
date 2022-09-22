// DecompilerFi decompiler from Assembly-CSharp.dll class: States.VignetteModule
using Common.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class VignetteModule : GameplayModule
	{
		public Image vignette;

		private HoverCar vehicle;

		protected override Fact[] listenedFacts => new Fact[1]
		{
			Fact.IN_VEHICLE
		};

		public override void Init()
		{
			base.Init();
		}

		protected override void Update()
		{
			if (vehicle != null)
			{
				vignette.color = new Color(1f, 1f, 1f, Easing.Ease(EaseType.OutQuad, 0f, 1f, vehicle.velocity / 5f));
			}
		}

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			InteractiveObjectContext factContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<InteractiveObjectContext>(Fact.IN_VEHICLE);
			if (factContext == null)
			{
				vignette.gameObject.SetActive(value: false);
				return;
			}
			vignette.gameObject.SetActive(value: true);
			vehicle = factContext.obj.GetComponentInChildren<HoverCar>();
		}
	}
}
