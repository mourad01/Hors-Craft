// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.LoadMainSceneAdditively
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay
{
	public class LoadMainSceneAdditively : MonoBehaviour
	{
		public delegate void OnLoaded();

		public OnLoaded onLoaded;

		private AsyncOperation loadOperation;

		private void Start()
		{
			loadOperation = SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Additive);
		}

		private void Update()
		{
			if (loadOperation.isDone && onLoaded != null)
			{
				onLoaded();
			}
		}
	}
}
