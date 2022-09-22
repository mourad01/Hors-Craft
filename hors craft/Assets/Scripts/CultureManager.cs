// DecompilerFi decompiler from Assembly-CSharp.dll class: CultureManager
using System.Globalization;
using System.Threading;
using UnityEngine;

public class CultureManager : MonoBehaviour
{
	private void Awake()
	{
		Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
	}
}
