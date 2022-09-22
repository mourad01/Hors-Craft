// DecompilerFi decompiler from Assembly-CSharp.dll class: TsgCommon.Crosspromo.SimpleIconSiblingAligner
using UnityEngine;

namespace TsgCommon.Crosspromo
{
	public class SimpleIconSiblingAligner : MonoBehaviour
	{
		private void OnGUI()
		{
			if (base.transform.parent != null)
			{
				base.transform.parent.SetParent(base.transform.root, worldPositionStays: false);
				base.transform.SetAsLastSibling();
			}
		}
	}
}
