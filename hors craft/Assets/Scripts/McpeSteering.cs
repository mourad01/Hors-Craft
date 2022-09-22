// DecompilerFi decompiler from Assembly-CSharp.dll class: McpeSteering
using Common.Managers;
using Gameplay;
using System;
using Uniblocks;

public class McpeSteering : MonoBehaviourSingleton<McpeSteering>, IGameCallbacksListener
{
	public float interactionDistance = 15f;

	public float tapBeforeTime = 0.2f;

	public float digAfterTime = 0.45f;

	public float digNextVoxelTimeInterval = 0.3f;

	public float dragThreshold = 5f;

	public bool skipAdding;

	public bool skipDigging;

	public bool passedHitTest;

	public VoxelInfo blueprintVoxelDirectHit;

	public Action OnAdd;

	public Action OnDig;

	public Action<float> OnPingIndicator;

	public bool isPointerDown
	{
		get;
		set;
	}

	public int? pointerID
	{
		get;
		set;
	}

	public bool isMoving
	{
		get;
		set;
	}

	public bool isDigging
	{
		get;
		set;
	}

	public void RegisterVoxelHoverActionsCallbacks(Action onAdd, Action onDig)
	{
		OnAdd = onAdd;
		OnDig = onDig;
	}

	public void RegisterPingCallback(Action<float> onPing)
	{
		OnPingIndicator = onPing;
	}

	public override string ToString()
	{
		return $"PointerDown: {isPointerDown.ToString()}; PointerID: {pointerID.ToString()}; IsMoving: {isMoving.ToString()}; IsDigging: {isDigging.ToString()}; ";
	}

	public override void Init()
	{
		base.Init();
		Manager.Get<GameCallbacksManager>().RegisterListener(this);
	}

	public void OnGameplayStarted()
	{
		if (Manager.Contains<ModelManager>())
		{
			if (Manager.Get<ModelManager>().mcpeSteering.GetEnabled())
			{
				MonoBehaviourSingleton<GameplayFacts>.get.AddFactIfNotExisting(Fact.MCPE_STEERING);
			}
			else
			{
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFact(Fact.MCPE_STEERING);
			}
		}
	}

	public void OnGameSavedFrequent()
	{
	}

	public void OnGameSavedInfrequent()
	{
	}

	public void OnGameplayRestarted()
	{
	}
}
