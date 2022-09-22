// DecompilerFi decompiler from Assembly-CSharp.dll class: States.LevelOrderInterpreter
using Common.Managers;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace States
{
	public class LevelOrderInterpreter : RankingDataInterpreter
	{
		public List<Sprite> icons = new List<Sprite>();

		public int levelStep = 5;

		public override KeyValuePair<string, string>? PrepareForSerialization()
		{
			if (Manager.Contains<ProgressManager>())
			{
				int level = Manager.Get<ProgressManager>().level;
				int a = level / levelStep;
				return new KeyValuePair<string, string>("order", Mathf.Min(a, icons.Count - 1).ToString());
			}
			return null;
		}

		public override void SetElement(RankingScoreElement element, Dictionary<string, string> data, RankingModel.ScoreData score)
		{
			element.playerAvatar.gameObject.SetActive(value: true);
			if (data.ContainsKey("order") && int.TryParse(data["order"], NumberStyles.Any, CultureInfo.InvariantCulture, out int result))
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
			element.playerAvatar.preserveAspect = true;
		}
	}
}
