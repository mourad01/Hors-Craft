// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Waterfall.AdWaterfallShowAdProcess
using System;
using System.Collections.Generic;
using UnityEngine;

/*namespace Common.Waterfall
{
	public class AdWaterfallShowAdProcess
	{
		public const string SPECIAL_FAILURE_STEP = "failed";

		private int nextStepIndex;

		//private AdWaterfallType type;

		//private string adTag;

		//private List<AdWaterfallStepInfo> waterfallSteps;

		//public OnShowRewardedAdCompleted onShowRewardedAdCompleted;

		//public Action OnShowFail;

		/*public float lastStepChangeTime
		{
			get;
			private set;
		}*/

		/*public string lastStepID
		{
			get;
			private set;
		}*/

		/*public AdWaterfallShowAdProcess(AdWaterfallType type, string adTag, List<AdWaterfallStepInfo> waterfallSteps, OnShowRewardedAdCompleted onShowRewardedAdCompleted = null, Action onShowFail = null)
		{
			this.type = type;
			this.adTag = adTag;
			this.waterfallSteps = waterfallSteps;
			this.onShowRewardedAdCompleted = onShowRewardedAdCompleted;
			OnShowFail = onShowFail;
			lastStepChangeTime = Time.realtimeSinceStartup;
			nextStepIndex = 0;
		}*/

		/*public bool IsBanner()
		{
			return AdWaterfallType.BANNER == type;
		}*/

		/*public void Process()
		{
			NextStep();
		}
		*/
		/*private void NextStep()
		{*/
			/*if (nextStepIndex >= waterfallSteps.Count)
			{
				//TrackStepTried("failed", nextStepIndex, type.ToString().ToLower());
				lastStepID = "failed";
				if (OnShowFail != null)
				{
					OnShowFail();
				}
				return;
			}*/
			//AdWaterfallStepInfo adWaterfallStepInfo = waterfallSteps[nextStepIndex];
			//TrackStepTried(adWaterfallStepInfo.stepId, nextStepIndex, type.ToString().ToLower());
			/*lastStepID = adWaterfallStepInfo.stepId;
			//if (AdWaterfall.get.verbose)
			{
				//UnityEngine.Debug.Log("Processing next waterfall step for type " + type + ", index: " + nextStepIndex + " priority: " + adWaterfallStepInfo.order + " description: " + adWaterfallStepInfo.instance.ToString());
			}
			nextStepIndex++;
			int delegateContextStep = nextStepIndex;
			OnShowAdFailed onShowAdFailed = delegate
			{
				if (nextStepIndex == delegateContextStep)
				{
					NextStep();
				}
			};*/
			//IAdWaterfallStep instance = adWaterfallStepInfo.instance;
			//switch ((!AdWaterfall.get.rewardedWithInterstitial) ? type : adWaterfallStepInfo.type)
			//{
			/*default:
				instance.ShowInterstitialAd(adTag, onShowAdFailed);
				break;
			case AdWaterfallType.REWARDED:
				instance.ShowRewardedAd(adTag, onShowRewardedAdCompleted, onShowAdFailed);
				break;
			case AdWaterfallType.BANNER:
				instance.ShowBanner(adTag, onShowAdFailed);
				break;
			}
			lastStepChangeTime = Time.realtimeSinceStartup;
		}*/

		/*private void TrackStepTried(string stepId, int stepNumber, string type)
		{
			AdWaterfall.get.statWaterfallStepTried(stepId, stepNumber, type);
		}*/
	//}
//}
