// DecompilerFi decompiler from Assembly-CSharp.dll class: MashToFillBarHealingGame
using Common.Managers;
using Gameplay;
using UnityEngine;

public class MashToFillBarHealingGame : TappingGameBehaviour
{
	private AnimationCurve[] opponentBehaviours;

	private const float winningCondition = 100f;

	private float progress;

	private float defaultProgress;

	private float tapStrnegth;

	private AnimationCurve opponentCurrentBehaviour;

	private float opponentCD;

	private float opponentTimer;

	private float gameTimer;

	private float difficultyMultiplier = 1f;

	public MashToFillBarHealingGame(AnimationCurve[] behaviours, float tap = 5f, float opponentCD = 0.1f, float defaultProgress = 50f)
	{
		tapStrnegth = tap;
		this.opponentCD = opponentCD;
		this.defaultProgress = defaultProgress;
		opponentBehaviours = behaviours;
		difficultyMultiplier = Manager.Get<ModelManager>().minigameSettings.GetMinigameDifficulty();
	}

	public override void LeftActionButton()
	{
		Tap();
	}

	public override void RightActionButton()
	{
		Tap();
	}

	private void Tap()
	{
		progress += tapStrnegth;
		tappingGameGraphics.OnProgress(progress / 100f);
	}

	public override void Update()
	{
		if (progress >= 100f)
		{
			state = GameState.won;
			return;
		}
		if (progress < 0f)
		{
			state = GameState.lost;
			return;
		}
		opponentTimer += Time.unscaledDeltaTime;
		gameTimer += Time.unscaledDeltaTime;
		if (opponentTimer >= opponentCD)
		{
			progress -= opponentCurrentBehaviour.Evaluate(gameTimer) * difficultyMultiplier;
			tappingGameGraphics.OnProgress(progress / 100f);
			opponentTimer = 0f;
		}
	}

	public override void ResetRound()
	{
		state = GameState.inProgress;
		progress = defaultProgress;
		opponentTimer = 0f;
		gameTimer = 0f;
		opponentCurrentBehaviour = opponentBehaviours[Random.Range(0, opponentBehaviours.Length)];
	}

	public override StatsManager.MinigameType GetGameType()
	{
		return StatsManager.MinigameType.MASHING;
	}
}
