using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace RythhmMagic.MusicEditor
{
	public class MusicEditorMain : MonoBehaviour
	{
		float bpm;

		[SerializeField] MusicSheetObject musicSheet;

		[SerializeField] AudioSource myAudio;
		float clipLenght;

		[SerializeField] ScrollRect mapScrollRect;
		[SerializeField] RectTransform musicMapContent;
		float defaultMapWidth;
		public float mapWidth { get; private set; }
		float zoomStep = 1;

		[SerializeField] RawImage musicMapImage;

		[SerializeField] RectTransform musicProgressBtn;

		//for dragging object in music map
		public RectTransform RectRefPoint;

		[SerializeField] Button btnEditKey;

		[SerializeField] MusicEditor musicEditor;
		[SerializeField] BeatInfoEditor beatInfoEditor;
		IMusicEditor currentEditor;

		void Start()
		{
			UnityEngine.XR.XRSettings.enabled = false;

			if (musicSheet == null || musicSheet.music == null) return;

			myAudio.clip = musicSheet.music;
			clipLenght = myAudio.clip.length;

			bpm = UniBpmAnalyzer.AnalyzeBpm(musicSheet.music);
			musicProgressBtn.GetComponent<BtnMusicProgress>().Init(Mathf.RoundToInt(clipLenght / 60 * bpm));

			//draw music map
			Texture2D map = musicSheet.music.PaintWaveformSpectrum(4800, 90, Color.green);
			musicMapImage.texture = map;
			defaultMapWidth = mapWidth = musicMapContent.sizeDelta.x;

			musicEditor.Init(musicSheet.beatList);

			btnEditKey.onClick.AddListener(OnClickChangeEditMode);
			currentEditor = musicEditor;
			beatInfoEditor.gameObject.SetActive(false);
		}

		private void Update()
		{
			if (myAudio.clip == null || !myAudio.isPlaying)
				return;

			var xPos = mapWidth * (myAudio.time / clipLenght);
			musicProgressBtn.anchoredPosition = new Vector2(xPos, musicProgressBtn.anchoredPosition.y);

			//auto follow progress when play
			if (mapScrollRect.horizontalScrollbar.value < (xPos - defaultMapWidth) / mapWidth)
				mapScrollRect.horizontalScrollbar.value = xPos / mapWidth;
		}

		public void OnClickSaveData()
		{
			musicSheet.beatList.Clear();
			foreach (var beat in musicEditor.beatList)
			{
				if(beat.leftInfos.Count > 0 || beat.rightInfos.Count > 0)
				{
					var beatObj = new MusicSheetObject.Beat();
					//if()
					//beatObj.startTime

					//musicSheet.beatList.Add();
				}
			}
		}

		public void OnClickStartMusic()
		{
			myAudio.time = GetTimeByPosition(musicProgressBtn.anchoredPosition.x);
			myAudio.Play();
		}

		public void OnClickPauseMusic()
		{
			myAudio.Pause();
		}

		public void OnClickStopMusic()
		{
			musicProgressBtn.anchoredPosition = Vector2.zero;
			mapScrollRect.horizontalScrollbar.value = 0;
			myAudio.Stop();
		}

		public void OnClickAddKey()
		{
			currentEditor.OnClickAddBeat(GetTimeByPosition(musicProgressBtn.anchoredPosition.x));
		}

		//call when move a key by hand
		public void AdjustKeyInKeyList(EditorBeat beat)
		{
			currentEditor.AdjustBeatInBeatList(beat);
		}

		public void OnClickRemoveKey()
		{
			currentEditor.OnClickRemoveKey(GetTimeByPosition(musicProgressBtn.anchoredPosition.x));
		}

		public void OnClickFindKey(bool findNext)
		{
			var key = currentEditor.FindClosestBeat(GetTimeByPosition(musicProgressBtn.anchoredPosition.x), findNext);
			if (key != null)
			{
				myAudio.Pause();
				musicProgressBtn.anchoredPosition = new Vector2(key.GetComponent<RectTransform>().anchoredPosition.x, 0);
			}
		}

		public void OnClickZoom(bool zoomIn)
		{
			zoomStep += zoomIn ? 1f : -1f;
			zoomStep = Mathf.Clamp(zoomStep, 1, 20);

			mapWidth = defaultMapWidth * zoomStep;
			musicMapContent.sizeDelta = new Vector2(mapWidth, musicMapContent.sizeDelta.y);

			//adjust all object
			musicEditor.AdjustKeysPos();
			if (currentEditor is BeatInfoEditor) beatInfoEditor.AdjustKeysPos();
		}

		public float GetPositionByTime(float time)
		{
			return time / clipLenght * mapWidth;
		}

		public float GetTimeByPosition(float xPos)
		{
			return xPos / mapWidth * clipLenght;
		}

		public void OnClickChangeEditMode()
		{
			if (currentEditor is BeatInfoEditor)
			{
				btnEditKey.GetComponent<Image>().color = Color.white;
				currentEditor = musicEditor;
				//save changes of one beat
				beatInfoEditor.SaveChanges();
				beatInfoEditor.gameObject.SetActive(false);
				return;
			}

			var beat = musicEditor.FindBeatByTime(GetTimeByPosition(musicProgressBtn.anchoredPosition.x));
			if (beat == null) return;

			beatInfoEditor.Init(beat);
			currentEditor = beatInfoEditor;
			beatInfoEditor.gameObject.SetActive(true);
			btnEditKey.GetComponent<Image>().color = Color.green;
		}
	}
}