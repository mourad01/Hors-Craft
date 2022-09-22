// DecompilerFi decompiler from Assembly-CSharp.dll class: Mob
using com.ootii.Cameras;
using Common.BehaviourTrees;
using Common.Managers;
using Gameplay;
using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MobNavigator))]
public abstract class Mob : MonoBehaviour
{
	[Serializable]
	public class LookConfig
	{
		public bool hasHead = true;

		public bool looksAtPlayer;

		public Transform headTransform;

		public float lookingSpeed = 5f;

		public float minDotToLook = 0.1f;
	}

	public class InitializeParameter
	{
	}

	[HideInInspector]
	public new string name;

	[HideInInspector]
	public PetManager.PetIndividualData petData;

	public LookConfig lookConfig = new LookConfig();

	public Sprite mobSprite;

	public float jumpPower = 10f;

	public GameObject particlesPrefab;

	public float particlesScale = 1f;

	[HideInInspector]
	private GameObject questIndicator;

	[SerializeField]
	[HideInInspector]
	public TriggerIndicator groundIndicator;

	[SerializeField]
	[HideInInspector]
	public TriggerIndicator forwardObstacleIndicator;

	[SerializeField]
	[HideInInspector]
	public Vector3 groundOffset;

	private Node topNode;

	private Renderer anyRenderer;

	private float behaviourTreeDisableTimer;

	private const float DISPOSE_BELOW_Y = -64f;

	private Vector3 lookAtTarget;

	public Vector3 groundPosition => base.transform.TransformPoint(groundOffset);

	public bool isVisible => anyRenderer != null && anyRenderer.isVisible;

	public Rigidbody body
	{
		get;
		private set;
	}

	public MobNavigator navigator
	{
		get;
		protected set;
	}

	public Animator animator
	{
		get;
		protected set;
	}

	public void ReconstructBehaviourTree()
	{
		topNode = ConstructTopParallelNode();
	}

	public void DisableBehaviourTree(float time)
	{
		behaviourTreeDisableTimer = time;
	}

	public virtual void Init(InitializeParameter parameter)
	{
	}

	protected virtual void Start()
	{
		topNode = ConstructTopParallelNode();
		navigator = GetComponent<MobNavigator>();
		animator = GetComponentInChildren<Animator>();
		if (animator != null)
		{
			animator.logWarnings = false;
			animator.cullingMode = AnimatorCullingMode.CullCompletely;
		}
		body = GetComponent<Rigidbody>();
		anyRenderer = GetComponentInChildren<Renderer>();
		if ((bool)navigator)
		{
			navigator.jumpPower = jumpPower;
		}
	}

	protected virtual void Update()
	{
		if (!isVisible)
		{
			return;
		}
		if (behaviourTreeDisableTimer > 0f)
		{
			behaviourTreeDisableTimer -= Time.deltaTime;
		}
		else
		{
			topNode.Update();
		}
		if (anyRenderer != null && isVisible)
		{
			UpdateLooking();
		}
		Vector3 position = base.transform.position;
		if (position.y < -64f)
		{
			Pettable component = base.transform.GetComponent<Pettable>();
			if (component == null || !component.TryToSpawnAgain())
			{
				Despawn();
			}
		}
	}

