using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
		public List<Beat> beatList = new List<Beat>();

		[Serializable]
		public class Beat
		{
			public float startTime;
			public List<BeatInfo> infos = new List<BeatInfo>();
		}

		[Serializable]
		public class BeatInfo
		{
			public BeatTypes type;
			public List<PosInfo> posList = new List<PosInfo>();
		}

		[Serializable]
		public class PosInfo
		{
			public float time;
			public Vector2 pos;
		}

		public void SaveData(string data)
		{
			JsonUtility.FromJsonOverwrite(data, this);
		}
	}
}
