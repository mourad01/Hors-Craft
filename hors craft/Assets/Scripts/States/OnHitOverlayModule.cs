// DecompilerFi decompiler from Assembly-CSharp.dll class: States.OnHitOverlayModule
using Common.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace States
{
	public class OnHitOverlayModule : GameplayModule
	{
		private const float ON_HIT_FLASH_DURATION = 0.3f;

		public CanvasGroup onHitOverlay;

		private float onHitFlashStartTime;

		protected override Fact[] listenedFacts => new Fact[1]
		{
			Fact.GOT_HIT
		};

		public override void Init()
		{
			base.Init();
		}

		public override void OnFactsChanged(HashSet<Fact> facts)
		{
			if (MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.GOT_HIT))
			{
				GotHitContext gotHitContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<GotHitContext>(Fact.GOT_HIT).FirstOrDefault();
				OnHitOverlay(gotHitContext.angle);
			}
		}

		private void OnHitOverlay(float angle)
		{
			onHitOverlay.gameObject.SetActive(value: true);
			onHitOverlay.transform.Find("Rotate").eulerAngles = Vector3.forward * angle;
			onHitOverlay.alpha = 0.75f;
			onHitFlashStartTime = Time.time;
		}

		protected override void Update()
		{
			base.Update();
			UpdateOnHitFlash();
		}

		private void UpdateOnHitFlash()
		{
			if (Time.time < onHitFlashStartTime + 0.3f + 0.1f)
			{
				float alpha = Easing.Ease(EaseType.InOutBounce, 0.75f, 0f, (Time.time - onHitFlashStartTime) / 0.3f);
				onHitOverlay.alpha = alpha;
			}
			else
			{
				onHitOverlay.alpha = 0f;
				onHitOverlay.gameObject.SetActive(value: false);
			}
		}
	}
}
