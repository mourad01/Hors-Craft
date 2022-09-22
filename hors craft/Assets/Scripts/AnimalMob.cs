// DecompilerFi decompiler from Assembly-CSharp.dll class: AnimalMob
using Common.BehaviourTrees;
using UnityEngine;

public class AnimalMob : Mob
{
	public enum AnimalLogic
	{
		WANDER,
		WANDER_RUN_FROM_PLAYER,
		WANDER_RUN_TO_PLAYER,
		FLY_AROUND,
		SWIM_AROUND,
		FOLLOW_PLAYER,
		STAY_IN_PLACE,
		RUN_TO_RESOURCE,
		FEAR_RUN_FROM_PLAYER,
		FEAR_FOLLOW_PLAYER
	}

	public AnimalLogic logic;

	private AnimalLogic previousLogic;

	public float spawnYOffset;

	public float resourceWaitFrom = 4f;

	public float resourceWaitTo = 6f;

	public float wanderWaitFrom = 8f;

	public float wanderWaitTo = 18f;

	public float wanderDistanceFrom = 3f;

	public float wanderDistanceTo = 10f;

	public float wanderSpeed = 10f;

	public float runSpeed = 15f;

	private bool mountModeValue;

	private bool boundsSet;

	private Bounds relativeBoundsValue;

	public bool observed;

	public bool mountMode
	{
		get
		{
			return mountModeValue;
		}
		set
		{
			base.enabled = !value;
			base.navigator.enabled = !value;
			base.animator.SetBool("walking", !value);
			Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
			foreach (Collider collider in componentsInChildren)
			{
				collider.enabled = !value;
			}
			base.body.useGravity = !value;
			mountModeValue = value;
		}
	}

	public Bounds relativeBounds
	{
		get
		{
			if (!boundsSet)
			{
				relativeBoundsValue = GetRelativeBounds();
				boundsSet = true;
			}
			return relativeBoundsValue;
		}
	}

	public void MoveAway(Vector3 direction, float distance, float duration = 2f, bool run = false)
	{
		if (duration > 0f)
		{
			DisableBehaviourTree(duration);
		}
		base.navigator.SetDestination(base.transform.position + direction.normalized * distance);
		base.navigator.speed = ((!run) ? wanderSpeed : runSpeed);
	}

	protected override void Start()
	{
		base.Start();
		base.transform.Translate(Vector3.up * spawnYOffset);
	}

	protected override ParallelNode ConstructTopParallelNode()
	{
		ParallelNode parallelNode = base.ConstructTopParallelNode();
		switch (logic)
		{
		case AnimalLogic.WANDER:
			ConstructLogicWander(parallelNode);
			break;
		case AnimalLogic.WANDER_RUN_FROM_PLAYER:
			ConstructLogicWanderRunFromPlayer(parallelNode);
			break;
		case AnimalLogic.WANDER_RUN_TO_PLAYER:
			ConstructLogicWanderRunToPlayer(parallelNode);
			break;
		case AnimalLogic.FLY_AROUND:
			ConstructLogicFlyAround(parallelNode);
			break;
		case AnimalLogic.SWIM_AROUND:
			ConstructLogicSwimAround(parallelNode);
			break;
		case AnimalLogic.FOLLOW_PLAYER:
			ConstructLogicFollowPlayer(parallelNode);
			break;
		case AnimalLogic.STAY_IN_PLACE:
			ConstructLogicStayInPlace(parallelNode);
			break;
		case AnimalLogic.RUN_TO_RESOURCE:
			ConstructLogicRunToResource(parallelNode);
			break;
		case AnimalLogic.FEAR_RUN_FROM_PLAYER:
			ConstructLogicFearRunFromPlayer(parallelNode);
			break;
		case AnimalLogic.FEAR_FOLLOW_PLAYER:
			ConstructLogicFearFallowPlayer(parallelNode);
			break;
		}
		return parallelNode;
	}

