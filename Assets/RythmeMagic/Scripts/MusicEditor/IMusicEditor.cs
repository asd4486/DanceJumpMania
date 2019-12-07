using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RythhmMagic.MusicEditor
{
	public interface IMusicEditor
	{
		void OnClickAddBeat(float time);

		void AdjustBeatInBeatList(EditorBeat key);

		void OnClickRemoveKey(float time);

		void AdjustKeysPos();

		EditorBeat FindBeatByTime(float time);
		EditorBeat FindClosestBeat(float targetTime);
		EditorBeat FindClosestBeat(float targetTime, bool findNext);
	}
}