// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.DoorsHoverAction
using System;
using UnityEngine;

namespace Uniblocks
{
	public class DoorsHoverAction : HoverAction
	{
		protected InteractiveObjectContext doorsContext;

		private RotateObjectContext rotateContext;

		public static Action<Vector3, Action<bool>> onDoorsInteract;

		public DoorsHoverAction(HoverActionsConnector connector)
			: base(connector)
		{
			doorsContext = new InteractiveObjectContext
			{
				useAction = OnDoorsClick
			};
		}

		public override void Update(RaycastHitInfo hitInfo)
		{
			base.Update(hitInfo);
			if (hitInfo.hit.collider != null && hitInfo.hit.collider.gameObject.layer == 18)
			{
				doorsContext.obj = hitInfo.hit.collider.gameObject;
				MonoBehaviourSingleton<GameplayFacts>.get.AddFactIfNotExisting(Fact.IN_FRONT_OF_DOORS, doorsContext);
				CheckRotateContext(hitInfo);
				return;
			}
			MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.IN_FRONT_OF_DOORS, doorsContext);
			if (rotateContext != null)
			{
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.IN_FRONT_OF_ROTATABLE, rotateContext);
				rotateContext = null;
			}
		}

		private void CheckRotateContext(RaycastHitInfo hitInfo)
		{
			if (rotateContext == null || rotateContext.obj != hitInfo.hit.collider.gameObject)
			{
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.IN_FRONT_OF_ROTATABLE, rotateContext);
				rotateContext = new RotateObjectContext
				{
					obj = hitInfo.hit.collider.gameObject,
					onRotate = OnRotate
				};
				MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.IN_FRONT_OF_ROTATABLE, rotateContext);
			}
		}

		public void OnDoorsClick()
		{
			DoorTrigger componentInChildren;
			if (!(hitInfo.hit.collider != null) || !((componentInChildren = hitInfo.hit.collider.GetComponentInChildren<DoorTrigger>()) != null))
			{
				return;
			}
			VoxelInfo raycast = componentInChildren.GetVoxelInfo();
			if (raycast != null)
			{
				if (onDoorsInteract == null)
				{
					hoverContext.OnVoxelPlace(raycast);
				}
				else
				{
					onDoorsInteract(componentInChildren.transform.position, delegate(bool result)
					{
						if (result)
						{
							hoverContext.OnVoxelPlace(raycast);
						}
					});
				}
			}
		}

		private void OnRotate()
		{
			DoorTrigger componentInChildren = hitInfo.hit.collider.GetComponentInChildren<DoorTrigger>();
			VoxelInfo voxelInfo = componentInChildren.GetVoxelInfo();
			hoverContext.OnVoxelRotate(voxelInfo);
		}
	}
}