	protected virtual ParallelNode ConstructTopParallelNode()
	{
		if (CameraController.instance.MainCamera == null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		ParallelNode parallelNode = new ParallelNode();
		if (lookConfig.hasHead)
		{
			if (lookConfig.looksAtPlayer)
			{
				SelectorNode selectorNode = new SelectorNode();
				parallelNode.Add(selectorNode);
				SequenceNode sequenceNode = new SequenceNode();
				selectorNode.Add(sequenceNode);
				sequenceNode.Add(new IsPlayerCloseNode(this));
				sequenceNode.Add(new LookAtPlayerNode(this));
				selectorNode.Add(new LookRandomlyNode(this));
			}
			else
			{
				parallelNode.Add(new LookRandomlyNode(this));
			}
		}
		if (GetComponent<Health>() != null)
		{
			parallelNode.Add(ConstructDieBehaviour());
		}
		return parallelNode;
	}

	public virtual void LookAt(Vector3 target)
	{
		lookAtTarget = target;
	}

	public virtual void LookAt(GameObject gameObject)
	{
	}

	public void ResetLook()
	{
		if (lookConfig.hasHead)
		{
			lookConfig.headTransform.localRotation = Quaternion.Euler(90f, 0f, 0f);
		}
	}

	private void UpdateLooking()
	{
		if (lookConfig.hasHead && Vector3.Dot(base.transform.forward, (lookAtTarget - groundPosition).normalized) > lookConfig.minDotToLook)
		{
			Quaternion b = Quaternion.LookRotation(lookAtTarget - lookConfig.headTransform.position);
			lookConfig.headTransform.rotation = Quaternion.Lerp(lookConfig.headTransform.rotation, b, Time.deltaTime * lookConfig.lookingSpeed);
		}
	}

	public virtual void Die()
	{
		Health component = GetComponent<Health>();
		if (component != null && particlesPrefab != null && component.renderers != null && component.renderers.Length > 0)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(particlesPrefab, base.transform.position + particlesPrefab.transform.position, Quaternion.identity);
			gameObject.transform.SetParent(base.transform);
			SetScale(gameObject);
			ParticleSystem component2 = gameObject.GetComponent<ParticleSystem>();
			component2.GetComponent<Renderer>().sharedMaterial = component.renderers.FirstOrDefault((Renderer r) => r.sharedMaterial != null).sharedMaterial;
			component2.Play();
			Renderer[] renderers = component.renderers;
			foreach (Renderer renderer in renderers)
			{
				renderer.enabled = false;
			}
			DropLoot();
			UnityEngine.Object.Destroy(base.gameObject, 2f);
		}
		else
		{
			Despawn();
		}
		if (animator != null)
		{
			animator.SetTrigger("die");
		}
	}

	private void SetScale(GameObject gameObject)
	{
		gameObject.transform.localScale = new Vector3(particlesScale, particlesScale, particlesScale);
	}

	public virtual void Despawn()
	{
		DropLoot();
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void DropLoot()
	{
		Loot component = GetComponent<Loot>();
		if (component != null)
		{
			component.DropLoot();
		}
	}

	protected virtual Node ConstructDieBehaviour()
	{
		SequenceNode sequenceNode = new SequenceNode();
		sequenceNode.Add(new IsDeadNode(this, GetComponent<Health>()));
		sequenceNode.Add(new StandInPlaceNode(this));
		SelectorNode selectorNode = new SelectorNode();
		selectorNode.Add(new HasAlreadyDiedNode(this));
		selectorNode.Add(new DieNode(this));
		sequenceNode.Add(selectorNode);
		return sequenceNode;
	}

	public void SetQuestIndicator(Quest quest)
	{
		if (Manager.Get<ModelManager>().worldsSettings.AreWorldQuestEnabled() && !(questIndicator != null))
		{
			questIndicator = Manager.Get<QuestManager>().GetNewIndicatorObject(quest.GenerateWorldId());
			questIndicator.transform.SetParent(base.transform);
			questIndicator.transform.localPosition = Vector3.zero;
			questIndicator.transform.localScale = Vector3.one;
			BoxCollider componentInParent = GetComponentInParent<BoxCollider>();
			Transform transform = questIndicator.transform;
			Vector3 center = componentInParent.center;
			Vector3 size = componentInParent.size;
			transform.localPosition = center.Add(0f, size.y + 0.5f, 0f);
			questIndicator.GetComponent<PositonPingPong>().Init();
		}
	}

	public void RemoveQuestIndicator()
	{
		if (questIndicator != null)
		{
			Manager.Get<QuestManager>().ReturnIndicator(questIndicator);
		}
		questIndicator = null;
	}

	public bool IsTamed()
	{
		return petData == null;
	}
}
