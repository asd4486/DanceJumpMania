using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RythhmMagic.MusicEditor
{
	public class BtnMusicProgress : EditorKeyBase
	{
		int beatTotal;

		public void Init(int _totalBpm)
		{
			beatTotal = _totalBpm;
		}
	}
}
