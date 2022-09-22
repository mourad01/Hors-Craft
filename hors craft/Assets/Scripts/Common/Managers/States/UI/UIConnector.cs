// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.States.UI.UIConnector
using System;
using UnityEngine;

namespace Common.Managers.States.UI
{
	public class UIConnector : MonoBehaviour
	{
		protected void AssertNotNull(params UnityEngine.Object[] objects)
		{
			int num = 0;
			while (true)
			{
				if (num < objects.Length)
				{
					UnityEngine.Object x = objects[num];
					if (x == null)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			throw new Exception("All params in " + GetType() + " has to be filled!");
		}
	}
}
