using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RythhmMagic
{
	public class ResultUI : MonoBehaviour
	{
		[SerializeField] Image imgCouverture;
		[SerializeField] Text textName;
		[SerializeField] Text textArtist;

		[SerializeField] Text textPrefectCount;
		[SerializeField] Text textGoodCount;
		[SerializeField] Text textBadCount;
		[SerializeField] Text textMissCount;

		[SerializeField] Text textMaxCombo;
		[SerializeField] Text textScore;

		public void SetInfos(MusicSheetObject musicSheet, ScoreData data)
		{
			imgCouverture.sprite = musicSheet.couverture;
			textName.text = musicSheet.name;
			textArtist.text = musicSheet.artistName;

			textPrefectCount.text = data.prefectCount.ToString();
			textGoodCount.text = data.goodCount.ToString();
			textBadCount.text = data.badCount.ToString();
			textMissCount.text = data.missCount.ToString();

			textMaxCombo.text = data.maxCombo.ToString();
			textScore.text = data.score.ToString();
		}
	}
}
