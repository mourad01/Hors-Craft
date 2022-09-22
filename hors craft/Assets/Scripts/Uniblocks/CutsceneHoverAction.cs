// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.CutsceneHoverAction
using UnityEngine;

namespace Uniblocks
{
	public class CutsceneHoverAction : HoverAction
	{
		private InteractiveObjectContext cutsceneContext;

		private VoxelEvents hoveredEvents;

		public CutsceneHoverAction(HoverActionsConnector connector)
			: base(connector)
		{
			cutsceneContext = new InteractiveObjectContext
			{
				useAction = OnCutsceneUse
			};
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
			if (hoveredEvents != null && hoveredEvents is CutsceneVoxelEvents)
			{
				MonoBehaviourSingleton<GameplayFacts>.get.AddFactIfNotExisting(Fact.IN_FRONT_OF_USABLE_CUTSCENE, cutsceneContext);
			}
			else
			{
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.IN_FRONT_OF_USABLE_CUTSCENE, cutsceneContext);
			}
		}

		public void OnCutsceneUse()
		{
			CutsceneVoxelEvents cutsceneVoxelEvents = hoveredEvents as CutsceneVoxelEvents;
			if (cutsceneVoxelEvents == null || cutsceneVoxelEvents.cutsceneAnimatorController == null)
			{
				UnityEngine.Debug.LogWarning("Tried to play Cutscene with null animator!");
				return;
			}
			if (hoverContext.movement.movementBlock)
			{
				UnityEngine.Debug.Log("Can't start cutscene during another cutscene.");
				return;
			}
			GameObject gameObject = new GameObject("CutsceneContainer");
			gameObject.transform.position = hitInfo.voxelHit.chunk.VoxelIndexToPosition(hitInfo.voxelHit.index);
			gameObject.transform.Rotate(Vector3.up * hitInfo.voxelHit.GetVoxelFinalRotationInDegrees());
			hoverContext.movement.StartCutscene(gameObject, cutsceneVoxelEvents);
		}
	}
}
