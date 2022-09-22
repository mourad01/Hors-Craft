// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.TapjoyPlacementsModule
using Common.Model;
using System.Collections.Generic;

namespace Common.Managers
{
	public class TapjoyPlacementsModule : ModelModule
	{
		private string[] allSuffixes = new string[7]
		{
			string.Empty,
			"_nofloor",
			"_floor1",
			"_floor2",
			"_floor3",
			"_floor4",
			"_floor5"
		};

		public TapjoyPlacementsModule(string[] placementsIds = null)
		{
			foreach (string item in (placementsIds != null) ? new List<string>(placementsIds) : new List<string>
			{
				"Common",
				"Rewarded"
			})
			{
				string pl = item;
				Manager.Get<TapjoyManager>().AddOnConnectedCallback(delegate
				{
					Manager.Get<TapjoyManager>().DefinePlacement(pl);
					string[] array = allSuffixes;
					foreach (string str in array)
					{
						Manager.Get<TapjoyManager>().DefinePlacement(pl + str);
					}
				});
			}
		}

		public override void FillModelDescription(ModelDescription descriptions)
		{
		}

		public override void OnModelDownloaded()
		{
		}
	}
}
