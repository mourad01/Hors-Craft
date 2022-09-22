// DecompilerFi decompiler from Assembly-CSharp.dll class: SpinableMachineAdventure
using Gameplay.Minigames;
using QuestSystems.Adventure;

public class SpinableMachineAdventure : SpinableMachine
{
	public QuestDataItem quest = new QuestDataItem();

	protected override void SpawnResources(int winCount, int winId)
	{
		for (int i = 0; i < winCount; i++)
		{
			RewardBase rewardBase = new RewardBase(ERewardType.ItemReward, winId, 1);
			rewardBase.Grant();
			quest.UpdateState();
		}
	}
}
