// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.AdRequirement
using UnityEngine;

namespace Gameplay
{
	[CreateAssetMenu(fileName = "AdsRequirement", menuName = "ScriptableObjects/Requirements/Ads", order = 1)]
	public class AdRequirement : Requirement
	{
		public override bool CheckIfMet(float requiredAmount = 0f, string id = "")
		{
			if (!AutoRefreshingStock.HasItem($"{id}.ads"))
			{
				AutoRefreshingStock.InitStockItem($"{id}.ads", float.NaN, int.MaxValue, 0);
			}
			return (float)AutoRefreshingStock.GetStockCount($"{id}.ads") >= requiredAmount;
		}
	}
}
