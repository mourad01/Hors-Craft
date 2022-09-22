// DecompilerFi decompiler from Assembly-CSharp.dll class: Cooking.CookingBaseObject
using Common.Managers;
using Common.Utils;
using UnityEngine;

namespace Cooking
{
	public class CookingBaseObject : MonoBehaviour
	{
		public Sprite sprite;

		[SerializeField]
		private string uniqueName;

		private WorkController _workController;

		protected string UniqueName => (!string.IsNullOrEmpty(uniqueName)) ? uniqueName : base.gameObject.name.Replace("(Clone)", string.Empty);

		public string TranslatedName => Manager.Get<TranslationsManager>().GetText(Key, UniqueName).ToUpper();

		public string Key => Misc.CustomNameToKey(UniqueName);

		public virtual int Price => workController.recipesList.GetProductPrice(Key);

		protected WorkController workController
		{
			get
			{
				if (_workController == null)
				{
					_workController = GetComponentInParent<WorkController>();
				}
				return _workController;
			}
		}
	}
}
