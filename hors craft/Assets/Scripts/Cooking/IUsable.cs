// DecompilerFi decompiler from Assembly-CSharp.dll class: Cooking.IUsable
using UnityEngine;

namespace Cooking
{
	public interface IUsable
	{
		string GetKey();

		bool Unlocked();

		int GetPrice();

		Sprite GetSprite();
	}
}
