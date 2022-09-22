// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.InteractivePrefabHoverAction
namespace Uniblocks
{
	public class InteractivePrefabHoverAction : HoverAction
	{
		protected InteractiveObject interactiveObject;

		protected InteractiveObjectContext interactiveObjectContext;

		protected RotateVoxelContext rotateContext;

		public InteractivePrefabHoverAction(HoverActionsConnector connector)
			: base(connector)
		{
		}

		public override void Update(RaycastHitInfo hitInfo)
		{
			base.Update(hitInfo);
			if (hitInfo.hit.collider != null)
			{
				interactiveObject = hitInfo.hit.collider.gameObject.GetComponent<InteractiveObject>();
			}
			else
			{
				interactiveObject = null;
			}
			RevalidateFactsFor(interactiveObject);
		}

		protected void RevalidateFactsFor(InteractiveObject interactiveObject)
		{
			if (interactiveObject != null && !InteractiveObject.isLocked && interactiveObject.isUsable)
			{
				if (interactiveObjectContext == null || interactiveObjectContext.obj != interactiveObject.gameObject)
				{
					CreateContext(interactiveObject);
					AddInteractionFact();
				}
			}
			else
			{
				RemoveInteractionFact();
			}
			if (interactiveObject != null && !InteractiveObject.isLocked && interactiveObject.isRotatable)
			{
				AddRotationFact();
			}
			else
			{
				RemoveRotationFact();
			}
		}

		protected virtual void CreateContext(InteractiveObject interactive)
		{
			RemoveInteractionFact();
			interactiveObjectContext = new InteractiveObjectContext
			{
				obj = interactive.gameObject,
				useAction = interactive.OnUse
			};
		}

		protected virtual void RemoveInteractionFact()
		{
			if (interactiveObjectContext != null)
			{
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.IN_FRONT_OF_INTERACTIVE_OBJECT, interactiveObjectContext);
				interactiveObjectContext = null;
			}
		}

		protected virtual void AddInteractionFact()
		{
			MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.IN_FRONT_OF_INTERACTIVE_OBJECT, interactiveObjectContext);
		}

		protected void AddRotationFact()
		{
			if (rotateContext == null)
			{
				rotateContext = new RotateVoxelContext
				{
					onRotate = OnRotate
				};
				MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.IN_FRONT_OF_ROTATABLE, rotateContext);
			}
		}

		protected void RemoveRotationFact()
		{
			if (rotateContext != null)
			{
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.IN_FRONT_OF_ROTATABLE, rotateContext);
				rotateContext = null;
			}
		}

		protected void OnRotate()
		{
			InteractiveObject component = hitInfo.hit.collider.gameObject.GetComponent<InteractiveObject>();
			component.Rotate();
		}
	}
}
