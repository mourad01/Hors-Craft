// DecompilerFi decompiler from Assembly-CSharp.dll class: Pettable
using com.ootii.Cameras;
using Common.Managers;
using Common.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;

public class Pettable : MonoBehaviour
{
	public string customParticlesPrefabName;

	public const float TAME_PER_CLICK = 10f;

	public const float TAME_PER_CHAT = 1f;

	public const float TAME_PER_DANCE = 10f;

	public const float TAME_PER_DANCE_FAIL = -1f;

	[HideInInspector]
	public int id;

	[HideInInspector]
	public Chunk chunk;

	[HideInInspector]
	public string prefabName;

	[HideInInspector]
	public string petName;

	[NonSerialized]
	public double lastEggTimeStamp;

	[Header("Where will tame panel be placed")]
	public Vector3 relativeTamePanelPos = Vector3.up * 2f;

	[Header("Tame amount per click = 10")]
	public float tameMaxValue = 100f;

	public float goWildAfter = 180f;

	public PettableFriend.SearchingForVoxel searchingForVoxel = PettableFriend.SearchingForVoxel.NONE;

	private AnimalMob _mob;

	private PettableFriend friend;

	public List<RememberedChatMessage> chatbotHistory = new List<RememberedChatMessage>();

	private const float SAVE_INTERVAL = 5f;

	private const float PARTICLE_SPAWNED_AT_HEIGHT = 0.8f;

	private float saveTimer;

	private AnimalMob.AnimalLogic baseLogic;

	private Vector3 spawnPosition = Vector3.zero;

	public float tameValue
	{
		get;
		protected set;
	}

	public bool tamed
	{
		get;
		protected set;
	}

	public AnimalMob mob
	{
		get
		{
			if (_mob == null)
			{
				_mob = GetComponentInParent<AnimalMob>();
			}
			return _mob;
		}
	}

	public bool following
	{
		get;
		private set;
	}

	public int chatbotSeed
	{
		get;
		private set;
	}

	public bool isSpecialPet => friend != null;

	protected virtual void Awake()
	{
		tameValue = 0f;
		following = false;
		baseLogic = mob.logic;
		chatbotSeed = GetHashCode();
		if (string.IsNullOrEmpty(customParticlesPrefabName))
		{
			customParticlesPrefabName = "hearts";
		}
	}

	public void LoadTamedPetData(int id, PetManager.PetIndividualData pet)
	{
		this.id = id;
		tameValue = pet.tame;
		tamed = (pet.tame > 0f);
		spawnPosition = pet.position;
		chunk = Engine.PositionToChunkComponent(base.transform.position);
		following = !pet.isFollowing;
		chatbotSeed = pet.chatbotSeed;
		chatbotHistory = pet.chatbotHistory;
		lastEggTimeStamp = pet.lastEggTime;
		MoveModeChange();
	}

	public void ForceTamed()
	{
		friend = base.gameObject.GetComponent<PettableFriend>();
		if (friend == null)
		{
			friend = base.gameObject.AddComponent<PettableFriend>();
		}
		Manager.Get<PetManager>().currentPet = friend;
		tamed = true;
		tameValue = tameMaxValue;
		MoveModeChange();
	}

	public void ChatTame()
	{
		tameValue = Mathf.Min(tameMaxValue, tameValue + 1f);
	}

	public virtual void Tame()
	{
		AddDefaultTameValue();
		PlayTameParticles();
		RunTameCallbacks();
	}

	public void OneCallToFullTame()
	{
		tameValue = tameMaxValue;
		PlayTameParticles();
		RunTameCallbacks();
	}

	private void AddDefaultTameValue()
	{
		tameValue = Mathf.Min(tameMaxValue, tameValue + 10f);
	}

	protected virtual void RunTameCallbacks()
	{
		if (tamed)
		{
			TryToSpawnEgg();
		}
		else
		{
			Manager.Get<QuestManager>().OnPetThePet();
		}
		if (!tamed)
		{
			if (tameValue >= tameMaxValue)
			{
				LastTame();
			}
			else
			{
				RunAway();
			}
		}
	}

	protected virtual void LastTame()
	{
		OnTamed();
		tamed = true;
	}

	protected virtual void RunAway()
	{
		Vector3 position = CameraController.instance.MainCamera.transform.position;
		Vector3 normalized = (base.transform.position - position).normalized;
		mob.MoveAway(normalized, 15f, 3f, run: true);
		mob.navigator.Jump();
	}

	public void MoveModeChange()
	{
		if (!following)
		{
			mob.logic = AnimalMob.AnimalLogic.FOLLOW_PLAYER;
			following = true;
		}
		else
		{
			if (mob.navigator is FlyingMobNavigator)
			{
				mob.logic = AnimalMob.AnimalLogic.STAY_IN_PLACE;
			}
			else
			{
				mob.logic = AnimalMob.AnimalLogic.WANDER;
			}
			following = false;
		}
		mob.ReconstructBehaviourTree();
	}

	public void FollowPlayerMode()
	{
		mob.logic = AnimalMob.AnimalLogic.FOLLOW_PLAYER;
		following = true;
		mob.ReconstructBehaviourTree();
	}

	public void FearFollowPlayerMode()
	{
		mob.SetLogicWithSavingPrevious(AnimalMob.AnimalLogic.FEAR_FOLLOW_PLAYER);
		mob.ReconstructBehaviourTree();
		TrySetVampireSkin();
	}

