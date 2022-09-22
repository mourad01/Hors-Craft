// DecompilerFi decompiler from Assembly-CSharp.dll class: TsgCommon.Crosspromo.CrosspromoController
using TsgCommon.Crosspromo.Connection;
using UnityEngine;

namespace TsgCommon.Crosspromo
{
	public class CrosspromoController : MonoBehaviourSingleton<CrosspromoController>
	{
		public string gamename = string.Empty;

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

		private CrosspromoDefinition definition;

		private RunningCrosspromoInstance runningInstance;

		private CrosspromoConnectionDownload connectionDownload;

		private CrosspromoConnectionClick connectionClick;

		private bool definitionDownloaded => definition != null;

		private bool instanceWaitingToBeStarted => runningInstance != null && !runningInstance.started;

		private bool instanceWaitingToBeReloaded => runningInstance != null && runningInstance.started;

		public override void Init()
		{
			definition = null;
		}

		public void TryToShowIcon(RectTransform pivotRectTransform, bool xButtonEnabled, string headerText = "", CrosspromoShowCallback additionalOnShowCallback = null, CrosspromoClickCallback additionalOnClickCallback = null, int animatorIndex = 0)
		{
			if (pivotRectTransform == null)
			{
				UnityEngine.Debug.LogWarning("Tried to show crosspromo on null pivot transform!");
				return;
			}
			Vector2 percentPosOnScreen = UIUtils.GetPercentPosOnScreen(pivotRectTransform);
			Canvas componentInParent = pivotRectTransform.GetComponentInParent<Canvas>();
			TryToShowIcon(componentInParent, percentPosOnScreen, xButtonEnabled, headerText, additionalOnShowCallback, additionalOnClickCallback, animatorIndex);
		}

		public void TryToShowIcon(Canvas canvas, Vector2 percentPosOnScreen, bool xButtonEnabled, string headerText = "", CrosspromoShowCallback additionalOnShowCallback = null, CrosspromoClickCallback additionalOnClickCallback = null, int animatorIndex = 0)
		{
			if (string.IsNullOrEmpty(playerId) || string.IsNullOrEmpty(gamename))
			{
				UnityEngine.Debug.LogWarning("Cannot show crosspromo before setting up Crosspromo Controller!");
			}
			DisposeRunningInstance();
			CrosspromoClickCallback clickCallback = delegate
			{
				CrosspromoClicked();
				if (additionalOnClickCallback != null)
				{
					additionalOnClickCallback();
				}
			};
			RunningCrosspromoInstanceConfig runningCrosspromoInstanceConfig = new RunningCrosspromoInstanceConfig();
			runningCrosspromoInstanceConfig.canvas = canvas;
			runningCrosspromoInstanceConfig.percentPosOnScreen = percentPosOnScreen;
			runningCrosspromoInstanceConfig.headerText = headerText;
			runningCrosspromoInstanceConfig.showCallback = additionalOnShowCallback;
			runningCrosspromoInstanceConfig.xButtonEnabled = xButtonEnabled;
			runningCrosspromoInstanceConfig.clickCallback = clickCallback;
			runningCrosspromoInstanceConfig.animatorIndex = animatorIndex;
			RunningCrosspromoInstanceConfig config = runningCrosspromoInstanceConfig;
			runningInstance = new RunningCrosspromoInstance(config);
			if (!definitionDownloaded)
			{
				AskServerForDefinition();
			}
			else
			{
				runningInstance.StartWithDefinition(definition);
			}
		}

		public void DisposeRunningInstance()
		{
			if (runningInstance != null)
			{
				runningInstance.Dispose();
				runningInstance = null;
				definition = null;
				AskServerForDefinition();
			}
		}

		public void CrosspromoClicked()
		{
			if (connectionClick == null || !connectionClick.running)
			{
				connectionClick = new CrosspromoConnectionClick(gamename, crosspromoUrl, playerId);
				connectionClick.Click(this);
			}
			ReloadRunningInstance();
		}

		private void OnGUI()
		{
			if (instanceWaitingToBeReloaded && runningInstance.HasToBeReloaded())
			{
				ReloadRunningInstance();
			}
		}

		private void ReloadRunningInstance()
		{
			runningInstance.Dispose();
			RunningCrosspromoInstanceConfig config = runningInstance.config;
			runningInstance = new RunningCrosspromoInstance(runningInstance.config);
			definition = null;
			if (!definitionDownloaded)
			{
				AskServerForDefinition();
			}
			else
			{
				runningInstance.StartWithDefinition(definition);
			}
		}

		private void AskServerForDefinition()
		{
			if (connectionDownload == null || !connectionDownload.running)
			{
				connectionDownload = new CrosspromoConnectionDownload(gamename, crosspromoUrl, playerId, containerFilePath, backgroundFilePath);
				connectionDownload.Download(this, OnDefinitionDownloaded);
			}
		}

		private void OnDefinitionDownloaded(CrosspromoDefinition d)
		{
			definition = d;
			if (instanceWaitingToBeStarted)
			{
				if (definition.containerSprite != null)
				{
					containerSprite = definition.containerSprite;
				}
				if (definition.backgroundSprite != null)
				{
					backgroundSprite = definition.backgroundSprite;
				}
				runningInstance.StartWithDefinition(definition);
			}
		}
	}
}
