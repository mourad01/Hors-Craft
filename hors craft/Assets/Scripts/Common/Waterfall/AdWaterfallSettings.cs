// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Waterfall.AdWaterfallSettings
using Common.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

//namespace Common.Waterfall
//{
	/*public class AdWaterfallSettings
	{
		private ModelDescription description;

		private Settings settings;

		private AdWaterfallType[] allWaterfallTypes;

		private AdWaterfallStepDefinition[] allWaterfallStepDefinitions;

		private const int MODEL_VERSION = 1;

		public AdWaterfallSettings(AdWaterfallStepDefinition[] stepsDefinitions)
		{
			allWaterfallTypes = (Enum.GetValues(typeof(AdWaterfallType)) as AdWaterfallType[]);
			allWaterfallStepDefinitions = stepsDefinitions;
			description = new ModelDescription(1);
			CreateDefaults();
			settings = new Settings();
			settings.AppendSettingsFrom(description.GetDefaultSettings());
		}

		public bool IsVerbose()
		{
			return settings.GetBool(keyVerbose());
		}

		public bool IsRewardedWithInterstitial()
		{
			return settings.GetBool(keyRewardedWithInterstitial());
		}

		private static string keyVerbose()
		{
			return "waterfall.verbose";
		}

		private static string keyUsesNewWaterfall()
		{
			return "waterfall.uses.new";
		}

		private static string keyWaterfallPriority(AdWaterfallType type, AdWaterfallStepDefinition def)
		{
			return "waterfall." + type.ToString().ToLower() + "." + def.GetStepSettingsIdentifier();
		}

		private static string keyWaterfallListOrder(AdWaterfallType type, int index)
		{
			return "waterfall." + type.ToString().ToLower() + ".config" + index + ".order";
		}

		private static string keyWaterfallListWeight(AdWaterfallType type, int index)
		{
			return "waterfall." + type.ToString().ToLower() + ".config" + index + ".weight";
		}

		private static string keyWaterfalImpression(AdWaterfallType type, int index)
		{
			return "waterfall." + type.ToString().ToLower() + ".impression." + index;
		}

		private static string keyRewardedWithInterstitial()
		{
			return "common.ad.rewarded.with.interstitial";
		}

		private void CreateDefaults()
		{
			description.AddDescription(keyVerbose(), defaultValue: false);
			description.AddDescription(keyUsesNewWaterfall(), defaultValue: false);
			description.AddDescription(keyRewardedWithInterstitial(), defaultValue: false);
			for (int i = 1; i <= 25; i++)
			{
				AdWaterfallType[] array = allWaterfallTypes;
				foreach (AdWaterfallType type in array)
				{
					description.AddDescription(keyWaterfallListOrder(type, i), (i != 1) ? string.Empty : "heyzap, tapjoy_without_floor");
					description.AddDescription(keyWaterfallListWeight(type, i), 1f);
					description.AddDescription(keyWaterfalImpression(type, i), string.Empty);
				}
			}
			AdWaterfallType[] array2 = allWaterfallTypes;
			foreach (AdWaterfallType type2 in array2)
			{
				AdWaterfallStepDefinition[] array3 = allWaterfallStepDefinitions;
				foreach (AdWaterfallStepDefinition adWaterfallStepDefinition in array3)
				{
					int defaultValue = -1;
					string stepSettingsIdentifier = adWaterfallStepDefinition.GetStepSettingsIdentifier();
					if (!stepSettingsIdentifier.Contains("without_floor"))
					{
						if (stepSettingsIdentifier.Contains("tapjoy") && !stepSettingsIdentifier.Contains("floor_2") && !stepSettingsIdentifier.Contains("floor_3") && !stepSettingsIdentifier.Contains("floor_4"))
						{
							defaultValue = 0;
						}
						else if (stepSettingsIdentifier.Contains("heyzap"))
						{
							defaultValue = 1;
						}
						else if (stepSettingsIdentifier.Contains("admob"))
						{
							defaultValue = 2;
						}
						else if (stepSettingsIdentifier.Contains("addapptr"))
						{
							defaultValue = 10;
						}
					}
					description.AddDescription(keyWaterfallPriority(type2, adWaterfallStepDefinition), defaultValue);
				}
			}
		}

		public void ReadSettingsFrom(object jsonObject)
		{
			Settings addition = JSONToSettings.ParseTwoStepsSettings(jsonObject, description);
			settings.AppendSettingsFrom(addition);
		}

		public void ReadSettingsFrom(Settings otherSettings)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			foreach (KeyValuePair<string, string> @string in otherSettings.GetStrings())
			{
				dictionary.AddOrReplace(@string.Key, @string.Value);
			}
			foreach (KeyValuePair<string, int> @int in otherSettings.GetInts())
			{
				dictionary.AddOrReplace(@int.Key, @int.Value);
			}
			foreach (KeyValuePair<string, float> @float in otherSettings.GetFloats())
			{
				dictionary.AddOrReplace(@float.Key, @float.Value);
			}
			foreach (KeyValuePair<string, bool> @bool in otherSettings.GetBools())
			{
				dictionary.AddOrReplace(@bool.Key, @bool.Value);
			}
			Settings addition = JSONToSettings.ParseTwoStepsSettings(dictionary, description);
			settings.AppendSettingsFrom(addition);
		}

		public void Execute()
		{
			AdWaterfall.get.ClearSteps();
			if (settings.GetBool(keyUsesNewWaterfall()))
			{
				ConstructNewWaterfallSteps();
			}
			else
			{
				ConstructOldWaterfallSteps();
			}
			ContructWaterfallStepsPerAdsWatched();
		}

		private void ConstructNewWaterfallSteps()
		{
			for (int i = 1; i <= 25; i++)
			{
				AdWaterfallType[] array = allWaterfallTypes;
				foreach (AdWaterfallType type in array)
				{
					string @string = settings.GetString(keyWaterfallListOrder(type, i));
					if (!string.IsNullOrEmpty(@string))
					{
						AdWaterfallStepList adWaterfallStepList = new AdWaterfallStepList();
						adWaterfallStepList.steps = ConstructStepInfosFromOrderString(type, @string);
						adWaterfallStepList.weight = settings.GetFloat(keyWaterfallListWeight(type, i));
						AdWaterfall.get.RegisterSteps(type, adWaterfallStepList);
					}
				}
			}
		}

		private void ContructWaterfallStepsPerAdsWatched()
		{
			AdWaterfallType[] array = Enum.GetValues(typeof(AdWaterfallType)) as AdWaterfallType[];
			for (int i = 0; i < array.Length; i++)
			{
				AdWaterfallType type = array[i];
				Dictionary<string, string> stringsMatching = settings.GetStringsMatching("waterfall." + type.ToString().ToLower() + ".impression.");
				stringsMatching = (from p in stringsMatching
					orderby p.Key
					select p).ToDictionary((KeyValuePair<string, string> p) => p.Key, (KeyValuePair<string, string> p) => p.Value);
				foreach (KeyValuePair<string, string> item in stringsMatching)
				{
					if (item.Value != null && item.Value != string.Empty)
					{
						AdWaterfallStepList adWaterfallStepList = new AdWaterfallStepList();
						adWaterfallStepList.steps = ConstructStepInfosFromOrderString(type, item.Value);
						adWaterfallStepList.weight = 1f;
						AdWaterfallStepList list = adWaterfallStepList;
						int num = item.Key.LastIndexOf('.');
						string text = item.Key.Substring(num + 1);
						if (int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out int result))
						{
							AdWaterfall.AddWaterfallStepsPerAdsWatched(type, result, list);
						}
						else
						{
							UnityEngine.Debug.LogError("Couldn't parse impression value: " + text);
						}
					}
				}
			}
		}

		private List<AdWaterfallStepInfo> ConstructStepInfosFromOrderString(AdWaterfallType type, string orderString)
		{
			List<AdWaterfallStepInfo> list = new List<AdWaterfallStepInfo>();
			int num = 0;
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			string[] array = orderString.Replace("\r", "\n").Replace("\n\n", "\n").Split(new char[4]
			{
				',',
				' ',
				'\n',
				'\t'
			}, StringSplitOptions.RemoveEmptyEntries);
			foreach (string text in array)
			{
				AdWaterfallStepDefinition adWaterfallStepDefinition = null;
				AdWaterfallStepDefinition[] array2 = allWaterfallStepDefinitions;
				foreach (AdWaterfallStepDefinition adWaterfallStepDefinition2 in array2)
				{
					if (adWaterfallStepDefinition2.GetStepSettingsIdentifier() == text)
					{
						adWaterfallStepDefinition = adWaterfallStepDefinition2;
						break;
					}
				}
				if (adWaterfallStepDefinition != null)
				{
					string serverSideID = adWaterfallStepDefinition.GetServerSideID();
					list.Add(new AdWaterfallStepInfo
					{
						stepId = serverSideID,
						type = type,
						order = num,
						instance = adWaterfallStepDefinition.GetAdWaterfallStep()
					});
					num++;
					stringBuilder.Append(serverSideID);
					stringBuilder.Append(", ");
				}
				else
				{
					stringBuilder2.Append(text);
					stringBuilder2.Append(", ");
				}
			}
			if (IsVerbose())
			{
				UnityEngine.Debug.Log("Enabled new waterfall type: " + type + " steps: " + stringBuilder.ToString());
				if (stringBuilder2.Length > 0)
				{
					UnityEngine.Debug.LogWarning("Couldn't find some steps (defined serverside), waterfall type: " + type + " steps: " + stringBuilder2.ToString() + ". This could mean GD thinks he can use Ad Network that is not included in the build!");
				}
			}
			return list;
		}

		private void ConstructOldWaterfallSteps()
		{
			AdWaterfallType[] array = allWaterfallTypes;
			foreach (AdWaterfallType adWaterfallType in array)
			{
				AdWaterfallStepList adWaterfallStepList = new AdWaterfallStepList();
				AdWaterfallStepDefinition[] array2 = allWaterfallStepDefinitions;
				foreach (AdWaterfallStepDefinition adWaterfallStepDefinition in array2)
				{
					int @int = settings.GetInt(keyWaterfallPriority(adWaterfallType, adWaterfallStepDefinition));
					if (@int >= 0)
					{
						string serverSideID = adWaterfallStepDefinition.GetServerSideID();
						IAdWaterfallStep adWaterfallStep = adWaterfallStepDefinition.GetAdWaterfallStep();
						if (adWaterfallStep != null)
						{
							adWaterfallStepList.steps.Add(new AdWaterfallStepInfo
							{
								stepId = serverSideID,
								type = adWaterfallType,
								order = @int,
								instance = adWaterfallStep
							});
						}
					}
				}
				adWaterfallStepList.steps.Sort((AdWaterfallStepInfo a, AdWaterfallStepInfo b) => a.order - b.order);
				StringBuilder stringBuilder = new StringBuilder();
				foreach (AdWaterfallStepInfo step in adWaterfallStepList.steps)
				{
					stringBuilder.Append(step.stepId);
					stringBuilder.Append(" [");
					stringBuilder.Append(step.order);
					stringBuilder.Append(" ],");
				}
				AdWaterfall.get.RegisterSteps(adWaterfallType, adWaterfallStepList);
				if (IsVerbose())
				{
					UnityEngine.Debug.Log("Enabled old waterfall type: " + adWaterfallType + " steps: " + stringBuilder.ToString());
				}
			}
		}

		private int FindInsertIndexForOrder(List<AdWaterfallStepInfo> list, int order)
		{
			if (list.Count == 0)
			{
				return 0;
			}
			for (int i = 0; i < list.Count; i++)
			{
				if (order < list[i].order)
				{
					return i;
				}
			}
			return list.Count;
		}
	}
}
	*/