// DecompilerFi decompiler from Assembly-CSharp.dll class: States.TicketsPanelFragment
using Common.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class TicketsPanelFragment : Fragment
	{
		public Slider prestigeProgress;

		public Text prestigeLevel;

		public Text ticketsNumber;

		public Text minigamesPlayed;

		public Text blueprintsFilled;

		public Text craftedItems;

		public Button leaderboardsButton;

		public Button addTicketsButton;

		private CraftingFragment.CrafttStartParameter startParam;

		private TicketsFragment ticketsFragment => startParam.parentFragment as TicketsFragment;

		public override void Init(FragmentStartParameter parameter)
		{
			base.transform.localScale = Vector3.one;
			startParam = (parameter as CraftingFragment.CrafttStartParameter);
			leaderboardsButton.onClick.RemoveAllListeners();
			leaderboardsButton.onClick.AddListener(delegate
			{
				OnLeaderboards();
			});
			addTicketsButton.onClick.RemoveAllListeners();
			addTicketsButton.onClick.AddListener(delegate
			{
				ticketsFragment.OnAddTickets();
			});
			InitFields();
		}

		public override void UpdateFragment()
		{
			base.UpdateFragment();
			InitFields();
		}

		private void InitFields()
		{
			int level = Manager.Get<ProgressManager>().level;
			float value = (float)Manager.Get<ProgressManager>().experience / (float)Manager.Get<ProgressManager>().experienceNeededToNextLevel;
			prestigeLevel.text = level.ToString();
			prestigeProgress.value = value;
			int ownedTickets = Manager.Get<TicketsManager>().ownedTickets;
			ticketsNumber.text = ownedTickets.ToString();
			minigamesPlayed.text = MonoBehaviourSingleton<ProgressCounter>.get.GetCountFor(ProgressCounter.Countables.MINIGAMES_PLAYED).ToString();
			blueprintsFilled.text = MonoBehaviourSingleton<ProgressCounter>.get.GetCountFor(ProgressCounter.Countables.BLUEPRINTS_FINISHED).ToString();
			craftedItems.text = MonoBehaviourSingleton<ProgressCounter>.get.GetCountFor(ProgressCounter.Countables.ITEMS_CRAFTED).ToString();
		}

		private void OnLeaderboards()
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
