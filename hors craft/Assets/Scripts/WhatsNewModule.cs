// DecompilerFi decompiler from Assembly-CSharp.dll class: WhatsNewModule
using Common.Managers;
using Common.Model;
using UnityEngine;

public class WhatsNewModule : ModelModule
{
	private string keyNewFeaturesCount()
	{
		return "whatsnew.features." + Application.version + ".count";
	}

	private string keyNewFeatureIconType(int id)
	{
		return "whatsnew.features." + Application.version + ".icontype." + id;
	}

	public override void FillModelDescription(ModelDescription descriptions)
	{
		descriptions.AddDescription(keyNewFeaturesCount(), 0);
	}

	public override void OnModelDownloaded()
	{
	}

	public int GetNewFeaturesCount()
	{
		return base.settings.GetInt(keyNewFeaturesCount(), 0);
	}

	public string GetNewFeatureIconType(int id)
	{
		return base.settings.GetString(keyNewFeatureIconType(id));
	}
}
