// DecompilerFi decompiler from Assembly-CSharp.dll class: SoftCurrencyImageSwitcher
using Common.Managers;
using UnityEngine;
using UnityEngine.UI;

public class SoftCurrencyImageSwitcher : MonoBehaviour
{
	[SerializeField]
	private Sprite sprite;

	private void Awake()
	{
		if (Manager.Contains<AbstractSoftCurrencyManager>())
		{
			Image component = GetComponent<Image>();
			component.sprite = sprite;
		}
		UnityEngine.Object.Destroy(this);
	}
}
