// DecompilerFi decompiler from Assembly-CSharp.dll class: ReactingOnClothes
using Common.Managers;
using Gameplay;
using UnityEngine;

public class ReactingOnClothes : MonoBehaviour
{
	public AudioClip[] reactionClips;

	private AudioSource _audioSource;

	public float rangeToReact;

	public float reactCooldown;

	public string customParticlesPrefabName;

	private const float PARTICLE_SPAWNED_AT_HEIGHT = 0.8f;

	private AnimalMob _mob;

	private Transform playerTransform;

	private float timeLeftToReact;

	private bool canReact;

	public AnimalMob mob
	{
		get
		{
			if (_mob == null)
			{
				_mob = GetComponent<AnimalMob>();
			}
			return _mob;
		}
		private set
		{
			_mob = value;
		}
	}

	private void Awake()
	{
		playerTransform = PlayerGraphic.GetControlledPlayerInstance().transform;
		_audioSource = GetComponent<AudioSource>();
		if (string.IsNullOrEmpty(customParticlesPrefabName))
		{
			customParticlesPrefabName = "hearts";
		}
	}

	private AudioClip GetRandomClip()
	{
		int num = UnityEngine.Random.Range(0, reactionClips.Length - 1);
		return reactionClips[num];
	}

	private void FixedUpdate()
	{
		if (canReact && Vector3.Distance(playerTransform.position, base.transform.position) <= rangeToReact)
		{
			ReactOnPlayer();
		}
	}

	private void Update()
	{
		if (timeLeftToReact > 0f)
		{
			canReact = false;
			timeLeftToReact -= Time.deltaTime;
		}
		else
		{
			canReact = true;
		}
	}

	private void ReactOnPlayer()
	{
		Manager.Get<QuestManager>().IncreaseQuestOfType(QuestType.GetXNpcReactions);
		base.transform.LookAt(playerTransform);
		Vector3 eulerAngles = base.transform.eulerAngles;
		eulerAngles.x = 0f;
		eulerAngles.z = 0f;
		base.transform.eulerAngles = eulerAngles;
		canReact = false;
		timeLeftToReact = reactCooldown;
		PlayParticles();
		GetComponentInChildren<Animator>().SetTrigger("happy");
		if (Manager.Contains<ProgressManager>())
		{
			Manager.Get<ProgressManager>().IncreaseExperience(Manager.Get<ModelManager>().dressupSettings.GetReactionGlamourValue());
		}
		if (Manager.Contains<AbstractSoftCurrencyManager>())
		{
			Manager.Get<AbstractSoftCurrencyManager>().OnCurrencyAmountChange(Manager.Get<ModelManager>().dressupSettings.GetReactionGoldValue());
		}
		_audioSource.clip = GetRandomClip();
		_audioSource.Play();
	}

    [System.Obsolete]
    private void PlayParticles()
	{
		GameObject original = Resources.Load<GameObject>("prefabs/" + customParticlesPrefabName);
		GameObject gameObject = UnityEngine.Object.Instantiate(original);
		gameObject.GetComponent<ParticleSystem>().emissionRate = 0f;
		Vector3 eulerAngles = base.transform.rotation.eulerAngles;
		Quaternion rotation = Quaternion.AngleAxis(eulerAngles.y, Vector3.up);
		Vector3 vector = mob.relativeBounds.center;
		vector.y = 1.567f;
		vector = rotation * vector;
		gameObject.transform.parent = mob.transform;
		gameObject.transform.position = vector + mob.transform.position;
	}
}
