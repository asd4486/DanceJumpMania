using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RythhmMagic.MusicEditor
{
    public class BtnMusicProgress : MonoBehaviour
    {
        MusicEditorMain main;
        RectTransform myRect;
        [SerializeField] RectTransform startPoint;

        private void Awake()
        {
            main = FindObjectOfType<MusicEditorMain>();
            myRect = GetComponent<RectTransform>();
        }

        public void OnDragging()
        {
            main.OnClickPauseMusic();

            Vector2 movePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(startPoint, Input.mousePosition, main.GetComponent<Canvas>().worldCamera, out movePos);
            //set move limit
            var xPos = Mathf.Clamp(movePos.x, 0, main.mapWidth);
            myRect.anchoredPosition = new Vector2(xPos, 0);
        }

    }
}
