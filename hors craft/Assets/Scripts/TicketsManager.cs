// DecompilerFi decompiler from Assembly-CSharp.dll class: TicketsManager
using Common.Managers;
using Gameplay;
using States;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TicketsManager : Manager
{
	public string leaderboardName = "leaderboardParkLevel";

	[SerializeField]
	private int ticketResourceId = 11;

	public GameObject ticketFragmentResourcesPrefab;

	public int ownedTickets
	{
		get
		{
			if (ticketResourceId == -1)
			{
				return int.MaxValue;
			}
			return Singleton<PlayerData>.get.playerItems.GetResourcesCount(ticketResourceId);
		}
		set
		{
			int count = value - ownedTickets;
			Singleton<PlayerData>.get.playerItems.AddToResources(ticketResourceId, count);
		}
	}

	public override void Init()
	{
		ProgressManager progressManager = Manager.Get<ProgressManager>();
		progressManager.onLevelUpCallbacks = (Action)Delegate.Combine(progressManager.onLevelUpCallbacks, new Action(SaveLevelToLeaderboards));
	}

	public static void TakeEntranceFeeIfPossible(Action useAction)
	{
		if (Manager.Contains<TicketsManager>())
		{
			Manager.Get<TicketsManager>().TakeEntranceFee(useAction);
		}
		else
		{
			useAction();
		}
	}

	public void TakeEntranceFee(Action onSuccess)
	{
		int ticketsForEntrance = Manager.Get<ModelManager>().ticketsSettings.GetTicketsForEntrance();
		if (ownedTickets < ticketsForEntrance)
		{
			OnAddTickets();
		}
		else
		{
			Manager.Get<StateMachineManager>().PushState<GenericPopupState>(new GenericPopupStateStartParameter
			{
				configureMessage = delegate(TranslateText t)
				{
					t.translationKey = "tickets.entrance.fee.message";
					t.defaultText = "Pay {0} ticket to use this machine";
					t.AddVisitor((string tt) => tt.Replace("{0}", ticketsForEntrance.ToString()));
				},
				configureLeftButton = delegate(Button b, TranslateText t)
				{
					b.onClick.AddListener(delegate
					{
						Manager.Get<StateMachineManager>().PopState();
					});
					t.translationKey = "menu.cancel";
					t.defaultText = "cancel";
				},
				configureRightButton = delegate(Button b, TranslateText t)
				{
					b.onClick.AddListener(delegate
					{
						ownedTickets -= ticketsForEntrance;
						Manager.Get<StateMachineManager>().PopState();
						onSuccess();
					});
					t.translationKey = "menu.ok";
					t.defaultText = "ok";
				}
			});
		}
	}

	public void OnAddTickets()
	{
		int ticketsForAd = Manager.Get<ModelManager>().ticketsSettings.GetTicketsForAd();
		string text = Manager.Get<TranslationsManager>().GetText("watch.ad.get.tickets", "Watch ad to get {0} tickets");
		text = text.Replace("{0}", ticketsForAd.ToString());
		Manager.Get<StateMachineManager>().PushState<WatchXAdsPopUpState>(new WatchXAdsPopUpStateStartParameter
		{
			onSuccess = delegate(bool s)
			{
				if (s)
				{
					Manager.Get<TicketsManager>().ownedTickets += ticketsForAd;
				}
			},
			reason = StatsManager.AdReason.XCRAFT_CURRENCY,
			type = AdsCounters.None,
			translationKey = "invalid.key.on.purpose.such.programming.much.sense",
			description = text,
			allowRemoveAdsButton = false,
			configWatchButton = delegate(GameObject go)
			{
				TranslateText componentInChildren = go.GetComponentInChildren<TranslateText>();
				componentInChildren.translationKey = "menu.watch";
				componentInChildren.defaultText = "watch";
				componentInChildren.ForceRefresh();
			}
		});
	}

	private void SaveLevelToLeaderboards()
	{
		Manager.Get<SocialPlatformManager>().social.SaveScore(Singleton<GooglePlayConstants>.get.GetIDFor(leaderboardName), Manager.Get<ProgressManager>().level);
	}
}
