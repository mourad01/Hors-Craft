// DecompilerFi decompiler from Assembly-CSharp.dll class: States.SoftCurrencyModule
using Common.GameUI;
using Common.Managers;
using Gameplay;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class SoftCurrencyModule : GameplayModule
	{
		public Button addButton;

		public Text moneyText;

		public AnimatedTextCounter textCounter;

		public Image currencyIcon;

		protected override Fact[] listenedFacts => new Fact[2]
		{
			Fact.SOFT_CURRENCY_CHANGED,
			Fact.EVENT_POSITION
		};

		private new void Update()
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.Semicolon))
			{
				Manager.Get<CraftSoftCurrencyManager>().OnCurrencyAmountChange(100);
				MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.EVENT_POSITION, new PositionSignalContext
				{
					position = PlayerGraphic.GetControlledPlayerInstance().transform.position + PlayerGraphic.GetControlledPlayerInstance().transform.forward * 3f
				});
			}
		}

		public override void Init()
		{
			base.Init();
			if (!Application.isPlaying)
			{
				return;
			}
			addButton.onClick.AddListener(delegate
			{
				OnAdd();
			});
			if (currencyIcon != null)
			{
				Sprite sprite = Manager.Get<ShopManager>()?.GetCurrencyItem("soft")?.sprite;
				if (sprite != null)
				{
					currencyIcon.sprite = sprite;
				}
			}
		}

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			if (Application.isPlaying && changedFacts.Contains(Fact.SOFT_CURRENCY_CHANGED) && !SpawnFromItemToPocket() && !SpawnFromPositionSignal())
			{
				textCounter.AnimateTo(Manager.Get<AbstractSoftCurrencyManager>().GetCurrencyAmount(), immediate: false);
			}
		}

		private bool SpawnFromItemToPocket()
		{
			ItemToPocketContext factContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<ItemToPocketContext>(Fact.SOFT_CURRENCY_CHANGED);
			if (factContext != null)
			{
				PickupJumpingToYourPocketUI.SpawnPickup(factContext.startPosition, base.gameObject, factContext.icon, 2f, delegate
				{
					textCounter.AnimateTo(Manager.Get<AbstractSoftCurrencyManager>().GetCurrencyAmount(), immediate: false);
				});
				return true;
			}
			return false;
		}

		private bool SpawnFromPositionSignal()
		{
			CurrencyChangedContext factContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<CurrencyChangedContext>(Fact.SOFT_CURRENCY_CHANGED);
			PositionSignalContext factContext2 = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<PositionSignalContext>(Fact.EVENT_POSITION);
			Sprite sprite = Manager.Get<ShopManager>()?.GetCurrencyItem("soft")?.sprite;
			if (factContext != null && factContext2 != null && sprite != null)
			{
				int valueChanged = factContext.valueChanged;
				int value = (int)Mathf.Log(valueChanged, (float)Math.E);
				value = Mathf.Clamp(value, 1, 10);
				StartCoroutine(SpawnCoins(factContext2.position, sprite, value, valueChanged));
				return true;
			}
			return false;
		}

		private IEnumerator SpawnCoins(Vector3 pos, Sprite sprite, int coinCount, int amount)
		{
			GameObject target = base.gameObject.transform.GetChild(0).gameObject;
			int currentSum = Manager.Get<AbstractSoftCurrencyManager>().GetCurrencyAmount() - amount;
			int amountPerCoin = amount / coinCount;
			while (coinCount > 0)
			{
				yield return new WaitForSeconds(0.15f);
				if (coinCount == 1)
				{
					currentSum = Manager.Get<AbstractSoftCurrencyManager>().GetCurrencyAmount();
				}
				else
				{
					currentSum += amountPerCoin;
				}
				PickupJumpingToYourPocketUI.SpawnPickup(scale: UnityEngine.Random.Range(0.6f, 1.2f), startPosition: pos + UnityEngine.Random.insideUnitSphere * 0.2f, target: target, sprite: sprite, onFinish: delegate
				{
					AnimateText(currentSum);
				});
				coinCount--;
			}
		}

		private void AnimateText(int target)
		{
			textCounter.AnimateTo(target, immediate: false);
		}

		private void OnAdd()
		{
			Manager.Get<AbstractSoftCurrencyManager>().GetMoreCurrency();
		}
	}
}