	private void ConstructLogicWander(ParallelNode topNode)
	{
		LoopNode loopNode = new LoopNode();
		topNode.Add(loopNode);
		loopNode.Add(new WaitNode(this, wanderWaitFrom, wanderWaitTo));
		loopNode.Add(new SetDestinationRandomNode(this, wanderDistanceFrom, wanderDistanceTo));
		loopNode.Add(new GoToDestinationNode(this, wanderSpeed));
	}

	private void ConstructLogicWanderRunFromPlayer(ParallelNode topNode)
	{
		SelectorNode selectorNode = new SelectorNode();
		topNode.Add(selectorNode);
		SequenceNode sequenceNode = new SequenceNode();
		selectorNode.Add(sequenceNode);
		sequenceNode.Add(new IsPlayerCloseNode(this, 4f));
		LoopNode loopNode = new LoopNode();
		sequenceNode.Add(loopNode);
		loopNode.Add(new SetDestinationAwayFromPlayer(this, 6f));
		loopNode.Add(new GoToDestinationNode(this, runSpeed));
		LoopNode loopNode2 = new LoopNode();
		selectorNode.Add(loopNode2);
		loopNode2.Add(new WaitNode(this, wanderWaitFrom, wanderWaitTo));
		loopNode2.Add(new SetDestinationRandomNode(this, wanderDistanceFrom, wanderDistanceTo));
		loopNode2.Add(new GoToDestinationNode(this, wanderSpeed));
	}

	private void ConstructLogicRunToResource(ParallelNode topNode)
	{
		SelectorNode selectorNode = new SelectorNode();
		topNode.Add(selectorNode);
		SequenceNode sequenceNode = new SequenceNode();
		selectorNode.Add(sequenceNode);
		sequenceNode.Add(new SetResourceDestinationNode(this));
		sequenceNode.Add(new GoToDestinationNode(this, runSpeed, waitToEndMoving: false));
		sequenceNode.Add(new IsResourceCloseNode(this));
		selectorNode.Add(new ResourceWaitNode(this, Random.Range(resourceWaitFrom, resourceWaitTo)));
	}

	private void ConstructLogicWanderRunToPlayer(ParallelNode topNode)
	{
		SelectorNode selectorNode = new SelectorNode();
		topNode.Add(selectorNode);
		SequenceNode sequenceNode = new SequenceNode();
		selectorNode.Add(sequenceNode);
		sequenceNode.Add(new IsPlayerCloseNode(this));
		sequenceNode.Add(new SetDestinationNearPlayer(this, 2f));
		sequenceNode.Add(new GoToDestinationNode(this, runSpeed));
		LoopNode loopNode = new LoopNode();
		selectorNode.Add(loopNode);
		loopNode.Add(new WaitNode(this, wanderWaitFrom, wanderWaitTo));
		loopNode.Add(new SetDestinationRandomNode(this, wanderDistanceFrom, wanderDistanceTo));
		loopNode.Add(new GoToDestinationNode(this, wanderSpeed));
	}

	private void ConstructLogicFlyAround(ParallelNode topNode)
	{
		SequenceNode sequenceNode = new SequenceNode();
		sequenceNode.Add(new IsCloseToWaterNode(this));
		sequenceNode.Add(new CooldownNode(autoSetCooldown: true, 2f));
		sequenceNode.Add(new SetDestinationRandomNode(this, wanderDistanceFrom, wanderDistanceTo));
		sequenceNode.Add(new GoToDestinationNode(this, wanderSpeed));
		SequenceNode sequenceNode2 = new SequenceNode();
		sequenceNode2.Add(new SetDestinationRandomNode(this, wanderDistanceFrom, wanderDistanceTo));
		sequenceNode2.Add(new GoToDestinationNode(this, wanderSpeed));
		SequenceNode sequenceNode3 = new SequenceNode();
		sequenceNode3.Add(new WaitNode(this, wanderWaitFrom, wanderWaitTo, Status.FAILURE));
		sequenceNode3.Add(sequenceNode2);
		SelectorNode selectorNode = new SelectorNode();
		selectorNode.Add(sequenceNode);
		selectorNode.Add(sequenceNode3);
		topNode.Add(selectorNode);
	}

