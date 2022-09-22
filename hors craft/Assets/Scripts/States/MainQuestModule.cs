// DecompilerFi decompiler from Assembly-CSharp.dll class: States.MainQuestModule
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class MainQuestModule : GameplayModule
	{
		public GameObject mainObject;

		public Slider slider;

		public Text label;

		public Text value;

		protected override Fact[] listenedFacts
		{
			[CompilerGenerated]
			get
			{
				return new Fact[2]
				{
					Fact.MAIN_QUEST,
					Fact.IN_BLUEPRINT_RANGE
				};
			}
		}

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			if (MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.IN_BLUEPRINT_RANGE))
			{
				mainObject.SetActive(value: false);
				return;
			}
			mainObject.SetActive(value: true);
			MainQuestContext mainQuestContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<MainQuestContext>(Fact.MAIN_QUEST).FirstOrDefault();
			slider.maxValue = mainQuestContext.currentQuest.amountToCompleted;
			slider.value = mainQuestContext.currentQuest.dataProvider.GetProgress();
			label.text = mainQuestContext.currentQuest.label;
			value.text = $"{mainQuestContext.currentQuest.dataProvider.GetProgress().ToString()}/{mainQuestContext.currentQuest.amountToCompleted.ToString()}";
		}
	}
}
