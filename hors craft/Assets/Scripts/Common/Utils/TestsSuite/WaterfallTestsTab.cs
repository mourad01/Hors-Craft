// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Utils.TestsSuite.WaterfallTestsTab
using Common.Waterfall;
using System;
using System.Collections.Generic;
using UnityEngine;

/*namespace Common.Utils.TestsSuite
{
	public class WaterfallTestsTab : TestsSuiteState.TestsTab
	{
		private class Log
		{
			public string text;
		}

		private Dictionary<AdWaterfallStepInfo, Log> stepToLog = new Dictionary<AdWaterfallStepInfo, Log>();

		private string tag = "Common";

		public override string name => "Waterfall";

		private Log GetStepToLog(AdWaterfallStepInfo info)
		{
			if (stepToLog.TryGetValue(info, out Log value))
			{
				return value;
			}
			value = new Log();
			stepToLog.Add(info, value);
			return value;
		}

		public override void OnContentGUI()
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Tag:");
			tag = GUILayout.TextField(tag);
			GUILayout.EndHorizontal();
			AdWaterfallType[] array = Enum.GetValues(typeof(AdWaterfallType)) as AdWaterfallType[];
			AdWaterfallStepInfo thisStep;
			string time;
			for (int i = 0; i < array.Length; i++)
			{
				AdWaterfallType adWaterfallType = array[i];
				GUILayout.BeginVertical(GUI.skin.box);
				GUILayout.Label(adWaterfallType.ToString().ToUpper());
				GUILayout.Space(10f);
				foreach (AdWaterfallStepInfo item in AdWaterfall.get.GetAllStepsForDebugging(adWaterfallType))
				{
					GUILayout.BeginHorizontal();
					GUILayout.Label(item.order + ": " + item.stepId + " ", GUILayout.Width(150f));
					thisStep = item;
					time = Time.realtimeSinceStartup.ToString("#0.00");
					if (adWaterfallType == AdWaterfallType.INTERSTITIAL && GUILayout.Button("Show Interst."))
					{
						item.instance.ShowInterstitialAd(tag, delegate
						{
							GetStepToLog(thisStep).text = "FAILED " + time;
						});
					}
					if (adWaterfallType == AdWaterfallType.REWARDED && GUILayout.Button("Show Rewarded"))
					{
						item.instance.ShowRewardedAd(tag, delegate(bool ok)
						{
							GetStepToLog(thisStep).text = "COMPLETED " + time + " " + ok;
						}, delegate
						{
							GetStepToLog(thisStep).text = "FAILED " + time;
						});
					}
					if (adWaterfallType == AdWaterfallType.BANNER)
					{
						if (GUILayout.Button("Show Banner"))
						{
							item.instance.ShowBanner(tag, delegate
							{
								GetStepToLog(thisStep).text = "FAILED " + time;
							});
						}
						if (GUILayout.Button("Hide Banner"))
						{
							item.instance.HideBanner();
						}
					}
					GUILayout.BeginHorizontal(GUI.skin.box);
					GUILayout.Label(GetStepToLog(item).text, GUILayout.Width(120f));
					GUILayout.EndHorizontal();
					GUILayout.EndHorizontal();
				}
				GUILayout.EndVertical();
			}
		}
	}
}*/
