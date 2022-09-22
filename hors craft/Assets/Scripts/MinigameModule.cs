// DecompilerFi decompiler from Assembly-CSharp.dll class: MinigameModule
using Common.Managers;
using Common.Model;

public class MinigameModule : ModelModule
{
	private string keyDifficultyMuliplier()
	{
		return "mashing.difficulty.multiplier";
	}

	public override void FillModelDescription(ModelDescription descriptions)
	{
		descriptions.AddDescription(keyDifficultyMuliplier(), 1f);
	}

	public override void OnModelDownloaded()
	{
	}

	public float GetMinigameDifficulty()
	{
		return base.settings.GetFloat(keyDifficultyMuliplier());
	}
}
