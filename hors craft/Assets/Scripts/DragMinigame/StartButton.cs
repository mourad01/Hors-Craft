// DecompilerFi decompiler from Assembly-CSharp.dll class: DragMinigame.StartButton
using UnityEngine.EventSystems;

namespace DragMinigame
{
	public class StartButton : DragGameButton, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
	{
		public void OnPointerDown(PointerEventData eventData)
		{
			if (base.gameManager.CheckIfButtonIsActive(base.gameObject) && base.gameManager.gameState == DragGameManager.GameState.Start)
			{
				base.gameManager.HandleStartInput(gasDown: true);
			}
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (base.gameManager.CheckIfButtonIsActive(base.gameObject) && base.gameManager.gameState == DragGameManager.GameState.Start)
			{
				base.gameManager.HandleStartInput(gasDown: false);
			}
		}
	}
}
