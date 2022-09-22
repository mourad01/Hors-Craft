// DecompilerFi decompiler from Assembly-CSharp.dll class: Borodar.FarlandSkies.Core.Demo.ForegroundToggle
using UnityEngine;

namespace Borodar.FarlandSkies.Core.Demo
{
	public class ForegroundToggle : MonoBehaviour
	{
		public GameObject[] GameObjects;

		public Renderer[] Renderers;

		public void OnValueChanged(bool value)
		{
			GameObject[] gameObjects = GameObjects;
			foreach (GameObject gameObject in gameObjects)
			{
				gameObject.SetActive(value);
			}
			Renderer[] renderers = Renderers;
			foreach (Renderer renderer in renderers)
			{
				renderer.enabled = value;
			}
		}
	}
}
