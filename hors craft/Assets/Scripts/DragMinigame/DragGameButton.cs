// DecompilerFi decompiler from Assembly-CSharp.dll class: DragMinigame.DragGameButton
using UnityEngine;

namespace DragMinigame
{
	public class DragGameButton : MonoBehaviour
	{
		private DragGameManager _gameManager;

		protected DragGameManager gameManager
		{
			get
			{
				if (_gameManager == null)
				{
					_gameManager = UnityEngine.Object.FindObjectOfType<DragGameManager>();
				}
				return _gameManager;
			}
		}
	}
}
