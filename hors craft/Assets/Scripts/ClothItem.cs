// DecompilerFi decompiler from Assembly-CSharp.dll class: ClothItem
using UnityEngine;
using UnityEngine.UI;

public class ClothItem : MonoBehaviour
{
	public Skin skin;

	public Image clothImage;

	public Text numberOfAdsNeeded;

	public GameObject tvBg;

	public GameObject currencyLock;

	public Text currencyText;

	public void SetValues(Skin skin, int value, bool ads)
	{
		EnableOneOfLocks(ads);
		this.skin = skin;
		Revalidate(value);
	}

	public void Revalidate(int value)
	{
		Text text = numberOfAdsNeeded;
		string text2 = value.ToString();
		currencyText.text = text2;
		text.text = text2;
		if (value <= 0)
		{
			DisableAll();
		}
	}

	public void EnableOneOfLocks(bool wich)
	{
		tvBg.SetActive(wich);
		currencyLock.SetActive(!wich);
	}

	private void DisableAll()
	{
		tvBg.SetActive(value: false);
		currencyLock.SetActive(value: false);
	}
}
