// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.FBFanpageModule
using Common.Model;

namespace Common.Managers
{
	public class FBFanpageModule : ModelModule
	{
		protected string keyFBURL()
		{
			return "fb.link.url";
		}

		protected string keyFBEnabled()
		{
			return "fb.link.enabled";
		}

		protected string keyFBTitleURL()
		{
			return "fb.link.title.url";
		}

		protected string keyFBTitleEnabled()
		{
			return "fb.link.title.enabled";
		}

		public override void FillModelDescription(ModelDescription descriptions)
		{
			descriptions.AddDescription(keyFBURL(), "https://www.facebook.com/tinydragonadventuregames/");
			descriptions.AddDescription(keyFBEnabled(), defaultValue: true);
			descriptions.AddDescription(keyFBTitleURL(), "https://www.facebook.com/tinydragonadventuregames/");
			descriptions.AddDescription(keyFBTitleEnabled(), defaultValue: false);
		}

		public override void OnModelDownloaded()
		{
		}

		public string GetFBURL()
		{
			return base.settings.GetString(keyFBURL());
		}

		public bool IsFBLinkEnabled()
		{
			return base.settings.GetBool(keyFBEnabled());
		}

		public string GetFBTitleURL()
		{
			return base.settings.GetString(keyFBTitleURL());
		}

		public bool IsFBLinkTitleEnabled()
		{
			return base.settings.GetBool(keyFBTitleEnabled());
		}
	}
}
