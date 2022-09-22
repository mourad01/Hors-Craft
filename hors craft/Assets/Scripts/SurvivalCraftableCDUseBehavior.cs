// DecompilerFi decompiler from Assembly-CSharp.dll class: SurvivalCraftableCDUseBehavior
using Common.Managers;
using System.Collections.Generic;
using UnityEngine;

public class SurvivalCraftableCDUseBehavior : AbstractSurvivalCraftableUse
{
	public string translationFailedTostKey = ".spawn.colldown";

	public string failedDefaultText = "You must wait {0}sec to use this ";

	public float cooldown;

	private static Dictionary<int, float> cooldownTimers;

	public override bool CanBeUse(int id)
	{
		if (cooldownTimers == null)
		{
			cooldownTimers = new Dictionary<int, float>();
		}
		if (cooldownTimers.ContainsKey(id) && cooldownTimers[id] > Time.time)
		{
			float num = cooldownTimers[id] - Time.time;
			string text = Manager.Get<TranslationsManager>().GetText(GetComponent<SurvivalCraftable>().Name + translationFailedTostKey, failedDefaultText + GetComponent<SurvivalCraftable>().Name);
			Manager.Get<ToastManager>().ShowToast(string.Format(text, num.ToString("f0")));
			return false;
		}
		return true;
	}

	public override void OnFailed(int id)
	{
	}

	public override void OnSuccess(int id, GameObject usedObject)
	{
		if (cooldownTimers.ContainsKey(id))
		{
			cooldownTimers[id] = Time.time + cooldown;
		}
		else
		{
			cooldownTimers.Add(id, Time.time + cooldown);
		}
	}

	public override void PrepareToUse(int id)
	{
	}
}
