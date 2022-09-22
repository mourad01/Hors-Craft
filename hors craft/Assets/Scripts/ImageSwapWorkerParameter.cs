// DecompilerFi decompiler from Assembly-CSharp.dll class: ImageSwapWorkerParameter
using UnityEngine;

public class ImageSwapWorkerParameter : WorkerParameter
{
	public Sprite newSprite;

	public ImageSwapWorkerParameter(Sprite newSprite)
		: base(string.Empty)
	{
		this.newSprite = newSprite;
	}
}
