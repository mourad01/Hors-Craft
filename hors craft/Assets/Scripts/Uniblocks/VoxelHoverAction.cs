// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.VoxelHoverAction
using com.ootii.Cameras;
using Common.Managers;
using Gameplay;
using States;
using System;
using UnityEngine;

namespace Uniblocks
{
	public class VoxelHoverAction : HoverAction
	{
		protected Renderer SelectedBlockGraphics;

		private Camera _mainCam;

		private BlueprintManager _blueprintManager;

		private BlueprintController _blueprintController;

		private GameplayState _gameplayInstance;

		private SelectedVoxelContext currentInteractionContext;

		private RotateVoxelContext rotateContext;

		private RotateObjectContext rotateObjectContext;

		private VoxelInfo editModeRectStartVoxel;

		private Camera mainCamera
		{
			get
			{
				if (_mainCam == null)
				{
					_mainCam = CameraController.instance.MainCamera;
				}
				return _mainCam;
			}
		}

		private BlueprintManager blueprintManager
		{
			get
			{
				if (_blueprintManager == null)
				{
					_blueprintManager = Manager.Get<BlueprintManager>();
				}
				return _blueprintManager;
			}
		}

		private BlueprintController blueprintController
		{
			get
			{
				if (_blueprintController == null)
				{
					_blueprintController = PlayerGraphic.GetControlledPlayerInstance().GetComponent<BlueprintController>();
				}
				return _blueprintController;
			}
		}

		private GameplayState gameplayInstance
		{
			get
			{
				if (_gameplayInstance == null)
				{
					_gameplayInstance = Manager.Get<StateMachineManager>().GetStateInstance<GameplayState>();
				}
				return _gameplayInstance;
			}
		}

		public Action customAddAction
		{
			get;
			private set;
		}

		public Action customDeleteAction
		{
			get;
			private set;
		}

		public Vector3 lastHitPosition
		{
			get;
			private set;
		}

		public VoxelHoverAction(HoverActionsConnector connector)
			: base(connector)
		{
			GameObject gameObject = GameObject.Find("SelectedBlock");
			if (gameObject != null)
			{
				SelectedBlockGraphics = gameObject.GetComponent<Renderer>();
			}
			currentInteractionContext = new SelectedVoxelContext
			{
				onAdd = OnAdd,
				onDig = OnDig
			};
		}

		public void SetCustomAddAction(Action act)
		{
			customAddAction = act;
		}

		public void SetCustomDeleteAction(Action act)
		{
			customDeleteAction = act;
		}

		public override void Update(RaycastHitInfo hitInfo)
		{
			base.Update(hitInfo);
			if (!(gameplayInstance.currentSubstate != null) || gameplayInstance.currentSubstate.substate == GameplayState.Substates.WALKING)
			{
				if (hitInfo.hit.collider != null || hitInfo.voxelHit == null)
				{
					UpdateNoVoxelHit();
				}
				else
				{
					UpdateYesVoxelHit();
				}
				if (Engine.EditMode)
				{
					DeveloperModeUpdateCursorEvents();
				}
			}
		}

		private void UpdateYesVoxelHit()
		{
			if (MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.MCPE_STEERING))
			{
				UpdateVoxelHitMcpeSteering();
			}
			else
			{
				UpdateVoxelHit();
			}
		}

