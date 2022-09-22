// DecompilerFi decompiler from Assembly-CSharp.dll class: HealthDisplay
using Common.Utils;
using UnityEngine;

public class HealthDisplay : MonoBehaviour
{
	public MeshRenderer sliderRenderer;

	private Health _health;

	private float lastHPValue;

	protected Material material
	{
		get
		{
			return sliderRenderer.material;
		}
		set
		{
			sliderRenderer.material = value;
		}
	}

	protected Health health => _health ?? (_health = GetComponentInParent<Health>());

	private void Start()
	{
		Place();
		UpdateHP();
	}

	private void Update()
	{
		if (lastHPValue != health.hp)
		{
			UpdateHP();
		}
	}

	private void UpdateHP()
	{
		lastHPValue = health.hp;
		material.SetFloat("_Fill", health.hp / health.maxHp);
	}

	private void Place()
	{
		base.transform.localPosition = Vector3.zero;
		Bounds bounds = RenderersBounds.MaximumBounds(base.transform.parent.gameObject);
		Transform transform = base.transform;
		Vector3 size = bounds.size;
		transform.localPosition = new Vector3(0f, size.y + 0.2f, 0f);
	}
}
