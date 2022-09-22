// DecompilerFi decompiler from Assembly-CSharp.dll class: Cooking.RecipesList
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cooking
{
	public class RecipesList : MonoBehaviour
	{
		public List<Recipe> recipes = new List<Recipe>();

		public GameObject burntDish;

		public int GetProductPrice(string productKey)
		{
			Recipe recipe = recipes.First((Recipe r) => r.resultScript.Key == productKey);
			IUsable deviceScript = recipe.deviceScript;
			return deviceScript.GetPrice() + ((recipe.product != null) ? GetProductPrice(recipe.productScript.Key) : 0);
		}

		public List<Recipe> GetRecipesToProduct(Product product, List<Recipe> recipesToProduct = null)
		{
			if (recipesToProduct == null)
			{
				recipesToProduct = new List<Recipe>();
			}
			Recipe recipe = (from r in recipes
				where r.resultScript.Key == product.Key
				select r).First();
			recipesToProduct.Add(recipe);
			if (recipe.deviceScript is Product)
			{
				recipesToProduct = GetRecipesToProduct(recipe.deviceScript as Product, recipesToProduct);
			}
			if (recipe.product != null)
			{
				return GetRecipesToProduct(recipe.productScript, recipesToProduct);
			}
			return recipesToProduct;
		}

		public List<Product> GetAvaibleProducts()
		{
			return (from p in FindViableProductsRec()
				select p.GetComponent<Product>()).ToList();
		}

		private List<GameObject> FindViableProductsRec(HashSet<GameObject> viableProducts = null)
		{
			if (viableProducts == null)
			{
				List<Recipe> source = (from r in recipes
					where r.product == null && r.deviceScript.Unlocked()
					select r).ToList();
				viableProducts = new HashSet<GameObject>(from r in source
					select r.result);
				return FindViableProductsRec(viableProducts);
			}
			bool foundSomething = false;
			viableProducts = AddNewProductsToList(viableProducts, out foundSomething);
			if (foundSomething)
			{
				return FindViableProductsRec(viableProducts);
			}
			return FilterViableProducts(viableProducts);
		}

		private HashSet<GameObject> AddNewProductsToList(HashSet<GameObject> viableProducts, out bool foundSomething)
		{
			foundSomething = false;
			List<GameObject> list = new List<GameObject>();
			foreach (GameObject viableProduct in viableProducts)
			{
				foreach (Recipe recipe in recipes)
				{
					if (recipe.product == viableProduct && recipe.deviceScript.Unlocked())
					{
						list.Add(recipe.result);
					}
				}
			}
			foreach (GameObject item in list)
			{
				if (viableProducts.Add(item))
				{
					foundSomething = true;
				}
			}
			return viableProducts;
		}

		private List<GameObject> FilterViableProducts(HashSet<GameObject> viableProducts)
		{
			return (from p in viableProducts
				where p.GetComponent<Product>().canBeOrdered
				select p).ToList();
		}

		public bool CanUse(Product product, IUsable device)
		{
			return device.Unlocked() && GetResult(product, device) != null;
		}

		public GameObject GetResult(Product product, IUsable device)
		{
			Recipe recipe = null;
			if (product != null)
			{
				recipe = recipes.FirstOrDefault((Recipe r) => r.product != null && r.productScript.GetKey() == product.GetKey() && r.deviceScript.GetKey() == device.GetKey());
				if (recipe == null && device is Product)
				{
					recipe = recipes.FirstOrDefault((Recipe r) => r.product != null && r.productScript.GetKey() == device.GetKey() && r.deviceScript.GetKey() == product.GetKey());
				}
			}
			else
			{
				recipe = recipes.FirstOrDefault((Recipe r) => r.product == null && r.deviceScript.GetKey() == device.GetKey());
			}
			return recipe?.result;
		}

		public bool IsProductUnlocked(Product product)
		{
			List<Recipe> source = (from r in recipes
				where r.resultScript.GetKey() == product.GetKey()
				select r).ToList();
			return source.Any((Recipe r) => r.deviceScript.Unlocked() && (r.product == null || IsProductUnlocked(r.productScript)));
		}
	}
}
