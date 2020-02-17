using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RythhmMagic
{
	public class ScoreData 
	{
		public int score;

		public int combo;
		public int maxCombo;

		public int prefectCount;
		public int goodCount;
		public int badCount;
		public int missCount;


		public void Reset()
		{
			score = 0;
			prefectCount = goodCount = badCount = missCount = 0;
			combo = maxCombo = 0;
		}
	}
}