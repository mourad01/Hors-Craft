// DecompilerFi decompiler from Assembly-CSharp.dll class: States.PlayerLevelInterpreter
using Common.Managers;
using System.Collections.Generic;
using System.Globalization;

namespace States
{
	public class PlayerLevelInterpreter : RankingDataInterpreter
	{
		public override KeyValuePair<string, string>? PrepareForSerialization()
		{
			if (Manager.Contains<ProgressManager>())
			{
				return new KeyValuePair<string, string>("level", Manager.Get<ProgressManager>().level.ToString());
			}
			return null;
		}

		public override void SetElement(RankingScoreElement element, Dictionary<string, string> data, RankingModel.ScoreData score)
		{
			int result;
			if (score.isItMe && Manager.Contains<ProgressManager>())
			{
				element.AddCustomDataElement("Level " + Manager.Get<ProgressManager>().level);
			}
			else if (data.ContainsKey("level") && int.TryParse(data["level"], NumberStyles.Any, CultureInfo.InvariantCulture, out result))
			{
				element.AddCustomDataElement("Level " + result);
			}
		}
	}
}
