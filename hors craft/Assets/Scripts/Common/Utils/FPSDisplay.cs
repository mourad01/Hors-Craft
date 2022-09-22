// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Utils.FPSDisplay
using UnityEngine;
using UnityEngine.UI;

namespace Common.Utils
{
	public class FPSDisplay : MonoBehaviour
	{
		private float deltaTime;

		private void Update()
		{
			deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
			float num = deltaTime * 1000f;
			float num2 = 1f / deltaTime;
			string text = $"{num:0.0} ms ({num2:0.} fps)";
			if (Time.timeScale > 0f)
			{
				GetComponent<Text>().text = text;
			}
			else
			{
				GetComponent<Text>().text = string.Empty;
			}
		}
	}
}
