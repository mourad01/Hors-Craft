// DecompilerFi decompiler from Assembly-CSharp.dll class: EmptyGraphic
using UnityEngine.UI;

public class EmptyGraphic : Graphic
{
	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();
	}
}
