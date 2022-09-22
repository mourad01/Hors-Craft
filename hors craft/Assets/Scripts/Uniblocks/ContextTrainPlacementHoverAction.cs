// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.ContextTrainPlacementHoverAction
using System;
using System.Linq;
using UnityEngine;

namespace Uniblocks
{
	public class ContextTrainPlacementHoverAction : HoverAction
	{
		private InteractiveObject interactive;

		private Action usableAction;

		private VoxelHoverAction _voxelHoverAction;

		private VoxelHoverAction voxelHoverAction
		{
			get
			{
				if (_voxelHoverAction == null)
				{
					_voxelHoverAction = PlayerGraphic.GetControlledPlayerInstance().GetComponentInParent<CameraEventsSender>().GetHoverAction<VoxelHoverAction>();
				}
				return _voxelHoverAction;
			}
		}

		public ContextTrainPlacementHoverAction(HoverActionsConnector connector)
			: base(connector)
		{
		}

		public override void Update(RaycastHitInfo hitInfo)
		{
			base.Update(hitInfo);
			bool flag = false;
			if (hitInfo.voxelHit != null)
			{
				Voxel voxelType = hitInfo.voxelHit.GetVoxelType();
				if (voxelType.gameObject.GetComponent<AutoConnectionRailsVoxelEvents>() != null)
				{
					voxelHoverAction.SetCustomAddAction(OnPlaceTrain);
					flag = true;
				}
			}
			if (!flag && voxelHoverAction.customAddAction == new Action(OnPlaceTrain))
			{
				voxelHoverAction.SetCustomAddAction(null);
			}
			bool flag2 = false;
			if (!flag && hitInfo.hit.collider != null)
			{
				WagonMountable componentInParent = hitInfo.hit.collider.GetComponentInParent<WagonMountable>();
				if (componentInParent != null && componentInParent.GetComponentInParent<PlayerMovement>() == null)
				{
					voxelHoverAction.SetCustomDeleteAction(OnRemoveTrain);
					flag2 = true;
				}
			}
			if (!flag2 && voxelHoverAction.customDeleteAction == new Action(OnRemoveTrain))
			{
				voxelHoverAction.SetCustomDeleteAction(null);
			}
		}

		private void OnPlaceTrain()
		{
			Voxel voxel = Engine.Blocks.FirstOrDefault((Voxel b) => b.gameObject.GetComponent<SpawnPrefab>() != null);
			voxel.GetComponent<SpawnPrefab>().Spawn(hitInfo.voxelHit);
		}

		private void OnRemoveTrain()
		{
			GameObject gameObject = hitInfo.hit.collider.gameObject;
			gameObject.GetComponent<SaveTransform>().Despawn();
		}
	}
}
