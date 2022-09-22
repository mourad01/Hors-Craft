// DecompilerFi decompiler from Assembly-CSharp.dll class: RecipeTutorial
using Common.Managers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeTutorial : MonoBehaviour
{
	public GameObject additionSign;

	public GameObject resultSign;

	public GameObject device;

	public Image jacobsImage;

	public Text jacobName;

	public Transform parent;

	private bool firstSpawned;

	private int maxColumns
	{
		get
		{
			float num = 0.8f * (float)Screen.width;
			Vector2 cellSize = grid.cellSize;
			float x = cellSize.x;
			Vector2 spacing = grid.spacing;
			float num2 = (x + spacing.x) * Manager.Get<CanvasManager>().canvas.scaleFactor;
			return (int)(num / num2);
		}
	}

	private GridLayoutGroup grid => parent.GetComponent<GridLayoutGroup>();

	public void SetText(string key, string defaultText)
	{
		TranslateText componentInChildren = GetComponentInChildren<TranslateText>();
		componentInChildren.translationKey = key;
		componentInChildren.defaultText = defaultText;
		componentInChildren.ForceRefresh();
		if (string.IsNullOrEmpty(key) && string.IsNullOrEmpty(defaultText))
		{
			componentInChildren.gameObject.SetActive(value: false);
		}
	}

	public void Clear()
	{
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < parent.childCount; i++)
		{
			if (parent.GetChild(i).gameObject.activeSelf)
			{
				list.Add(parent.GetChild(i).gameObject);
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			UnityEngine.Object.Destroy(list[j]);
		}
		UpdateGridLayoutColumnCount();
	}

	public void AddItem(Sprite deviceSprite)
	{
		if (firstSpawned)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(additionSign, parent, worldPositionStays: false);
			gameObject.transform.localScale = Vector3.one;
			gameObject.SetActive(value: true);
		}
		else
		{
			firstSpawned = true;
		}
		GameObject gameObject2 = UnityEngine.Object.Instantiate(device, parent, worldPositionStays: false);
		gameObject2.transform.localScale = Vector3.one;
		gameObject2.SetActive(value: true);
		gameObject2.GetComponentInChildren<Image>().sprite = deviceSprite;
		UpdateGridLayoutColumnCount();
	}

	public void AddResult(Sprite resultSprite)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(resultSign, parent, worldPositionStays: false);
		gameObject.transform.localScale = Vector3.one;
		gameObject.SetActive(value: true);
		gameObject = UnityEngine.Object.Instantiate(device, parent, worldPositionStays: false);
		gameObject.transform.localScale = Vector3.one;
		gameObject.SetActive(value: true);
		gameObject.GetComponentInChildren<Image>().sprite = resultSprite;
		UpdateGridLayoutColumnCount();
	}

	private void UpdateGridLayoutColumnCount()
	{
		int num = 0;
		for (int i = 0; i < parent.childCount; i++)
		{
			if (parent.GetChild(i).gameObject.activeSelf)
			{
				num++;
			}
		}
		int constraintCount = Mathf.Min(maxColumns, num);
		grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
		grid.constraintCount = constraintCount;
	}
}
