// DecompilerFi decompiler from Assembly-CSharp.dll class: Cooking.BaseIngredientDevice
using Common.Managers;
using States;
using System.Linq;
using UnityEngine;

namespace Cooking
{
	public class BaseIngredientDevice : TappableDevice
	{
		public int portionsMax;

		public bool canPickUp = true;

		[HideInInspector]
		public int moneyValue;

		public int portionsLeft
		{
			get
			{
				int num = base.workController.workData.PortionsLeft(this);
				if (base.workController.cookingGameplay != null && base.workController.cookingGameplay.isTutorialOrMinigame)
				{
					num = Mathf.Max(num, 1);
				}
				return num;
			}
			set
			{
				if (value >= portionsLeft || !(base.workController.cookingGameplay != null) || !base.workController.cookingGameplay.isTutorialOrMinigame)
				{
					base.workController.workData.SetPortionsLeft(this, value);
				}
			}
		}

		public void Refill()
		{
			portionsLeft = portionsMax;
		}

		public override bool CanTap(Worker worker)
		{
			return (canPickUp && worker.CanPickSomething()) || CanUseOnSomething(worker);
		}

		private bool CanUseOnSomething(Worker worker)
		{
			if (portionsLeft == 0 && !canPickUp)
			{
				ShowFillIngredientsPopup();
			}
			return portionsLeft > 0 && worker.heldProducts.Any((Product p) => base.workController.recipesList.CanUse(p, this));
		}

		private bool CanPickUpSomething(Worker worker)
		{
			if (portionsLeft == 0)
			{
				ShowFillIngredientsPopup();
			}
			return worker.CanPickSomething() && portionsLeft > 0;
		}

		public override void OnInteraction(Worker worker)
		{
			if (CanUseOnSomething(worker))
			{
				Product product = worker.heldProducts.First((Product p) => base.workController.recipesList.CanUse(p, this));
				worker.ReplaceProduct(this, product);
				portionsLeft--;
			}
			else if (CanPickUpSomething(worker))
			{
				worker.PickUpProduct(SpawnNewProduct(null));
				portionsLeft--;
			}
		}

		private void ShowFillIngredientsPopup()
		{
			CookingGameplayState cookingGameplayState = Manager.Get<StateMachineManager>().currentState as CookingGameplayState;
			if (cookingGameplayState != null)
			{
				cookingGameplayState.ShowFillIngredientsGO();
			}
		}

		protected override void SetUpgradeValues(UpgradeEffect effect, float value)
		{
		}
	}
}
