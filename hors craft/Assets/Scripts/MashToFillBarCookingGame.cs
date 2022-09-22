// DecompilerFi decompiler from Assembly-CSharp.dll class: MashToFillBarCookingGame
using Common.Managers;
using Gameplay;
using UnityEngine;

public class MashToFillBarCookingGame : TappingGameBehaviour
{
	private const float winningCondition = 100f;

	private float progress;

	private float defaultProgress;

	private float tapStrnegth;

	private float opponentCD;

	private float opponentTapStrength;

	private float opponentTimer;

	private float difficultyMultiplier = 1f;

	public MashToFillBarCookingGame(float tap = 3f, float defaultProgress = 0f, float opponentCD = 0.2f, float opponentTapStrength = 0.05f)
	{
		tapStrnegth = tap;
		this.defaultProgress = defaultProgress;
		this.opponentCD = opponentCD;
		this.opponentTapStrength = opponentTapStrength;
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
		opponentTimer += Time.unscaledDeltaTime;
		if (opponentTimer >= opponentCD)
		{
			progress -= opponentTapStrength * difficultyMultiplier;
			if (progress < 0f)
			{
				progress = 0f;
			}
			tappingGameGraphics.OnProgress(progress / 100f);
			opponentTimer = 0f;
		}
	}

	public override void ResetRound()
	{
		state = GameState.inProgress;
		progress = defaultProgress;
		opponentTimer = 0f;
	}

	public override StatsManager.MinigameType GetGameType()
	{
		return StatsManager.MinigameType.MASHING;
	}
}
