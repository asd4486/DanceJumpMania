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

		[SerializeField] Text textGoodCount;
		[SerializeField] Text textMissCount;
		[SerializeField] Text textCombo;
		[SerializeField] Text textScore;

		public void SetInfos(MusicSheetObject musicSheet, int good, int miss, int combo, int score)
		{
			imgCouverture.sprite = musicSheet.couverture;
			textName.text = musicSheet.name;
			textArtist.text = musicSheet.artistName;

			textGoodCount.text = good.ToString();
			textMissCount.text = miss.ToString();

			textCombo.text = combo.ToString();
			textScore.text = score.ToString();
		}
	}
}
