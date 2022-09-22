// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Gameplay.Reward
using Common.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Gameplay
{
	public abstract class Reward : ScriptableObject
	{
		public string key;

		public Sprite baseSprite;

		public int amount = 1;

		public abstract void ClaimReward();

		public abstract List<Sprite> GetSprites();

		public virtual string GetName(string key)
		{
			return Manager.Get<TranslationsManager>().GetText("reward." + key, key.ToLower());
		}
	}
}
