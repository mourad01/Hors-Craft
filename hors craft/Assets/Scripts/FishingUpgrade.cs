// DecompilerFi decompiler from Assembly-CSharp.dll class: FishingUpgrade
using Common.Managers;
using UnityEngine;

public class FishingUpgrade : MonoBehaviour
{
	public enum UpgradeType
	{
		Line,
		Float,
		Reel,
		Hook,
		Baits
	}

	public UpgradeType upgrade;

	public void EnableUpgrade()
	{
		switch (upgrade)
		{
		case UpgradeType.Line:
			Manager.Get<FishingManager>().lineUpgrade = true;
			break;
		case UpgradeType.Float:
			Manager.Get<FishingManager>().floatUpgrade = true;
			break;
		case UpgradeType.Reel:
			Manager.Get<FishingManager>().reelUpgrade = true;
			break;
		case UpgradeType.Hook:
			Manager.Get<FishingManager>().hookUpgrade = true;
			break;
		case UpgradeType.Baits:
			Manager.Get<FishingManager>().baitsUpgrade = true;
			break;
		}
		Manager.Get<StatsManager>().BoughtUpgrade();
	}
}
