// DecompilerFi decompiler from Assembly-CSharp.dll class: States.TicketsFragment
using Common.Managers;
using Common.Utils;
using Gameplay;
using System.Collections.Generic;
using UnityEngine;

namespace States
{
	public class TicketsFragment : CraftingFragment
	{
		public class BlueprintUnlockStats
		{
			public int id;

			public bool unlocked;
		}

		public int ticketsForAd => Manager.Get<ModelManager>().ticketsSettings.GetTicketsForAd();

		private static List<BlueprintUnlockStats> blueprintsUnlockStatus
		{
			get
			{
				string @string = PlayerPrefs.GetString("blueprints.unlocked.by.tickets");
				if (string.IsNullOrEmpty(@string))
				{
					return new List<BlueprintUnlockStats>();
				}
				return JSONHelper.Deserialize<List<BlueprintUnlockStats>>(@string);
			}
			set
			{
				string value2 = JSONHelper.ToJSON(value);
				PlayerPrefs.SetString("blueprints.unlocked.by.tickets", value2);
			}
		}

		public static List<BlueprintUnlockStats> GetBlueprintsUnlockList()
		{
			return blueprintsUnlockStatus;
		}

		public static void SaveList(List<BlueprintUnlockStats> list)
		{
			blueprintsUnlockStatus = list;
		}

		public override void Init(FragmentStartParameter parameter)
		{
			if (Manager.Contains<TicketsManager>())
			{
				TicketsManager ticketsManager = Manager.Get<TicketsManager>();
				if (ticketsManager.ticketFragmentResourcesPrefab != null)
				{
					resourcesPrefab = ticketsManager.ticketFragmentResourcesPrefab;
				}
			}
			base.Init(parameter);
			enableResourcesForAd = false;
			resourcesInstance.SetActive(value: true);
			resourcesInstance.transform.SetAsLastSibling();
		}

		public override void UpdateFragment()
		{
			base.UpdateFragment();
			resourcesInstance.transform.SetAsLastSibling();
			resourcesInstance.SetActive(value: true);
		}

		public override void SetState(State stateToActive, CrafttStartParameter parameter)
		{
			base.SetState(stateToActive, parameter);
			resourcesInstance.transform.SetAsLastSibling();
		}

		public void OnAddTickets()
		{
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

		public override void onCraft(int itemId)
		{
			enableResourcesForAd = true;
			base.onCraft(itemId);
			enableResourcesForAd = false;
		}
	}
}
