// DecompilerFi decompiler from Assembly-CSharp.dll class: FillQuestDescriptionsFragment
using Common.Managers;
using QuestSystems.Adventure;
using States;
using System.Collections.Generic;
using UnityEngine;

public class FillQuestDescriptionsFragment : FillScrollListFragment
{
	public Sprite unfinishedQuest;

	public Sprite finishedQuest;

	public override void Init(FragmentStartParameter parameter)
	{
		base.Init(parameter);
		AdventureQuestManager adventureQuestManager = Manager.Get<AdventureQuestManager>();
		Fill(adventureQuestManager.GetActiveQuestsCopy());
	}

	public override void Fill<T>(List<T> objects)
	{
		if (objects.Count != 0)
		{
			List<EQuestState> orderMap = new List<EQuestState>
			{
				EQuestState.partiallyDone,
				EQuestState.notDone,
				EQuestState.started,
				EQuestState.notStarted,
				EQuestState.disabled,
				EQuestState.afterDone,
				EQuestState.done
			};
			objects.Sort(delegate(T x, T y)
			{
				int num = orderMap.IndexOf((x as QuestDataItem).QuestState);
				int value = orderMap.IndexOf((y as QuestDataItem).QuestState);
				return num.CompareTo(value);
			});
			foreach (T @object in objects)
			{
				RectTransform rectTransform = InstantiateElement();
				QuestUIElement component = rectTransform.GetComponent<QuestUIElement>();
				if (!(component == null))
				{
					component.Changedescription((@object as QuestDataItem).GenerateDescriptionTranslationKey());
					if ((@object as QuestDataItem).QuestState == EQuestState.done || (@object as QuestDataItem).QuestState == EQuestState.afterDone)
					{
						component.ChangeImage(finishedQuest);
					}
					else
					{
						component.ChangeImage(unfinishedQuest);
					}
				}
			}
		}
	}
}
