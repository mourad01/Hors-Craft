// DecompilerFi decompiler from Assembly-CSharp.dll class: SimpleQuestManager
public class SimpleQuestManager : QuestManager
{
	public override void Init()
	{
		indicatorPool = new SpawnerPool("questIndicators", questIndicatorPrefab);
		Singleton<PlayerData>.get.playerQuests.ClearAndAddWorldQuestReadyForClaimListener(base.RemoveIndicatorsForQuests);
		worldsQuests = new SimpleWorldQuests();
		worldsQuests.Init();
		ProgressManager.onExpIncrease -= OnGlamourChange;
		ProgressManager.onExpIncrease += OnGlamourChange;
	}

	private void OnDestroy()
	{
		ProgressManager.onExpIncrease -= OnGlamourChange;
	}

	protected void OnGlamourChange(int increase, int wholeAmount)
	{
		IncreaseQuestOfType(QuestType.GetXWardrobeValue, increase);
	}

	public override void OnWorldChange(string id)
	{
	}

	public override void IncreaseQuestOfType(QuestType type, int inc = 1)
	{
		worldsQuests.GetCurrentQuests().ForEach(delegate(Quest quest)
		{
			if (quest.type == type)
			{
				Singleton<PlayerData>.get.playerQuests.OnWorldQuestIncreased(quest, inc);
			}
		});
	}
}
