// DecompilerFi decompiler from Assembly-CSharp.dll class: CanInteractWithBlueprintsTutorialReq
using Common.Managers;
using Gameplay;
using States;
using Uniblocks;

public class CanInteractWithBlueprintsTutorialReq : TutorialRequirement
{
	private SurvivalPhaseContext survivalPhaseContext;

	private PlayerMovement _playerMovement;

	private GameplayState _gameplayInstance;

	private PlayerMovement playerMovement
	{
		get
		{
			if (_playerMovement == null)
			{
				_playerMovement = PlayerGraphic.GetControlledPlayerInstance().GetComponentInParent<PlayerMovement>();
			}
			return _playerMovement;
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

	public override void Init()
	{
		if (Manager.Contains<SurvivalManager>())
		{
			survivalPhaseContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<SurvivalPhaseContext>(Fact.SURVIVAL_PHASE);
		}
	}

	public override bool IsFulfilled()
	{
		return gameplayInstance.currentSubstate.substate == GameplayState.Substates.WALKING && !playerMovement.inCutscene && (survivalPhaseContext == null || !survivalPhaseContext.isCombat);
	}
}
