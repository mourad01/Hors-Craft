// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.UsablePrefabHoverAction
using System;

namespace Uniblocks
{
	public class UsablePrefabHoverAction : HoverAction
	{
		private InteractiveObject interactive;

		private Action usableAction;

		private InteractiveObjectContext context;

		public UsablePrefabHoverAction(HoverActionsConnector connector)
			: base(connector)
		{
		}

		public override void Update(RaycastHitInfo hitInfo)
		{
			base.Update(hitInfo);
			if (hitInfo.hit.collider != null)
			{
				interactive = hitInfo.hit.collider.gameObject.GetComponent<InteractiveObject>();
			}
			else
			{
				interactive = null;
			}
			if (interactive != null && !InteractiveObject.isLocked)
			{
				InteractiveObject interactiveObject = interactive;
				if (new Action(interactiveObject.OnUse) != usableAction)
				{
					InteractiveObject interactiveObject2 = interactive;
					usableAction = interactiveObject2.OnUse;
					DisableInteractiveObject();
					EnableInteractiveObject();
				}
			}
			else if (usableAction != null || context != null)
			{
				DisableInteractiveObject();
				usableAction = null;
			}
		}

		private void EnableInteractiveObject()
		{
			InteractiveObjectContext interactiveObjectContext = new InteractiveObjectContext
			{
				obj = interactive.gameObject
			};
			InteractiveObjectContext interactiveObjectContext2 = interactiveObjectContext;
			InteractiveObject interactiveObject = interactive;
			interactiveObjectContext2.useAction = interactiveObject.OnUse;
			context = interactiveObjectContext;
			MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.IN_FRONT_OF_INTERACTIVE_OBJECT, context);
		}

		private void DisableInteractiveObject()
		{
			if (context != null)
			{
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.IN_FRONT_OF_INTERACTIVE_OBJECT, context);
				context = null;
			}
		}
	}
}
