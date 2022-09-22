// DecompilerFi decompiler from Assembly-CSharp.dll class: States.UnderwaterOverlayModule
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class UnderwaterOverlayModule : GameplayModule
	{
		private const float WATER_OVERLAY_VISIBLE_ALFA = 0.31f;

		public GameObject overlay;

		private Image image;

		protected override Fact[] listenedFacts => new Fact[1]
		{
			Fact.UNDERWATER
		};

		public override void Init()
		{
			image = overlay.GetComponentInChildren<Image>();
			base.Init();
		}

		public override void OnFactsChanged(HashSet<Fact> facts)
		{
			UnderwaterContext underwaterContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<UnderwaterContext>(Fact.UNDERWATER).FirstOrDefault();
			if (underwaterContext != null)
			{
				Color waterColor = underwaterContext.waterColor;
				waterColor.a = 0.31f;
				image.color = waterColor;
			}
			else
			{
				Color color = image.color;
				color.a = 0f;
				image.color = color;
			}
			overlay.SetActive(underwaterContext != null);
		}
	}
}
