// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.ChangeClothesHoverAction
using Common.Managers;
using Gameplay;
using States;
using UnityEngine;

namespace Uniblocks
{
	public class ChangeClothesHoverAction : HoverAction
	{
		private GameObject enabledHalo;

		private InteractiveObjectContext context;

		private RotateObjectContext rotateContext;

		public ChangeClothesHoverAction(HoverActionsConnector connector)
			: base(connector)
		{
			context = new InteractiveObjectContext
			{
				useAction = OnChangeClothesClick
			};
		}

		public override void Update(RaycastHitInfo hitInfo)
		{
			Collider collider = hitInfo.hit.collider;
			if (collider != null && collider.gameObject.layer == 22)
			{
				if (Manager.Get<ModelManager>().clothesSetting.GetClothesEnabled())
				{
					GameObject gameObject = collider.gameObject.transform.GetChild(0).gameObject;
					if (enabledHalo != null && !enabledHalo.Equals(gameObject))
					{
						deactivateCurrentHalo();
					}
					enabledHalo = gameObject;
					enabledHalo.SetActive(value: true);
					MonoBehaviourSingleton<GameplayFacts>.get.AddFactIfNotExisting(Fact.CHANGING_CLOTHES, context);
					CheckRotateContext(hitInfo);
				}
			}
			else
			{
				deactivateCurrentHalo();
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFact(Fact.CHANGING_CLOTHES);
				if (rotateContext != null)
				{
					MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.IN_FRONT_OF_ROTATABLE, rotateContext);
					rotateContext = null;
				}
			}
		}

		private void deactivateCurrentHalo()
		{
			if (enabledHalo != null)
			{
				enabledHalo.SetActive(value: false);
				enabledHalo = null;
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

		public void OnChangeClothesClick()
		{
			if (Manager.Get<StateMachineManager>().ContainsState(typeof(DressupState)))
			{
				Manager.Get<StateMachineManager>().PushState<DressupState>();
			}
			else
			{
				Manager.Get<StateMachineManager>().PushState<ChangeClothesState>();
			}
		}

		private void OnRotate()
		{
			DoorTrigger componentInChildren = rotateContext.obj.GetComponentInChildren<DoorTrigger>();
			VoxelInfo voxelInfo = componentInChildren.GetVoxelInfo();
			hoverContext.OnVoxelRotate(voxelInfo);
		}
	}
}