	public void FearRunFromPlayerMode()
	{
		if (mob.logic != AnimalMob.AnimalLogic.FEAR_FOLLOW_PLAYER && !tamed && !(mob == null) && Manager.Contains<HospitalManager>())
		{
			HospitalManager hospitalManager = Manager.Get<HospitalManager>();
			if (hospitalManager.vampireMode.scaredEmoticon != null)
			{
				MobStateIcon.SetIcon(this, hospitalManager.vampireMode.scaredEmoticon, 10f, hospitalManager.vampireMode.scaredEmoticonYOffset);
			}
			mob.SetLogicWithSavingPrevious(AnimalMob.AnimalLogic.FEAR_RUN_FROM_PLAYER);
			mob.ReconstructBehaviourTree();
		}
	}

	public bool TryToSpawnAgain()
	{
		if (chunk != null && chunk.Spawned)
		{
			base.transform.position = spawnPosition;
			return true;
		}
		return false;
	}

	protected virtual void Update()
	{
		UpdateTameProgress();
		UpdateSaving();
	}

	protected virtual void UpdateTameProgress()
	{
		if (!mob.mountMode && !isSpecialPet)
		{
			if (tameValue > 0f)
			{
				tameValue -= Time.deltaTime / goWildAfter * tameMaxValue;
			}
			else if (tamed)
			{
				GoWild();
				tamed = false;
				following = false;
			}
		}
	}

	private void UpdateSaving()
	{
		if (tamed && Time.time > saveTimer && mob.groundIndicator.active)
		{
			Manager.Get<PetManager>().SavePetPosition(this);
			saveTimer = Time.time + 5f;
		}
	}

	private void TryToSpawnEgg()
	{
		int num = Manager.Get<CraftingManager>().ShouldSpawnEgg();
		if (num > 0 && Misc.GetTimeStampDouble() > lastEggTimeStamp + (double)Manager.Get<CraftingManager>().GetResourceDefinition(num).timeLimit)
		{
			lastEggTimeStamp = Misc.GetTimeStampDouble();
			GameObject gameObject = UnityEngine.Object.Instantiate(Manager.Get<CraftingManager>().lootPrefab);
			gameObject.GetComponent<ResourceSprite>().InitWithResourceId(base.transform.position, num);
		}
	}

	protected virtual void OnTamed()
	{
		MoveModeChange();
		mob.navigator.SetDestination(CameraController.instance.MainCamera.transform.position);
		mob.navigator.speed = mob.runSpeed;
		string name = base.name.Replace("(Clone)", string.Empty);
		Manager.Get<QuestManager>().OnPetTame(name);
		TryToSpawnEgg();
		Manager.Get<PetManager>().RegisterPet(this);
		Manager.Get<PetManager>().TryToUnlockTamedPet(this);
		Manager.Get<QuestManager>().HandleMobIndicator(mob.gameObject);
		Manager.Get<AbstractAchievementManager>()?.RegisterEvent("npc.pet");
	}

	public void SetAsSpecialPet()
	{
		friend = base.gameObject.AddComponent<PettableFriend>();
	}

	private void GoWild()
	{
		Manager.Get<PetManager>().UnregisterPet(this);
		mob.logic = baseLogic;
		mob.ReconstructBehaviourTree();
	}

	private void PlayTameParticles()
	{
		Vector3 eulerAngles = base.transform.rotation.eulerAngles;
		Quaternion rotation = Quaternion.AngleAxis(eulerAngles.y, Vector3.up);
		Bounds relativeBounds = mob.relativeBounds;
		Vector3 vector = relativeBounds.center;
		Vector3 min = relativeBounds.min;
		float y = min.y;
		Vector3 max = relativeBounds.max;
		float y2 = max.y;
		Vector3 min2 = relativeBounds.min;
		vector.y = y + 0.8f * (y2 - min2.y);
		vector = rotation * vector;
		GameObject prefab = Resources.Load<GameObject>("prefabs/" + customParticlesPrefabName);
		PlayParticles(prefab, mob.transform, vector, string.Empty);
	}

    [Obsolete]
    protected void PlayParticles(GameObject prefab, Transform parent, Vector3 position, string layer = "")
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(prefab);
       gameObject.GetComponent<ParticleSystem>().emissionRate = (!(tameValue >= tameMaxValue)) ? Mathf.Lerp(0f, 6f, (tameValue - 10f) / tameMaxValue) : 10f;
		position = position.Valid();
		gameObject.transform.parent = parent;
		gameObject.transform.localPosition = position;
		if (!string.IsNullOrEmpty(layer))
		{
			gameObject.transform.SetLayerRecursively(LayerMask.NameToLayer(layer));
		}
	}

	public void TryBePatient()
	{
		if (HasToBePatient())
		{
			base.gameObject.AddComponent<Patient>();
		}
	}

	protected virtual bool HasToBePatient()
	{
		HospitalManager hospitalManager = Manager.Get<HospitalManager>();
		return hospitalManager != null && hospitalManager.patientsType == HospitalManager.PatientsType.Animal && !isSpecialPet;
	}

	public void TrySetVampireSkin()
	{
		HumanMob humanMob = mob as HumanMob;
		if (humanMob == null)
		{
			return;
		}
		HospitalManager hospitalManager = Manager.Get<HospitalManager>();
		if (!(hospitalManager == null) && hospitalManager.vampireMode.enabled)
		{
			PlayerGraphic componentInChildren = GetComponentInChildren<PlayerGraphic>();
			if (componentInChildren != null)
			{
				componentInChildren.SetRandomSkinWithGender(hospitalManager.vampireMode.vampireSkinList, humanMob.currentGender);
			}
		}
	}

	private IEnumerator DoActionAfterTime(float seconds, Action action)
	{
		yield return new WaitForSeconds(seconds);
		action?.Invoke();
	}

	private void OnDestroy()
	{
		if (tamed)
		{
			Manager.Get<PetManager>()?.PetDespawned(this);
		}
	}
}
