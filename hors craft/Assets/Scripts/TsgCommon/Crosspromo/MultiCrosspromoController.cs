// DecompilerFi decompiler from Assembly-CSharp.dll class: TsgCommon.Crosspromo.MultiCrosspromoController
using System.Collections.Generic;
using System.Linq;
using TsgCommon.Crosspromo.Connection;
using UnityEngine;

namespace TsgCommon.Crosspromo
{
	public class MultiCrosspromoController : MonoBehaviourSingleton<MultiCrosspromoController>
	{
		[HideInInspector]
		public int crosspromoAmount;

		[HideInInspector]
		public string[] tags;

		public int iconCount;

		public string gamename = string.Empty;

		public long clickedBannerId;

		public long lastShownId;

		public Sprite containerSprite;

		public Sprite backgroundSprite;

		public CrosspromoContainerSetup containerSetup;

		[HideInInspector]
		public string playerId = string.Empty;

		public string crosspromoUrl;

		public string containerUrl;

		public string backgroundUrl;

		public string containerFilePath = "\\Crosspromo\\Container.png";

		public string backgroundFilePath = "\\Crosspromo\\Background.png";

		public float iconPercentSize = 0.17f;

		private List<MultiCrosspromoDefinition> definitions = new List<MultiCrosspromoDefinition>();

		private List<RunningCrosspromoInstance> runningInstances = new List<RunningCrosspromoInstance>();

		private List<RunningCrosspromoInstanceConfig> configs = new List<RunningCrosspromoInstanceConfig>();

		private MultiCrosspromoConnectionDownload connectionDownload;

		private MultiCrosspromoConnectionClick connectionClick;

		private MultiCrosspromoConnectionShow connectionShow;

		private bool definitionsDownloaded
		{
			get
			{
				if (definitions == null || definitions.Count == 0)
				{
					return false;
				}
				foreach (MultiCrosspromoDefinition definition in definitions)
				{
					if (definition == null)
					{
						return false;
					}
				}
				return true;
			}
		}

		private bool InstancesWaitingToBeReloaded()
		{
			for (int i = 0; i < runningInstances.Count; i++)
			{
				if (runningInstances[i] == null || !runningInstances[i].started)
				{
					return false;
				}
			}
			return true;
		}

		public override void Init()
		{
			definitions = new List<MultiCrosspromoDefinition>();
		}

		public void TryToShowIcons(RectTransform[] pivotRectTransforms, bool xButtonEnabled, string[] tags, int amount, string headerText = "", CrosspromoShowCallback additionalOnShowCallback = null, CrosspromoClickCallback additionalOnClickCallback = null, int animatorIndex = 0)
		{
			crosspromoAmount = Mathf.Min(pivotRectTransforms.Length, amount);
			this.tags = tags;
			DisposeRunningInstances();
			runningInstances.Clear();
			for (int i = 0; i < crosspromoAmount; i++)
			{
				if (pivotRectTransforms[i] == null)
				{
					UnityEngine.Debug.LogWarning("Tried to show crosspromo on null pivot transform!");
					return;
				}
				Vector2 percentPosOnScreen = UIUtils.GetPercentPosOnScreen(pivotRectTransforms[i]);
				Vector3[] fourCornersArray = new Vector3[4];
				pivotRectTransforms[i].GetWorldCorners(fourCornersArray);
				Canvas componentInParent = pivotRectTransforms[i].GetComponentInParent<Canvas>();
				if (string.IsNullOrEmpty(playerId) || string.IsNullOrEmpty(gamename))
				{
					UnityEngine.Debug.LogWarning("Cannot show crosspromo before setting up Crosspromo Controller!");
				}
				CrosspromoClickCallback clickCallback = delegate
				{
					CrosspromoClicked();
					if (additionalOnClickCallback != null)
					{
						additionalOnClickCallback();
					}
				};
				RunningCrosspromoInstanceConfig runningCrosspromoInstanceConfig = new RunningCrosspromoInstanceConfig();
				runningCrosspromoInstanceConfig.canvas = componentInParent;
				runningCrosspromoInstanceConfig.percentPosOnScreen = percentPosOnScreen;
				runningCrosspromoInstanceConfig.headerText = headerText;
				runningCrosspromoInstanceConfig.showCallback = additionalOnShowCallback;
				runningCrosspromoInstanceConfig.xButtonEnabled = xButtonEnabled;
				runningCrosspromoInstanceConfig.clickCallback = clickCallback;
				runningCrosspromoInstanceConfig.animatorIndex = animatorIndex;
				RunningCrosspromoInstanceConfig runningCrosspromoInstanceConfig2 = runningCrosspromoInstanceConfig;
				configs.Add(runningCrosspromoInstanceConfig2);
				RunningCrosspromoInstance item = new RunningCrosspromoInstance(runningCrosspromoInstanceConfig2);
				runningInstances.Add(item);
			}
			if (!definitionsDownloaded)
			{
				definitions.Clear();
				AskServerForDefinitions();
				return;
			}
			for (int j = 0; j < definitions.Count; j++)
			{
				AddRunningInstance(definitions[j]);
			}
		}

