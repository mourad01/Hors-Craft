// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Waterfall.AdWaterfall
using System;
using System.Collections.Generic;
using UnityEngine;

//namespace Common.Waterfall
//{
	/*public class AdWaterfall : MonoBehaviour
	{
		private struct StepList
		{
			public int adsWatched;

			public AdWaterfallStepList list;
		}

		public delegate void StatAdShownSuccessBasedOnTime(string id);

		public delegate void StatWaterfallStepTried(string stepId, int stepNumber, string type);

		private static int adsWatched;

		private static AdWaterfall instance;

		private static Dictionary<AdWaterfallType, Queue<StepList>> waterfallStepsPerAdsWatched;

		public bool rewardedWithInterstitial;

		public StatAdShownSuccessBasedOnTime statAdShownSuccessBasedOnTime;

		public StatWaterfallStepTried statWaterfallStepTried;

		private Dictionary<AdWaterfallType, AdWaterfallStepsListForType> typeToPossibleSteps;

		private List<AdWaterfallShowAdProcess> runningProcesses;

		private const float ASSUME_PROCESS_FINISHED_AFTER = 5f;

		public Action onInterstitialShown;

		public Action onRewardedShown;

		public static AdWaterfall get
		{
			get
			{
				if (instance == null)
				{
					GameObject gameObject = new GameObject(typeof(AdWaterfall).ToString());
					UnityEngine.Object.DontDestroyOnLoad(gameObject);
					instance = gameObject.AddComponent<AdWaterfall>();
					waterfallStepsPerAdsWatched = new Dictionary<AdWaterfallType, Queue<StepList>>();
					AdWaterfallType[] array = Enum.GetValues(typeof(AdWaterfallType)) as AdWaterfallType[];
					foreach (AdWaterfallType key in array)
					{
						waterfallStepsPerAdsWatched.Add(key, new Queue<StepList>());
					}
				}
				return instance;
			}
		}

		public bool verbose
		{
			get;
			private set;
		}

		public static void AddWaterfallStepsPerAdsWatched(AdWaterfallType type, int adsWatched, AdWaterfallStepList list)
		{
			StepList stepList = default(StepList);
			stepList.adsWatched = adsWatched;
			stepList.list = list;
			StepList item = stepList;
			waterfallStepsPerAdsWatched[type].Enqueue(item);
		}

		public List<AdWaterfallStepInfo> GetAllStepsForDebugging(AdWaterfallType type)
		{
			return typeToPossibleSteps[type].GetAllSteps();
		}

		private void Awake()
		{
			typeToPossibleSteps = new Dictionary<AdWaterfallType, AdWaterfallStepsListForType>();
			AdWaterfallType[] array = Enum.GetValues(typeof(AdWaterfallType)) as AdWaterfallType[];
			foreach (AdWaterfallType adWaterfallType in array)
			{
				typeToPossibleSteps[adWaterfallType] = new AdWaterfallStepsListForType(adWaterfallType);
			}
			runningProcesses = new List<AdWaterfallShowAdProcess>();
		}

		public void InitWithConfig(AdWaterfallConfig config)
		{
			config.Deserialize();
			config.Execute();
			verbose = config.IsVerbose();
			rewardedWithInterstitial = config.IsRewardedWithInterstitial();
			UpdateStepListForAllTypes();
		}

		public void RegisterSteps(AdWaterfallType type, AdWaterfallStepList steps)
		{
			typeToPossibleSteps[type].Register(steps);
		}

		public void ClearSteps()
		{
			foreach (KeyValuePair<AdWaterfallType, AdWaterfallStepsListForType> typeToPossibleStep in typeToPossibleSteps)
			{
				typeToPossibleStep.Value.Clear();
			}
		}

		public void ClearStepsForType(AdWaterfallType type)
		{
			typeToPossibleSteps[type].Clear();
		}

		public void ShowInterstitialAd(string adTag)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				HideBanner();
			}
			List<AdWaterfallStepInfo> randomStepList = typeToPossibleSteps[AdWaterfallType.INTERSTITIAL].GetRandomStepList();
			if (randomStepList == null)
			{
				UnityEngine.Debug.LogWarning("No steps lists defined in AdWaterfall for: INTERSTITIAL! Cannot evaluate waterfall.");
				return;
			}
			AdWaterfallShowAdProcess adWaterfallShowAdProcess = new AdWaterfallShowAdProcess(AdWaterfallType.INTERSTITIAL, adTag, randomStepList);
			adWaterfallShowAdProcess.Process();
			runningProcesses.Add(adWaterfallShowAdProcess);
			if (onInterstitialShown != null)
			{
				onInterstitialShown();
			}
		}

		public void ShowRewardedAd(string adTag, OnShowRewardedAdCompleted onShowRewardedAdCompleted, Action onFail = null)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				HideBanner();
			}
			List<AdWaterfallStepInfo> list = new List<AdWaterfallStepInfo>(typeToPossibleSteps[AdWaterfallType.REWARDED].GetRandomStepList());
			if (rewardedWithInterstitial)
			{
				list.AddRange(typeToPossibleSteps[AdWaterfallType.INTERSTITIAL].GetRandomStepList());
			}
			if (list == null)
			{
				UnityEngine.Debug.LogWarning("No steps lists defined in AdWaterfall for: REWARDED! Cannot evaluate waterfall.");
				return;
			}
			UnityEngine.Debug.Log(rewardedWithInterstitial + ":   " + list.Count);
			AdWaterfallShowAdProcess adWaterfallShowAdProcess = new AdWaterfallShowAdProcess(AdWaterfallType.REWARDED, adTag, list, onShowRewardedAdCompleted, onFail);
			adWaterfallShowAdProcess.Process();
			runningProcesses.Add(adWaterfallShowAdProcess);
			if (onRewardedShown != null)
			{
				onRewardedShown();
			}
		}

		public void ShowBanner(string adTag)
		{
			List<AdWaterfallStepInfo> randomStepList = typeToPossibleSteps[AdWaterfallType.BANNER].GetRandomStepList();
			if (randomStepList == null)
			{
				UnityEngine.Debug.LogWarning("No steps lists defined in AdWaterfall for: BANNER! Cannot evaluate waterfall.");
				return;
			}
			AdWaterfallShowAdProcess adWaterfallShowAdProcess = new AdWaterfallShowAdProcess(AdWaterfallType.BANNER, adTag, randomStepList);
			adWaterfallShowAdProcess.Process();
			runningProcesses.Add(adWaterfallShowAdProcess);
		}

		public void HideBanner()
		{
			foreach (AdWaterfallStepInfo allStep in typeToPossibleSteps[AdWaterfallType.BANNER].GetAllSteps())
			{
				allStep.instance.HideBanner();
			}
		}

		private void OnGUI()
		{
			TrackAndRemoveFinishedProcesses();
		}

		private void TrackAndRemoveFinishedProcesses()
		{
			int num = 0;
			while (num < runningProcesses.Count)
			{
				if (Time.realtimeSinceStartup > runningProcesses[num].lastStepChangeTime + 5f)
				{
					if (runningProcesses[num].lastStepID != "failed")
					{
						statAdShownSuccessBasedOnTime(runningProcesses[num].lastStepID);
						if (!runningProcesses[num].IsBanner())
						{
							OnAdWatched();
						}
					}
					runningProcesses.RemoveAt(num);
				}
				else
				{
					num++;
				}
			}
		}

		private void OnAdWatched()
		{
			adsWatched++;
			UpdateStepListForAllTypes();
		}

		private void UpdateStepListForAllTypes()
		{
			AdWaterfallType[] array = Enum.GetValues(typeof(AdWaterfallType)) as AdWaterfallType[];
			foreach (AdWaterfallType type in array)
			{
				UpdateStepList(type);
			}
		}

		private void UpdateStepList(AdWaterfallType type)
		{
			if (waterfallStepsPerAdsWatched[type].Count > 0)
			{
				StepList stepList = waterfallStepsPerAdsWatched[type].Peek();
				if (stepList.adsWatched <= adsWatched)
				{
					StepList stepList2 = waterfallStepsPerAdsWatched[type].Dequeue();
					instance.ClearStepsForType(type);
					instance.RegisterSteps(type, stepList2.list);
				}
			}
		}
	}
}*/
