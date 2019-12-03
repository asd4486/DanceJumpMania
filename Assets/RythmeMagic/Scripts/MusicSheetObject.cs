using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RythhmMagic
{
	[CreateAssetMenu(fileName = "NewMusicSheet", menuName = "Rythm magic/MusicSheetObject", order = 1)]
	public class MusicSheetObject : ScriptableObject
	{
		public AudioClip music;
		public Beat[] beatList;

		[Serializable]
		public class Beat
		{
			public float time;
			public Vector2[] positions;
		}
	}
}
