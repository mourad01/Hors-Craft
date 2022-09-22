// DecompilerFi decompiler from Assembly-CSharp.dll class: States.Fragment
using UnityEngine;

namespace States
{
	public class Fragment : MonoBehaviour
	{
		protected FragmentStartParameter startParameter;

		public virtual void Init(FragmentStartParameter parameter)
		{
			startParameter = parameter;
			base.transform.localScale = Vector3.one;
			GetComponent<RectTransform>().offsetMin = Vector2.zero;
			GetComponent<RectTransform>().offsetMax = Vector2.zero;
		}

		public virtual void Destroy()
		{
		}

		public virtual void UpdateFragment()
		{
		}

		public virtual void Disable()
		{
		}

		public static T GetFragment<T>(FragmentComponent[] fragments) where T : Fragment
		{
			foreach (FragmentComponent fragmentComponent in fragments)
			{
				T component = fragmentComponent.instance.GetComponent<T>();
				if (component is T)
				{
					return component;
				}
			}
			return (T)null;
		}
	}
}
