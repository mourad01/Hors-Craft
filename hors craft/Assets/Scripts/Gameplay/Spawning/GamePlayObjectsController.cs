// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.Spawning.GamePlayObjectsController
using com.ootii.Cameras;
using Common.Managers;
using States;
using System;
using Uniblocks;
using UnityEngine;

namespace Gameplay.Spawning
{
	public class GamePlayObjectsController : MonoBehaviourSingleton<GamePlayObjectsController>
	{
		public WorldObjectsData objData = new WorldObjectsData();

		public GameObject[] npcPrefabGroups = new GameObject[0];

		private int currentWorldIndex;

		private Transform cameraTransform;

		public void Start()
		{
			currentWorldIndex = Manager.Get<SavedWorldManager>().currentWorldDataIndex;
			GameplayState gameplayState = Manager.Get<StateMachineManager>().GetStateInstance(typeof(GameplayState)) as GameplayState;
			cameraTransform = CameraController.instance.MainCamera.transform;
			if (gameplayState != null)
			{
				GameplayState gameplayState2 = gameplayState;
				gameplayState2.onResourceGathered = (Action<int, ResourceSprite>)Delegate.Remove(gameplayState2.onResourceGathered, new Action<int, ResourceSprite>(RegisterGatheredResource));
				GameplayState gameplayState3 = gameplayState;
				gameplayState3.onResourceGathered = (Action<int, ResourceSprite>)Delegate.Combine(gameplayState3.onResourceGathered, new Action<int, ResourceSprite>(RegisterGatheredResource));
			}
			DoorsHoverAction.onDoorsInteract = (Action<Vector3, Action<bool>>)Delegate.Remove(DoorsHoverAction.onDoorsInteract, new Action<Vector3, Action<bool>>(OnDoorOpened));
			DoorsHoverAction.onDoorsInteract = (Action<Vector3, Action<bool>>)Delegate.Combine(DoorsHoverAction.onDoorsInteract, new Action<Vector3, Action<bool>>(OnDoorOpened));
			LoadWorldObjects();
			SpawnNpcs();
		}

		public void OnDoorOpened(Vector3 doorPosition, Action<bool> canOpen)
		{
			if (objData != null)
			{
				GamePlayDoorObject doorData = objData.GetDoorData(doorPosition);
				if (doorData == null && canOpen != null)
				{
					canOpen(obj: true);
					return;
				}
				bool obj = doorData.Interact();
				canOpen(obj);
			}
		}

		public void TryToAddDoor()
		{
			Vector3 position = Vector3.zero;
			if (CheckForDoor(out position))
			{
				AddDoor(position);
			}
		}

		public void TryToRemoveDoor()
		{
			Vector3 position = Vector3.zero;
			if (CheckForDoor(out position))
			{
				GamePlayDoorObject doorData = objData.GetDoorData(position);
				if (doorData != null)
				{
					objData.RemoveDoorObject(doorData);
				}
			}
		}

		private bool CheckForDoor(out Vector3 position)
		{
			position = Vector3.zero;
			UnityEngine.Debug.DrawRay(cameraTransform.position, cameraTransform.forward * 4f, Color.magenta, 3f);
			if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hitInfo, 4f))
			{
				DoorTrigger component = hitInfo.collider.gameObject.GetComponent<DoorTrigger>();
				if (component == null)
				{
					return false;
				}
				position = component.transform.position;
				return true;
			}
			return false;
		}

		private void SpawnNpcs()
		{
			if (npcPrefabGroups != null && npcPrefabGroups.Length >= 0 && currentWorldIndex < npcPrefabGroups.Length)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(npcPrefabGroups[currentWorldIndex]);
				gameObject.transform.SetParent(base.transform);
			}
		}

		public void OnDisable()
		{
			if (!(Manager.Get<StateMachineManager>() == null))
			{
				GameplayState gameplayState = Manager.Get<StateMachineManager>().GetStateInstance(typeof(GameplayState)) as GameplayState;
				if (!(gameplayState == null))
				{
					GameplayState gameplayState2 = gameplayState;
					gameplayState2.onResourceGathered = (Action<int, ResourceSprite>)Delegate.Remove(gameplayState2.onResourceGathered, new Action<int, ResourceSprite>(RegisterGatheredResource));
					DoorsHoverAction.onDoorsInteract = (Action<Vector3, Action<bool>>)Delegate.Remove(DoorsHoverAction.onDoorsInteract, new Action<Vector3, Action<bool>>(OnDoorOpened));
				}
			}
		}

		public void RegisterSceneResource(GamePlayObjectBase newResource)
		{
			if (objData != null)
			{
				objData.AddResource(newResource);
			}
		}

		public void RegisterGatheredResource(int id, ResourceSprite resourceInstance)
		{
			if (objData != null)
			{
				GamePlayObjectBase resourceData = objData.GetResourceData(resourceInstance.gameObject);
				if (resourceData != null)
				{
					resourceData.isGathered = true;
				}
				SaveWorldObjects();
			}
		}

		public void LoadWorldObjects(bool forceFromFile = false)
		{
			GamePlayObjectsLoader gamePlayObjectsLoader = new GamePlayObjectsLoader();
			objData = gamePlayObjectsLoader.LoadSceneObjects(currentWorldIndex, forceFromFile);
			if (objData != null)
			{
				objData.SpawnAllResources(base.transform);
			}
		}

		public void SaveWorldObjects(bool forceToFile = false)
		{
			GamePlayObjectsLoader gamePlayObjectsLoader = new GamePlayObjectsLoader();
			objData.worldId = currentWorldIndex;
			if (forceToFile)
			{
				foreach (GamePlayObjectBase resource in objData.resources)
				{
					if (resource.objectInstance != null)
					{
						resource.position = resource.objectInstance.transform.position;
					}
				}
			}
			gamePlayObjectsLoader.SaveSceneObjects(objData, currentWorldIndex, forceToFile);
		}

		public void CreateResource(int id, Vector3 position)
		{
			GamePlayObjectBase gamePlayObjectBase = new GamePlayObjectBase(id, position, null);
			gamePlayObjectBase.Spawn(base.transform);
			RegisterSceneResource(gamePlayObjectBase);
		}

		public void AddDoor(Vector3 position)
		{
			GamePlayDoorObject objectData = new GamePlayDoorObject(0, position, null);
			AddDoorObject(objectData);
		}

		public void AddDoorObject(GamePlayDoorObject objectData)
		{
			if (objData != null)
			{
				objData.AddDoorObject(objectData);
			}
		}
	}
}