		private void AddRunningInstance(MultiCrosspromoDefinition multiCrosspromoDefinition)
		{
			RunningCrosspromoInstance runningCrosspromoInstance = runningInstances.FirstOrDefault((RunningCrosspromoInstance r) => r.isAvaliable);
			if (runningCrosspromoInstance != null)
			{
				runningCrosspromoInstance.bannerId = long.Parse(multiCrosspromoDefinition.bannerId);
				runningCrosspromoInstance.StartWithDefinition(multiCrosspromoDefinition);
				connectionShow = new MultiCrosspromoConnectionShow(gamename, crosspromoUrl, playerId, runningCrosspromoInstance.bannerId);
				connectionShow.Show(this);
			}
		}

		public void DisposeRunningInstances()
		{
			if (runningInstances == null)
			{
				return;
			}
			for (int i = 0; i < runningInstances.Count; i++)
			{
				RunningCrosspromoInstance runningCrosspromoInstance = runningInstances[i];
				if (runningCrosspromoInstance != null)
				{
					runningCrosspromoInstance.Dispose();
					runningCrosspromoInstance = null;
				}
			}
			runningInstances.Clear();
			definitions.Clear();
			AskServerForDefinitions();
		}

		public void CrosspromoClicked()
		{
			if (connectionClick == null || !connectionClick.running)
			{
				connectionClick = new MultiCrosspromoConnectionClick(gamename, crosspromoUrl, playerId);
				connectionClick.Click(this);
			}
		}

		private void OnGUI()
		{
			if (!InstancesWaitingToBeReloaded())
			{
				return;
			}
			for (int i = 0; i < runningInstances.Count; i++)
			{
				if (runningInstances[i].HasToBeReloaded())
				{
					ReloadRunningInstance(runningInstances[i]);
				}
			}
		}

		internal void ReloadRunningInstance(RunningCrosspromoInstance runningInstance)
		{
			runningInstance.Dispose();
			definitions.Clear();
			if (!definitionsDownloaded)
			{
				AskServerForDefinitions();
			}
		}

		private void AskServerForDefinitions()
		{
			if (connectionDownload == null || !connectionDownload.running)
			{
				connectionDownload = new MultiCrosspromoConnectionDownload(gamename, crosspromoUrl, playerId, containerFilePath, backgroundFilePath);
				connectionDownload.Download(this, OnDefinitionDownloaded);
			}
		}

		private void OnDefinitionDownloaded(MultiCrosspromoDefinition d)
		{
			definitions.Add(d);
			if (d.containerSprite != null)
			{
				containerSprite = d.containerSprite;
			}
			if (d.backgroundSprite != null)
			{
				backgroundSprite = d.backgroundSprite;
			}
			AddRunningInstance(d);
		}
	}
}
