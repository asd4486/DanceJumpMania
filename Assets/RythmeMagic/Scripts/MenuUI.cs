using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.Extras;

namespace RythhmMagic
{
    public class MenuUI : MonoBehaviour
    {
        RythmMagicMain main;

        //music infos
        [SerializeField] Image imgCouverture;
        [SerializeField] Text textName;
        [SerializeField] Text textArtist;
        [SerializeField] Text textDuration;
        [SerializeField] Button[] btnDifficulty;

        [SerializeField] MusicSheetObject[] musicSheets;

        int selectCount;

        // Start is called before the first frame update
        void Start()
        {
            main = FindObjectOfType<RythmMagicMain>();

            //load all music sheets
            LoadMusicSheets();
        }

        void LoadMusicSheets()
        {
            var firstSheet = musicSheets[0];
            imgCouverture.sprite = firstSheet.couverture;
            textName.text = firstSheet.name;
            textArtist.text = firstSheet.artistName;

            var timeSpan = System.TimeSpan.FromMinutes(firstSheet.duration > 0 ? firstSheet.duration : firstSheet.music.length);
            textDuration.text = timeSpan.Hours.ToString("00") + ":" + timeSpan.Minutes.ToString("00");

            //foreach(var sheet in musicSheets) { }
        }

        public void OnClickLaunchGame()
        {
            main.StartGame(musicSheets[0]);
        }

        public void ActiveUI(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}