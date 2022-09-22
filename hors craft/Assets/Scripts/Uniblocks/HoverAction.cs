// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.HoverAction
namespace Uniblocks
{
	public abstract class HoverAction
	{
		protected HoverActionsConnector hoverContext;

		protected RaycastHitInfo hitInfo;

		public HoverAction(HoverActionsConnector connector)
		{
			hoverContext = connector;
		}

		public virtual void Update(RaycastHitInfo hitInfo)
		{
			this.hitInfo = hitInfo;
		}
	}
}
