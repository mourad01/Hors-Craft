// DecompilerFi decompiler from Assembly-CSharp.dll class: ExperienceModule
using Common.Managers;
using Common.Model;
using System;
using System.Collections;

public class ExperienceModule : ModelModule
{
	private string keyExperiencePerCount(string countable)
	{
		return "player.experience.per." + countable;
	}

	public override void FillModelDescription(ModelDescription descriptions)
	{
		IEnumerator enumerator = Enum.GetValues(typeof(ProgressCounter.Countables)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				descriptions.AddDescription(keyExperiencePerCount(((ProgressCounter.Countables)enumerator.Current).ToString().ToLower()), -1);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public override void OnModelDownloaded()
	{
	}

	public int GetExperiencePerCount(string countable)
	{
		return base.settings.GetInt(keyExperiencePerCount(countable), -1);
	}
}
