// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.MountMobsHoverAction
using Common.Managers;
using Gameplay;
using UnityEngine;

namespace Uniblocks
{
	public class MountMobsHoverAction : HoverAction
	{
		private Mountable mountable;

		private InteractiveObjectContext mountableContext;

		private InteractiveObjectContext mountedContext;

		public MountMobsHoverAction(HoverActionsConnector connector)
			: base(connector)
		{
			mountable = null;
		}

		public override void Update(RaycastHitInfo hitInfo)
		{
			base.Update(hitInfo);
			if (hitInfo.hit.collider != null)
			{
				mountable = hitInfo.hit.collider.gameObject.GetComponentInParent<Mountable>();
				if (mountable != null && IsTamed(hitInfo.hit.collider.gameObject))
				{
					if (mountableContext == null || mountable.gameObject != mountableContext.obj)
					{
						RemoveMountableFact();
						CreateContext(mountable.gameObject);
						AddMountableFact();
					}
				}
				else
				{
					RemoveMountableFact();
				}
			}
			else
			{
				RemoveMountableFact();
			}
		}

		private void CreateContext(GameObject go)
		{
			mountableContext = new InteractiveObjectContext
			{
				obj = go,
				useAction = OnClickMount
			};
		}

		private void RemoveMountableFact()
		{
			MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.IN_FRONT_OF_MOUNTABLE_MOB, mountableContext);
			mountableContext = null;
		}

		private void AddMountableFact()
		{
			MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.IN_FRONT_OF_MOUNTABLE_MOB, mountableContext);
		}

		private void OnClickMount()
		{
			if (mountable != null && !hoverContext.movement.IsMounted())
			{
				Mount();
			}
			else if (hoverContext.movement.IsMounted())
			{
				Unmount();
			}
		}

		public void ForceMount(Mountable mountable)
		{
			if (hoverContext.movement.IsMounted())
			{
				Unmount();
			}
			this.mountable = mountable;
			Mount();
		}

		private void Mount()
		{
			if (mountedContext == null)
			{
				mountedContext = new InteractiveObjectContext
				{
					obj = mountable.gameObject,
					useAction = Unmount
				};
			}
			MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.MOUNTED_MOB, mountedContext);
			hoverContext.movement.Mount(mountable);
		}

		public void Unmount()
		{
			MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.MOUNTED_MOB, mountedContext);
			mountedContext = null;
			hoverContext.movement.Unmount();
		}

		private bool IsTamed(GameObject mob)
		{
			Pettable componentInParent = mob.GetComponentInParent<Pettable>();
			if (componentInParent == null || componentInParent.tamed)
			{
				return true;
			}
			return false;
		}

		private bool IsTalkEnabled()
		{
			Collider collider = hitInfo.hit.collider;
			if (collider != null && collider.gameObject.layer == 16 && Manager.Get<ModelManager>().chatbotSettings.IsChatBotEnabled() && Manager.Get<ModelManager>().chatbotSettings.IsWebChatBotEnabled())
			{
				Mob componentInParent = collider.gameObject.GetComponentInParent<Mob>();
				if (componentInParent != null && componentInParent.GetComponentInChildren<PlayerGraphic>() != null)
				{
					return true;
				}
			}
			return false;
		}
	}
}
