// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.IsometricPlaceableObject
using Common.Managers;
using Gameplay;
using GameUI;
using States;
using UnityEngine;

namespace Uniblocks
{
	public abstract class IsometricPlaceableObject : MonoBehaviour
	{
		[HideInInspector]
		public int craftableId;

		protected Vector3 position;

		protected IsometricCamera cam;

		protected int rotation;

		protected float deltaTime;

		public const float MAX_DISTANCE_FROM_PLAYER = 32f;

		private const float MOVE_SPEED = 10f;

		private float previousTime;

		private bool firstUpdate = true;

		private UniversalAnalogInput analogInput;

		private Vector3 initialPosition;

		private IsometricObjectPlacementState _isometricObjectPlacementState;

		protected float rotationEuler => NormalizeAngle((float)rotation * 90f);

		private IsometricObjectPlacementState isometricObjectPlacementState
		{
			get
			{
				if (_isometricObjectPlacementState == null)
				{
					_isometricObjectPlacementState = Manager.Get<StateMachineManager>().GetStateInstance<IsometricObjectPlacementState>();
					_isometricObjectPlacementState.SetupErrorMessage(GetErrorKey(), GetErrorDefaultText());
				}
				return _isometricObjectPlacementState;
			}
		}

		protected virtual string GetErrorKey()
		{
			return "iso.cant.place";
		}

		protected virtual string GetErrorDefaultText()
		{
			return "You can't place it here";
		}

		protected virtual void Awake()
		{
		}

		protected virtual void Start()
		{
		}

		public void SetAnalog(AnalogController analog, SimpleRepeatButton button)
		{
			analogInput = new UniversalAnalogInput(analog, button);
		}

		public void SetCamera(IsometricCamera cam)
		{
			this.cam = cam;
			position = base.transform.position;
			initialPosition = base.transform.position;
			previousTime = Time.realtimeSinceStartup;
		}

		protected virtual void Update()
		{
			deltaTime = Time.realtimeSinceStartup - previousTime;
			previousTime = Time.realtimeSinceStartup;
			Vector2 vector = analogInput.CalculatePosition();
			if (vector != Vector2.zero || firstUpdate)
			{
				Move(vector);
				SnapToGround();
			}
			firstUpdate = false;
		}

		protected virtual void Move(Vector2 input)
		{
			Vector3 forward = cam.transform.forward;
			forward.y = 0f;
			forward.Normalize();
			Vector3 right = cam.transform.right;
			right.y = 0f;
			right.Normalize();
			Vector3 a = input.x * right + input.y * forward;
			a *= 10f * deltaTime;
			position += a;
			position = LimitDistanceFromPlayer(position);
			base.transform.position = Snap(position);
		}

		protected virtual void SnapToGround()
		{
			if (Physics.BoxCast(halfExtents: GetBounds().extents, center: base.transform.position + Vector3.up * 200f, direction: Vector3.down, hitInfo: out RaycastHit hitInfo))
			{
				Vector3 vector = base.transform.position;
				Vector3 point = hitInfo.point;
				vector.y = point.y + 0.1f;
				base.transform.position = Snap(vector);
			}
		}

		public virtual Bounds GetBounds()
		{
			return default(Bounds);
		}

		protected Vector3 Snap(Vector3 position)
		{
			Vector3 zero = Vector3.zero;
			zero.x = Mathf.Floor(position.x);
			zero.y = Mathf.Floor(position.y);
			zero.z = Mathf.Floor(position.z);
			return zero;
		}

		protected virtual Vector3 LimitDistanceFromPlayer(Vector3 position)
		{
			initialPosition.y = position.y;
			Vector3 a = position - initialPosition;
			float magnitude = a.magnitude;
			if (magnitude > 32f)
			{
				float d = magnitude / 32f;
				a /= d;
				position = initialPosition + a;
			}
			return position;
		}

		public virtual void OnRotate()
		{
			rotation = (rotation + 1) % 4;
			firstUpdate = true;
		}

		public virtual bool OnPlace()
		{
			Singleton<PlayerData>.get.playerItems.AddCraftable(craftableId, -1);
			UnityEngine.Object.Destroy(base.gameObject);
			return true;
		}

		public virtual void OnDelete()
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}

		private float NormalizeAngle(float angle)
		{
			while (angle > 180f)
			{
				angle -= 360f;
			}
			while (angle < -180f)
			{
				angle += 360f;
			}
			return angle;
		}

		protected void EnablePlacement(bool enable)
		{
			isometricObjectPlacementState.ShowAcceptButton(enable);
			isometricObjectPlacementState.ShowErrorMessage(!enable);
		}
	}
}
