// DecompilerFi decompiler from Assembly-CSharp.dll class: DailyRewardsReq
using Common.Managers;
using Common.Utils;
using Gameplay;
using States;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Initial Popup Req/Daily Rewards")]
public class DailyRewardsReq : InitialPopupRequirements, IFactChangedListener
{
	public override bool CanBeShown()
	{
		if (PlayerSession.GetSessionNo() > 1 || !Manager.Get<ModelManager>().dailyRewardsSettings.IsDelayedWindow())
		{
			return Manager.Get<DailyRewardsManager>().hasRewardToday;
		}
		if (Manager.Get<DailyRewardsManager>().hasRewardToday)
		{
			MonoBehaviourSingleton<GameplayFacts>.get.RegisterFactChangedListener(this, Fact.WAS_IN_PAUSE, Fact.IN_TUTORIAL);
		}
		return false;
	}

	public void OnFactsChanged(HashSet<Fact> facts)
	{
		if (!MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.IN_TUTORIAL) && (facts.Contains(Fact.IN_TUTORIAL) || (facts.Contains(Fact.WAS_IN_PAUSE) && !MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.WAS_IN_PAUSE))) && Manager.Get<DailyRewardsManager>().hasRewardToday)
		{
			MonoBehaviourSingleton<GameplayFacts>.get.RegisterFactChangedListener(this, Fact.WAS_IN_PAUSE, Fact.IN_TUTORIAL);
			Manager.Get<StateMachineManager>().StartCoroutine(DelayRun());
		}
	}

	private IEnumerator DelayRun()
	{
		yield return new WaitForSeconds(1f);
		StateMachineManager machine = Manager.Get<StateMachineManager>();
		while (!machine.IsCurrentStateA<GameplayState>())
		{
			yield return null;
		}
		Manager.Get<StateMachineManager>().PushState<DailyRewardsState>();
	}
}
