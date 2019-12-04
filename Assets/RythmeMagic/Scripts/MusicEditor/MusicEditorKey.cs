using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RythhmMagic.MusicEditor
{
    public class MusicEditorKey : MonoBehaviour
    {
        MusicSheetObject.Beat beat = new MusicSheetObject.Beat();
        public float Time { get { return beat.time; } }

        public void Init(float xPos, float time)
        {
            GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, 0);
            beat.time = time;
        }
    }
}