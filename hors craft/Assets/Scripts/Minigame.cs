// DecompilerFi decompiler from Assembly-CSharp.dll class: Minigame
using System.Collections.Generic;
using UnityEngine;

public abstract class Minigame : ScriptableObject
{
	public List<MinigameSingleConfig> gameSpecificConfigs = new List<MinigameSingleConfig>();

	public List<MinigameSingleConfig> playerSpecificConfigs = new List<MinigameSingleConfig>();

	public List<MinigameSingleConfig> customConfigs = new List<MinigameSingleConfig>();

	public abstract void Play(MinigameStartParameter minigameParameter);
}
