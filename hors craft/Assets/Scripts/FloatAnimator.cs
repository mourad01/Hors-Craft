// DecompilerFi decompiler from Assembly-CSharp.dll class: FloatAnimator
using Common.Utils;
using System;
using UnityEngine;

public class FloatAnimator : MonoBehaviour
{
	public bool ended;

	public float progress;

	private Action<float> setter;

	private Action onFinish;

	private float start;

	private float end;

	private float speed;

	private bool useUnscaledTime;

	private EaseType easeType;

	public static GameObject CreateNewAnimation(Action<float> setter, float start, float end, float speed, Action onFinish, bool useUnscaledTime = false, EaseType easeType = EaseType.InOutQuad)
	{
		GameObject gameObject = new GameObject("float_anim");
		gameObject.AddComponent<FloatAnimator>().Init(setter, start, end, speed, onFinish, useUnscaledTime, easeType);
		return gameObject;
	}

	public void Init(Action<float> setter, float start, float end, float speed, Action onFinish, bool useUnscaledTime = false, EaseType easeType = EaseType.InOutQuad)
	{
		this.setter = setter;
		this.start = start;
		this.end = end;
		this.speed = speed;
		this.useUnscaledTime = useUnscaledTime;
		this.easeType = easeType;
		progress = 0f;
		ended = false;
		this.onFinish = onFinish;
	}

	private float GetDeltaTime()
	{
		return (!useUnscaledTime) ? Time.deltaTime : Time.unscaledDeltaTime;
	}

	private void Update()
	{
		if (!ended)
		{
			if (progress >= 1f)
			{
				ended = true;
				setter(end);
				OnFinish();
			}
			if (!(GetDeltaTime() < float.Epsilon))
			{
				float num = GetDeltaTime() / speed;
				setter(Easing.Ease(easeType, start, end, progress));
				progress += num;
			}
		}
	}

	private void OnFinish()
	{
		if (onFinish != null)
		{
			onFinish();
		}
		DestroyUtil.DestroyIfNotNull(base.gameObject);
	}
}
