// DecompilerFi decompiler from Assembly-CSharp.dll class: Health
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
	public delegate void DoOnHit(Vector3 dir);

	public delegate void DoOnHpChange();

	protected class ColorMaterial
	{
		private readonly Material material;

		private readonly string colorPropery;

		public ColorMaterial(Material material)
		{
			this.material = material;
			colorPropery = ((!material.HasProperty("_AdditionalColor")) ? "_Color" : "_AdditionalColor");
		}

		public void SetShaderColor(Color color)
		{
			material.SetColor(colorPropery, color);
		}

		public int GetHashCode()
		{
			return material.GetHashCode();
		}

		public bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			ColorMaterial colorMaterial = obj as ColorMaterial;
			if (colorMaterial == null)
			{
				return false;
			}
			return GetHashCode() == colorMaterial.GetHashCode();
		}

		public static bool operator ==(ColorMaterial one, ColorMaterial two)
		{
			return one.Equals(two);
		}

		public static bool operator !=(ColorMaterial one, ColorMaterial two)
		{
			return !one.Equals(two);
		}
	}

	public Renderer[] renderers;

	public float maxHp = 5f;

	public DoOnHpChange onHpChangeAction;

	public Action onDieAction;

	public Action<Armor> onArmorChanged;

	public bool invulnerability;

	public bool canBeKnockedBack = true;

	public PlayerStats stat;

	private float _hp;

	private const float HIT_FORCE_MULTIPLIER = 2f;

	private const float PLAYER_HIT_FORCE_MULTIPLIER = 2f;

	protected ColorMaterial[] _renderesMaterials;

	private Armor _armor;

	public float hp
	{
		get
		{
			return _hp;
		}
		set
		{
			if (value < _hp)
			{
				float dmg = _hp - value;
				TakeDamage(dmg);
			}
			else
			{
				_hp = Mathf.Min(maxHp, value);
			}
			if (onHpChangeAction != null)
			{
				onHpChangeAction();
			}
		}
	}

	protected ColorMaterial[] materials
	{
		get
		{
			if (_renderesMaterials == null)
			{
				if (renderers == null)
				{
					_renderesMaterials = new ColorMaterial[0];
				}
				else
				{
					HashSet<ColorMaterial> hashSet = new HashSet<ColorMaterial>();
					for (int i = 0; i < renderers.Length; i++)
					{
						for (int j = 0; j < renderers[i].sharedMaterials.Length; j++)
						{
							hashSet.Add(new ColorMaterial(renderers[i].sharedMaterials[j]));
						}
					}
					_renderesMaterials = new ColorMaterial[hashSet.Count];
					hashSet.CopyTo(_renderesMaterials);
				}
			}
			return _renderesMaterials;
		}
	}

	public Armor armor
	{
		get
		{
			if (_armor == null)
			{
				_armor = GetComponent<Armor>();
			}
			return _armor;
		}
		set
		{
			_armor = value;
			ArmorChanged();
		}
	}

	public event DoOnHit onHitAction;

	private void Awake()
	{
		hp = maxHp;
		if (stat != null)
		{
			stat.Add(new PlayerStats.Modifier
			{
				value = maxHp,
				priority = 0,
				Action = ((float toAction, float value) => value + toAction)
			});
			stat.Register(OnStatsChanged);
		}
		for (int i = 0; i < materials.Length; i++)
		{
			materials[i].SetShaderColor(new Color(0f, 0f, 0f, 0f));
		}
	}

	public bool IsDead()
	{
		return hp <= 0f;
	}

	public void OnHit(float dmg, Vector3 hitDirection, float knockbackMultiplier = 1f)
	{
		hp -= dmg;
		KnockBack(hitDirection, knockbackMultiplier);
		StopAllCoroutines();
		StartCoroutine(AnimateHit());
		if (this.onHitAction != null)
		{
			this.onHitAction(hitDirection);
		}
		if (IsDead() && onDieAction != null)
		{
			onDieAction();
			onDieAction = null;
		}
	}

	private void KnockBack(Vector3 hitDirection, float knockbackMultiplier)
	{
		if (!canBeKnockedBack)
		{
			return;
		}
		Rigidbody component = GetComponent<Rigidbody>();
		if (component != null)
		{
			Vector3 vector = hitDirection.normalized * 2f;
			vector.y = 2f;
			if (Vector3.Dot(component.velocity, vector) > 0.4f)
			{
				component.AddForce(vector * knockbackMultiplier, ForceMode.Impulse);
			}
			return;
		}
		CharacterMotor component2 = GetComponent<CharacterMotor>();
		if (component2 != null)
		{
			CharacterController component3 = component2.gameObject.GetComponent<CharacterController>();
			Vector3 normalized = hitDirection.normalized;
			if (Vector3.Dot(component3.velocity, normalized) > 0.4f)
			{
				component2.KnockBack(normalized * 2f * knockbackMultiplier);
			}
		}
	}

	public void ArmorChanged()
	{
		if (onArmorChanged != null)
		{
			onArmorChanged(armor);
		}
	}

	public void OnStatsChanged()
	{
		float num = stat.GetStats() - maxHp;
		maxHp = stat.GetStats();
		hp += num;
	}

	private IEnumerator AnimateHit()
	{
		if (renderers == null)
		{
			yield break;
		}
		Color red = new Color(0.9f, 0f, 0f, 0f);
		Color transparent = new Color(0f, 0f, 0f, 0f);
		for (int i = 0; i < materials.Length; i++)
		{
			materials[i].SetShaderColor(red);
		}
		yield return new WaitForSeconds(0.1f);
		float time = 0f;
		while (time < 0.2f)
		{
			Color addColor = Color.Lerp(red, transparent, time / 0.2f);
			for (int j = 0; j < materials.Length; j++)
			{
				materials[j].SetShaderColor(addColor);
			}
			time += Time.deltaTime;
			yield return null;
		}
		for (int k = 0; k < materials.Length; k++)
		{
			materials[k].SetShaderColor(transparent);
		}
	}

	private void TakeDamage(float dmg)
	{
		if (!invulnerability)
		{
			if (armor != null)
			{
				dmg = armor.TakeDamage(dmg);
				armor.TryDestroy();
			}
			_hp = Mathf.Max(0f, _hp - dmg);
		}
	}

	private void OnDestroy()
	{
		if (stat != null)
		{
			stat.Unregister(OnStatsChanged);
		}
	}
}
