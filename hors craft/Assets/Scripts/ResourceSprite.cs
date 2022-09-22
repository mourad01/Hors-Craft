// DecompilerFi decompiler from Assembly-CSharp.dll class: ResourceSprite
using Common.Audio;
using Common.Managers;
using Common.Managers.Audio;
using Gameplay.Audio;
using States;
using System.Linq;
using Uniblocks;
using UnityEngine;

public class ResourceSprite : MonoBehaviour
{
	private int resourceId;

	private int craftableId;

	private bool pickedUp;

	private bool isResource;

	private bool isFalling;

	private float fallingStartTime;

	protected bool ignoreGravity;

	public bool IgnoreGravity
	{
		get
		{
			return ignoreGravity;
		}
		set
		{
			ignoreGravity = value;
		}
	}

	public void InitWithCraftableId(Vector3 position, ushort blockId, int craftableId)
	{
		Sprite voxelSprite = VoxelSprite.GetVoxelSprite(blockId);
		this.craftableId = craftableId;
		isResource = false;
		InitWithSprite(position, voxelSprite);
	}

	public void InitWithCustomCraftable(Vector3 position, Craftable craftable)
	{
		Sprite sprite = craftable.sprite;
		craftableId = (ushort)craftable.id;
		isResource = false;
		InitWithSprite(position, sprite);
	}

	public void InitWithResourceId(Vector3 position, int resourceId)
	{
		Sprite resourceImage = Manager.Get<CraftingManager>().GetResourceImage(resourceId);
		this.resourceId = resourceId;
		isResource = true;
		InitWithSprite(position, resourceImage);
	}

	private void InitWithSprite(Vector3 position, Sprite sprite)
	{
		base.transform.position = position;
		GetComponentInChildren<SpriteRenderer>().sprite = sprite;
		float num = 64f / sprite.rect.height;
		GetComponentInChildren<SpriteRenderer>().gameObject.transform.localScale = new Vector3(num, num, num);
	}

	private void Update()
	{
		VoxelInfo voxelInfo = Engine.PositionToVoxelInfo(base.transform.position + Vector3.down * 0.5f);
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
		if (!ignoreGravity)
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
	}

	private void OnTriggerEnter(Collider other)
	{
		if (pickedUp)
		{
			return;
		}
		VehicleController componentInParent = other.GetComponentInParent<VehicleController>();
		if (other.gameObject.CompareTag("Player") || (componentInParent != null && componentInParent.enabled))
		{
			pickedUp = true;
			Manager.Get<QuestManager>().OnResourcePickedUp(resourceId);
			UnityEngine.Object.Destroy(base.gameObject);
			PlaySound(GameSound.RESOURCE_PICKUP);
			if (isResource)
			{
				GatherResource(resourceId, this);
			}
			else
			{
				GatherCraftable();
			}
			PickupJumpingToYourPocketUI.SpawnPickup(base.transform.position, ((GameplayState)Manager.Get<StateMachineManager>().currentState).GetModules().FirstOrDefault((GameplayModule module) => module is PauseModule).gameObject, GetComponentInChildren<SpriteRenderer>().sprite);
		}
	}

	private void GatherCraftable()
	{
		Singleton<PlayerData>.get.playerItems.AddCraftable(craftableId, 1);
	}

	public static void GatherResource(int resourceId, ResourceSprite resourceInstance = null)
	{
		Singleton<PlayerData>.get.playerItems.AddToResources(resourceId, 1);
	}

	private void PlaySound(GameSound clip)
	{
		Sound sound = new Sound();
		sound.clip = Manager.Get<ClipsManager>().GetClipFor(clip);
		sound.mixerGroup = Manager.Get<Gameplay.Audio.MixersManager>().uiMixerGroup;
		Sound sound2 = sound;
		sound2.Play();
	}
}
