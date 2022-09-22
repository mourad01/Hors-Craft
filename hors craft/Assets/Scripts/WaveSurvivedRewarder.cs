// DecompilerFi decompiler from Assembly-CSharp.dll class: WaveSurvivedRewarder
using Common.Managers;
using Gameplay;
using States;
using System;
using UnityEngine;

public class WaveSurvivedRewarder : MonoBehaviour, ISurvivalContextListener
{
	public bool hasSaveToLeaderboard = true;

	public string leaderboardId;

	protected CombatTimeContext context => SurvivalContextsBroadcaster.instance.GetContext<CombatTimeContext>();

	public bool AddBySurvivalManager()
	{
		return true;
	}

	public Type[] ContextTypes()
	{
		return new Type[1]
		{
			typeof(CombatTimeContext)
		};
	}

	public Action[] OnContextsUpdated()
	{
		return new Action[1]
		{
			OnCombatTimeChange
		};
	}

	private void OnCombatTimeChange()
	{
		if (context == null || context.isCombat || context.becauseRestarted)
		{
			return;
		}
		if (ArmedPlayer.died)
		{
			ArmedPlayer.died = false;
			return;
		}
		if (hasSaveToLeaderboard && Manager.Contains<SocialPlatformManager>() && Manager.Get<SocialPlatformManager>().social.isLoggedIn)
		{
			Manager.Get<SocialPlatformManager>().social.SaveScore(Singleton<GooglePlayConstants>.get.GetIDFor(leaderboardId), SurvivalContextsBroadcaster.instance.GetContext<DayTimeContext>().day);
		}
		Manager.Get<StateMachineManager>().PushState<WaveSurvivedState>(new WaveSurvivedState.WaveSurvivedStartParameter
		{
			earnedMoney = GetMoney()
		});
	}

	private int GetMoney()
	{
		return Manager.Get<ModelManager>().waveRewardSettings.GetWaveEarn(SurvivalContextsBroadcaster.instance.GetContext<DayTimeContext>().day);
	}
}
