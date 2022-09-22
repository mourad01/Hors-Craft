// DecompilerFi decompiler from Assembly-CSharp.dll class: com.ootii.Cameras.ViewMotor
using com.ootii.Actors;
using com.ootii.Base;
using com.ootii.Geometry;
using com.ootii.Helpers;
using UnityEngine;

namespace com.ootii.Cameras
{
	[BaseName("1st Person View")]
	[BaseDescription("Motor that allows the rig to rotate for 1st person views.")]
	public class ViewMotor : YawPitchMotor
	{
		public bool _RotateAnchor = true;

		public string _RotateAnchorAlias = string.Empty;

		public bool _IsActorMatchingRotation;

		protected ICharacterController mCharacterController;

		protected bool mWasRotatingAnchor;

		public bool RotateAnchor
		{
			get
			{
				return _RotateAnchor;
			}
			set
			{
				_RotateAnchor = value;
			}
		}

		public string RotateAnchorAlias
		{
			get
			{
				return _RotateAnchorAlias;
			}
			set
			{
				_RotateAnchorAlias = value;
			}
		}

		public bool IsActorMatchingRotation
		{
			get
			{
				return _IsActorMatchingRotation;
			}
			set
			{
				_IsActorMatchingRotation = value;
			}
		}

		public ViewMotor()
		{
			_MaxDistance = 0f;
			mDistance = 0f;
		}

		public override void Awake()
		{
			base.Awake();
			if (Anchor != null)
			{
				mCharacterController = InterfaceHelper.GetComponent<ICharacterController>(Anchor.gameObject);
			}
			if (Application.isPlaying && Anchor == null)
			{
				mRigTransform.Position = RigController._Transform.position;
				mRigTransform.Rotation = RigController._Transform.rotation;
			}
		}

		public override void Clear()
		{
			mCharacterController = null;
			base.Clear();
		}

		public override CameraTransform RigLateUpdate(float rDeltaTime, int rUpdateIndex, float rTiltAngle = 0f)
		{
			if (RigController == null)
			{
				return mRigTransform;
			}
			if (RigController.Anchor == null)
			{
				return mRigTransform;
			}
			Quaternion tilt = RigController.Tilt;
			Transform anchor = Anchor;
			Transform transform = RigController._Transform;
			if (_RotateAnchor && anchor != null && (_RotateAnchorAlias.Length == 0 || RigController.InputSource == null || RigController.InputSource.IsPressed(_RotateAnchorAlias)))
			{
				mWasRotatingAnchor = true;
			}
			mFrameEuler = GetFrameEuler(!mWasRotatingAnchor);
			Vector3 vector = tilt.Up();
			Vector3 right = transform.right;
			Vector3 a = anchor.position + anchor.rotation * AnchorOffset + anchor.rotation * _Offset;
			Matrix4x4 matrix4x = Matrix4x4.TRS(mAnchorLastPosition, mAnchorLastRotation, Vector3.one);
			Matrix4x4 matrix4x2 = Matrix4x4.TRS(anchor.position, anchor.rotation, Vector3.one);
			Vector3 vector2 = matrix4x.inverse.MultiplyVector(transform.forward);
			Vector3 forward = Quaternion.AngleAxis(mFrameEuler.x, right) * Quaternion.AngleAxis(mFrameEuler.y, vector) * matrix4x2.MultiplyVector(vector2);
			Quaternion rotation = Quaternion.LookRotation(forward, vector);
			Vector3 b = Vector3.zero;
			if (!_UseRigAnchor && _Anchor != null)
			{
				Vector3 a2 = _Anchor.position + _Anchor.rotation * _AnchorOffset;
				b = a2 - AnchorPosition;
			}
			mRigTransform.Position = a + b;
			mRigTransform.Rotation = rotation;
			return mRigTransform;
		}

		public override void PostRigLateUpdate()
		{
			base.PostRigLateUpdate();
			Transform anchor = Anchor;
			if (_IsActorMatchingRotation && RigController.ActiveMotor == this)
			{
				RigController._Transform.position = anchor.position + anchor.rotation * AnchorOffset + anchor.rotation * _Offset;
				RigController._Transform.rotation = anchor.rotation;
			}
			if (_RotateAnchor && anchor != null)
			{
				bool flag = _RotateAnchorAlias.Length == 0 || RigController.InputSource == null || RigController.InputSource.IsPressed(_RotateAnchorAlias);
				if (flag || mWasRotatingAnchor)
				{
					if (mCharacterController != null)
					{
						float angle = anchor.forward.HorizontalAngleTo(RigController.Transform.forward, anchor.up);
						Quaternion rhs = Quaternion.AngleAxis(angle, Vector3.up);
						mCharacterController.Yaw *= rhs;
						anchor.rotation = mCharacterController.Tilt * mCharacterController.Yaw;
					}
					else
					{
						float angle2 = anchor.forward.HorizontalAngleTo(RigController.Transform.forward, anchor.up);
						Quaternion quaternion = Quaternion.AngleAxis(angle2, Vector3.up);
						anchor.rotation *= quaternion;
					}
					mAnchorLastRotation = anchor.rotation;
					if (!flag)
					{
						_Euler.y = LocalYaw;
						_Euler.x = LocalPitch;
						_EulerTarget = _Euler;
						mViewVelocityY = 0f;
						mViewVelocityX = 0f;
						mWasRotatingAnchor = false;
					}
				}
			}
			else
			{
				mWasRotatingAnchor = false;
			}
		}

		public override void DeserializeMotor(string rDefinition)
		{
			base.DeserializeMotor(rDefinition);
			if (Anchor != null)
			{
				mCharacterController = InterfaceHelper.GetComponent<ICharacterController>(Anchor.gameObject);
			}
		}
	}
}
