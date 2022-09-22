// DecompilerFi decompiler from Assembly-CSharp.dll class: CameraMoverWorker
using Common.Managers;
using Uniblocks;
using UnityEngine;

public class CameraMoverWorker : CutSceneWorker
{
	public Transform cameraHolder;

	private Transform oldPlayerCameraParent;

	private Vector3 oldPosition;

	public override void DoJob(WorkerParameter parameters = null)
	{
		UnityEngine.Debug.Log("CameraMoverWorker doing its job.");
		CutScenesManager cutScenesManager = Manager.Get<CutScenesManager>();
		if (cutScenesManager != null && cutScenesManager.CurrentCaller != null)
		{
			base.transform.SetParent(cutScenesManager.CurrentCaller);
		}
		cameraHolder.gameObject.SetActive(value: true);
		ChunkLoader component = cameraHolder.GetComponent<ChunkLoader>();
		TurnPlayerOff();
		if (component != null)
		{
			component.enabled = true;
		}
		base.DoJob(parameters);
	}

	public override void CleanUp()
	{
		base.transform.SetParent(null);
		cameraHolder.gameObject.SetActive(value: false);
		TurnPlayerOn();
		ChunkLoader component = cameraHolder.GetComponent<ChunkLoader>();
		if (component != null)
		{
			component.enabled = false;
		}
	}

	[ContextMenu("Turn player on")]
	private void TurnPlayerOn()
	{
	}

	[ContextMenu("Turn player off")]
	private void TurnPlayerOff()
	{
	}
}
