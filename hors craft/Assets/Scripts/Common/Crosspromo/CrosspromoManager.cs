// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Crosspromo.CrosspromoManager
using Common.Managers;
using Common.Managers.States;
using Common.Utils;
using System;
using System.Collections;
using TsgCommon;
using TsgCommon.Crosspromo;
using UnityEngine;

namespace Common.Crosspromo
{
	public class CrosspromoManager : Manager
	{
		private bool containerDownloaded;

		private bool backgroundDownloaded;

		private Type currentlyShowingAtType;

		private int headerTextsShown
		{
			get
			{
				return PlayerPrefs.GetInt("crosspromo.header.texts.shown", 0);
			}
			set
			{
				PlayerPrefs.SetInt("crosspromo.header.texts.shown", value);
			}
		}

		private bool removeAdsBought
		{
			get
			{
				try
				{
					return Manager.Get<AbstractModelManager>().modulesContext.isAdsFree;
				}
				catch (UndefinedManagerException)
				{
					return false;
				}
			}
		}

		public override void Init()
		{
			TsgCommon.MonoBehaviourSingleton<CrosspromoController>.get.playerId = PlayerId.GetId();
			TsgCommon.MonoBehaviourSingleton<CrosspromoController>.get.gamename = Manager.Get<ConnectionInfoManager>().gameName;
			TsgCommon.MonoBehaviourSingleton<CrosspromoController>.get.crosspromoUrl = Manager.Get<ConnectionInfoManager>().crosspromoUrl;
			TsgCommon.MonoBehaviourSingleton<CrosspromoController>.get.containerSprite = null;
			TsgCommon.MonoBehaviourSingleton<CrosspromoController>.get.backgroundSprite = null;
			TsgCommon.MonoBehaviourSingleton<MultiCrosspromoController>.get.playerId = PlayerId.GetId();
			TsgCommon.MonoBehaviourSingleton<MultiCrosspromoController>.get.gamename = Manager.Get<ConnectionInfoManager>().gameName;
			TsgCommon.MonoBehaviourSingleton<MultiCrosspromoController>.get.crosspromoUrl = Manager.Get<ConnectionInfoManager>().crosspromoUrl;
			TsgCommon.MonoBehaviourSingleton<MultiCrosspromoController>.get.containerSprite = null;
			TsgCommon.MonoBehaviourSingleton<MultiCrosspromoController>.get.backgroundSprite = null;
		}

		public void DownloadContainerImages()
		{
			StartCoroutine(DownloadUrls(Manager.Get<AbstractModelManager>().crosspromoSettings.GetTextureUrl(), Manager.Get<AbstractModelManager>().crosspromoSettings.GetBackgroundTextureUrl()));
		}

		public void TryToShowIconsAt(State state, RectTransform[] pivotRectTransforms, string[] tags, int amount)
		{
			if (!removeAdsBought)
			{
				currentlyShowingAtType = ((!(state != null)) ? null : state.GetType());
				CrosspromoShowCallback additionalOnShowCallback = delegate
				{
					Manager.Get<StatsManager>().CrosspromoShown();
				};
				CrosspromoClickCallback additionalOnClickCallback = delegate
				{
					Manager.Get<StatsManager>().CrosspromoClicked();
				};
				string headerText;
				if (headerTextsShown < Manager.Get<AbstractModelManager>().commonAdSettings.GetCrosspromoHeadersCountLimit())
				{
					headerText = Manager.Get<TranslationsManager>().GetText("crosspromo.header", "SEE OTHER GAMES");
					headerTextsShown++;
				}
				else
				{
					headerText = string.Empty;
				}
				bool xButtonEnabled = Manager.Get<AbstractModelManager>().commonAdSettings.IsCrosspromoXButtonEnabled();
				Manager.Get<AbstractModelManager>().commonAdSettings.GetCrosspromoAnimatorIndex();
				TsgCommon.MonoBehaviourSingleton<MultiCrosspromoController>.get.TryToShowIcons(pivotRectTransforms, xButtonEnabled, tags, amount, headerText, additionalOnShowCallback, additionalOnClickCallback, -1);
			}
		}

		public void TryToShowIconAt(State state, RectTransform pivotRectTransform)
		{
			if (!removeAdsBought)
			{
				currentlyShowingAtType = ((!(state != null)) ? null : state.GetType());
				CrosspromoShowCallback additionalOnShowCallback = delegate
				{
					Manager.Get<StatsManager>().CrosspromoShown();
				};
				CrosspromoClickCallback additionalOnClickCallback = delegate
				{
					Manager.Get<StatsManager>().CrosspromoClicked();
				};
				string headerText;
				if (headerTextsShown < Manager.Get<AbstractModelManager>().commonAdSettings.GetCrosspromoHeadersCountLimit())
				{
					headerText = Manager.Get<TranslationsManager>().GetText("crosspromo.header", "SEE OTHER GAMES");
					headerTextsShown++;
				}
				else
				{
					headerText = string.Empty;
				}
				bool xButtonEnabled = Manager.Get<AbstractModelManager>().commonAdSettings.IsCrosspromoXButtonEnabled();
				int crosspromoAnimatorIndex = Manager.Get<AbstractModelManager>().commonAdSettings.GetCrosspromoAnimatorIndex();
				TsgCommon.MonoBehaviourSingleton<CrosspromoController>.get.TryToShowIcon(pivotRectTransform, xButtonEnabled, headerText, additionalOnShowCallback, additionalOnClickCallback, crosspromoAnimatorIndex);
			}
		}

