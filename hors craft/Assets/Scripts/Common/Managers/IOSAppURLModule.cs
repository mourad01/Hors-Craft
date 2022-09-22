// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.IOSAppURLModule
using Common.Model;

namespace Common.Managers
{
	public class IOSAppURLModule : ModelModule
	{
		protected string keyiOSAppURL()
		{
			return "ios.app.url";
		}

		public override void FillModelDescription(ModelDescription descriptions)
		{
			descriptions.AddDescription(keyiOSAppURL(), string.Empty);
		}

		public override void OnModelDownloaded()
		{
		}

		public string GetiOSAppURL()
		{
			return base.settings.GetString(keyiOSAppURL());
		}
	}
}
