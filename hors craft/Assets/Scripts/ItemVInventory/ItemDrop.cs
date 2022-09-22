// DecompilerFi decompiler from Assembly-CSharp.dll class: ItemVInventory.ItemDrop
using Common.Managers;
using States;
using System.Linq;
using Uniblocks;
using UnityEngine;

namespace ItemVInventory
{
	public class ItemDrop : MonoBehaviour
	{
		[HideInInspector]
		public string id;

		[HideInInspector]
		public int level;

		[HideInInspector]
		public int amount;

		[HideInInspector]
		public ItemType itemType;

		private MeshFilter meshFilter;

		private MeshRenderer meshRenderer;

		private SpriteRenderer spriteRenderer;

		private ItemDefinition itemDefinition;

		private bool isFalling;

		private float fallingStartTime;

		private void Awake()
		{
			meshFilter = GetComponentInChildren<MeshFilter>();
			meshRenderer = GetComponentInChildren<MeshRenderer>();
			spriteRenderer = GetComponentInChildren<SpriteRenderer>();
			meshRenderer.enabled = false;
			spriteRenderer.enabled = false;
		}

		public void Init(Sprite sprite, Mesh mesh, ItemType itemType, string id, int amount, int level = 0)
		{
			if (mesh != null)
			{
				meshFilter.mesh = mesh;
				meshRenderer.enabled = true;
			}
			else
			{
				if (!(sprite != null))
				{
					UnityEngine.Debug.LogError("NO MESH! NO Sprite so would I need to do???");
					UnityEngine.Object.Destroy(base.gameObject);
					return;
				}
				spriteRenderer.sprite = sprite;
				spriteRenderer.enabled = true;
			}
			this.itemType = itemType;
			this.id = id;
			this.amount = amount;
			this.level = level;
			itemDefinition = null;
		}

		public void Init(ItemDefinition itemDefinition, int amount, int level = 0)
		{
			if (itemDefinition.itemMesh != null)
			{
				meshFilter.mesh = itemDefinition.itemMesh;
				meshRenderer.enabled = true;
			}
			else
			{
				if (!(itemDefinition.itemSprite != null))
				{
					UnityEngine.Debug.LogError("NO MESH! NO Sprite so would I need to do???");
					UnityEngine.Object.Destroy(base.gameObject);
					return;
				}
				spriteRenderer.sprite = itemDefinition.itemSprite;
				spriteRenderer.enabled = true;
			}
			itemType = itemDefinition.itemType;
			id = itemDefinition.id;
			this.amount = amount;
			this.level = level;
			this.itemDefinition = itemDefinition;
		}

		private void Update()
		{
			VoxelInfo voxelInfo = Engine.PositionToVoxelInfo(base.transform.position + Vector3.down * 0.8f);
			if (voxelInfo != null && voxelInfo.GetVoxel() != 0 && voxelInfo.GetVoxel() != Engine.usefulIDs.waterBlockID)
			{
				isFalling = false;
			}
			else
			{
				UpdateFalling();
			}
		}

		private void UpdateFalling()
		{
			if (!isFalling)
			{
				fallingStartTime = Time.time;
				isFalling = true;
			}
			else
			{
				Vector3 gravity = Physics.gravity;
				float d = gravity.y * (Time.time - fallingStartTime);
				base.transform.position -= Vector3.down * d * Time.deltaTime;
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			VehicleController componentInParent = other.GetComponentInParent<VehicleController>();
			if (!other.gameObject.CompareTag("Player") && (!(componentInParent != null) || !componentInParent.enabled))
			{
				return;
			}
			if (itemDefinition != null && itemDefinition.HasCustomPickupBehaviours())
			{
				if (itemDefinition.OnPickup(amount, level))
				{
					GoToMenu();
					UnityEngine.Object.Destroy(base.gameObject);
				}
				return;
			}
			GameObject gameObject = PlayerGraphic.GetControlledPlayerInstance().gameObject;
			if (AddToBackpack(gameObject.GetComponentInChildren<Backpack>()))
			{
				GoToMenu();
				UnityEngine.Object.Destroy(base.gameObject);
			}
			else if (AddToEquipment(gameObject.GetComponentInChildren<Equipment>()))
			{
				GoToMenu();
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		private void GoToMenu()
		{
			PickupJumpingToYourPocketUI.SpawnPickup(base.transform.position, ((GameplayState)Manager.Get<StateMachineManager>().currentState).GetModules().FirstOrDefault((GameplayModule module) => module is PauseModule).gameObject, spriteRenderer.sprite);
		}

		private bool AddToBackpack(Backpack backpack)
		{
			return backpack.TryPlace(itemType, level, id);
		}

		private bool AddToEquipment(Equipment equipment)
		{
			return equipment.Equip(id, level);
		}
	}
}
