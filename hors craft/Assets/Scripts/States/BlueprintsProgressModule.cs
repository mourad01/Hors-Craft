// DecompilerFi decompiler from Assembly-CSharp.dll class: States.BlueprintsProgressModule
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace States
{
	public class BlueprintsProgressModule : GameplayModule
	{
		public Slider progressSlider;

		public Text progressText;

		public Button autoFillButton;

		public Button removeButton;

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
			List<BlueprintFillContext> contexts = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<BlueprintFillContext>(Fact.IN_BLUEPRINT_RANGE);
			bool flag = contexts.Count > 0;
			if (progressSlider.gameObject.activeSelf != flag)
			{
				progressSlider.gameObject.SetActive(flag);
			}
			if (flag)
			{
				contexts.First().setProgressSlider(progressSlider, progressText);
				if (autoFillButton != null)
				{
					autoFillButton.onClick.RemoveAllListeners();
					autoFillButton.onClick.AddListener(delegate
					{
						contexts.First().instantFill();
					});
				}
				if (removeButton != null)
				{
					removeButton.onClick.RemoveAllListeners();
					removeButton.onClick.AddListener(delegate
					{
						contexts.First().destroy();
					});
				}
			}
		}
	}
}
