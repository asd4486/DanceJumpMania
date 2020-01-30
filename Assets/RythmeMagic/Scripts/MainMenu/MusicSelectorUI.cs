using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RythhmMagic
{
	public class MusicSelectorUI : MonoBehaviour
	{
		MainMenu mainMenu;

		//music infos
		[SerializeField] Image imgCouverture;
		[SerializeField] Text textName;
		[SerializeField] Text textArtist;
		[SerializeField] Text textDuration;
		[SerializeField] Button[] btnsDifficulty;
		[SerializeField] Material matSelectedDifficulty;

		SongMusicSheets[] songMusicSheetsList;
		SongMusicSheets selectedSong;
		MusicDifficulty selectedDifficulty;

		public void Init(MainMenu menu, SongMusicSheets[] songList)
		{
			mainMenu = menu;
			songMusicSheetsList = songList;

			//load all music sheets
			LoadMusicSheet(0);

			btnsDifficulty[0].onClick.AddListener(() => OnClickSelectDifficulty(MusicDifficulty.Easy));
			btnsDifficulty[1].onClick.AddListener(() => OnClickSelectDifficulty(MusicDifficulty.Normal));
			btnsDifficulty[2].onClick.AddListener(() => OnClickSelectDifficulty(MusicDifficulty.Hard));
		}

		void LoadMusicSheet(int index)
		{
			selectedSong = songMusicSheetsList[index];

			var musicSheet = selectedSong.sheets[0];
			imgCouverture.sprite = musicSheet.couverture;
			textName.text = musicSheet.name;
			textArtist.text = musicSheet.artistName;

			var timeSpan = System.TimeSpan.FromMinutes(musicSheet.duration > 0 ? musicSheet.duration : musicSheet.music.length);
			textDuration.text = timeSpan.Hours.ToString("00") + ":" + timeSpan.Minutes.ToString("00");

			ShowDifficulty(selectedSong);
		}

		void ShowDifficulty(SongMusicSheets song)
		{
			foreach (var b in btnsDifficulty)
				b.gameObject.SetActive(false);

			foreach (var sheet in song.sheets)
			{
				switch (sheet.difficulty)
				{
					case MusicDifficulty.Easy:
						btnsDifficulty[0].gameObject.SetActive(true);
						break;
					case MusicDifficulty.Normal:
						btnsDifficulty[1].gameObject.SetActive(true);
						break;
					case MusicDifficulty.Hard:
						btnsDifficulty[2].gameObject.SetActive(true);
						break;
				}
			}

			//auto set start difficulty
			for (int i = 0; i < btnsDifficulty.Length; i++)
			{
				if (btnsDifficulty[i].gameObject.activeSelf)
				{
					OnClickSelectDifficulty((MusicDifficulty)i);
					break;
				}
			}
		}

		void OnClickSelectDifficulty(MusicDifficulty difficulty)
		{
			for (int i = 0; i < btnsDifficulty.Length; i++)
			{
				btnsDifficulty[i].image.material = i == (int)difficulty ? matSelectedDifficulty : null;
				btnsDifficulty[i].transform.DOScale(i == (int)difficulty ? new Vector3(1.1f, 1.1f) : Vector3.one, 0.1f);
			}
			selectedDifficulty = difficulty;
		}

		public void OnClickLaunchGame()
		{
			MusicSheetObject selectedSheet = null;
			foreach (var sheet in selectedSong.sheets)
			{
				if (sheet.difficulty == selectedDifficulty)
				{
					selectedSheet = sheet;
					break;
				}
			}

			if (selectedSheet == null)
				return;
			mainMenu.LaunchGame(selectedSheet);
		}
	}
}
