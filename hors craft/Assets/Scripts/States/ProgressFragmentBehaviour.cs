// DecompilerFi decompiler from Assembly-CSharp.dll class: States.ProgressFragmentBehaviour
using Common.Managers;
using Gameplay;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	[RequireComponent(typeof(ProgressFragment))]
	public class ProgressFragmentBehaviour : MonoBehaviour
	{
		public List<string> trackedProgress = new List<string>();

		protected ProgressFragment fragment;

		private void Awake()
		{
			fragment = GetComponent<ProgressFragment>();
			SetFragment();
		}

		public void UpdateFragment()
		{
			SetFragment();
		}

		private void SetFragment()
		{
			SetLevelBar();
			SetStats();
			SetCurrencies();
			SetListener(fragment.rankingButton, OnRankings);
		}

		private void SetLevelBar()
		{
			int level = Manager.Get<ProgressManager>().level;
			float value = (float)Manager.Get<ProgressManager>().experience / (float)Manager.Get<ProgressManager>().experienceNeededToNextLevel;
			fragment.levelNumber.text = level.ToString();
			fragment.levelSlider.value = value;
		}

		protected virtual void SetStats()
		{
			fragment.ClearStats();
			foreach (string item in trackedProgress)
			{
				int countFor = MonoBehaviourSingleton<ProgressCounter>.get.GetCountFor(item);
				string text = Manager.Get<TranslationsManager>().GetText("progress." + item.ToLower(), item.ToUpper().Replace('_', ' ').Replace('.', ' ') + ": ");
				fragment.AddProgressStat(text, countFor.ToString());
			}
		}

		private void SetCurrencies()
		{
			foreach (CurrencySlot currencySlot in fragment.currencySlots)
			{
				object obj;
				if (currencySlot.currency.CanGetMoreCurrency())
				{
					CurrencyScriptableObject currency = currencySlot.currency;
					obj = new Action(currency.GetMoreCurrency);
				}
				else
				{
					obj = null;
				}
				Action onAdd = (Action)obj;
				currencySlot.itemUI.Init(currencySlot.currency, onAdd);
			}
		}

		private void SetListener(Button button, Action action)
		{
			if (!(button == null) && action != null)
			{
				button.onClick.RemoveAllListeners();
				button.onClick.AddListener(delegate
				{
					action();
				});
			}
		}

		private void OnRankings()
		{
			if (Manager.Contains<RankingManager>())
			{
				Manager.Get<StateMachineManager>().GetStateInstance<PauseState>().TrySwitchFragmenTo("Ranking");
			}
			else
			{
				Manager.Get<SocialPlatformManager>().social.ShowRankings();
			}
		}
	}
}
