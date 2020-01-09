using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace RythhmMagic
{
    public enum BeatTypes
    {
        Default,
        Holding
    }

    public enum MarkerType
    {
        Default,
        Trigger,
        TwoHand
    }

    [CreateAssetMenu(fileName = "NewMusicSheet", menuName = "Rythm magic/MusicSheetObject", order = 1)]
    public class MusicSheetObject : ScriptableObject
    {
        public AudioClip music;

        public Sprite couverture;
        public string name;
        public string artistName;

        public float completeTime;
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
            [FormerlySerializedAs("type")] public BeatTypes beatType;
            public MarkerType markerType;
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
            EditorUtility.SetDirty(this);
        }
    }
}
