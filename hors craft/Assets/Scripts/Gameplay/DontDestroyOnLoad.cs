// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.DontDestroyOnLoad
using UnityEngine;

namespace Gameplay
{
	public class DontDestroyOnLoad : MonoBehaviour
	{
		private void Awake()
		{
			Object.DontDestroyOnLoad(base.gameObject);
		}
	}
}
