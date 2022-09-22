// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Behaviours.Reactors.AddForceReactor
using UnityEngine;

namespace Common.Behaviours.Reactors
{
	public class AddForceReactor : AbstractRestorableReactor
	{
		[Header("Who's 'pushing' the object forward. For example, set player weapon here.")]
		public Transform pushOrigin;

		[Header("Force parameters.")]
		public float forcePower = 100f;

		public ForceMode forceMode = ForceMode.Impulse;

		[Header("Destination. Leave empty if rigidbody is self.")]
		public Rigidbody applyForceTo;

		[Header("Rigidbody parameters to set on react.")]
		public bool useGravity;

		public bool isKinematic;

		private Rigidbody body;

		private Vector3 basePosition;

		private Quaternion baseRotation;

		private Vector3 baseScale;

		private bool baseUseGravity;

		private bool baseIsKinematic;

		private void Awake()
		{
			body = ((!(applyForceTo != null)) ? GetComponent<Rigidbody>() : applyForceTo);
			basePosition = base.transform.position;
			baseRotation = base.transform.rotation;
			baseScale = base.transform.localScale;
			baseUseGravity = body.useGravity;
			baseIsKinematic = body.isKinematic;
		}

		public override void React()
		{
			body.useGravity = useGravity;
			body.isKinematic = isKinematic;
			Vector3 normalized = (body.transform.position - pushOrigin.position).normalized;
			body.AddForce(normalized * forcePower, forceMode);
		}

		public override void Restore()
		{
			body.useGravity = baseUseGravity;
			body.isKinematic = baseIsKinematic;
			base.transform.position = basePosition;
			base.transform.rotation = baseRotation;
			base.transform.localScale = baseScale;
		}
	}
}
