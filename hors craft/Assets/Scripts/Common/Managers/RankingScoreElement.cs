// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.RankingScoreElement
using UnityEngine;
using UnityEngine.UI;

namespace Common.Managers
{
	public class RankingScoreElement : MonoBehaviour
	{
		public Image background;

		public Sprite[] rankingSprites;

		public Image rankingImage;

		public Text rankingPosition;

		public Text rankingScore;

		public Image playerAvatar;

		public GameObject customPlayerDataElement;

		public void SetElement(RankingModel.ScoreData score)
		{
			string text = Manager.Get<TranslationsManager>().GetText("rankings.score.prefix", string.Empty);
			if (!string.IsNullOrEmpty(text))
			{
				text += " ";
			}
			rankingScore.text = text + score.score.ToString();
			if (score.position == -1)
			{
				rankingPosition.text = "-";
			}
			else
			{
				rankingPosition.text = (score.position + 1).ToString();
			}
			int num = 0;
			num = ((score.position != -1) ? Mathf.Min(rankingSprites.Length - 1, score.position) : (rankingSprites.Length - 1));
			rankingImage.sprite = rankingSprites[num];
			Manager.Get<RankingManager>().SetElementWithPlayerData(this, score);
		}

		public void AddCustomDataElement(string text)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(customPlayerDataElement, customPlayerDataElement.transform.parent, worldPositionStays: false);
			gameObject.transform.localScale = Vector3.one;
			gameObject.GetComponentInChildren<Text>().text = text;
			gameObject.gameObject.SetActive(value: true);
		}
	}
}
