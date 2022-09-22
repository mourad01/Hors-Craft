// DecompilerFi decompiler from Assembly-CSharp.dll class: WalkWayPartEndless
using System.Collections;
using UnityEngine;

public class WalkWayPartEndless : WalkWayPart
{
	public float lengthOfObject = 1f;

	public float speed = 1f;

	public Transform[] procObj = new Transform[0];

	public bool move;

	protected Vector3 tempPos = default(Vector3);

	private int nrOfParts = 5;

	public override void Init()
	{
		base.Init();
		procObj = new Transform[nrOfParts];
		childObject.gameObject.SetActive(value: true);
		for (int i = 0; i < nrOfParts; i++)
		{
			procObj[i] = Object.Instantiate(childObject).transform;
			procObj[i].SetParent(base.transform, worldPositionStays: false);
			Vector3 localPosition = procObj[i].localPosition;
			localPosition.z += lengthOfObject * (float)i;
			localPosition.z -= lengthOfObject * (float)nrOfParts / 2f;
			procObj[i].localPosition = localPosition;
		}
		childObject.gameObject.SetActive(value: false);
	}

	public override void Stop()
	{
		base.Stop();
		move = false;
	}

	public override void Move()
	{
		base.Move();
		move = true;
	}

	public override void Pause(float time)
	{
		base.Pause(time);
		move = false;
		Invoke("Move", time);
		StartCoroutine(WaitForRealTime(time));
	}

	public IEnumerator WaitForRealTime(float delay)
	{
		float pauseEndTime = Time.realtimeSinceStartup + delay;
		while (Time.realtimeSinceStartup < pauseEndTime)
		{
			yield return 0;
		}
		Move();
	}

	private void Update()
	{
		for (int i = 0; i < procObj.Length; i++)
		{
			tempPos = procObj[i].localPosition;
			if (move)
			{
				tempPos.z -= speed * Time.unscaledDeltaTime;
			}
			if (tempPos.z < lengthOfObject * (float)nrOfParts / -2f)
			{
				tempPos.z = lengthOfObject * (float)nrOfParts / 2f;
			}
			procObj[i].localPosition = tempPos;
		}
	}

	public override void CleanUp()
	{
		base.CleanUp();
		for (int i = 0; i < procObj.Length; i++)
		{
			UnityEngine.Object.Destroy(procObj[i].gameObject);
		}
		procObj = new Transform[0];
		CancelInvoke();
		Stop();
		childObject.gameObject.SetActive(value: true);
	}
}
