// DecompilerFi decompiler from Assembly-CSharp.dll class: Indicator
using Common.Managers;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;
using UnityEngine.UI;

public class Indicator : MonoBehaviour
{
	[SerializeField]
	private Sprite playerBaseImage;

	[SerializeField]
	private Sprite enemyImage;

	[SerializeField]
	[Range(0f, 1f)]
	private float radiusFromCenter = 0.6f;

	[SerializeField]
	private float maxDistanceToFade = 200f;

	[SerializeField]
	private string[] gameNames;

	private RectTransform rectTransform;

	private List<GameObject> images;

	public List<Transform> indicatorTransforms;

	private float indicatorTime;

	private void Awake()
	{
		bool flag = false;
		string gameName = Manager.Get<ConnectionInfoManager>().gameName;
		string[] array = gameNames;
		foreach (string a in array)
		{
			if (a == gameName)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		if (indicatorTransforms == null)
		{
			indicatorTransforms = new List<Transform>();
		}
	}

	private void Start()
	{
		rectTransform = GetComponent<RectTransform>();
		images = new List<GameObject>();
	}

	private void Update()
	{
		indicatorTime += Time.deltaTime;
		DestroyCurrentIndicators();
		FindNewIndicators();
	}

	private void DestroyCurrentIndicators()
	{
		foreach (GameObject image in images)
		{
			UnityEngine.Object.Destroy(image);
		}
		images = new List<GameObject>();
	}

	private void FindNewIndicators()
	{
		Transform transform = PlayerGraphic.GetControlledPlayerInstance().gameObject.transform;
		AddPlayerBase(transform);
		AddEnemies(transform);
	}

	private void AddPlayerBase(Transform currentPlayerTransform)
	{
		Vector3 startPlayerPosition = Engine.EngineInstance.startPlayerPosition;
		Vector3 directionInViewSpace = GetDirectionInViewSpace(currentPlayerTransform, startPlayerPosition);
		if (Vector3.Distance(currentPlayerTransform.position, startPlayerPosition) > 50f)
		{
			GameObject item = CreateIndicatorAtPosition(directionInViewSpace, playerBaseImage, 0.5f);
			images.Add(item);
		}
	}

	private void AddEnemies(Transform currentPlayerTransform)
	{
		if (indicatorTransforms.Count > 0)
		{
			foreach (Transform indicatorTransform in indicatorTransforms)
			{
				Vector3 directionInViewSpace = GetDirectionInViewSpace(currentPlayerTransform, indicatorTransform.position);
				float scale = CalculateScaleBetweenObjects(currentPlayerTransform, indicatorTransform);
				GameObject item = CreateIndicatorAtPosition(directionInViewSpace, enemyImage, scale);
				images.Add(item);
			}
		}
	}

	private float CalculateScaleBetweenObjects(Transform currentPlayerTransform, Transform objectIndicatorTransform)
	{
		float num = Vector3.Distance(currentPlayerTransform.position, objectIndicatorTransform.position);
		return 1f - Mathf.Clamp(num / maxDistanceToFade, 0f, 1f);
	}

	private static Vector3 GetDirectionInViewSpace(Transform from, Vector3 to)
	{
		float x = to.x;
		Vector3 position = from.position;
		float x2 = x - position.x;
		float z = to.z;
		Vector3 position2 = from.position;
		float z2 = z - position2.z;
		Vector3 point = new Vector3(x2, 0f, z2);
		Vector3 eulerAngles = from.eulerAngles;
		return Quaternion.AngleAxis(0f - eulerAngles.y, Vector3.up) * point;
	}

	private GameObject CreateIndicatorAtPosition(Vector3 direction, Sprite imageToUse, float scale = 1f)
	{
		direction.Normalize();
		GameObject gameObject = new GameObject();
		gameObject.transform.parent = base.transform;
		gameObject.name = "Indicator";
		Image image = gameObject.AddComponent<Image>();
		image.sprite = imageToUse;
		float x = direction.x * radiusFromCenter * rectTransform.rect.height / 2f;
		float y = direction.z * radiusFromCenter * rectTransform.rect.height / 2f;
		Vector3 localPosition = new Vector3(x, y);
		Vector3 localScale = new Vector3(scale, scale, scale);
		gameObject.transform.localScale = localScale;
		gameObject.transform.localPosition = localPosition;
		gameObject.transform.LookAt(base.transform, base.transform.forward);
		gameObject.transform.Rotate(Vector3.right, -90f);
		return gameObject;
	}

	private void SetPositionAndRotationOfImages()
	{
		foreach (GameObject image in images)
		{
			if ((bool)image)
			{
				float x = Mathf.Cos(indicatorTime) * 0.7f * rectTransform.rect.height / 2f;
				float y = Mathf.Sin(indicatorTime) * 0.7f * rectTransform.rect.height / 2f;
				Vector3 localPosition = new Vector3(x, y);
				image.transform.localPosition = localPosition;
				image.transform.LookAt(base.transform, base.transform.forward);
				image.transform.Rotate(Vector3.right, -90f);
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		if ((bool)PlayerGraphic.GetControlledPlayerInstance())
		{
			Gizmos.DrawLine(PlayerGraphic.GetControlledPlayerInstance().gameObject.transform.position, Engine.EngineInstance.startPlayerPosition);
		}
	}
}
