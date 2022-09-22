// DecompilerFi decompiler from Assembly-CSharp.dll class: RunDailyRewardsOnDestroy
using Common.Managers;
using States;
using UnityEngine;

public class RunDailyRewardsOnDestroy : MonoBehaviour
{
	private void OnDestroy()
	{
		if (!(MonoBehaviourSingleton<ManagersContainer>.get == null))
		{
			if (!Manager.Contains<DailyRewardsManager>())
			{
				UnityEngine.Debug.LogError("BRAK DAILY REWARDS MANAGERA");
			}
			else if (Manager.Get<DailyRewardsManager>().hasRewardToday)
			{
				Manager.Get<StateMachineManager>().PushState<DailyRewardsState>();
			}
		}
	}
}
