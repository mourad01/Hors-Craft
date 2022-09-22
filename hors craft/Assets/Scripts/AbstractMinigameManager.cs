// DecompilerFi decompiler from Assembly-CSharp.dll class: AbstractMinigameManager
using UnityEngine;

public abstract class AbstractMinigameManager : MonoBehaviour
{
	[SerializeField]
	private MinigameStartParameter customParameter;

	[SerializeField]
	public bool useCustomParameter;

	protected Minigame minigame;

	protected MinigameSingleConfig playerConfig;

	protected MinigameSingleConfig gameConfig;

	private bool wasInit;

	private void Start()
	{
		if (useCustomParameter)
		{
			Init(customParameter);
			wasInit = true;
		}
	}

	public virtual void Init(MinigameStartParameter parameter)
	{
		if (!wasInit)
		{
			ProcessParameter(parameter);
		}
	}

	public virtual void ProcessParameter(MinigameStartParameter parameter)
	{
		minigame = (parameter.minigame as CustomSceneMinigame);
		playerConfig = parameter.GetPlayerConfig(parameter.playerConfig);
		gameConfig = parameter.GetGameConfig(parameter.gameConfig);
	}

	public abstract void ExitGame();
}
