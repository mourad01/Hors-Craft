// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.RequirementWrapper
using System;

namespace Gameplay
{
	[Serializable]
	public class RequirementWrapper
	{
		public Requirement requirement;

		public float value;

		public string data;

		public bool CheckIfMet(float value, string id = "")
		{
			return requirement.CheckIfMet(value, id, data);
		}

		public bool CheckIfMet(string id = "")
		{
			return CheckIfMet(value, id);
		}
	}
}
