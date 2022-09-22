// DecompilerFi decompiler from Assembly-CSharp.dll class: DynamicBlocksGridLayoutSquareCellSize
using Common.Managers;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class DynamicBlocksGridLayoutSquareCellSize : MonoBehaviour
{
	public RectTransform rectTransform;

	public GridLayoutGroup gridLayout;

	private float size;

	private void Awake()
	{
		gridLayout.constraintCount = Manager.Get<ModelManager>().blocksUnlocking.GetBlocksViewColumnsCount();
		float width = rectTransform.rect.width;
		float num = gridLayout.constraintCount - 1;
		Vector2 spacing = gridLayout.spacing;
		size = width - num * spacing.x;
		gridLayout.cellSize = new Vector2(size / (float)gridLayout.constraintCount, size / (float)gridLayout.constraintCount);
	}
}
