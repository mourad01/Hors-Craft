// DecompilerFi decompiler from Assembly-CSharp.dll class: MinigameStartParameter
using Common.Managers.States;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class MinigameStartParameter : StartParameter
{
	public string playerConfig;

	public string gameConfig;

	public List<string> customConfigs = new List<string>();

	public Minigame minigame;

	public MinigameSingleConfig GetGameConfig(string name)
	{
		return minigame.gameSpecificConfigs.FirstOrDefault((MinigameSingleConfig c) => c.Name == name);
	}

	public MinigameSingleConfig GetPlayerConfig(string name)
	{
		return minigame.playerSpecificConfigs.FirstOrDefault((MinigameSingleConfig c) => c.Name == name);
	}

	public MinigameSingleConfig GetCustomConfig(string name)
	{
		return minigame.customConfigs.FirstOrDefault((MinigameSingleConfig c) => c.Name == name);
	}
}
