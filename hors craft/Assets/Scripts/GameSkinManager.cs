// DecompilerFi decompiler from Assembly-CSharp.dll class: GameSkinManager
using Common.Managers;
using Gameplay;
using System.Collections.Generic;

public class GameSkinManager : Manager, IGameCallbacksListener
{
	public List<GameSkin> skins = new List<GameSkin>();

	protected Dictionary<int, GameSkin> skinsGenerated = new Dictionary<int, GameSkin>();

	public override void Init()
	{
		Manager.Get<GameCallbacksManager>().RegisterListener(this);
		PrepeareSkins();
	}

	private void PrepeareSkins()
	{
		if (skins != null)
		{
			skinsGenerated = new Dictionary<int, GameSkin>();
			foreach (GameSkin skin in skins)
			{
				skinsGenerated.Add(skin.levelId, skin);
			}
		}
	}

	public void SetTileSetForCurrentLevel()
	{
		if (Manager.Contains<SavedWorldManager>())
		{
			SavedWorldManager savedWorldManager = Manager.Get<SavedWorldManager>();
			if (!(savedWorldManager == null))
			{
				GetSkinForLevel(savedWorldManager.currentWorldDataIndex)?.InjectTileSetIntoEngine();
			}
		}
	}

	public GameSkin GetSkinForLevel(int levelId = 0)
	{
		if (skinsGenerated == null)
		{
			return null;
		}
		if (!skinsGenerated.ContainsKey(levelId))
		{
			return null;
		}
		return skinsGenerated[levelId];
	}

	public void OnGameplayRestarted()
	{
	}

	public void OnGameplayStarted()
	{
		SetTileSetForCurrentLevel();
	}

	public void OnGameSavedFrequent()
	{
	}

	public void OnGameSavedInfrequent()
	{
	}
}
