// DecompilerFi decompiler from Assembly-CSharp.dll class: UIPulsingEffect
using System.Collections;
using UnityEngine;

public class UIPulsingEffect : MonoBehaviour
{
	[SerializeField]
	private RectTransform _Target;

	[Header("Buling")]
	[SerializeField]
	private AnimationCurve _BulkCurve;

	[SerializeField]
	private float _BulkDuration = 1f;

	[SerializeField]
	private float _BulkScale = 1.2f;

	[Header("Compressing")]
	[SerializeField]
	private AnimationCurve _CompressingCurve;

	[SerializeField]
	private float _CompressingDuration = 1f;

	[SerializeField]
	private float _CompressingScale = 0.8f;

	[Header("Options")]
	[SerializeField]
	private bool _StartOnEnalbe = true;

	[SerializeField]
	private float _StartTime = 1f;

	[SerializeField]
	private float _PauseTime = 1f;

	[SerializeField]
	private float _BetweenPause = 1f;

	[SerializeField]
	private bool _DefaultPauseSize = true;

	[SerializeField]
	private float _DefaultTime = 0.3f;

	private Vector3 _StartScale;

	private AnimationCurve _DefaultCurve;

	private void Awake()
	{
		if (_Target == null)
		{
			_Target = GetComponent<RectTransform>();
		}
		_DefaultCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));
		_StartScale = _Target.localScale;
	}

	private void OnEnable()
	{
		if (_StartOnEnalbe)
		{
			StartEffect();
		}
	}

	private void OnDisable()
	{
		StopEffect();
	}

	public void StartEffect()
	{
		StopAllCoroutines();
		if (base.gameObject.activeInHierarchy)
		{
			StartCoroutine(StartAnimation());
		}
	}

	public void StopEffect()
	{
		StopAllCoroutines();
		_Target.localScale = _StartScale;
	}

	private IEnumerator StartAnimation()
	{
		yield return new WaitForSecondsRealtime(_StartTime);
		Vector3 bulkedScale = _StartScale * _BulkScale;
		Vector3 compressedScale = _StartScale * _CompressingScale;
		Vector3 lastScale = _StartScale;
		float num = 1f / _BulkDuration;
		bool bulking = true;
		while (true)
		{
			float curveTime2 = 0f;
			float mul2 = (!bulking) ? (1f / _CompressingDuration) : (1f / _BulkDuration);
			Vector3 targetScale = (!bulking) ? compressedScale : bulkedScale;
			while (curveTime2 <= 1f)
			{
				float lerpValue = (!bulking) ? _CompressingCurve.Evaluate(curveTime2) : _BulkCurve.Evaluate(curveTime2);
				curveTime2 += Time.unscaledDeltaTime * mul2;
				_Target.localScale = Vector3.Lerp(lastScale, targetScale, lerpValue);
				yield return new WaitForEndOfFrame();
			}
			bulking = !bulking;
			lastScale = targetScale;
			if (!bulking)
			{
				yield return new WaitForSecondsRealtime(_BetweenPause);
				continue;
			}
			if (_DefaultPauseSize)
			{
				curveTime2 = 0f;
				mul2 = 1f / _DefaultTime;
				while (curveTime2 <= 1f)
				{
					float lerpValue = _DefaultCurve.Evaluate(curveTime2);
					curveTime2 += Time.unscaledDeltaTime * mul2;
					_Target.localScale = Vector3.Lerp(lastScale, _StartScale, lerpValue);
					yield return new WaitForEndOfFrame();
				}
				lastScale = _StartScale;
			}
			yield return new WaitForSecondsRealtime(_PauseTime);
		}
	}
}
