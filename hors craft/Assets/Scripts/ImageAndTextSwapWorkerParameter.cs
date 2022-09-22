// DecompilerFi decompiler from Assembly-CSharp.dll class: ImageAndTextSwapWorkerParameter
using UnityEngine;

public class ImageAndTextSwapWorkerParameter : WorkerParameter
{
	public Sprite newSprite;

	public ImageAndTextSwapWorkerParameter(string text, Sprite newSprite)
		: base(string.Empty)
	{
		this.newSprite = newSprite;
		base.text = text;
	}
}
