// DecompilerFi decompiler from Assembly-CSharp.dll class: Borodar.FarlandSkies.CloudyCrownPro.PausetButton
using UnityEngine;

namespace Borodar.FarlandSkies.CloudyCrownPro
{
	public class PausetButton : MonoBehaviour
	{
		public void OnClick()
		{
			SkyboxCycleManager get = MonoBehaviourSingleton<SkyboxCycleManager>.get;
			get.Paused = !get.Paused;
		}
	}
}
