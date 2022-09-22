// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.CrosspromoSetupModule
using Common.Crosspromo;
using Common.Model;
using System.Collections.Generic;
using TsgCommon;
using TsgCommon.Crosspromo;
using UnityEngine;

namespace Common.Managers
{
	public class CrosspromoSetupModule : ModelModule
	{
		public const int MAX_POPUPS = 5;

		public override void FillModelDescription(ModelDescription descriptions)
		{
			descriptions.AddDescription(keyTextureName(), "bubble.png");
			descriptions.AddDescription(keyBackgroundTextureName(), string.Empty);
			descriptions.AddDescription(keySerializedData(), string.Empty);
			descriptions.AddDescription(keyIconPercentSize(), 0.17f);
			descriptions.AddDescription(keyMultiCrosspromoAmount(), 0);
		}

		public override void OnModelDownloaded()
		{
			Manager.Get<CrosspromoManager>().DownloadContainerImages();
			TsgCommon.MonoBehaviourSingleton<CrosspromoController>.get.containerSetup = GetContainerSetup();
			TsgCommon.MonoBehaviourSingleton<CrosspromoController>.get.containerSetup = GetContainerSetup();
			TsgCommon.MonoBehaviourSingleton<CrosspromoController>.get.iconPercentSize = GetIconPercentSize();
			TsgCommon.MonoBehaviourSingleton<MultiCrosspromoController>.get.containerSetup = GetContainerSetup();
			TsgCommon.MonoBehaviourSingleton<MultiCrosspromoController>.get.containerSetup = GetContainerSetup();
			TsgCommon.MonoBehaviourSingleton<MultiCrosspromoController>.get.iconPercentSize = GetIconPercentSize();
		}

		private string keyTextureName()
		{
			return "crosspromo.container.textureName";
		}

		private string keyBackgroundTextureName()
		{
			return "crosspromo.container.backgroundTextureName";
		}

		private string keySerializedData()
		{
			return "crosspromo.container.serializedData";
		}

		private string keyIconPercentSize()
		{
			return "crosspromo.icon.percent.size";
		}

		private string keyMultiCrosspromoAmount()
		{
			return "multi.crosspromo.amount";
		}

		private string keyMultiCrosspromoCampainTag(int index)
		{
			return "multi.crosspromo.campain.tag." + index;
		}

		public string GetSerializedData()
		{
			return base.settings.GetString(keySerializedData());
		}

		public string GetTextureName()
		{
			return base.settings.GetString(keyTextureName());
		}

		public string GetBackgroundTextureName()
		{
			return base.settings.GetString(keyBackgroundTextureName());
		}

		public int GetMultiCrosspromoAmount()
		{
			return base.settings.GetInt(keyMultiCrosspromoAmount());
		}

		public string GetMultiCrosspromoCampainTag(int index)
		{
			return base.settings.GetString(keyMultiCrosspromoCampainTag(index));
		}

		public bool HasTag(int index)
		{
			return ModelSettingsHelper.HasField(base.settings, keyMultiCrosspromoCampainTag(index));
		}

		public List<string> GetMultiCrosspromoCampainTags()
		{
			List<string> list = new List<string>();
			for (int i = 0; HasTag(i); i++)
			{
				list.Add(GetMultiCrosspromoCampainTag(i));
			}
			return list;
		}

		public string GetTextureUrl()
		{
			string homeURL = Manager.Get<ConnectionInfoManager>().homeURL;
			return $"{homeURL}CrosspromoContainer/ImagePath?filePath=Crosspromo/Container/{GetTextureName()}";
		}

		public string GetBackgroundTextureUrl()
		{
			string homeURL = Manager.Get<ConnectionInfoManager>().homeURL;
			return $"{homeURL}CrosspromoContainer/ImagePath?filePath=Crosspromo/Background/{GetBackgroundTextureName()}";
		}

		public float GetIconPercentSize()
		{
			return base.settings.GetFloat(keyIconPercentSize(), 0.17f);
		}

		public CrosspromoContainerSetup GetContainerSetup()
		{
			CrosspromoContainerSetup result = new CrosspromoContainerSetup();
			CrosspromoContainerSetup crosspromoContainerSetup = JsonUtility.FromJson<CrosspromoContainerSetup>(GetSerializedData());
			if (crosspromoContainerSetup != null)
			{
				result = crosspromoContainerSetup;
			}
			return result;
		}
	}
}
