// DecompilerFi decompiler from Assembly-CSharp.dll class: KeyboardKeyFakeInput
using UnityEngine;
using UnityEngine.EventSystems;

public class KeyboardKeyFakeInput : MonoBehaviour
{
	[SerializeField]
	private int mouseButton = -1;

	[SerializeField]
	private KeyCode key;

	private RectTransform rect;

	private int lastPointerId;

	private bool last;

	public void ChangeKeyTo(int mouseButton)
	{
		if (mouseButton != this.mouseButton)
		{
			this.mouseButton = mouseButton;
		}
	}

	public void ChangeKeyTo(KeyCode key)
	{
		if (this.key != key)
		{
			this.key = key;
		}
	}

	private void Awake()
	{
		rect = GetComponent<RectTransform>();
		last = false;
	}

	private void Update()
	{
		bool flag = false;
		flag = ((mouseButton < 0) ? UnityEngine.Input.GetKey(key) : Input.GetMouseButton(mouseButton));
		if (!last && flag)
		{
			PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
			pointerEventData.pointerId = (lastPointerId = UnityEngine.Random.Range(0, 10));
			pointerEventData.position = rect.position;
			ExecuteEvents.Execute(base.gameObject, pointerEventData, ExecuteEvents.pointerDownHandler);
			UnityEngine.Debug.Log("KeyboardFakeInput: " + base.name + " DOWN with mouseButton " + mouseButton + " and key " + key);
		}
		if (last && !flag)
		{
			PointerEventData pointerEventData2 = new PointerEventData(EventSystem.current);
			pointerEventData2.pointerId = lastPointerId;
			pointerEventData2.position = rect.position;
			ExecuteEvents.Execute(base.gameObject, pointerEventData2, ExecuteEvents.pointerUpHandler);
			BaseEventData baseEventData = new BaseEventData(EventSystem.current);
			baseEventData.selectedObject = base.gameObject;
			ExecuteEvents.Execute(base.gameObject, baseEventData, ExecuteEvents.submitHandler);
			UnityEngine.Debug.Log("KeyboardFakeInput: " + base.name + " UP with mouseButton " + mouseButton + " and key " + key);
		}
		last = flag;
	}
}
