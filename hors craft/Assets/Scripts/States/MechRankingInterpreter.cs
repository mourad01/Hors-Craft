// DecompilerFi decompiler from Assembly-CSharp.dll class: States.MechRankingInterpreter
using Common.Managers;
using ItemVInventory;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace States
{
	public class MechRankingInterpreter : RankingDataInterpreter
	{
		public List<Sprite> icons = new List<Sprite>();

		public override KeyValuePair<string, string>? PrepareForSerialization()
		{
			if (PlayerGraphic.GetControlledPlayerInstance() != null)
			{
				Equipment componentInParent = PlayerGraphic.GetControlledPlayerInstance().GetComponentInParent<Equipment>();
				ItemDefinition[] equipment = componentInParent.GetEquipment();
				int num = equipment.Sum((ItemDefinition i) => i.level);
				int a = num / 6;
				return new KeyValuePair<string, string>("mech", Mathf.Min(a, icons.Count - 1).ToString());
			}
			return null;
		}

		public override void SetElement(RankingScoreElement element, Dictionary<string, string> data, RankingModel.ScoreData score)
		{
			element.playerAvatar.gameObject.SetActive(value: true);
			if (data.ContainsKey("mech") && int.TryParse(data["mech"], NumberStyles.Any, CultureInfo.InvariantCulture, out int result))
			{
				if (result >= 0 && result < icons.Count)
				{
					element.playerAvatar.sprite = icons[result];
				}
				else
				{
					element.playerAvatar.sprite = icons[0];
				}
			}
			else
			{
				element.playerAvatar.sprite = icons[0];
			}
		}
	}
}
