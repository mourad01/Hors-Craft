// DecompilerFi decompiler from Assembly-CSharp.dll class: States.BlackoutModule
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class BlackoutModule : GameplayModule
	{
		public Image blackoutImage;

		public Color normalColor = new Color(0f, 0f, 0f, 0f);

		public Color blackoutColor = new Color(1f, 1f, 1f, 1f);

		protected override Fact[] listenedFacts => new Fact[1]
		{
			Fact.BLACKOUT
		};

		public override void Init()
		{
			base.Init();
			blackoutImage.color = normalColor;
		}

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			float t = (!MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.BLACKOUT)) ? 0f : MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<BlackoutContext>(Fact.BLACKOUT).blackoutLevel;
			blackoutImage.color = Color.Lerp(normalColor, blackoutColor, t);
		}
	}
}
