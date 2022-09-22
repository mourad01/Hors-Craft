// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.UsableInsideHoverAction
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Uniblocks
{
	public class UsableInsideHoverAction : InteractivePrefabHoverAction
	{
		private static List<InteractiveObject> interactiveObjects = new List<InteractiveObject>();

		private Transform _player;

		private Transform player
		{
			get
			{
				if (_player == null)
				{
					_player = PlayerGraphic.GetControlledPlayerInstance().transform;
				}
				return _player;
			}
		}

		public UsableInsideHoverAction(HoverActionsConnector connector)
			: base(connector)
		{
		}

		public static void AddObject(InteractiveObject obj)
		{
			if (!interactiveObjects.Contains(obj))
			{
				interactiveObjects.Add(obj);
			}
			RevalidateList();
		}

		public override void Update(RaycastHitInfo hitInfo)
		{
			base.Update(hitInfo);
			if (interactiveObject == null || !ObjectContainsPlayer(interactiveObject))
			{
				interactiveObject = interactiveObjects.FirstOrDefault((InteractiveObject obj) => ObjectContainsPlayer(obj));
			}
			RevalidateFactsFor(interactiveObject);
		}

		protected override void RemoveInteractionFact()
		{
			if (interactiveObjectContext != null)
			{
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.INSIDE_INTERACTIVE_OBJECT, interactiveObjectContext);
				interactiveObjectContext = null;
			}
		}

		protected override void AddInteractionFact()
		{
			MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.INSIDE_INTERACTIVE_OBJECT, interactiveObjectContext);
		}

		private static void RevalidateList()
		{
			interactiveObjects = (from obj in interactiveObjects
				where obj != null && obj.gameObject != null
				select obj).ToList();
		}

		private bool ObjectContainsPlayer(InteractiveObject obj)
		{
			if (obj == null)
			{
				return false;
			}
			return GetBounds(obj).Contains(player.position);
		}

		private Bounds GetBounds(InteractiveObject obj)
		{
			Collider componentInChildren = obj.GetComponentInChildren<Collider>();
			if (componentInChildren != null)
			{
				return componentInChildren.bounds;
			}
			return default(Bounds);
		}
	}
}
