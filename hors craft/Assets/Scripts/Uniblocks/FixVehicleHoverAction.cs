// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.FixVehicleHoverAction
using Common.Managers;
using UnityEngine;

namespace Uniblocks
{
	public class FixVehicleHoverAction : HoverAction
	{
		private Fixable fixingTarget;

		private FixableContext fixableContext;

		private const float FIX_CD = 1f;

		private float fixTimer;

		public FixVehicleHoverAction(HoverActionsConnector connector)
			: base(connector)
		{
		}

		public override void Update(RaycastHitInfo hitInfo)
		{
			base.Update(hitInfo);
			bool flag = false;
			if (hitInfo.hit.collider != null)
			{
				Fixable componentInParent = hitInfo.hit.collider.gameObject.GetComponentInParent<Fixable>();
				if (componentInParent != null && componentInParent.health.hp < componentInParent.health.maxHp)
				{
					if (fixableContext == null || fixableContext.fixable != componentInParent)
					{
						AddFixableFact(componentInParent);
					}
					flag = true;
					fixingTarget = componentInParent;
				}
			}
			if (!flag)
			{
				RemoveFixableFact();
			}
			if (fixTimer > 0f)
			{
				fixTimer -= Time.deltaTime;
			}
		}

		private void AddFixableFact(Fixable fixable)
		{
			RemoveFixableFact();
			fixableContext = new FixableContext
			{
				fixable = fixable.gameObject,
				OnFix = OnFix
			};
			MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.IN_FRONT_OF_FIXABLE, fixableContext);
		}

		private void RemoveFixableFact()
		{
			if (fixableContext != null)
			{
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.IN_FRONT_OF_FIXABLE, fixableContext);
				fixableContext = null;
			}
		}

		private bool IsPlayerInside(VehicleController vehicle)
		{
			return vehicle.GetComponentInChildren<PlayerMovement>() != null;
		}

		private void OnFix()
		{
			if (!(fixTimer <= 0f))
			{
				return;
			}
			if (fixingTarget.TryFix())
			{
				if (fixingTarget.health.hp >= fixingTarget.health.maxHp)
				{
					string text = Manager.Get<TranslationsManager>().GetText("fixing.full", "Fully fixed!").ToUpper();
					Manager.Get<ToastManager>().ShowToast(text);
				}
				fixTimer = 1f;
			}
			else
			{
				string text2 = Manager.Get<TranslationsManager>().GetText("fixing.no.resources", "Not enough {0}!").ToUpper();
				text2 = text2.Replace("{0}", Manager.Get<CraftingManager>().GetResourceDefinition(fixingTarget.resourceId).image.name);
				Manager.Get<ToastManager>().ShowToast(text2);
			}
		}
	}
}
