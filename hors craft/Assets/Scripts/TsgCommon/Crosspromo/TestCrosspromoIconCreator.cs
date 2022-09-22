// DecompilerFi decompiler from Assembly-CSharp.dll class: TsgCommon.Crosspromo.TestCrosspromoIconCreator
using UnityEngine;

namespace TsgCommon.Crosspromo
{
	public class TestCrosspromoIconCreator : MonoBehaviour
	{
		public Texture2D texture;

		public RectTransform pivotRectTransform;

		private void Start()
		{
			Canvas componentInParent = pivotRectTransform.GetComponentInParent<Canvas>();
			Vector2 percentPosOnScreen = UIUtils.GetPercentPosOnScreen(pivotRectTransform);
			new CrosspromoIconCreator(componentInParent, percentPosOnScreen, "Test!", texture, delegate
			{
				Application.OpenURL("https://liaoid.dfppl");
			}, xButtonEnabled: false, delegate
			{
			}, 0, string.Empty);
		}
	}
}
