// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.SwitchHoverAction
using UnityEngine;

namespace Uniblocks
{
	public class SwitchHoverAction : HoverAction
	{
		private VoxelEvents hoveredEvents;

		private SwitchableVoxelContext context;

		public SwitchHoverAction(HoverActionsConnector connector)
			: base(connector)
		{
		}

		public override void Update(RaycastHitInfo hitInfo)
		{
			base.Update(hitInfo);
			if (hitInfo.voxelHit != null)
			{
				hoveredEvents = Singleton<VoxelEventsInstancesCache>.get.GetInstanceForVoxelId(hitInfo.voxelHit.GetVoxel());
			}
			else
			{
				hoveredEvents = null;
			}
			if (hoveredEvents != null && hoveredEvents is SwitchVoxelEvents)
			{
				if (context == null || hitInfo.voxelHit.index != context.voxelInfo.index)
				{
					CreateContext(hitInfo.voxelHit);
				}
				MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.IN_FRONT_OF_SWITCHABLE_VOXEL, context);
			}
			else if (context != null)
			{
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.IN_FRONT_OF_SWITCHABLE_VOXEL, context);
				context = null;
			}
		}

		private void CreateContext(VoxelInfo info)
		{
			MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.IN_FRONT_OF_SWITCHABLE_VOXEL, context);
			context = new SwitchableVoxelContext
			{
				voxelInfo = info,
				useAction = OnSwitch
			};
		}

		private void OnSwitch()
		{
			GameObject prefabInstance = (hoveredEvents as SwitchVoxelEvents).prefabInstance;
			if (prefabInstance != null)
			{
				UsableIndicator component = prefabInstance.GetComponent<UsableIndicator>();
				if (component != null)
				{
					component.Interact();
				}
			}
			(hoveredEvents as SwitchVoxelEvents).Switch(hitInfo.voxelHit);
			hitInfo.voxelHit.chunk.Changed = true;
		}
	}
}
