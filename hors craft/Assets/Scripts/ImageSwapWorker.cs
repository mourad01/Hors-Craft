// DecompilerFi decompiler from Assembly-CSharp.dll class: ImageSwapWorker
using UnityEngine;
using UnityEngine.UI;

public class ImageSwapWorker : CutSceneWorker
{
	public Image imageToSwap;

	public override void DoJob(WorkerParameter parameters = null)
	{
		if (!(imageToSwap == null))
		{
			if (parameters is ImageSwapWorkerParameter)
			{
				imageToSwap.enabled = true;
				imageToSwap.sprite = (parameters as ImageSwapWorkerParameter).newSprite;
			}
			else
			{
				UnityEngine.Debug.LogError("No parameters");
			}
			base.DoJob(parameters);
		}
	}

	public override void CleanUp()
	{
		if (!(imageToSwap == null))
		{
			imageToSwap.enabled = false;
		}
	}
}
