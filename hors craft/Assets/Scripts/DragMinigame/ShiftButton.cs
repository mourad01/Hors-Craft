// DecompilerFi decompiler from Assembly-CSharp.dll class: DragMinigame.ShiftButton
using UnityEngine.EventSystems;

namespace DragMinigame
{
	public class ShiftButton : DragGameButton, IPointerDownHandler, IEventSystemHandler
	{
		public void OnPointerDown(PointerEventData eventData)
		{
			if (base.gameManager.CheckIfButtonIsActive(base.gameObject) && base.gameManager.gameState == DragGameManager.GameState.Race)
			{
				base.gameManager.HandleRaceInput();
			}
		}
	}
}
