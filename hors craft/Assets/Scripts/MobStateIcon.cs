// DecompilerFi decompiler from Assembly-CSharp.dll class: MobStateIcon
using Common.Behaviours;
using UnityEngine;

[RequireComponent(typeof(Pettable))]
public class MobStateIcon : MonoBehaviour
{
	private Pettable pettable;

	private GameObject stickedObj;

	private float timer;

	private void Awake()
	{
		pettable = GetComponent<Pettable>();
	}

	public static void RemoveIcon(GameObject go)
	{
		MobStateIcon component = go.GetComponent<MobStateIcon>();
		if (component != null)
		{
			component.Remove();
		}
	}

	public static void SetIcon(Pettable pettable, Sprite sprite, float duration = 3f, float offsetY = 0f)
	{
		SetIcon(pettable.gameObject, sprite, duration, offsetY);
	}

	public static void SetIcon(GameObject go, Sprite sprite, float duration = 3f, float offsetY = 0f, float scale = 1f)
	{
		MobStateIcon mobStateIcon = go.GetComponent<MobStateIcon>();
		if (mobStateIcon == null)
		{
			mobStateIcon = go.AddComponent<MobStateIcon>();
		}
		mobStateIcon.SetIcon(sprite, duration, offsetY, scale);
	}

	public void SetIcon(Sprite sprite, float duration = 3f, float offsetY = 0f, float scale = 1f)
	{
		if (stickedObj == null)
		{
			CreateSpriteSticker(sprite, offsetY, scale);
		}
		else
		{
			SpriteRenderer componentInChildren = stickedObj.GetComponentInChildren<SpriteRenderer>();
			componentInChildren.sprite = sprite;
			stickedObj.transform.localScale = Vector3.one * scale;
			stickedObj.transform.localPosition = pettable.relativeTamePanelPos + Vector3.up * offsetY;
		}
		timer = duration;
	}

	private void CreateSpriteSticker(Sprite sprite, float offsetY, float scale)
	{
		stickedObj = new GameObject("Sticker");
		stickedObj.transform.SetParent(pettable.mob.transform, worldPositionStays: false);
		stickedObj.transform.rotation = Quaternion.identity;
		stickedObj.transform.localScale = Vector3.one * scale;
		stickedObj.transform.localPosition = pettable.relativeTamePanelPos + Vector3.up * offsetY;
		SpriteRenderer spriteRenderer = stickedObj.AddComponent<SpriteRenderer>();
		spriteRenderer.sprite = sprite;
		stickedObj.AddComponent<RotateTowardsCamera>();
	}

	private void Update()
	{
		if (!(stickedObj == null))
		{
			if (timer <= 0f)
			{
				UnityEngine.Object.Destroy(stickedObj);
			}
			else
			{
				timer -= Time.deltaTime;
			}
		}
	}

	private void Remove()
	{
		if (stickedObj != null)
		{
			UnityEngine.Object.Destroy(stickedObj);
		}
		UnityEngine.Object.Destroy(this);
	}

	private void OnDestroy()
	{
		if (stickedObj != null)
		{
			UnityEngine.Object.Destroy(stickedObj);
		}
	}
}
