// DecompilerFi decompiler from Assembly-CSharp.dll class: PlayerFishingController
using com.ootii.Cameras;
using Common.Managers;
using Gameplay;
using GameUI;
using System;
using Uniblocks;
using UnityEngine;

public class PlayerFishingController : MonoBehaviour
{
	[Serializable]
	public struct FishingRodRenderers
	{
		public GameObject basePart;

		public GameObject endPart;

		public GameObject spinningPart;

		public GameObject rotatingPart;
	}

	public FishingRodRenderers[] rodsRenderes;

	public RuntimeAnimatorController humanFishingAnimator;

	public RuntimeAnimatorController humanNormalAnimator;

	private Animator animator;

	public GameObject rodObject;

	public bool isFishing;

	public bool isInBoat;

	public bool isOnBoat;

	public Boat currentBoat;

	public GameObject spawnedPlayerRepresentation;

	public Transform fishHookPivot;

	public Transform rodLinePivot;

	public Transform fishCatchedPivot;

	public LineRenderer fishingLineRenderer;

	public bool alwaysDefaultFishingRange = true;

	public int defaultFishingRange = 2;

	public int defaultFishingDeep = 3;

	public int increasedFishingRange = 3;

	public int increasedFishingDeep = 2;

	private HumanRepresentation playerRepresentation;

	public PlayerGraphic playerGraphic;

	private Vector3 playerOriginalRotation;

	private Vector3[] boatDirections = new Vector3[2]
	{
		new Vector3(0f, -90f, 0f),
		new Vector3(0f, 90f, 0f)
	};

	private Vector3 pos;

	public bool HaveRotate
	{
		get
		{
			if (currentBoat == null)
			{
				return false;
			}
			return isInBoat && currentBoat.boatData.rotateWhenFishing;
		}
	}

	public bool BoatFishing
	{
		get
		{
			if (currentBoat == null)
			{
				return false;
			}
			return isInBoat || isOnBoat;
		}
	}

	private void Awake()
	{
		animator = GetComponentInChildren<Animator>();
		fishingLineRenderer = animator.GetComponent<LineRenderer>();
		if (!Manager.Get<ModelManager>().fishingSettings.IsFishingEnabled())
		{
			SetRodRenderer(9999);
			animator.runtimeAnimatorController = humanNormalAnimator;
		}
		DisableFishing();
	}

	public void SetRodRenderer(int rodId)
	{
		for (int i = 0; i < rodsRenderes.Length; i++)
		{
			if (rodId == i)
			{
				rodsRenderes[i].basePart.SetActive(value: true);
				rodsRenderes[i].endPart.SetActive(value: true);
				rodsRenderes[i].spinningPart.SetActive(value: true);
				rodsRenderes[i].rotatingPart.SetActive(value: true);
			}
			else
			{
				rodsRenderes[i].basePart.SetActive(value: false);
				rodsRenderes[i].endPart.SetActive(value: false);
				rodsRenderes[i].spinningPart.SetActive(value: false);
				rodsRenderes[i].rotatingPart.SetActive(value: false);
			}
		}
	}

	public void TurnToWater(Vector3 direction)
	{
		playerGraphic = CameraController.instance.Anchor.GetComponent<PlayerGraphic>();
		if (!isInBoat)
		{
			playerGraphic.transform.rotation = Quaternion.LookRotation(direction);
			return;
		}
		UnityEngine.Debug.Log("Bef Boat fishing rotation " + playerGraphic.transform.rotation);
		Vector3 vector = boatDirections[UnityEngine.Random.Range(0, 2)];
		playerGraphic.transform.rotation = Quaternion.Euler(playerGraphic.transform.rotation.eulerAngles + vector);
		UnityEngine.Debug.Log("Aft Boat fishing rotation " + vector);
	}

	public void TeleportToWater(Vector3 pos)
	{
		if (!isInBoat)
		{
			float x = pos.x;
			Vector3 position = base.transform.position;
			Vector3 position2 = new Vector3(x, position.y, pos.z);
			base.transform.position = position2;
		}
	}

	public void StartFishing()
	{
		playerGraphic = CameraController.instance.Anchor.GetComponent<PlayerGraphic>();
		playerRepresentation = new HumanRepresentation(playerGraphic);
		playerRepresentation.UIModeOn(SetRepresentation, setLayerToClothes: false);
		playerRepresentation.HideHead();
		rodObject.SetActive(value: true);
		fishingLineRenderer.enabled = true;
		animator.runtimeAnimatorController = humanFishingAnimator;
		animator.updateMode = AnimatorUpdateMode.UnscaledTime;
		animator.SetBool("fishing", value: true);
	}

