// DecompilerFi decompiler from Assembly-CSharp.dll class: Borodar.FarlandSkies.Core.Demo.CameraOrbitController
using UnityEngine;
using UnityEngine.EventSystems;

namespace Borodar.FarlandSkies.Core.Demo
{
	public class CameraOrbitController : MonoBehaviour
	{
		public Transform Target;

		public float Distance = 5f;

		public float DistanceMin = 0.5f;

		public float DistanceMax = 15f;

		public Vector3 Speed = new Vector3(250f, 250f, 250f);

		public Vector2 VerticalRotationLimit = new Vector2(-90f, 90f);

		private Vector2 _angles;

		private bool _isPointerOverGui;

		protected void Awake()
		{
			Vector3 eulerAngles = base.transform.eulerAngles;
			_angles.x = eulerAngles.y;
			_angles.y = eulerAngles.x;
			Quaternion rotation = Quaternion.Euler(_angles.y, _angles.x, 0f);
			Vector3 point = new Vector3(0f, 0f, 0f - Distance);
			Vector3 position = rotation * point + Target.position;
			base.transform.rotation = rotation;
			base.transform.position = position;
		}

		protected void LateUpdate()
		{
			if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject())
			{
				_isPointerOverGui = true;
			}
			if (Input.GetMouseButtonUp(0))
			{
				_isPointerOverGui = false;
			}
			if (!_isPointerOverGui)
			{
				float axis = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
				if (Input.GetMouseButton(0) || Input.GetMouseButton(1) || !(Mathf.Abs(axis) < 0.01f))
				{
					float axis2 = UnityEngine.Input.GetAxis("Mouse X");
					float axis3 = UnityEngine.Input.GetAxis("Mouse Y");
					_angles.x += axis2 * Speed.x * Time.deltaTime;
					_angles.y -= axis3 * Speed.y * Time.deltaTime;
					_angles.y = ClampAngle(_angles.y, VerticalRotationLimit.x, VerticalRotationLimit.y);
					Quaternion rotation = Quaternion.Euler(_angles.y, _angles.x, 0f);
					Distance = Mathf.Clamp(Distance - axis * Speed.z, DistanceMin, DistanceMax);
					Vector3 point = new Vector3(0f, 0f, 0f - Distance);
					Vector3 position = rotation * point + Target.position;
					base.transform.rotation = rotation;
					base.transform.position = position;
				}
			}
		}

		private static float ClampAngle(float angle, float min, float max)
		{
			if (angle < -360f)
			{
				angle += 360f;
			}
			if (angle > 360f)
			{
				angle -= 360f;
			}
			return Mathf.Clamp(angle, min, max);
		}
	}
}
