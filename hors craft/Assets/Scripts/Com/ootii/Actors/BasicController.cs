// DecompilerFi decompiler from Assembly-CSharp.dll class: com.ootii.Actors.BasicController
using UnityEngine;

namespace com.ootii.Actors
{
	public class BasicController : MonoBehaviour
	{
		public Transform Camera;

		public bool UseGamepad;

		public bool MovementRelative = true;

		public float MovementSpeed = 3f;

		public bool RotationEnabled = true;

		public bool RotateToInput;

		public float RotationSpeed = 180f;

		protected Transform mTransform;

		public void Awake()
		{
			mTransform = base.gameObject.transform;
		}

		public void Update()
		{
			if (RotationEnabled)
			{
				float num = (!UnityEngine.Input.GetKey(KeyCode.E)) ? 0f : 1f;
				num -= ((!UnityEngine.Input.GetKey(KeyCode.Q)) ? 0f : 1f);
				if (num != 0f)
				{
					mTransform.rotation *= Quaternion.AngleAxis(num * RotationSpeed * Time.deltaTime, Vector3.up);
				}
			}
			Vector3 vector = Vector3.zero;
			vector.z = ((!UnityEngine.Input.GetKey(KeyCode.W) && !UnityEngine.Input.GetKey(KeyCode.UpArrow)) ? 0f : 1f);
			vector.z -= ((!UnityEngine.Input.GetKey(KeyCode.S) && !UnityEngine.Input.GetKey(KeyCode.DownArrow)) ? 0f : 1f);
			vector.x = ((!UnityEngine.Input.GetKey(KeyCode.D) && !UnityEngine.Input.GetKey(KeyCode.RightArrow)) ? 0f : 1f);
			vector.x -= ((!UnityEngine.Input.GetKey(KeyCode.A) && !UnityEngine.Input.GetKey(KeyCode.LeftArrow)) ? 0f : 1f);
			if (UseGamepad && vector.x == 0f && vector.z == 0f)
			{
				vector.z = UnityEngine.Input.GetAxis("Vertical");
				vector.x = UnityEngine.Input.GetAxis("Horizontal");
			}
			if (RotateToInput && Camera != null && vector.sqrMagnitude > 0f)
			{
				Vector3 eulerAngles = Camera.rotation.eulerAngles;
				Quaternion rotation = Quaternion.Euler(0f, eulerAngles.y, 0f);
				mTransform.rotation = Quaternion.LookRotation(rotation * vector, Vector3.up);
				vector.z = vector.magnitude;
				vector.x = 0f;
			}
			if (vector.magnitude >= 1f)
			{
				vector.Normalize();
			}
			if (MovementRelative)
			{
				vector = mTransform.rotation * vector;
			}
			mTransform.position += vector * (MovementSpeed * Time.deltaTime);
		}
	}
}