	public void CleanBoatSettings()
	{
		isOnBoat = false;
		isInBoat = false;
		currentBoat = null;
	}

	private void SetRepresentation(GameObject player)
	{
		spawnedPlayerRepresentation = player;
		spawnedPlayerRepresentation.transform.position = playerGraphic.transform.position;
		spawnedPlayerRepresentation.transform.rotation = playerGraphic.transform.rotation;
	}

	public void DisableFishing()
	{
		if (playerRepresentation != null)
		{
			playerRepresentation.ShowHead();
			playerRepresentation.UIModeOff();
		}
		rodObject.SetActive(value: false);
		fishingLineRenderer.enabled = false;
		animator.runtimeAnimatorController = humanNormalAnimator;
		animator.updateMode = AnimatorUpdateMode.Normal;
		animator.SetBool("fishing", value: false);
		if (isInBoat)
		{
			playerGraphic.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		}
	}

	public void ThrowRod()
	{
		SetAnimatorTrigger("FishingIdle", "Throw");
	}

	public void FishFight()
	{
		SetAnimatorTrigger("FishingWait", "FishFight");
	}

	public void FishCatched()
	{
		SetAnimatorTrigger("Fishing", "Success");
	}

	public void FishBreak()
	{
		SetAnimatorTrigger("Fishing", "Fail");
	}

	public void EndShowing()
	{
		SetAnimatorTrigger("FishShow", "EndShow");
	}

	public int GetFishingRange()
	{
		if (alwaysDefaultFishingRange || currentBoat == null)
		{
			return defaultFishingRange;
		}
		return currentBoat.boatData.increaseFishingRangeValue + defaultFishingRange;
	}

	public int GetFishingDeep()
	{
		if (alwaysDefaultFishingRange || currentBoat == null)
		{
			return defaultFishingDeep;
		}
		return currentBoat.boatData.increaseFishingDeepValue + defaultFishingDeep;
	}

	private void SetAnimatorTrigger(string currentState, string triggerName)
	{
		UnityEngine.Debug.Log("Calling: " + triggerName + " current state: " + currentState);
		if (animator.GetCurrentAnimatorStateInfo(0).IsName(currentState))
		{
			animator.SetTrigger(triggerName);
		}
	}

	private void OnDrawGizmos()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		pos = base.transform.position + base.transform.forward * 3f + Vector3.down;
		VoxelInfo voxelInfo = Engine.PositionToVoxelInfo(base.transform.position);
		if (voxelInfo != null)
		{
			ChunkData chunk = voxelInfo.chunk;
			if (chunk != null && chunk.NeighborChunks != null)
			{
				DrawCilinder(GetFishingRange(), GetFishingDeep());
			}
		}
	}

	private bool IsWaterVoxel(ushort id)
	{
		return id == Engine.usefulIDs.waterBlockID;
	}

	private void DrawCilinder(int radius, int height)
	{
		for (int i = 0; i < height; i++)
		{
			DrawCircle(radius, -i);
		}
	}

	private void DrawCircle(int radius, int yOffset = 0)
	{
		Vector3 vector = pos;
		vector.x = Mathf.FloorToInt(Mathf.RoundToInt(vector.x / Engine.ChunkScale.x));
		vector.y = Mathf.FloorToInt(Mathf.RoundToInt(vector.y / Engine.ChunkScale.y));
		vector.z = Mathf.FloorToInt(Mathf.RoundToInt(vector.z / Engine.ChunkScale.z));
		for (int i = -radius; i <= radius; i++)
		{
			for (int j = -radius; j <= radius; j++)
			{
				if (j * j + i * i <= radius * radius)
				{
					DrawCube(vector.x + (float)j, vector.y, vector.z + (float)i);
				}
			}
		}
	}

	private void DrawCube(float x, float y, float z)
	{
		Gizmos.DrawCube(new Vector3(x, y, z), Vector3.one);
	}

	public void OnWorldReset()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (hit.rigidbody == null)
		{
			CleanBoatSettings();
			return;
		}
		currentBoat = hit.rigidbody.gameObject.GetComponentInChildren<Boat>();
		if (currentBoat == null)
		{
			CleanBoatSettings();
		}
		else
		{
			isOnBoat = true;
		}
	}
}
