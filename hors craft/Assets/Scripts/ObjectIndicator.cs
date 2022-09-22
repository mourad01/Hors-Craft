// DecompilerFi decompiler from Assembly-CSharp.dll class: ObjectIndicator
using System.Collections.Generic;
using UnityEngine;

public class ObjectIndicator : MonoBehaviour
{
	[SerializeField]
	private GameObject indicatorPrefab;

	[SerializeField]
	private float position = 10f;

	[SerializeField]
	private float scale = 2f;

	private Indicator uiIndicator;

	private GameObject indicator;

	private void OnEnable()
	{
		uiIndicator = UnityEngine.Object.FindObjectOfType<Indicator>();
		if (!(uiIndicator == null))
		{
			if (uiIndicator.indicatorTransforms == null)
			{
				uiIndicator.indicatorTransforms = new List<Transform>();
			}
			uiIndicator.indicatorTransforms.Add(base.gameObject.transform);
		}
	}

	private void OnDisable()
	{
		if ((bool)uiIndicator)
		{
			uiIndicator.indicatorTransforms.Remove(base.gameObject.transform);
		}
	}

	private void Start()
	{
		indicator = UnityEngine.Object.Instantiate(indicatorPrefab);
		indicator.transform.localScale *= scale;
		indicator.transform.SetParent(base.transform);
	}

	private void Update()
	{
		indicator.transform.rotation = Quaternion.identity;
		indicator.transform.position = base.transform.position + Vector3.up * position;
	}
}
