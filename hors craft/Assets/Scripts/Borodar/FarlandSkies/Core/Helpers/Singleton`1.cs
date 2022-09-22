// DecompilerFi decompiler from Assembly-CSharp.dll class: Borodar.FarlandSkies.Core.Helpers.Singleton`1
using UnityEngine;

namespace Borodar.FarlandSkies.Core.Helpers
{
	public class Singleton<T> : MonoBehaviour where T : Component
	{
		private static T _instance;

		public static T get
		{
			get
			{
				if ((Object)_instance != (Object)null)
				{
					T[] array = UnityEngine.Object.FindObjectsOfType<T>();
					if (array.Length == 1)
					{
						_instance = array[0];
					}
					else if (array.Length > 1)
					{
						UnityEngine.Debug.LogError(typeof(T) + ": There is more than 1 instance in the scene.");
					}
					else
					{
						UnityEngine.Debug.LogError(typeof(T) + ": Instance doesn't exist in the scene.");
					}
				}
				return _instance;
			}
		}
	}
}
