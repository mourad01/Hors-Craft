// DecompilerFi decompiler from Assembly-UnityScript.dll class: Joystick
using System;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
[RequireComponent(typeof(Text))]
public class Joystick : MonoBehaviour
{
	[NonSerialized]
	private static Joystick[] joysticks;

	[NonSerialized]
	private static bool enumeratedJoysticks;

	[NonSerialized]
	private static float tapTimeDelta = 0.3f;

	public bool touchPad;

	public Rect touchZone;

	public Vector2 deadZone;

	public bool normalize;

	public Vector2 position;

	public int tapCount;

	private int lastFingerId;

	private float tapTimeWindow;

	private Vector2 fingerDownPos;

	private float fingerDownTime;

	private float firstDeltaTime;

	private Text gui;

	private Rect defaultRect;

	private Boundary guiBoundary;

	private Vector2 guiTouchOffset;

	private Vector2 guiCenter;
    private Rect x;

    public Joystick()
	{
		deadZone = Vector2.zero;
		lastFingerId = -1;
		firstDeltaTime = 0.5f;
		guiBoundary = new Boundary();
	}

	public void Start()
	{
		gui = (Text)GetComponent(typeof(Text));
		//defaultRect = gui.pixelInset;
		ref Rect reference = ref defaultRect;
		float x = defaultRect.x;
		Vector3 vector = transform.position;
		reference.x = x + vector.x * (float)Screen.width;
		ref Rect reference2 = ref defaultRect;
		float y = defaultRect.y;
		Vector3 vector2 = transform.position;
		reference2.y = y + vector2.y * (float)Screen.height;
		float x2 = 0f;
		Vector3 vector3 = transform.position;
		vector3.x = x2;
		Vector3 vector5 = transform.position = vector3;
		float y2 = 0f;
		Vector3 vector6 = transform.position;
		vector6.y = y2;
		Vector3 vector8 = transform.position = vector6;
		/*if (touchPad)
		{
			if ((bool)gui.)
			{
				touchZone = defaultRect;
			}
			return;
		}*/
		guiTouchOffset.x = defaultRect.width * 0.5f;
		guiTouchOffset.y = defaultRect.height * 0.5f;
		guiCenter.x = defaultRect.x + guiTouchOffset.x;
		guiCenter.y = defaultRect.y + guiTouchOffset.y;
		guiBoundary.min.x = defaultRect.x - guiTouchOffset.x;
		guiBoundary.max.x = defaultRect.x + guiTouchOffset.x;
		guiBoundary.min.y = defaultRect.y - guiTouchOffset.y;
		guiBoundary.max.y = defaultRect.y + guiTouchOffset.y;
	}

	public void Disable()
	{
		gameObject.SetActive(value: false);
		enumeratedJoysticks = false;
	}

	public void ResetJoystick()
	{
		x= gui.GetComponent<RectTransform>().rect; 
		x= defaultRect;
		lastFingerId = -1;
		position = Vector2.zero;
		fingerDownPos = Vector2.zero;
		if (touchPad)
		{
			float a = 0.025f;
			Color color = gui.color;
			color.a = a;
			Color color3 = gui.color = color;
		}
	}

	public bool IsFingerDown()
	{
		return lastFingerId != -1;
	}

	public void LatchedFinger(int fingerId)
	{
		if (lastFingerId == fingerId)
		{
			ResetJoystick();
		}
	}

	public void Update()
	{
		if (!enumeratedJoysticks)
		{
			joysticks = (((Joystick[])UnityEngine.Object.FindObjectsOfType(typeof(Joystick))) as Joystick[]);
			enumeratedJoysticks = true;
		}
		int touchCount = UnityEngine.Input.touchCount;
		if (!(tapTimeWindow <= 0f))
		{
			tapTimeWindow -= Time.deltaTime;
		}
		else
		{
			tapCount = 0;
		}
		if (touchCount == 0)
		{
			ResetJoystick();
		}
		else
		{
			for (int i = 0; i < touchCount; i++)
			{
				Touch touch = UnityEngine.Input.GetTouch(i);
				Vector2 vector = touch.position - guiTouchOffset;
				bool flag = false;
				if (touchPad)
				{
					if (touchZone.Contains(touch.position))
					{
						flag = true;
					}
				}
				/*else if (gui.HitTest(touch.position))
				{
					flag = true;
				}*/
				if (flag && (lastFingerId == -1 || lastFingerId != touch.fingerId))
				{
					if (touchPad)
					{
						float a = 0.15f;
						Color color = gui.color;
						color.a = a;
						Color color3 = gui.color = color;
						lastFingerId = touch.fingerId;
						fingerDownPos = touch.position;
						fingerDownTime = Time.time;
					}
					lastFingerId = touch.fingerId;
					if (!(tapTimeWindow <= 0f))
					{
						tapCount++;
					}
					else
					{
						tapCount = 1;
						tapTimeWindow = tapTimeDelta;
					}
					int j = 0;
					Joystick[] array = joysticks;
					for (int length = array.Length; j < length; j++)
					{
						if (array[j] != this)
						{
							array[j].LatchedFinger(touch.fingerId);
						}
					}
				}
				if (lastFingerId == touch.fingerId)
				{
					if (touch.tapCount > tapCount)
					{
						tapCount = touch.tapCount;
					}
					if (touchPad)
					{
						ref Vector2 reference = ref position;
						Vector2 vector2 = touch.position;
						reference.x = Mathf.Clamp((vector2.x - fingerDownPos.x) / (touchZone.width / 2f), -1f, 1f);
						ref Vector2 reference2 = ref position;
						Vector2 vector3 = touch.position;
						reference2.y = Mathf.Clamp((vector3.y - fingerDownPos.y) / (touchZone.height / 2f), -1f, 1f);
					}
					else
					{
						float num = Mathf.Clamp(vector.x, guiBoundary.min.x, guiBoundary.max.x);
						Rect pixelInset = gui.GetComponent<RectTransform>().rect;
						float num3 = pixelInset.x = num;
						Rect rect2 = gui.GetComponent<RectTransform>().rect;
						float num4 = Mathf.Clamp(vector.y, guiBoundary.min.y, guiBoundary.max.y);
						Rect pixelInset2 = gui.GetComponent<RectTransform>().rect;
						float num6 = pixelInset2.y = num4;
						Rect rect4 = gui.GetComponent<RectTransform>().rect; rect4 = pixelInset2;
					}
					if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
					{
						ResetJoystick();
					}
				}
			}
		}
		if (!touchPad)
		{
			position.x = (gui.GetComponent<RectTransform>().rect.x + guiTouchOffset.x - guiCenter.x) / guiTouchOffset.x;
			position.y = (gui.GetComponent<RectTransform>().rect.y + guiTouchOffset.y - guiCenter.y) / guiTouchOffset.y;

			float num7 = Mathf.Abs(position.x);
			float num8 = Mathf.Abs(position.y);
			if (!(num7 >= deadZone.x))
			{
				position.x = 0f;
			}
			else if (normalize)
			{
				position.x = Mathf.Sign(position.x) * (num7 - deadZone.x) / (1f - deadZone.x);
			}
			if (!(num8 >= deadZone.y))
			{
				position.y = 0f;
			}
			else if (normalize)
			{
				position.y = Mathf.Sign(position.y) * (num8 - deadZone.y) / (1f - deadZone.y);
			}
		}
	}

		public void Main()
		{
		}
	} 
