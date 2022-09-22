// DecompilerFi decompiler from Assembly-CSharp.dll class: PickupJumpingToYourPocketUI
using com.ootii.Cameras;
using Common.Managers;
using Common.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PickupJumpingToYourPocketUI : MonoBehaviour
{
	public RectTransform target;

	public float travelTime = 1.5f;

	private float targetScale = 1f;

	private float targetY;

	private float middleY;

	private float targetX;

	private float startY;

	private float startX;

	private float timer;

	private float progress;

	private Action onFinish;

	private Rect targetRect
	{
		get
		{
			Rect result = default(Rect);
			result.center = target.rect.center;
			result.size = target.rect.size * Manager.Get<CanvasManager>().canvas.scaleFactor;
			return result;
		}
	}

	public static void SpawnPickup(Vector3 startPosition, GameObject target, Sprite sprite, float scale = 1f, Action onFinish = null)
	{
		GameObject gameObject = new GameObject("pickup");
		gameObject.transform.SetParent(Manager.Get<CanvasManager>().canvas.transform, worldPositionStays: false);
		gameObject.transform.localScale = Vector3.one * 0.5f * scale;
		gameObject.transform.SetAsFirstSibling();
		gameObject.transform.position = CameraController.instance.MainCamera.WorldToScreenPoint(startPosition);
		Image image = gameObject.gameObject.AddComponent<Image>();
		image.sprite = sprite;
		PickupJumpingToYourPocketUI pickupJumpingToYourPocketUI = gameObject.AddComponent<PickupJumpingToYourPocketUI>();
		pickupJumpingToYourPocketUI.Init(target, scale, onFinish);
	}

	public void Init(GameObject targetGO, float targetScale, Action onFinish = null)
	{
		this.onFinish = onFinish;
		Vector3 position = base.transform.position;
		startY = position.y;
		Vector3 position2 = base.transform.position;
		startX = position2.x;
		target = targetGO.GetComponent<RectTransform>();
		Rect targetRect = this.targetRect;
		Vector3[] array = new Vector3[4];
		target.GetWorldCorners(array);
		targetY = array[1].y;
		middleY = targetY + UnityEngine.Random.Range(80f, 180f);
		targetX = UnityEngine.Random.Range(array[0].x, array[2].x);
		this.targetScale = targetScale;
		timer = 0f;
		progress = 0f;
	}

	private void Update()
	{
		if (Time.timeScale == 0f)
		{
			base.gameObject.GetComponent<Image>().enabled = false;
			return;
		}
		base.gameObject.GetComponent<Image>().enabled = true;
		UpdateHorizontalMovement();
		UpdateVerticalMovement();
		UpdateScaling();
		timer += Time.deltaTime;
		progress = timer / travelTime;
		if (progress >= 1f)
		{
			if (onFinish != null)
			{
				onFinish();
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void UpdateHorizontalMovement()
	{
		Vector3 position = base.transform.position;
		position.x = Easing.Ease(EaseType.InQuart, startX, targetX, progress);
		base.transform.position = position;
	}

	private void UpdateVerticalMovement()
	{
		Vector3 position = base.transform.position;
		float b = Easing.Ease(EaseType.InQuint, middleY, targetY, progress * progress);
		position.y = Mathf.Lerp(startY, b, progress);
		base.transform.position = position;
	}

	private void UpdateScaling()
	{
		float num = 1f;
		num = ((progress < 0.5f) ? (0.5f + Easing.Ease(EaseType.OutCubic, 0f, 0.5f, progress / 0.5f)) : ((!(progress < 0.7f)) ? Easing.Ease(EaseType.InExpo, 1f, 0.5f, (progress - 0.7f) / 0.3f) : 1f));
		base.transform.localScale = Vector3.one * num * targetScale;
	}
}
