// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.ExampleInventory
using UnityEngine;

namespace Uniblocks
{
	public class ExampleInventory : MonoBehaviour
	{
		private const string prefsKey = "last.held.block";

		private static ushort _block;

		public static ushort HeldBlock
		{
			get
			{
				return _block;
			}
			set
			{
				_block = value;
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFact(Fact.BLOCK_HELD);
				MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.BLOCK_HELD, new HeldBlockContext
				{
					block = value
				});
				PlayerPrefs.SetInt("last.held.block", value);
			}
		}

		private void Start()
		{
			HeldBlock = (ushort)PlayerPrefs.GetInt("last.held.block", Engine.usefulIDs.dirtBlockID);
		}
	}
}
