// DecompilerFi decompiler from Assembly-CSharp.dll class: ModelDragRotator
using UnityEngine;
using UnityEngine.EventSystems;

public class ModelDragRotator : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IEventSystemHandler
{
	private GameObject _modelToRotate;

	public float power = 0.5f;

	public bool autoRotate = true;

	public float autoRotePower = -0.25f;

	public bool resetChildToOneScale;

	private bool autoRotateEnabled = true;

	public GameObject modelToRotate
	{
		get
		{
			return _modelToRotate;
		}
		set
		{
			_modelToRotate = value;
			if (resetChildToOneScale && _modelToRotate.transform.childCount > 0)
			{
				_modelToRotate.transform.GetChild(0).localScale = Vector3.one;
			}
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (!(modelToRotate == null))
		{
			Transform transform = modelToRotate.transform;
			Vector2 delta = eventData.delta;
			transform.Rotate(new Vector3(0f, (0f - delta.x) * power, 0f));
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		autoRotateEnabled = false;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		autoRotateEnabled = true;
	}

	private void Update()
	{
		if (!(modelToRotate == null) && autoRotateEnabled)
		{
			modelToRotate.transform.Rotate(new Vector3(0f, autoRotePower, 0f));
		}
	}
}
