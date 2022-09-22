// DecompilerFi decompiler from Assembly-CSharp.dll class: WorldFilter
using Common.Managers;
using System;

[Serializable]
public class WorldFilter
{
	public string id;

	public string translationKey;

	public string tag;

	public int enabled;

	public string text => Manager.Get<TranslationsManager>().GetText(translationKey, id);

	public bool isEnabled => (enabled != 0) ? true : false;

	public WorldFilter()
	{
	}

	public WorldFilter(string id, string translationKey, bool enabled)
	{
		this.id = id;
		this.translationKey = translationKey;
		this.enabled = (enabled ? 1 : 0);
	}
}
