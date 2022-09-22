// DecompilerFi decompiler from Assembly-CSharp.dll class: States.GameOverState
using Common.Audio;
using Common.Managers;
using Common.Managers.Audio;
using Common.Managers.States;
using Common.Utils;
using Common.Waterfall;
using Gameplay;
using Gameplay.Audio;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace States
{
	public class GameOverState : XCraftUIState<GameOverStateConnector>
	{
		public bool showMotivationalQuotes;

		public bool showSurvivalTime = true;

		public bool showScore;

		public bool showLoadSave = true;

		public bool showToMenu = true;

		public bool showAd;

		protected override bool hasBackground => false;

		protected override bool hasBackgroundOverlay => true;

		private string quoteKey(int i)
		{
			return "motivational.quote." + i;
		}

		private string quoteAuthorKey(int i)
		{
			return "motivational.quote.author." + i;
		}

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			base.connector.onShowLeaderboardClicked = OnShowLeaderboard;
			base.connector.onReturnToMenuClicked = OnReturn;
			base.connector.onLoadLastSaveClicked = OnLoadLastSave;
			base.connector.onRestartGameClicked = OnRestart;
			PlaySound(GameSound.PLAYER_DIE);
			if (base.connector != null && base.connector.loadLastSaveButton.transform.parent as RectTransform != null)
			{
				(base.connector.loadLastSaveButton.transform.parent as RectTransform).gameObject.SetActive(showLoadSave);
			}
			base.connector.returnToMenuButton.gameObject.SetActive(showToMenu);
			if (showSurvivalTime)
			{
				ShowSurvivedTime();
			}
			else if (showScore)
			{
				ShowScore();
			}
			else
			{
				HideScore();
			}
			if (showMotivationalQuotes)
			{
				ShowMotivationalQuote();
			}
			else
			{
				base.connector.titlePanel.SetActive(value: true);
				base.connector.motivationalPanel.SetActive(value: false);
			}
			if (Manager.Contains<SurvivalManager>())
			{
				Manager.Get<StatsManager>().XcraftPlayerDeath(SurvivalContextsBroadcaster.instance.GetContext<DayTimeContext>().day);
			}
			if (Manager.Contains<SurvivalRankManager>() && !showScore)
			{
				SocialPlatformManager socialPlatformManager = Manager.Get<SocialPlatformManager>();
				if (socialPlatformManager == null || !socialPlatformManager.social.isLoggedIn)
				{
					return;
				}
				SurvivalRankManager survivalRankManager = Manager.Get<SurvivalRankManager>();
				survivalRankManager.Increase("SurvivedTime", SurvivalContextsBroadcaster.instance.GetContext<DayTimeContext>().day + 1);
				socialPlatformManager.social.SaveScore(Singleton<GooglePlayConstants>.get.GetIDFor(survivalRankManager.leaderboardID), (int)survivalRankManager.leaderboardPoints);
				survivalRankManager.PlayerDie();
			}
			if (showAd && Manager.Get<ModelManager>().allInOneAdRequirements.HasToShowAd("gameover").enabled)
			{
				//AdWaterfall.get.ShowInterstitialAd("gameover");
				Manager.Get<StatsManager>().AdShownWithReason(StatsManager.AdReason.XSURVIVAL_DEATH);
			}
		}

		public void OnShowLeaderboard()
		{
			if (Manager.Get<StateMachineManager>().ContainsState(typeof(RankingsState)))
			{
				Manager.Get<StateMachineManager>().PushState(typeof(RankingsState));
			}
		}

		private void ShowSurvivedTime()
		{
			float fullPassedTime = SurvivalContextsBroadcaster.instance.GetContext<DayTimeContext>().fullPassedTime;
			int days = (int)fullPassedTime;
			int hours = (int)((fullPassedTime - (float)(int)fullPassedTime) * 24f);
			base.connector.survived.AddVisitor(delegate(string text)
			{
				string text2 = text.Replace("{1}", days.ToString());
				return text2.Replace("{2}", hours.ToString());
			});
		}

		private void ShowScore()
		{
			UnityEngine.Debug.Log("Showing score");
			base.connector.survived.defaultText = "YOUR SCORE: ";
			base.connector.survived.translationKey = "gameover.points";
			base.connector.survived.AddVisitor(delegate(string text)
			{
				SurvivalRankManager survivalRankManager2 = Manager.Get<SurvivalRankManager>();
				int num = (!(survivalRankManager2 == null)) ? survivalRankManager2.LastPoints : (-1);
				return (!text.Contains("{0}")) ? $"{text}{num.ToString()}" : text.Replace("{0}", num.ToString());
			});
			base.connector.survived.ForceRefresh();
			SocialPlatformManager socialPlatformManager = Manager.Get<SocialPlatformManager>();
			if (!(socialPlatformManager == null) && socialPlatformManager.social.isLoggedIn)
			{
				SurvivalRankManager survivalRankManager = Manager.Get<SurvivalRankManager>();
				UnityEngine.Debug.Log(string.Format("Saving score for leaderboards : ", survivalRankManager.LastPoints));
				socialPlatformManager.social.SaveScore(Singleton<GooglePlayConstants>.get.GetIDFor(survivalRankManager.leaderboardID), survivalRankManager.LastPoints);
				survivalRankManager.PlayerDie();
			}
		}

		private void HideScore()
		{
			base.connector.survived.transform.gameObject.SetActive(value: false);
		}

		private void ShowMotivationalQuote()
		{
			base.connector.titlePanel.SetActive(value: false);
			base.connector.motivationalPanel.SetActive(value: true);
			List<string> allTranslationsWithKey = TranslationHelper.GetAllTranslationsWithKey(quoteKey);
			string text = allTranslationsWithKey.Random();
			if (allTranslationsWithKey != null && allTranslationsWithKey.Count > 0)
			{
				int num = allTranslationsWithKey.IndexOf(text);
				base.connector.motivationalQuote.text = "\"" + text + "\"";
				List<string> allTranslationsWithKey2 = TranslationHelper.GetAllTranslationsWithKey(quoteAuthorKey);
				if (num >= allTranslationsWithKey2.Count || num < 0)
				{
					num = 0;
				}
				base.connector.motivationalQuoteAuthor.text = "- " + allTranslationsWithKey2[num];
			}
		}

		private void OnReturn()
		{
			Manager.Get<GameCallbacksManager>().Restart();
			GameObject[] rootGameObjects = SceneManager.GetSceneByName("Initial").GetRootGameObjects();
			foreach (GameObject obj in rootGameObjects)
			{
				UnityEngine.Object.Destroy(obj);
			}
			Chunk[] array = Object.FindObjectsOfType<Chunk>();
			foreach (Chunk chunk in array)
			{
				UnityEngine.Object.Destroy(chunk.gameObject);
			}
			ChunkAnimation[] array2 = Object.FindObjectsOfType<ChunkAnimation>();
			foreach (ChunkAnimation chunkAnimation in array2)
			{
				UnityEngine.Object.Destroy(chunkAnimation.gameObject);
			}
			ChunkManager.ResetPool();
			SceneManager.UnloadScene("Gameplay");
			Manager.Get<StateMachineManager>().PopState();
			Manager.Get<StateMachineManager>().SetState<TitleState>();
			UnityEngine.Object.Destroy(GameObject.FindGameObjectWithTag("Player"));
		}

		private void OnLoadLastSave()
		{
			Manager.Get<GameCallbacksManager>().Restart();
			Chunk[] array = Object.FindObjectsOfType<Chunk>();
			foreach (Chunk chunk in array)
			{
				UnityEngine.Object.Destroy(chunk.gameObject);
			}
			ChunkAnimation[] array2 = Object.FindObjectsOfType<ChunkAnimation>();
			foreach (ChunkAnimation chunkAnimation in array2)
			{
				UnityEngine.Object.Destroy(chunkAnimation.gameObject);
			}
			ChunkManager.ResetPool();
			foreach (ChunkData value in ChunkManager.ChunkDatas.Values)
			{
				value.GetVoxelData();
			}
			SurvivalManager survivalManager = Manager.Get<SurvivalManager>();
			if (survivalManager != null)
			{
				survivalManager.DestroyEnemyMobs();
			}
			PlayerGraphic controlledPlayerInstance = PlayerGraphic.GetControlledPlayerInstance();
			if (controlledPlayerInstance != null)
			{
				UnityEngine.Object.Destroy(controlledPlayerInstance.gameObject);
			}
			Manager.Get<MobsManager>().DespawnAll();
			if (Manager.Contains<SaveTransformsManager>())
			{
				Manager.Get<SaveTransformsManager>().DespawnAll();
			}
			Manager.Get<StateMachineManager>().PopState();
			SceneManager.UnloadScene("Gameplay");
			Manager.Get<StateMachineManager>().SetState<LoadLevelState>();
			MonoBehaviourSingleton<GameplayFacts>.get.OnGameplayRestarted();
		}

		private void OnRestart()
		{
			Manager.Get<StateMachineManager>().PushState<ResetWorldPopUpState>();
		}

		private void PlaySound(GameSound gameSound)
		{
			Sound sound = new Sound();
			sound.clip = Manager.Get<ClipsManager>().GetClipFor(gameSound);
			sound.mixerGroup = Manager.Get<Gameplay.Audio.MixersManager>().uiMixerGroup;
			Sound sound2 = sound;
			sound2.Play();
		}
	}
}