	private void ConstructLogicSwimAround(ParallelNode topNode)
	{
		LoopNode loopNode = new LoopNode();
		topNode.Add(loopNode);
		loopNode.Add(new IsNotMovingNode(this));
		loopNode.Add(new SetDestinationRandomNode(this, wanderDistanceFrom, wanderDistanceTo));
		loopNode.Add(new GoToDestinationNode(this, wanderSpeed));
	}

	private void ConstructLogicFollowPlayer(ParallelNode topNode)
	{
		SelectorNode selectorNode = new SelectorNode();
		topNode.Add(selectorNode);
		selectorNode.Add(new IsPlayerCloseNode(this, 5f));
		SequenceNode sequenceNode = new SequenceNode();
		selectorNode.Add(sequenceNode);
		sequenceNode.Add(new WaitNode(this, 1f, 2f));
		sequenceNode.Add(new SetDestinationNearPlayer(this, 4f));
		sequenceNode.Add(new GoToDestinationNode(this, runSpeed));
	}

	private void ConstructLogicStayInPlace(ParallelNode topNode)
	{
	}

	private void ConstructLogicFearRunFromPlayer(ParallelNode topNode)
	{
		SelectorNode selectorNode = new SelectorNode();
		topNode.Add(selectorNode);
		SequenceNode sequenceNode = new SequenceNode();
		selectorNode.Add(sequenceNode);
		sequenceNode.Add(new FearRunAwayPlayerLoopNode(this, 50f, 10f, runSpeed));
		sequenceNode.Add(new ChangeLogicNode(this, previousLogic));
	}

	private void ConstructLogicFearFallowPlayer(ParallelNode topNode)
	{
		SelectorNode selectorNode = new SelectorNode();
		topNode.Add(selectorNode);
		SequenceNode sequenceNode = new SequenceNode();
		selectorNode.Add(sequenceNode);
		sequenceNode.Add(new FearFallowPlayerLoopNode(this, 2f, 15f, runSpeed * 0.5f, 2f));
		sequenceNode.Add(new ChangeLogicNode(this, previousLogic));
	}

	private Bounds GetRelativeBounds()
	{
		float num = float.MaxValue;
		float num2 = float.MaxValue;
		float num3 = float.MaxValue;
		float num4 = float.MinValue;
		float num5 = float.MinValue;
		float num6 = float.MinValue;
		Vector3 eulerAngles = base.transform.rotation.eulerAngles;
		Quaternion rotation = Quaternion.AngleAxis(0f - eulerAngles.y, Vector3.up);
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			if (!(renderer is ParticleSystemRenderer))
			{
				Vector3 vector = rotation * (renderer.bounds.min - base.transform.position);
				Vector3 vector2 = rotation * (renderer.bounds.max - base.transform.position);
				num = Mathf.Min(num, vector.x);
				num2 = Mathf.Min(num2, vector.y);
				num3 = Mathf.Min(num3, vector.z);
				num4 = Mathf.Max(num4, vector2.x);
				num5 = Mathf.Max(num5, vector2.y);
				num6 = Mathf.Max(num6, vector2.z);
			}
		}
		Vector3 vector3 = new Vector3(num, num2, num3);
		Vector3 vector4 = new Vector3(num4, num5, num6);
		Vector3 center = (vector3 + vector4) / 2f;
		Vector3 size = vector4 - vector3;
		return new Bounds(center, size);
	}

	public void SetLogicWithSavingPrevious(AnimalLogic newLogic)
	{
		previousLogic = logic;
		logic = newLogic;
	}
}
