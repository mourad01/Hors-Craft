// DecompilerFi decompiler from Assembly-CSharp.dll class: States.LevelModule
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace States
{
	public class LevelModule : GameplayModule
	{
		public Text levelText;

		public Slider expSlider;

		protected override Fact[] listenedFacts => new Fact[1]
		{
			Fact.LEVEL
		};

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			LevelContext levelContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<LevelContext>(Fact.LEVEL).FirstOrDefault();
			if (levelContext != null)
			{
				levelText.text = levelContext.level.ToString();
				expSlider.value = levelContext.levelProgress;
			}
		}
	}
}
