using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
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

		[SerializeField] BtnMusicProgress progressBtn;
		[SerializeField] Text textProgress;

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
			progressBtn.Init(Mathf.RoundToInt(clipLenght / 60 * bpm));

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
			SetProgressText();

			if (myAudio.clip == null || !myAudio.isPlaying)
				return;

			var xPos = mapWidth * (myAudio.time / clipLenght);
			progressBtn.rectTransfom.anchoredPosition = new Vector2(xPos, 0);

			//auto follow progress when play
			if (mapScrollRect.horizontalScrollbar.value < (xPos - defaultMapWidth) / mapWidth)
				mapScrollRect.horizontalScrollbar.value = xPos / mapWidth;
		}

		private void SetProgressText()
		{
			var timeSpan = System.TimeSpan.FromMinutes(GetTimeByPosition(progressBtn.rectTransfom.anchoredPosition.x));
			string hh = timeSpan.Hours.ToString("00");
			string mm = timeSpan.Minutes.ToString("00");
			string ss = timeSpan.Seconds.ToString("00");
			textProgress.text = hh + ":" + mm + ":" + ss;
		}

		public void OnClickSaveData()
		{
			musicSheet.beatList.Clear();
			foreach (var beat in musicEditor.beatList)
			{
				if (beat.beatInfoList.Count > 0)
				{
					var beatObj = new MusicSheetObject.Beat();
					beatObj.infos = new List<MusicSheetObject.BeatInfo>(beat.beatInfoList.Count);

					for (int i = 0; i < beat.beatInfoList.Count; i++)
					{
						var infos = beat.beatInfoList[i];
						beatObj.startTime = infos[0].time;

						var newInfo = new MusicSheetObject.BeatInfo();
						newInfo.type = infos.Count < 2 ? BeatTypes.Default : BeatTypes.Holding;
						foreach (var detail in infos)
							newInfo.posList.Add(new MusicSheetObject.PosInfo() { time = detail.time, pos = detail.pos });

						beatObj.infos.Add(newInfo);
					}
					musicSheet.beatList.Add(beatObj);
				}
			}

			musicSheet.SaveData(JsonUtility.ToJson(musicSheet));
			EditorGUIUtility.PingObject(musicSheet);
			Debug.Log("Data saved");
		}

		public void OnClickStartMusic()
		{
			myAudio.time = GetTimeByPosition(progressBtn.rectTransfom.anchoredPosition.x);
			myAudio.Play();
		}

		public void OnClickPauseMusic()
		{
			myAudio.Pause();
		}

		public void OnClickStopMusic()
		{
			progressBtn.rectTransfom.anchoredPosition = Vector2.zero;
			mapScrollRect.horizontalScrollbar.value = 0;
			myAudio.Stop();
		}

		public void OnClickAddKey()
		{
			currentEditor.OnClickAddBeat(GetTimeByPosition(progressBtn.rectTransfom.anchoredPosition.x));
			//auto switch to beat info edit mode if create a new beat
			if (currentEditor is MusicEditor) OnClickChangeEditMode();
		}

		//call when move a key by hand
		public void AdjustKeyInKeyList(EditorBeat beat)
		{
			currentEditor.AdjustBeatInBeatList(beat);
		}

		public void OnClickRemoveKey()
		{
			currentEditor.OnClickRemoveKey(GetTimeByPosition(progressBtn.rectTransfom.anchoredPosition.x));
		}

		public void OnClickFindKey(bool findNext)
		{
			var beat = currentEditor.FindClosestBeat(GetTimeByPosition(progressBtn.rectTransfom.anchoredPosition.x), findNext);
			if (beat == null)
				return;

			myAudio.Pause();
			progressBtn.rectTransfom.anchoredPosition = new Vector2(beat.GetComponent<RectTransform>().anchoredPosition.x, 0);
			//refresh markers info
			if (currentEditor is BeatInfoEditor) beatInfoEditor.SetBeatInfoPosToMarker();
		}

		public void OnClickZoom(bool zoomIn)
		{
			zoomStep += zoomIn ? 1f : -1f;
			zoomStep = Mathf.Clamp(zoomStep, 1, 50);

			var progressTime = GetTimeByPosition(progressBtn.rectTransfom.anchoredPosition.x);

			mapWidth = defaultMapWidth * zoomStep;
			musicMapContent.sizeDelta = new Vector2(mapWidth, musicMapContent.sizeDelta.y);

			//adjust progress button
			progressBtn.rectTransfom.anchoredPosition = new Vector2(GetPositionByTime(progressTime), 0);

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
				beatInfoEditor.QuitAndSave();
				beatInfoEditor.gameObject.SetActive(false);
				return;
			}

			var beat = musicEditor.FindBeatByTime(GetTimeByPosition(progressBtn.rectTransfom.anchoredPosition.x));
			if (beat == null) return;

			beatInfoEditor.Init(beat);
			currentEditor = beatInfoEditor;
			beatInfoEditor.gameObject.SetActive(true);
			btnEditKey.GetComponent<Image>().color = Color.green;
		}
	}
}