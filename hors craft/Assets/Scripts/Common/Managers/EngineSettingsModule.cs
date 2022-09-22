// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.EngineSettingsModule
using Common.Model;

namespace Common.Managers
{
	public class EngineSettingsModule : ModelModule
	{
		protected string keyEnableEngineSettings()
		{
			return "engine.enable";
		}

		protected string keyEngineMinTargetFPS()
		{
			return "engine.mintargetfps";
		}

		protected string keyEngineDefaultViewDistance()
		{
			return "engine.defaultviewdistance";
		}

		protected string keyEngineDefaultFov()
		{
			return "engine.default.fov";
		}

		public override void FillModelDescription(ModelDescription descriptions)
		{
			descriptions.AddDescription(keyEnableEngineSettings(), 1);
			descriptions.AddDescription(keyEngineMinTargetFPS(), 23);
			descriptions.AddDescription(keyEngineDefaultViewDistance(), 90);
			descriptions.AddDescription(keyEngineDefaultFov(), 70);
		}

		public override void OnModelDownloaded()
		{
		}

		public bool GetEnableEngineSettings()
		{
			return base.settings.GetInt(keyEnableEngineSettings(), 0) == 1;
		}

		public int GetEngineMinTargetFPS()
		{
			return base.settings.GetInt(keyEngineMinTargetFPS(), 23);
		}

		public int GetEngineDefaultViewDistance()
		{
			return base.settings.GetInt(keyEngineDefaultViewDistance(), 90);
		}

		public int GetEngineDefaultFov()
		{
			return base.settings.GetInt(keyEngineDefaultFov(), 70);
		}
	}
}
