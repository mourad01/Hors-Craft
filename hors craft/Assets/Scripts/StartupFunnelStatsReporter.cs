// DecompilerFi decompiler from Assembly-CSharp.dll class: StartupFunnelStatsReporter
using Common.Managers;
using Common.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

public class StartupFunnelStatsReporter : StatReporter
{
	public class StartupFunnelStatsReporterStat : StatsManager.Stat
	{
		public string actionType;

		public StartupFunnelStatsReporterStat(StartupFunnelActionType action)
		{
			serversideId = "startupFunnel";
			actionType = action.ToString();
		}
	}

	private static StartupFunnelStatsReporter _instance;

	private Queue<StartupFunnelStatsReporterStat> stats = new Queue<StartupFunnelStatsReporterStat>();

	private HashSet<Func<StartupFunnelActionType, bool>> filters = new HashSet<Func<StartupFunnelActionType, bool>>();

	private HashSet<StartupFunnelActionType> sendedActions = new HashSet<StartupFunnelActionType>();

	public static StartupFunnelStatsReporter Instance => _instance ?? (_instance = new StartupFunnelStatsReporter());

	private StartupFunnelStatsReporter()
	{
		filters.Add(FirstSessionFilter);
		filters.Add(NoEverSended);
	}

	public override StatsManager.Stat GetStat()
	{
		return stats.Dequeue();
	}

	public override bool UpdateAndCheck()
	{
		return stats.Count > 0;
	}

	public void RaiseFunnelEvent(StartupFunnelActionType action)
	{
		foreach (Func<StartupFunnelActionType, bool> filter in filters)
		{
			if (!filter(action))
			{
				return;
			}
		}
		UnityEngine.Debug.Log("Sending: " + action.ToString());
		stats.Enqueue(new StartupFunnelStatsReporterStat(action));
		sendedActions.Add(action);
	}

	private bool FirstSessionFilter(StartupFunnelActionType action)
	{
		return PlayerSession.GetSessionNo() == 1;
	}

	private bool NoEverSended(StartupFunnelActionType action)
	{
		return !sendedActions.Contains(action);
	}
}
