// DecompilerFi decompiler from Assembly-CSharp.dll class: Cooking.Recipe
using System;
using UnityEngine;

namespace Cooking
{
	[Serializable]
	public class Recipe
	{
		public GameObject product;

		public GameObject device;

		public GameObject result;

		public Product productScript => (!(product == null)) ? product.GetComponent<Product>() : null;

		public IUsable deviceScript => device.GetComponent<IUsable>();

		public Product resultScript => result.GetComponent<Product>();

		public override string ToString()
		{
			return ((!(product != null)) ? "null" : productScript.GetKey()) + " " + deviceScript.GetKey() + " " + resultScript.GetKey();
		}
	}
}
