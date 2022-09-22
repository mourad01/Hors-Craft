// DecompilerFi decompiler from Assembly-CSharp.dll class: States.RankingsFragment
using Common.Managers;
using GameUI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class RankingsFragment : Fragment
	{
		public GameObject scoreElement;

		public Transform scoreParent;

		public Transform playerScoreParent;

		public BookmarksPanelController bookmarkController;

		private bool skipFragmentUpdate;

		public override void Init(FragmentStartParameter parameter)
		{
			base.Init(parameter);
			UpdateFragment();
		}

		public override void UpdateFragment()
		{
			if (skipFragmentUpdate)
			{
				skipFragmentUpdate = false;
				return;
			}
			base.UpdateFragment();
			List<string> allowedRankings = Manager.Get<RankingManager>().allowedRankings;
			List<BookmarksPanelController.BookmarkConfig> list = new List<BookmarksPanelController.BookmarkConfig>();
			for (int i = 0; i < allowedRankings.Count; i++)
			{
				string ranking = allowedRankings[i];
				BookmarksPanelController.BookmarkConfig bookmarkConfig = new BookmarksPanelController.BookmarkConfig();
				bookmarkConfig.translationKey = "ranking." + ranking;
				bookmarkConfig.defaultText = ranking;
				bookmarkConfig.onChoose = delegate
				{
					UpdateRanking(ranking);
				};
				list.Add(bookmarkConfig);
			}
			bookmarkController.Rebuild(list);
			if (list.Count > 1)
			{
				bookmarkController.Choose(0);
			}
			else
			{
				UpdateRanking(allowedRankings[0]);
			}
		}

		private void UpdateRanking(string rankingId)
		{
			Manager.Get<RankingManager>().ShowRanking(rankingId, delegate(RankingModel m)
			{
				OnRankingDownloaded(m, rankingId);
			}, OnRankingError);
		}

		private void OnRankingDownloaded(RankingModel model, string rankingId)
		{
			while (scoreParent.childCount > 0)
			{
				UnityEngine.Object.DestroyImmediate(scoreParent.GetChild(0).gameObject);
			}
			bool flag = false;
			for (int i = 0; i < model.top.Count; i++)
			{
				RankingModel.ScoreData scoreData = model.top[i];
				if (scoreData.isItMe)
				{
					CreatePlayerInTopElement(scoreData);
					flag = true;
				}
				else
				{
					CreateScoreElement(scoreData);
				}
			}
			RankingModel.ScoreData scoreData2 = model.neighbours.FirstOrDefault((RankingModel.ScoreData s) => s.isItMe);
			if (!flag)
			{
				if (scoreData2 == null)
				{
					CreatePlayerNotInRankingElement();
				}
				else
				{
					CreatePlayerNotInTopElement(scoreData2);
				}
			}
		}

		private RankingScoreElement CreateScoreElement(RankingModel.ScoreData score)
		{
			GameObject gameObject = Object.Instantiate(scoreElement, scoreParent, worldPositionStays: false);
			gameObject.transform.localScale = Vector3.one;
			RankingScoreElement component = gameObject.GetComponent<RankingScoreElement>();
			component.SetElement(score);
			return component;
		}

		private void CreatePlayerInTopElement(RankingModel.ScoreData score)
		{
			RankingScoreElement rankingScoreElement = CreateScoreElement(score);
			playerScoreParent.gameObject.SetActive(value: false);
			rankingScoreElement.background.color = Manager.Get<ColorManager>().GetColorForCategory(ColorManager.ColorCategory.MAIN_COLOR);
		}

		private void CreatePlayerNotInTopElement(RankingModel.ScoreData score)
		{
			playerScoreParent.gameObject.SetActive(value: true);
			RankingScoreElement rankingScoreElement;
			if (playerScoreParent.childCount > 0)
			{
				rankingScoreElement = playerScoreParent.GetChild(0).GetComponent<RankingScoreElement>();
				rankingScoreElement.SetElement(score);
			}
			else
			{
				rankingScoreElement = CreateScoreElement(score);
				rankingScoreElement.transform.SetParent(playerScoreParent, worldPositionStays: false);
				rankingScoreElement.transform.localScale = Vector3.one;
			}
			RectTransform rectTransform = rankingScoreElement.transform as RectTransform;
			rectTransform.anchorMin = new Vector2(0f, 0.5f);
			rectTransform.anchorMax = new Vector2(1f, 0.5f);
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;
			RectTransform rectTransform2 = rectTransform;
			Vector2 sizeDelta = rectTransform.sizeDelta;
			rectTransform2.sizeDelta = new Vector2(sizeDelta.x, rankingScoreElement.gameObject.GetComponent<LayoutElement>().preferredHeight);
		}

		private void CreatePlayerNotInRankingElement()
		{
			RankingModel.ScoreData lastScoreFromPrefs = Manager.Get<RankingManager>().GetLastScoreFromPrefs();
			if (lastScoreFromPrefs == null)
			{
				playerScoreParent.gameObject.SetActive(value: false);
				return;
			}
			lastScoreFromPrefs.position = -1;
			CreatePlayerNotInTopElement(lastScoreFromPrefs);
		}

		private void OnRankingError()
		{
			Manager.Get<StateMachineManager>().PushState<GenericPopupState>(new GenericPopupStateStartParameter
			{
				configureMessage = delegate(TranslateText t)
				{
					t.translationKey = "error.occured";
					t.defaultText = "Lost connection. Try again later!";
				},
				configureLeftButton = delegate(Button b, TranslateText t)
				{
					b.gameObject.SetActive(value: false);
				},
				configureRightButton = delegate(Button b, TranslateText t)
				{
					b.onClick.AddListener(delegate
					{
						skipFragmentUpdate = true;
						Manager.Get<StateMachineManager>().PopState();
						Manager.Get<StateMachineManager>().PopState();
					});
					t.translationKey = "menu.ok";
					t.defaultText = "OK :(";
				}
			});
		}
	}
}
