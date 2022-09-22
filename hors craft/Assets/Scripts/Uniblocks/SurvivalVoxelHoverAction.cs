// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.SurvivalVoxelHoverAction
using Common.Managers;
using Gameplay;

namespace Uniblocks
{
	public class SurvivalVoxelHoverAction : VoxelHoverAction
	{
		public SurvivalVoxelHoverAction(HoverActionsConnector connector)
			: base(connector)
		{
		}

		public override void Update(RaycastHitInfo hitInfo)
		{
			if (!Manager.Get<SurvivalManager>().IsCombatTime())
			{
				base.Update(hitInfo);
			}
			else if ((bool)SelectedBlockGraphics)
			{
				SelectedBlockGraphics.enabled = false;
			}
		}
	}
}
