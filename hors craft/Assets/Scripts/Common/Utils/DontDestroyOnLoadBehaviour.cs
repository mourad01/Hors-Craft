// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Utils.DontDestroyOnLoadBehaviour
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Common.Utils
{
	public class DontDestroyOnLoadBehaviour : MonoBehaviour
	{
		private void Awake()
		{
			Object.DontDestroyOnLoad(base.gameObject);
			Object.DontDestroyOnLoad(base.transform.root.gameObject);
		}

		private void DestroyOnMenuScreen(Scene oldScene, Scene newScene)
		{
		}
	}
}