		private void UpdateNoVoxelHit()
		{
			bool flag = false;
			SelectedBlockGraphics.enabled = false;
			if (IsMobHit())
			{
				Mountable componentInParent = hitInfo.hit.collider.GetComponentInParent<Mountable>();
				if (componentInParent != null && componentInParent.isVehicle)
				{
					EnableObjectRotation(componentInParent);
					flag = true;
				}
			}
			if (!flag)
			{
				DisableObjectRotation();
			}
			DisableVoxelRotation();
			if (IsInteractiveObjectHit())
			{
				MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.VOXEL_SELECTED, currentInteractionContext);
			}
			else if (IsCarHit())
			{
				MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.VOXEL_SELECTED, currentInteractionContext);
			}
			else if (IsUndefinedObjectHit() && hitInfo.gridRaycastHitNoWater != null)
			{
				hitInfo.voxelHit = hitInfo.gridRaycastHitNoWater;
				UpdateYesVoxelHit();
			}
			else
			{
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFact(Fact.VOXEL_SELECTED);
			}
		}

		private void UpdateVoxelHit()
		{
			ushort voxel = hitInfo.voxelHit.GetVoxel();
			currentInteractionContext.voxel = hitInfo.voxelHit;
			MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.VOXEL_SELECTED, currentInteractionContext);
			VoxelEvents instanceForVoxelId = Singleton<VoxelEventsInstancesCache>.get.GetInstanceForVoxelId(voxel);
			if (instanceForVoxelId != null)
			{
				instanceForVoxelId.OnLook(hitInfo.voxelHit);
			}
			if (Engine.GetVoxelType(voxel).canRotate)
			{
				EnableRotation(hitInfo.voxelHit);
			}
			else
			{
				DisableVoxelRotation();
			}
			UpdateSelectedBlockGraphic(hitInfo);
			DisableObjectRotation();
		}

		private void UpdateVoxelHitMcpeSteering()
		{
			ushort voxel = hitInfo.voxelHit.GetVoxel();
			currentInteractionContext.voxel = hitInfo.voxelHit;
			MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.VOXEL_SELECTED, currentInteractionContext);
			VoxelEvents instanceForVoxelId = Singleton<VoxelEventsInstancesCache>.get.GetInstanceForVoxelId(voxel);
			if (instanceForVoxelId != null)
			{
				instanceForVoxelId.OnLook(hitInfo.voxelHit);
			}
			if (Engine.GetVoxelType(voxel).canRotate)
			{
				EnableRotation(hitInfo.voxelHit);
				UpdateSelectedBlockGraphic(hitInfo);
			}
			else
			{
				DisableVoxelRotation();
				if (!MonoBehaviourSingleton<McpeSteering>.get.isMoving && MonoBehaviourSingleton<McpeSteering>.get.isPointerDown)
				{
					UpdateSelectedBlockGraphic(hitInfo);
				}
				else if ((bool)SelectedBlockGraphics)
				{
					SelectedBlockGraphics.enabled = false;
				}
			}
			DisableObjectRotation();
		}

		private void EnableRotation(VoxelInfo info)
		{
			if (rotateContext == null || rotateContext.info.rawIndex != info.rawIndex)
			{
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.IN_FRONT_OF_ROTATABLE, rotateContext);
				rotateContext = new RotateVoxelContext
				{
					info = info,
					onRotate = OnRotateVoxel
				};
				MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.IN_FRONT_OF_ROTATABLE, rotateContext);
			}
		}

		private void EnableObjectRotation(Mountable mob)
		{
			if (rotateObjectContext == null || rotateObjectContext.obj != mob.gameObject)
			{
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.IN_FRONT_OF_ROTATABLE, rotateObjectContext);
				rotateObjectContext = new RotateObjectContext
				{
					obj = mob.gameObject,
					onRotate = OnRotateMob
				};
				MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.IN_FRONT_OF_ROTATABLE, rotateObjectContext);
			}
		}

		private void DisableObjectRotation()
		{
			if (rotateObjectContext != null)
			{
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.IN_FRONT_OF_ROTATABLE, rotateObjectContext);
				rotateObjectContext = null;
			}
		}

		private void DisableVoxelRotation()
		{
			if (rotateContext != null)
			{
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.IN_FRONT_OF_ROTATABLE, rotateContext);
				rotateContext = null;
			}
		}

		private void UpdateSelectedBlockGraphic(RaycastHitInfo hitInfo)
		{
			if (customAddAction != null || customDeleteAction != null)
			{
				return;
			}
			ushort voxel = hitInfo.voxelHit.GetVoxel();
			if (SelectedBlockGraphics != null)
			{
				SelectedBlockGraphics.enabled = true;
				SelectedBlockGraphics.transform.position = hitInfo.voxelHit.chunk.VoxelIndexToPosition(hitInfo.voxelHit.index);
				SelectedBlockGraphics.transform.rotation = Quaternion.identity;
			}
			Voxel voxelType = Engine.GetVoxelType(voxel);
			if (voxelType.VColliderMesh != null)
			{
				Mesh vColliderMesh = voxelType.VColliderMesh;
				if (SelectedBlockGraphics != null)
				{
					SelectedBlockGraphics.transform.localScale = vColliderMesh.bounds.size + new Vector3(0.02f, 0.02f, 0.02f);
				}
				MeshRotation rotation = RotateBasicRotation(hitInfo.voxelHit.GetVoxelRotation(), voxelType.VRotation);
				Vector3 eulerRotation = GetEulerRotation(rotation);
				if (SelectedBlockGraphics != null)
				{
					SelectedBlockGraphics.transform.Rotate(eulerRotation);
					SelectedBlockGraphics.transform.position += Quaternion.Euler(eulerRotation) * vColliderMesh.bounds.center;
				}
			}
			else if (SelectedBlockGraphics != null)
			{
				SelectedBlockGraphics.transform.localScale = Vector3.one + new Vector3(0.02f, 0.02f, 0.02f);
			}
		}

		public void OnAdd()
		{
			if (customAddAction != null)
			{
				customAddAction();
			}
			else if (!IsInteractiveObjectHit() && !IsMobHit())
			{
				if (!CheckCraftable())
				{
					Manager.Get<StateMachineManager>().PushState<PauseState>(new PauseStateStartParameter
					{
						allowTimeChange = true,
						canSave = true,
						categoryToOpen = "Crafting",
						blockCaused = ExampleInventory.HeldBlock
					});
				}
				else if (hitInfo.voxelHit != null && CanAddBlock(hitInfo.voxelHit) && hoverContext.OnVoxelPlace(hitInfo.voxelHit))
				{
					lastHitPosition = Engine.VoxelInfoToPosition(hitInfo.voxelHit);
					DecreaseCraftable();
					Manager.Get<QuestManager>().OnBlockPut(ExampleInventory.HeldBlock);
				}
			}
		}

		private bool CheckCraftable()
		{
			if (!Manager.Get<CraftingManager>().IsBlockCraftable(ExampleInventory.HeldBlock))
			{
				return true;
			}
			return Singleton<PlayerData>.get.playerItems.GetCraftableCountByBlock(ExampleInventory.HeldBlock) > 0;
		}

		private void DecreaseCraftable()
		{
			if (Manager.Get<CraftingManager>().IsBlockCraftable(ExampleInventory.HeldBlock))
			{
				Singleton<PlayerData>.get.playerItems.UseCraftableBlock(ExampleInventory.HeldBlock, 1);
				if (Manager.Get<ModelManager>().craftingSettings.AreCraftingFree())
				{
					Singleton<PlayerData>.get.playerItems.AddCraftable(Manager.Get<CraftingManager>().GetCraftableIdFromBlock(ExampleInventory.HeldBlock), 1);
				}
			}
		}

		public void OnDig()
		{
			if (customDeleteAction != null)
			{
				customDeleteAction();
			}
			else if (IsBlueprintHit())
			{
				OnDestroyBlueprint();
			}
			else if (IsMobHit())
			{
				OnDigMob();
			}
			else if (IsDoorsHit() || IsChangeClothesHit())
			{
				OnDigDoors();
			}
			else if (IsInteractiveObjectHit())
			{
				OnDigInteractivePrefab();
			}
			else if (IsCarHit())
			{
				TryToDestroyCar(hitInfo.hit.collider.gameObject);
			}
			else if (hitInfo.voxelHit != null)
			{
				OnDigVoxel();
			}
		}

		private void OnDestroyBlueprint()
		{
			blueprintController.DeleteBlueprint(Engine.VoxelInfoToPosition(hitInfo.voxelHit));
		}

		private void OnDigMob()
		{
			Mob componentInParent = hitInfo.hit.collider.GetComponentInParent<Mob>();
			if (componentInParent != null && CanBeDespawned(componentInParent))
			{
				componentInParent.Despawn();
			}
			Mountable componentInParent2 = hitInfo.hit.collider.GetComponentInParent<Mountable>();
			if (componentInParent2 != null && componentInParent2.isVehicle && !componentInParent2.GetComponentInParent<PlayerMovement>())
			{
				if ((bool)componentInParent2.GetComponent<SaveTransform>())
				{
					componentInParent2.GetComponent<SaveTransform>().Despawn();
				}
				else
				{
					componentInParent2.gameObject.Despawn();
				}
			}
		}

		private void OnDigDoors()
		{
			DoorTrigger componentInChildren = hitInfo.hit.collider.GetComponentInChildren<DoorTrigger>();
			VoxelInfo voxelInfo = componentInChildren.GetVoxelInfo();
			hoverContext.OnVoxelDestroyed(voxelInfo);
		}

		private void OnDigVoxel()
		{
			if (Physics.Raycast(Engine.VoxelInfoToPosition(base.hitInfo.voxelHit) + Vector3.up, -Vector3.up, out RaycastHit hitInfo, 1f))
			{
				if (!hitInfo.transform.GetComponent<TrainMountable>())
				{
					hoverContext.OnVoxelDestroyed(base.hitInfo.voxelHit);
				}
			}
			else
			{
				hoverContext.OnVoxelDestroyed(base.hitInfo.voxelHit);
			}
		}

		private void OnDigInteractivePrefab()
		{
			InteractiveObject component = hitInfo.hit.collider.gameObject.GetComponent<InteractiveObject>();
			if (component.isDestroyable)
			{
				component.Destroy();
			}
		}

		private void TryToDestroyCar(GameObject collider)
		{
			SaveTransform saveTransform = collider.GetComponentInParent<SaveTransform>();
			if (saveTransform == null)
			{
				saveTransform = collider.GetComponentInChildren<SaveTransform>();
			}
			VehicleController vehicleController = collider.GetComponentInParent<VehicleController>();
			if (vehicleController == null)
			{
				vehicleController = collider.GetComponentInChildren<VehicleController>();
			}
			vehicleController.OnVehicleDestroy();
			try
			{
				UnityEngine.Object.Destroy(saveTransform.gameObject);
			}
			catch
			{
			}
		}

		private void OnRotateVoxel()
		{
			if (Physics.Raycast(Engine.VoxelInfoToPosition(base.hitInfo.voxelHit) + Vector3.up, -Vector3.up, out RaycastHit hitInfo, 1f))
			{
				if (!hitInfo.transform.GetComponent<TrainMountable>())
				{
					hoverContext.OnVoxelRotate(base.hitInfo.voxelHit);
				}
			}
			else
			{
				hoverContext.OnVoxelRotate(base.hitInfo.voxelHit);
			}
		}

		private void OnRotateMob()
		{
			TrainMountable componentInParent = hitInfo.hit.collider.GetComponentInParent<TrainMountable>();
			if (componentInParent != null && componentInParent.isVehicle)
			{
				componentInParent.RotateSelf();
			}
		}

		private bool CanAddBlock(VoxelInfo raycast)
		{
			Voxel adjacentVoxelType = raycast.GetAdjacentVoxelType();
			if (adjacentVoxelType.GetUniqueID() != 0 && adjacentVoxelType.GetUniqueID() != 12 && !adjacentVoxelType.isThatFlower)
			{
				return false;
			}
			int result;
			if (hoverContext.movement.IsGrounded())
			{
				if (raycast.chunk.VoxelIndexToPosition(raycast.index).DistanceTo(mainCamera.transform.position) >= 1.3f)
				{
					Vector3 forward = mainCamera.transform.forward;
					if (forward.y > -0.7f)
					{
						result = 1;
						goto IL_00c3;
					}
				}
				result = ((raycast.chunk.VoxelIndexToPosition(raycast.index).DistanceTo(mainCamera.transform.position) > 2.4f) ? 1 : 0);
				goto IL_00c3;
			}
			return raycast.chunk.VoxelIndexToPosition(raycast.index).DistanceTo(mainCamera.transform.position) > 2.7f;
			IL_00c3:
			return (byte)result != 0;
		}

		protected virtual bool CanBeDespawned(Mob mob)
		{
			if (mob is Soldier)
			{
				return false;
			}
			return mob.GetComponent<Loot>() != null || mob.GetComponent<Pettable>() == null;
		}

		private bool IsBlueprintHit()
		{
			return hitInfo.voxelHit != null && blueprintManager.IsBlueprint(Engine.VoxelInfoToPosition(hitInfo.voxelHit));
		}

		private bool IsMobHit()
		{
			return hitInfo.hit.collider != null && hitInfo.hit.collider.gameObject.layer == 16;
		}

		private bool IsDoorsHit()
		{
			return hitInfo.hit.collider != null && hitInfo.hit.collider.gameObject.layer == 18;
		}

		private bool IsChangeClothesHit()
		{
			return hitInfo.hit.collider != null && hitInfo.hit.collider.gameObject.layer == 22;
		}

		private bool IsCarHit()
		{
			return hitInfo.hit.collider != null && hitInfo.hit.collider.gameObject.tag.ToUpper().Equals("VEHICLE");
		}

		private bool IsInteractiveObjectHit()
		{
			return hitInfo.hit.collider != null && hitInfo.hit.collider.gameObject.GetComponent<InteractiveObject>() != null;
		}

		private bool IsUndefinedObjectHit()
		{
			return hitInfo.hit.collider != null && hitInfo.hit.collider.gameObject.layer == LayerMask.NameToLayer("Default");
		}

		private void DeveloperModeUpdateCursorEvents()
		{
			if (!Manager.Get<StateMachineManager>().IsCurrentStateA<GameplayState>())
			{
				return;
			}
			VoxelInfo voxelInfo = Engine.VoxelRaycast(CameraController.instance.MainCamera.ScreenPointToRay(UnityEngine.Input.mousePosition), 9999.9f, ignoreTransparent: false);
			if (voxelInfo != null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(Engine.GetVoxelGameObject(voxelInfo.GetVoxel()));
				VoxelEvents component = gameObject.GetComponent<VoxelEvents>();
				if (component != null)
				{
					component.OnLook(voxelInfo);
				}
				UnityEngine.Object.Destroy(gameObject);
			}
			else if (SelectedBlockGraphics != null)
			{
				SelectedBlockGraphics.enabled = false;
			}
		}

		private void DeveloperModeFillVoxelsFromTo(VoxelInfo fr, VoxelInfo to, bool place)
		{
			if (fr.chunk == to.chunk)
			{
				Fill(fr.chunk, fr.adjacentIndex.x, fr.adjacentIndex.y, fr.adjacentIndex.z, to.adjacentIndex.x, to.adjacentIndex.y, to.adjacentIndex.z, place);
				return;
			}
			Index index = new Index(fr.chunk.ChunkIndex.x * ChunkData.SideLength + fr.adjacentIndex.x, fr.chunk.ChunkIndex.y * ChunkData.SideLength + fr.adjacentIndex.y, fr.chunk.ChunkIndex.z * ChunkData.SideLength + fr.adjacentIndex.z);
			Index index2 = new Index(to.chunk.ChunkIndex.x * ChunkData.SideLength + to.adjacentIndex.x, to.chunk.ChunkIndex.y * ChunkData.SideLength + to.adjacentIndex.y, to.chunk.ChunkIndex.z * ChunkData.SideLength + to.adjacentIndex.z);
			Index index3 = new Index(fr.index.x + (index2.x - index.x), fr.index.y + (index2.y - index.y), fr.index.z + (index2.z - index.z));
			Fill(fr.chunk, fr.adjacentIndex.x, fr.adjacentIndex.y, fr.adjacentIndex.z, index3.x, index3.y, index3.z, place);
		}

		private void Fill(ChunkData chunk, int fromX, int fromY, int fromZ, int toX, int toY, int toZ, bool place)
		{
			int num = (fromX < toX) ? 1 : (-1);
			int num2 = (fromY < toY) ? 1 : (-1);
			int num3 = (fromZ < toZ) ? 1 : (-1);
			for (int i = fromX; (num <= 0) ? (i >= toX) : (i <= toX); i += num)
			{
				for (int j = fromY; (num2 <= 0) ? (j >= toY) : (j <= toY); j += num2)
				{
					for (int k = fromZ; (num3 <= 0) ? (k >= toZ) : (k <= toZ); k += num3)
					{
						VoxelInfo voxelInfo = chunk.GetVoxelInfo(i, j, k);
						voxelInfo.SetVoxel(ExampleInventory.HeldBlock, updateMesh: true, 0);
					}
				}
			}
		}

		private Vector3 GetEulerRotation(MeshRotation rotation)
		{
			switch (rotation)
			{
			default:
				return Vector3.zero;
			case MeshRotation.back:
				return new Vector3(0f, 180f, 0f);
			case MeshRotation.left:
				return new Vector3(0f, -90f, 0f);
			case MeshRotation.right:
				return new Vector3(0f, 90f, 0f);
			}
		}

		private MeshRotation RotateBasicRotation(byte rotation, MeshRotation basicRotation)
		{
			for (int i = 0; i < rotation; i++)
			{
				switch (basicRotation)
				{
				default:
					basicRotation = MeshRotation.right;
					break;
				case MeshRotation.right:
					basicRotation = MeshRotation.back;
					break;
				case MeshRotation.back:
					basicRotation = MeshRotation.left;
					break;
				case MeshRotation.left:
					basicRotation = MeshRotation.none;
					break;
				}
			}
			return basicRotation;
		}
	}
}