		public void TryToShowIconAt(Canvas canvas, State state, Vector2 percentPos)
		{
			if (!removeAdsBought)
			{
				currentlyShowingAtType = ((!(state != null)) ? null : state.GetType());
				CrosspromoShowCallback additionalOnShowCallback = delegate
				{
					Manager.Get<StatsManager>().CrosspromoShown();
				};
				CrosspromoClickCallback additionalOnClickCallback = delegate
				{
					Manager.Get<StatsManager>().CrosspromoClicked();
				};
				string headerText;
				try
				{
					if (headerTextsShown < Manager.Get<AbstractModelManager>().commonAdSettings.GetCrosspromoHeadersCountLimit())
					{
						headerText = Manager.Get<TranslationsManager>().GetText("crosspromo.header", "SEE OTHER GAMES");
						headerTextsShown++;
					}
					else
					{
						headerText = string.Empty;
					}
				}
				catch (UndefinedManagerException arg)
				{
					UnityEngine.Debug.LogWarning("Crosspromo config undefined manager: " + arg + " using SEE OTHER GAMES text instead.");
					headerText = "SEE OTHER GAMES";
				}
				bool xButtonEnabled = Manager.Get<AbstractModelManager>().commonAdSettings.IsCrosspromoXButtonEnabled();
				int crosspromoAnimatorIndex = Manager.Get<AbstractModelManager>().commonAdSettings.GetCrosspromoAnimatorIndex();
				TsgCommon.MonoBehaviourSingleton<CrosspromoController>.get.TryToShowIcon(canvas, percentPos, xButtonEnabled, headerText, additionalOnShowCallback, additionalOnClickCallback, crosspromoAnimatorIndex);
			}
		}

		public void TryToShowIconAt(Canvas canvas, Vector2 percentPos)
		{
			if (!removeAdsBought)
			{
				CrosspromoShowCallback additionalOnShowCallback = delegate
				{
					Manager.Get<StatsManager>().CrosspromoShown();
				};
				CrosspromoClickCallback additionalOnClickCallback = delegate
				{
					Manager.Get<StatsManager>().CrosspromoClicked();
				};
				string headerText;
				try
				{
					if (headerTextsShown < Manager.Get<AbstractModelManager>().commonAdSettings.GetCrosspromoHeadersCountLimit())
					{
						headerText = Manager.Get<TranslationsManager>().GetText("crosspromo.header", "SEE OTHER GAMES");
						headerTextsShown++;
					}
					else
					{
						headerText = string.Empty;
					}
				}
				catch (UndefinedManagerException arg)
				{
					UnityEngine.Debug.LogWarning("Crosspromo config undefined manager: " + arg + " using SEE OTHER GAMES text instead.");
					headerText = "SEE OTHER GAMES";
				}
				bool xButtonEnabled = Manager.Get<AbstractModelManager>().commonAdSettings.IsCrosspromoXButtonEnabled();
				TsgCommon.MonoBehaviourSingleton<CrosspromoController>.get.TryToShowIcon(canvas, percentPos, xButtonEnabled, headerText, additionalOnShowCallback, additionalOnClickCallback);
			}
		}

		public void DisposeRunningInstance()
		{
			currentlyShowingAtType = null;
			TsgCommon.MonoBehaviourSingleton<CrosspromoController>.get.DisposeRunningInstance();
			TsgCommon.MonoBehaviourSingleton<MultiCrosspromoController>.get.DisposeRunningInstances();
		}

		private void Update()
		{
			State currentState = Manager.Get<StateMachineManager>().currentState;
			if (currentlyShowingAtType != null && currentState != null && currentlyShowingAtType != currentState.GetType())
			{
				DisposeRunningInstance();
			}
		}

		private IEnumerator DownloadUrls(string containerUrl, string backgroundUrl)
		{
			WWW containerDownload = new WWW(containerUrl);
			yield return containerDownload;
			if (containerDownload.text.IsNullOrEmpty())
			{
				yield return new WaitForSecondsRealtime(0.5f);
				StartCoroutine(DownloadUrls(containerUrl, backgroundUrl));
				yield break;
			}
			string containerImageUrl = JSONHelper.Deserialize<string>(containerDownload.text);
			if (Manager.Get<AbstractModelManager>().crosspromoSettings.GetBackgroundTextureName().IsNOTNullOrEmpty())
			{
				WWW bgDownload = new WWW(backgroundUrl);
				yield return bgDownload;
				string backgroundImageUrl = JSONHelper.Deserialize<string>(bgDownload.text);
				TsgCommon.MonoBehaviourSingleton<CrosspromoController>.get.backgroundUrl = backgroundImageUrl;
				TsgCommon.MonoBehaviourSingleton<MultiCrosspromoController>.get.backgroundUrl = backgroundUrl;
			}
			TsgCommon.MonoBehaviourSingleton<CrosspromoController>.get.containerUrl = containerImageUrl;
			TsgCommon.MonoBehaviourSingleton<MultiCrosspromoController>.get.containerUrl = containerImageUrl;
		}
	}
}
