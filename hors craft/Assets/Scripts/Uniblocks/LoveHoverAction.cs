// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.LoveHoverAction
using Common.Managers;
using UnityEngine;

namespace Uniblocks
{
	public class LoveHoverAction : HoverAction
	{
		private InteractiveObjectContext pickupContext;

		private InteractiveObjectContext giftContext;

		public LoveHoverAction(HoverActionsConnector connector)
			: base(connector)
		{
		}

		public override void Update(RaycastHitInfo hitInfo)
		{
			base.Update(hitInfo);
			Collider collider = hitInfo.hit.collider;
			if (IsTargetLovedOne(collider))
			{
				UpdateInFrontOfLovedOne(collider);
			}
			else if (IsTargetUnLovable(collider))
			{
				UpdateInFrontOfUnLovable(collider);
			}
			else if (IsTargetLovable(collider))
			{
				UpdateInFrontOfLovable(collider);
			}
			else
			{
				RemoveContexts();
			}
		}

		private void UpdateInFrontOfLovable(Collider hit)
		{
			if (IsDifferentThanBefore(pickupContext, hit))
			{
				RemoveContexts();
				pickupContext = new InteractiveObjectContext
				{
					useAction = OnPickup,
					obj = hit.gameObject
				};
				MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.IN_FRONT_OF_LOVABLE, pickupContext);
			}
		}

		private void UpdateInFrontOfUnLovable(Collider hit)
		{
			MonoBehaviourSingleton<GameplayFacts>.get.AddFactIfNotExisting(Fact.IN_FRONT_OF_UNLOVABLE);
		}

		private void UpdateInFrontOfLovedOne(Collider hit)
		{
			if (IsDifferentThanBefore(giftContext, hit))
			{
				RemoveContexts();
				giftContext = new InteractiveObjectContext
				{
					useAction = OnGiveGift,
					obj = hit.gameObject
				};
				MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.IN_FRONT_OF_LOVED_ONE, giftContext);
			}
		}

		private void RemoveContexts()
		{
			if (giftContext != null)
			{
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.IN_FRONT_OF_LOVED_ONE, giftContext);
				giftContext = null;
			}
			if (pickupContext != null)
			{
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.IN_FRONT_OF_LOVABLE, pickupContext);
				pickupContext = null;
			}
			MonoBehaviourSingleton<GameplayFacts>.get.RemoveFact(Fact.IN_FRONT_OF_UNLOVABLE);
		}

		private bool IsDifferentThanBefore(InteractiveObjectContext context, Collider target)
		{
			return context == null || context.obj != target.gameObject;
		}

		private bool IsTargetLovedOne(Collider hit)
		{
			return hit != null && hit.gameObject.GetComponentInParent<LovedOne>() != null;
		}

		private bool IsTargetLovable(Collider hit)
		{
			return hit != null && hit.gameObject.GetComponentInParent<Mob>() != null && Manager.Get<LoveManager>().IsTargetLovable(hit.gameObject.GetComponentInParent<Mob>().gameObject);
		}

		private bool IsTargetUnLovable(Collider hit)
		{
			return hit != null && hit.gameObject.GetComponentInParent<Mob>() != null && Manager.Get<LoveManager>().IsTargetUnLovable(hit.gameObject.GetComponentInParent<Mob>().gameObject);
		}

		private void OnGiveGift()
		{
			Manager.Get<LoveManager>().OpenGifts();
		}

		private void OnPickup()
		{
			Mob componentInParent = hitInfo.hit.collider.gameObject.GetComponentInParent<Mob>();
			Manager.Get<LoveManager>().TryToDate(componentInParent.gameObject);
		}
	}
}
