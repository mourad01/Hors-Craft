// DecompilerFi decompiler from Assembly-CSharp.dll class: TicketCraftItem
using Common.Managers;
using Gameplay;
using States;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TicketCraftItem : CraftItem
{
	public Button buyWithTicketsButton;

	public Action onAddTickets;

	private int ticketsForUnlock => Manager.Get<ModelManager>().ticketsSettings.GetBlueprintUnlockPrice();

	public override void Init(int id, int count, CraftableStatus status, Sprite sprite, string textToFill = "{0}", bool resource = false)
	{
		base.Init(id, count, status, sprite, textToFill);
		buyWithTicketsButton.onClick.RemoveAllListeners();
		buyWithTicketsButton.onClick.AddListener(delegate
		{
			OnUnlockBlueprint();
		});
		TranslateText componentInChildren = buyWithTicketsButton.GetComponentInChildren<TranslateText>();
		componentInChildren.AddVisitor((string t) => t.Replace("{0}", ticketsForUnlock.ToString()));
	}

	private void OnUnlockBlueprint()
	{
		if (Manager.Get<TicketsManager>().ownedTickets < ticketsForUnlock)
		{
			onAddTickets();
		}
		else
		{
			Manager.Get<StateMachineManager>().PushState<GenericPopupState>(new GenericPopupStateStartParameter
			{
				configureMessage = delegate(TranslateText t)
				{
					t.translationKey = "unlock.blueprint.for.tickets";
					t.defaultText = "Do you want to unlock blueprint for {0} tickets?";
					t.AddVisitor((string tt) => tt.Replace("{0}", ticketsForUnlock.ToString()));
				},
				configureLeftButton = delegate(Button b, TranslateText t)
				{
					t.translationKey = "menu.cancel";
					t.defaultText = "cancel";
					b.onClick.AddListener(delegate
					{
						Manager.Get<StateMachineManager>().PopState();
					});
				},
				configureRightButton = delegate(Button b, TranslateText t)
				{
					t.translationKey = "menu.ok";
					t.defaultText = "ok";
					b.onClick.AddListener(delegate
					{
						UnlockBlueprint();
						Manager.Get<StateMachineManager>().PopState();
					});
				}
			});
		}
	}

	private void UnlockBlueprint()
	{
		Manager.Get<TicketsManager>().ownedTickets -= ticketsForUnlock;
		List<TicketsFragment.BlueprintUnlockStats> blueprintsUnlockList = TicketsFragment.GetBlueprintsUnlockList();
		blueprintsUnlockList.FirstOrDefault((TicketsFragment.BlueprintUnlockStats bl) => bl.id == id).unlocked = true;
		TicketsFragment.SaveList(blueprintsUnlockList);
	}
}
