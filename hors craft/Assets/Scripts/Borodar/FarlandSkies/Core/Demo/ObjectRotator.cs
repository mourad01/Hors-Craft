// DecompilerFi decompiler from Assembly-CSharp.dll class: Borodar.FarlandSkies.Core.Demo.ObjectRotator
using UnityEngine;

namespace Borodar.FarlandSkies.Core.Demo
{
	public class ObjectRotator : MonoBehaviour
	{
		[SerializeField]
		protected Vector3 EulerAngles;

		protected void Update()
		{
			base.transform.Rotate(EulerAngles * Time.deltaTime);
		}
	}
}
