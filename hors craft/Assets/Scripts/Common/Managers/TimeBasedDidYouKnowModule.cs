// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.TimeBasedDidYouKnowModule
using Common.Model;
using Common.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Managers
{
	public class TimeBasedDidYouKnowModule : ModelModule
	{
		public delegate void OnModuleDownloaded();

		protected string mainKey = "didyouknow.popup";

		private const string invalidName = "invalidName";

		private int showsCount;

		public OnModuleDownloaded OnModelModuleDownloaded;

		private string keyDidYouKnowFirstTime()
		{
			return $"{mainKey}.firsttime";
		}

		private string keyDidYouKnowTimeDivider()
		{
			return $"{mainKey}.nexttimedivider";
		}

		private string keyDidYouKnowEnabled()
		{
			return $"{mainKey}.enabled";
		}

		private string keyDidYouKnowVideoName(int didYouKnowIndex)
		{
			return string.Format("{1}.{0}.videoname", didYouKnowIndex, mainKey);
		}

		private string keyDidYouKnowPlayImageName(int didYouKnowIndex)
		{
			return string.Format("{1}.{0}.playimagename", didYouKnowIndex, mainKey);
		}

		private string keyDidYouKnowGoToFeatureEnabled(int didYouKnowIndex)
		{
			return string.Format("{1}.{0}.gotofeature.enabled", didYouKnowIndex, mainKey);
		}

		private string keyDidYouKnowGoToState(int didYouKnowIndex)
		{
			return string.Format("{1}.{0}.gotofeature.state", didYouKnowIndex, mainKey);
		}

		public override void FillModelDescription(ModelDescription descriptions)
		{
			descriptions.AddDescription(keyDidYouKnowFirstTime(), 600);
			descriptions.AddDescription(keyDidYouKnowTimeDivider(), 900);
			descriptions.AddDescription(keyDidYouKnowEnabled(), defaultValue: false);
			for (int i = 0; i < 50; i++)
			{
				descriptions.AddDescription(keyDidYouKnowVideoName(i), "invalidName");
				descriptions.AddDescription(keyDidYouKnowPlayImageName(i), "invalidName");
				descriptions.AddDescription(keyDidYouKnowGoToState(i), 0);
				descriptions.AddDescription(keyDidYouKnowGoToFeatureEnabled(i), defaultValue: false);
			}
		}

		public override void OnModelDownloaded()
		{
			if (OnModelModuleDownloaded != null)
			{
				OnModelModuleDownloaded();
			}
		}

		public bool HasToShowDidYouKnow(float passedTime)
		{
			if (PlayerSession.GetSessionNo() <= 1)
			{
				return false;
			}
			if (!IsDidYouKnowEnabled())
			{
				return false;
			}
			int @int = base.settings.GetInt(keyDidYouKnowFirstTime());
			int int2 = base.settings.GetInt(keyDidYouKnowTimeDivider());
			bool flag = false;
			if (showsCount == 0)
			{
				if (passedTime > (float)@int)
				{
					flag = true;
				}
			}
			else
			{
				passedTime -= (float)@int;
				passedTime -= (float)((showsCount - 1) * Mathf.Max(int2, 1));
				if (passedTime > (float)int2)
				{
					flag = true;
				}
			}
			if (flag)
			{
				showsCount++;
			}
			return flag;
		}

		public bool IsGoToFeatureEnabled(int didYouKnowIndex)
		{
			return base.settings.GetBool(keyDidYouKnowGoToFeatureEnabled(didYouKnowIndex));
		}

		public string GetVideoUrl(string videoName)
		{
			string homeURL = Manager.Get<ConnectionInfoManager>().homeURL;
			return $"{homeURL}DidYouKnow/VideoPath?filePath=didYouKnow/{videoName}";
		}

		public string GetPlayImageUrl(string imageName)
		{
			string homeURL = Manager.Get<ConnectionInfoManager>().homeURL;
			return $"{homeURL}DidYouKnow/VideoPath?filePath=didYouKnow/{imageName}";
		}

		public Pair<string, string>[] GetVideoImagePairNames()
		{
			List<Pair<string, string>> list = new List<Pair<string, string>>();
			for (int i = 0; base.settings.HasKey(keyDidYouKnowVideoName(i)) && base.settings.HasKey(keyDidYouKnowPlayImageName(i)); i++)
			{
				string @string = base.settings.GetString(keyDidYouKnowVideoName(i));
				string string2 = base.settings.GetString(keyDidYouKnowPlayImageName(i));
				if (!@string.Equals("invalidName") && !string2.Equals("invalidName"))
				{
					list.Add(new Pair<string, string>(@string, string2));
				}
			}
			return list.ToArray();
		}

		public GoToState GetGoToState(int didYouKnowIndex)
		{
			return (GoToState)base.settings.GetInt(keyDidYouKnowGoToState(didYouKnowIndex));
		}

		private bool IsDidYouKnowEnabled()
		{
			return base.settings.GetBool(keyDidYouKnowEnabled());
		}
	}
}
