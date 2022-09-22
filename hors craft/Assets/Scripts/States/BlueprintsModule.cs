// DecompilerFi decompiler from Assembly-CSharp.dll class: States.BlueprintsModule
using System.Collections.Generic;
using UnityEngine.UI;

namespace States
{
	public class BlueprintsModule : GameplayModule
	{
		public Button fillBlueprintButton;

		public Button removeButton;

		public Slider progressSlider;

		public Text progressText;

		protected override Fact[] listenedFacts => new Fact[1]
		{
			Fact.IN_BLUEPRINT_RANGE
		};

		public override void Init()
		{
			base.Init();
		}

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			BlueprintFillContext factContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<BlueprintFillContext>(Fact.IN_BLUEPRINT_RANGE);
			bool flag = factContext != null;
			if (base.gameObject.activeSelf != flag)
			{
				base.gameObject.SetActive(flag);
			}
			if (flag)
			{
				SetListenerToButton(fillBlueprintButton, factContext.instantFill);
				if (removeButton != null)
				{
					SetListenerToButton(removeButton, factContext.destroy);
				}
				if (progressSlider != null && progressText != null)
				{
					factContext.setProgressSlider(progressSlider, progressText);
				}
			}
		}
	}
}
