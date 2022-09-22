// DecompilerFi decompiler from Assembly-CSharp.dll class: CutSceneWorker
using UnityEngine;

public class CutSceneWorker : MonoBehaviour
{
	public TranslateText translationText;

	public virtual void Init()
	{
	}

	public virtual void DoJob(WorkerParameter parameters = null)
	{
		UnityEngine.Debug.Log("CutSceneWorker doing its job.");
		if (translationText != null && parameters != null && !string.IsNullOrEmpty(parameters.text))
		{
			translationText.translationKey = parameters.text;
			translationText.ForceRefresh();
		}
	}

	public virtual void CleanUp()
	{
	}
}
