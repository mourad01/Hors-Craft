// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.QuestsV2.MainQuestManager
using Common.Managers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityToolbag;

namespace Gameplay.QuestsV2
{
	public class MainQuestManager : Manager
	{
		[Reorderable]
		[SerializeField]
		protected List<MainQuest> quests = new List<MainQuest>();

		private int currentQuestIndex;

		private MainQuestContext _mainQuestContext;

		private MainQuest currentQuest
		{
			[CompilerGenerated]
			get
			{
				return (currentQuestIndex < quests.Count && currentQuestIndex >= 0) ? quests[currentQuestIndex] : quests.Last();
			}
		}

		private MainQuestContext mainQuestContext
		{
			[CompilerGenerated]
			get
			{
				return _mainQuestContext ?? (_mainQuestContext = new MainQuestContext
				{
					currentQuest = quests[currentQuestIndex]
				});
			}
		}

		public override void Init()
		{
			LoadLastQuest();
			AddContext();
		}

		private void Update()
		{
			UpdateMainQuestProgress();
			MonoBehaviourSingleton<GameplayFacts>.get.NotifyFactChanged(Fact.MAIN_QUEST);
		}

		private void AddContext()
		{
			MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.MAIN_QUEST, mainQuestContext);
		}

		private void LoadLastQuest()
		{
			currentQuestIndex = PlayerPrefs.GetInt("main.quest.current", currentQuestIndex);
		}

		private void UpdateMainQuestProgress()
		{
			if (currentQuest.IsDone())
			{
				currentQuestIndex++;
				mainQuestContext.currentQuest = currentQuest;
			}
		}
	}
}
