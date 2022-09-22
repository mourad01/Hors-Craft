// DecompilerFi decompiler from Assembly-CSharp.dll class: ConfigModule
using Common.Behaviours;
using Common.Managers;
using Common.Model;
using Gameplay.Audio;
using System.Collections.Generic;

public class ConfigModule : ModelModule
{
	protected string keyPlayerMoveSpeed()
	{
		return "player.default.move.speed";
	}

	protected string keyPlayerFlySpeed()
	{
		return "player.default.fly.speed";
	}

	protected string keyPlayerJumpForce()
	{
		return "player.default.jump.force";
	}

	protected string keyDisabledPauseTabs()
	{
		return "pause.disabled.tabs";
	}

	protected string keyForceInstantStartTitleScreen()
	{
		return "title.force.instant.start";
	}

	protected string keyPercentOfBlocksHidden()
	{
		return "pause.blocks.hidden.percent";
	}

	protected string keyEnableUsableIndicators()
	{
		return "enable.usable.indicators";
	}

	protected string keyEnableBackgroundMusic()
	{
		return "enable.background.music";
	}

	protected string keyEnableFlyingForAd()
	{
		return "flying.for.ad.enabled";
	}

	protected string keyFlyingForAdTime()
	{
		return "flying.for.ad.time";
	}

	public override void FillModelDescription(ModelDescription descriptions)
	{
		descriptions.AddDescription(keyPlayerMoveSpeed(), 1f);
		descriptions.AddDescription(keyPlayerFlySpeed(), 1f);
		descriptions.AddDescription(keyPlayerJumpForce(), 1f);
		descriptions.AddDescription(keyDisabledPauseTabs(), string.Empty);
		descriptions.AddDescription(keyForceInstantStartTitleScreen(), 0);
		descriptions.AddDescription(keyPercentOfBlocksHidden(), 0f);
		descriptions.AddDescription(keyEnableUsableIndicators(), 1);
		descriptions.AddDescription(keyEnableBackgroundMusic(), 0);
		descriptions.AddDescription(keyEnableFlyingForAd(), 0);
		descriptions.AddDescription(keyFlyingForAdTime(), 600);
	}

	public override void OnModelDownloaded()
	{
		SunController instance = SunController.instance;
		if (instance != null)
		{
			instance.InitFromModel();
		}
		Manager.Get<MusicManager>().SetStatus(IsBackgroundMusicEnabled());
	}

	public float GetPlayerMoveSpeed()
	{
		float @float = base.settings.GetFloat(keyPlayerMoveSpeed(), 1f);
		return (!(@float > 0f)) ? 1f : @float;
	}

	public float GetPlayerFlySpeed()
	{
		float @float = base.settings.GetFloat(keyPlayerFlySpeed(), 1f);
		return (!(@float > 0f)) ? 1f : @float;
	}

	public float GetJumpForce()
	{
		float @float = base.settings.GetFloat(keyPlayerJumpForce(), 1f);
		return (!(@float > 0f)) ? 1f : @float;
	}

	public List<string> GetDisabledPauseTabs()
	{
		return ModelSettingsHelper.GetStringListFromSettings(base.settings, keyDisabledPauseTabs());
	}

	public bool ForceInstantStart()
	{
		return base.settings.GetInt(keyForceInstantStartTitleScreen()) == 1;
	}

	public float GetHiddenBlocksValue()
	{
		return base.settings.GetFloat(keyPercentOfBlocksHidden(), 0f) / 100f;
	}

	public bool IsUsableIndicatorsEnabled()
	{
		return base.settings.GetInt(keyEnableUsableIndicators()) == 1;
	}

	public bool IsBackgroundMusicEnabled()
	{
		return base.settings.GetInt(keyEnableBackgroundMusic()) == 1;
	}

	public bool IsFlyingForAdEnabled()
	{
		if (MonoBehaviourSingleton<DeveloperModeBehaviour>.get.isDeveloper)
		{
			return false;
		}
		return base.settings.GetInt(keyEnableFlyingForAd()) == 1;
	}

	public int GetFlyingForAdTime()
	{
		return base.settings.GetInt(keyFlyingForAdTime());
	}

	public int GetExpAmountForCategory(string categoryName, int defaultValue = 5)
	{
		return ModelSettingsHelper.GetIntFromStringSettings(base.settings, $"exp.category.{categoryName}", defaultValue);
	}
}
