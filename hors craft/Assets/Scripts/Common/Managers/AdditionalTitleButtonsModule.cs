// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.AdditionalTitleButtonsModule
using Common.Model;

namespace Common.Managers
{
	public class AdditionalTitleButtonsModule : ModelModule
	{
		private string keyButtonEnabled(int buttonNo)
		{
			return "title.additional.button." + buttonNo + ".enabled";
		}

		private string keyButtonLink(int buttonNo)
		{
			return "title.additional.button." + buttonNo + ".link";
		}

		public override void FillModelDescription(ModelDescription descriptions)
		{
			for (int i = 1; i < 10; i++)
			{
				descriptions.AddDescription(keyButtonEnabled(i), defaultValue: false);
				descriptions.AddDescription(keyButtonLink(i), string.Empty);
			}
		}

		public override void OnModelDownloaded()
		{
		}

		public bool IsButtonEnabled(int buttonNo)
		{
			return base.settings.GetBool(keyButtonEnabled(buttonNo));
		}

		public string GetButtonLink(int buttonNo)
		{
			return base.settings.GetString(keyButtonLink(buttonNo));
		}
	}
}
