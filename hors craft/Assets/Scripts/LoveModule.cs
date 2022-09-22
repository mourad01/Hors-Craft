// DecompilerFi decompiler from Assembly-CSharp.dll class: LoveModule
using Common.Managers;
using Common.Model;

public class LoveModule : ModelModule
{
	private string keyLoveDecrease(int stage)
	{
		return "love.decrease." + stage;
	}

	private string keyLovePerTap(int stage)
	{
		return "love.per.tap." + stage;
	}

	private string keyLoveRequired(int stage)
	{
		return "love.required." + stage;
	}

	private string keyLovePerNormalGift()
	{
		return "love.per.normal.gift";
	}

	private string keyLovePerFavouriteGift()
	{
		return "love.per.favourite.gift";
	}

	private string keyLovePerMinigame()
	{
		return "love.per.minigame";
	}

	private string keyDateCooldown()
	{
		return "date.cooldown";
	}

	private string keyMaxDates()
	{
		return "date.max.stack";
	}

	private string keyPhotoReward(int index)
	{
		return "photo.reward." + index;
	}

	public override void FillModelDescription(ModelDescription descriptions)
	{
		for (int i = 0; i < 3; i++)
		{
			descriptions.AddDescription(keyLoveDecrease(i), "0.2");
			descriptions.AddDescription(keyLovePerTap(i), "10");
			descriptions.AddDescription(keyLoveRequired(i), "100");
		}
		for (int j = 1; j <= 23; j++)
		{
			descriptions.AddDescription(keyPhotoReward(j), 0);
		}
		descriptions.AddDescription(keyLovePerMinigame(), 10f);
		descriptions.AddDescription(keyLovePerNormalGift(), 10f);
		descriptions.AddDescription(keyLovePerFavouriteGift(), 20f);
		descriptions.AddDescription(keyDateCooldown(), 30f);
		descriptions.AddDescription(keyMaxDates(), 3);
	}

	public override void OnModelDownloaded()
	{
	}

	public float GetLoveDecrease(int stage)
	{
		return float.Parse(base.settings.GetString(keyLoveDecrease(stage)));
	}

	public float GetLovePerTap(int stage)
	{
		return float.Parse(base.settings.GetString(keyLovePerTap(stage)));
	}

	public float GetLoveRequired(int stage)
	{
		return float.Parse(base.settings.GetString(keyLoveRequired(stage)));
	}

	public float GetLovePerGift(bool favourite)
	{
		return (!favourite) ? base.settings.GetFloat(keyLovePerNormalGift()) : base.settings.GetFloat(keyLovePerFavouriteGift());
	}

	public float GetLovePerMinigame()
	{
		return base.settings.GetFloat(keyLovePerMinigame());
	}

	public float GetDateCooldown()
	{
		return base.settings.GetFloat(keyDateCooldown());
	}

	public int GetMaxDates()
	{
		return base.settings.GetInt(keyMaxDates());
	}

	public int GetPhotoReward(int index)
	{
		return base.settings.GetInt(keyPhotoReward(index));
	}
}
