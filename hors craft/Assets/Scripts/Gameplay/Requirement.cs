// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.Requirement
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Gameplay
{
	public abstract class Requirement : ScriptableObject
	{
		public virtual bool customEditor
		{
			[CompilerGenerated]
			get
			{
				return false;
			}
		}

		public virtual bool CheckIfMet(float requiredAmount = 0f)
		{
			return CheckIfMet(requiredAmount, string.Empty);
		}

		public abstract bool CheckIfMet(float requiredAmount = 0f, string id = "");

		public virtual bool CheckIfMet(float requiredAmount = 0f, string id = "", string data = "")
		{
			return CheckIfMet(requiredAmount, id);
		}
	}
}
