using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace RythhmMagic.MusicEditor
{
    public class MusicEditorMain : MonoBehaviour
    {
        [SerializeField] MusicSheetObject musicSheet;
        List<MusicEditorKey> keyList = new List<MusicEditorKey>();

        [SerializeField] AudioSource myAudio;

        [SerializeField] RawImage musicMapImage;
        public float mapWidth { get; private set; }

        [SerializeField] RectTransform musicProgressBtn;

        [SerializeField] Transform keyParent;
        [SerializeField] MusicEditorKey keyPrefab;

        void Start()
        {
            UnityEngine.XR.XRSettings.enabled = false;

            if (musicSheet == null) return;

            myAudio.clip = musicSheet.music;

            //draw music map
            Texture2D map = musicSheet.music.PaintWaveformSpectrum(1280, 90, Color.green);
            musicMapImage.texture = map;
            mapWidth = musicMapImage.transform.parent.GetComponent<RectTransform>().sizeDelta.x;
        }

        private void Update()
        {
            if (myAudio.clip == null || !myAudio.isPlaying)
                return;

            var xPos = mapWidth * (myAudio.time / myAudio.clip.length);
            musicProgressBtn.anchoredPosition = new Vector2(xPos, musicProgressBtn.anchoredPosition.y);
        }

        public void OnClickStartMusic()
        {
            myAudio.time = GetProgressTime();
            myAudio.Play();
        }

        public void OnClickPauseMusic()
        {
            myAudio.Pause();
        }

        public void OnClickStopMusic()
        {
            myAudio.Stop();
        }

        public void OnClickAddKey()
        {
            var time = GetProgressTime();
            //don't create key when key existed
            if (FindKeyByTime(time) != null)
                return;

            var o = Instantiate(keyPrefab.gameObject);
            o.transform.SetParent(keyParent, false);

            var key = o.GetComponent<MusicEditorKey>();
            key.Init(musicProgressBtn.anchoredPosition.x, time);

            AddKeyToList(key);
        }

        void AddKeyToList(MusicEditorKey key)
        {
            if (keyList.Count < 1)
            {
                keyList.Add(key);
                return;
            }

            var index = 0;
            var keyTime = key.Time;

            for (int i = 0; i < keyList.Count; i++)
            {
                if (keyTime < keyList[i].Time)
                    index += 1;
                else
                    break;
            }

            keyList.Insert(index, key);
        }

        public void OnClickRemoveKey()
        {
            var time = GetProgressTime();
            var key = FindKeyByTime(time);
            //return when can't find key
            if (key == null)
                return;

            keyList.Remove(key);
            Destroy(key.gameObject);
        }

        public void OnClickFindKey(bool findNext)
        {
            var key = FindClosestKey(findNext);
            if (key != null)
            {
                myAudio.Pause();
                musicProgressBtn.anchoredPosition = new Vector2(key.GetComponent<RectTransform>().anchoredPosition.x, 0);
            }
        }

        MusicEditorKey FindKeyByTime(float time)
        {
            foreach (var k in keyList)
            {
                if (Mathf.Abs(k.Time - time) < .01) return k;
            }

            return null;
        }

        MusicEditorKey FindClosestKey(bool findNext)
        {
            MusicEditorKey closestKey = null;
            if (keyList.Count < 1) return closestKey;

            var progressTime = GetProgressTime();

            List<MusicEditorKey> list = new List<MusicEditorKey>();
            if (findNext) list = keyList.Where(k => k.Time > progressTime).ToList();
            else list = keyList.Where(k => k.Time < progressTime).ToList();

            var closestTime = float.MaxValue;

            foreach (var k in list)
            {
                var time = Mathf.Abs(k.Time - progressTime);
                if (time < closestTime && time >= .01)
                {
                    closestTime = time;
                    closestKey = k;
                }
            }

            return closestKey;
        }

        //get time by progress button position
        float GetProgressTime()
        {
            var time = musicProgressBtn.anchoredPosition.x / mapWidth * myAudio.clip.length;
            return time;
        }
    }
}