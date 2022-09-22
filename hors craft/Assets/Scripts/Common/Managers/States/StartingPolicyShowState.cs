// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.States.StartingPolicyShowState
using Common.Managers.States.UI;
using System;
using UnityEngine;

namespace Common.Managers.States
{
	public class StartingPolicyShowState : UIConnectedState<StartingPolicyShowStateConnector>
	{
		private const string KEY_TO_CHECK_SHOWN = "starting.policy.shown";

		public Action onFinish;

		public static bool wasInitialPolicyShown
		{
			get
			{
				return PlayerPrefs.GetInt("starting.policy.shown", 0) == 1;
			}
			set
			{
				UnityEngine.Debug.Log("Setting Policy as Shown");
				PlayerPrefs.SetInt("starting.policy.shown", value ? 1 : 0);
				PlayerPrefs.Save();
			}
		}

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			StartupPolicyParam startupPolicyParam = parameter as StartupPolicyParam;
			onFinish = startupPolicyParam.onFinish;
			if (!CheckPolicy(delegate
			{
				onFinish();
				wasInitialPolicyShown = true;
			}))
			{
				onFinish();
				wasInitialPolicyShown = true;
			}
		}
	}
}
