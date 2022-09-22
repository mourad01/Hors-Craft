// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.AdsFreeCraftManager
using Common.Managers;
using UnityEngine;

namespace Gameplay
{
	public class AdsFreeCraftManager : Manager
	{
		public override void Init()
		{
			PlayerPrefs.SetInt("debugAdsFree", 1);
			Manager.Get<ModelManager>().modulesContext.isAdsFree = true;
		}
	}
}
