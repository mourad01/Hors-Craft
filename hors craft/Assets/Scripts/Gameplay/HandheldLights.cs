// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.HandheldLights
using UnityEngine;

namespace Gameplay
{
	public class HandheldLights : MonoBehaviour
	{
		public GameObject torch;

		public void TorchOn()
		{
			if (torch.activeSelf)
			{
				torch.SetActive(value: false);
			}
			else
			{
				torch.SetActive(value: true);
			}
		}
	}
}
