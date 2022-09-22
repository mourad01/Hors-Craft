// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.TranslationSettingsModule
using Common.Model;

namespace Common.Managers
{
	public class TranslationSettingsModule : ModelModule
	{
		protected string keyAvailableLanguages()
		{
			return "available.languages";
		}

		public override void FillModelDescription(ModelDescription descriptions)
		{
			descriptions.AddDescription(keyAvailableLanguages(), "english;polish");
		}

		public override void OnModelDownloaded()
		{
		}

		public string[] GetAvailableLanguages()
		{
			return base.settings.GetString(keyAvailableLanguages()).Split(';');
		}
	}
}
