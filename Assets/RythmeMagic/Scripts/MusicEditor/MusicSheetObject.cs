using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RythhmMagic
{
    public enum BeatTypes
    {
        Default,
        Holding
    }

	[CreateAssetMenu(fileName = "NewMusicSheet", menuName = "Rythm magic/MusicSheetObject", order = 1)]
    public class MusicSheetObject : ScriptableObject
	{
		public AudioClip music;
		public Beat[] beatList;

        [Serializable]
        public class Beat
        {
            public float time;
            public BeatItem[] items;
        }

		[Serializable]
		public class BeatItem
		{
            public BeatTypes type;
			public Vector2 startPos;
            public HoldingPos[] holdingPos;
		}

        [Serializable]
        public class HoldingPos
        {
            public float time;
            public Vector2 pos;
        }
	}
}
