using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace RythhmMagic.MusicEditor
{
	public enum MoveModes
	{
		Free,
		Magnet
	}

	public class MusicEditorMain : MonoBehaviour
	{
		MarkerEditor markerEditor;
		float bpm;

		[SerializeField] MusicSheetObject musicSheet;

		[SerializeField] AudioSource myAudio;
		float clipLenght;

		[SerializeField] Button btnMetronome;
		[SerializeField] AudioSource metronomeAudio;

		[SerializeField] ScrollRect mapScrollRect;
		[SerializeField] RectTransform musicMapContent;

		float defaultMapWidth;
		public float mapWidth { get; private set; }
		[SerializeField] float mapWidthOffset;
		int zoomStep = 1;

		[SerializeField] RawImage musicMapImage;
		[SerializeField] RawImage musicMapMiniImage;

		[SerializeField] Transform timeLineParent;
		[SerializeField] GameObject timeLineTextPrefab;
		List<RectTransform> timeLineTexts = new List<RectTransform>();

		[SerializeField] BtnMusicProgress progressBtn;
		[SerializeField] Text textProgress;

		[SerializeField] BeatInfoEditor beatInfoEditor;

		public MoveModes moveMode { get; private set; }
		[SerializeField] Button btnMagnet;

		void Start()
		{
			UnityEngine.XR.XRSettings.enabled = false;
			progressBtn.onDragAction += PauseMusic;
			progressBtn.onSetPosAction += ShowBeatMarkerPos;

			markerEditor = FindObjectOfType<MarkerEditor>();

			if (musicSheet == null || musicSheet.music == null) return;

			myAudio.clip = musicSheet.music;
			clipLenght = myAudio.clip.length;

			bpm = UniBpmAnalyzer.AnalyzeBpm(musicSheet.music);

			//draw music map
			Texture2D map = musicSheet.music.PaintWaveformSpectrum(9600, 100, Color.green);
			musicMapImage.texture = musicMapMiniImage.texture = map;
			defaultMapWidth = mapWidth = musicMapContent.sizeDelta.x - mapWidthOffset * 2;

			//create time line text
			var roundTime = clipLenght.RoundUp(5);
			int count = Mathf.RoundToInt(roundTime / 5);
			for (int i = 0; i < count; i++)
			{
				var o = Instantiate(timeLineTextPrefab);
				o.transform.SetParent(timeLineParent, false);
				var timeSpan = System.TimeSpan.FromMinutes(5 * i);
				if (5 * i % 10 == 0)
					o.GetComponent<Text>().text = timeSpan.Hours.ToString("00") + ":" + timeSpan.Minutes.ToString("00");
				else
					o.GetComponent<Text>().enabled = false;

				timeLineTexts.Add(o.GetComponent<RectTransform>());
			}
			SetupTimeLineTexts();

			beatInfoEditor.Init(musicSheet.beatList);

			btnMetronome.onClick.AddListener(OnClickActiveMetronome);
			btnMagnet.onClick.AddListener(OnClickActiveMagnet);
		}


		private void Update()
		{
			SetProgressText();

			if (myAudio.clip == null || !myAudio.isPlaying)
				return;

			var xPos = mapWidth * (myAudio.time / clipLenght);
			progressBtn.SetXPos(xPos);

			//auto follow progress when play
			if (mapScrollRect.horizontalScrollbar.value < (xPos - defaultMapWidth) / mapWidth)
				mapScrollRect.horizontalScrollbar.value = xPos / mapWidth;

			PlayMetronome();
		}


		void OnClickActiveMetronome()
		{
			metronomeAudio.enabled = !metronomeAudio.enabled;
			btnMetronome.image.color = metronomeAudio.enabled ? Color.white : new Color(1, 1, 1, 0.5f);
		}

		//for metronome 
		float nowBeatTime = -1;
		void PlayMetronome()
		{
			if (!metronomeAudio.enabled) return;

			//play metronome
			var currentBeat = beatInfoEditor.FindBeatByTimeInAllPiste(myAudio.time);
			if (currentBeat != null && currentBeat.info.time != nowBeatTime)
			{
				metronomeAudio.Play();
				nowBeatTime = currentBeat.info.time;
			}
		}

		private void SetProgressText()
		{
			var timeSpan = System.TimeSpan.FromMinutes(GetProgressTime());
			string hh = timeSpan.Hours.ToString("00");
			string mm = timeSpan.Minutes.ToString("00");
			string ss = timeSpan.Seconds.ToString("00");
			textProgress.text = hh + ":" + mm + ":" + ss;
		}

		public void OnClickSaveData()
		{
			musicSheet.beatList.Clear();

			for (int i = 0; i < beatInfoEditor.beatGroupsList.Count; i++)
			{
				foreach (var group in beatInfoEditor.beatGroupsList[i])
				{
					var newInfo = new MusicSheetObject.BeatInfo();
					newInfo.type = group.beatList.Count < 2 ? BeatTypes.Default : BeatTypes.Holding;
					foreach (var beat in group.beatList)
						newInfo.posList.Add(new MusicSheetObject.PosInfo() { time = beat.info.time.Round3Decimal(), pos = beat.info.pos });

					var beatObj = musicSheet.beatList.Where(b => Mathf.Abs(b.startTime - group.beatList[0].info.time.Round3Decimal()) < 0.02f).FirstOrDefault();
					if (beatObj == null)
					{
						beatObj = new MusicSheetObject.Beat();
						beatObj.startTime = group.beatList[0].info.time.Round3Decimal();

						musicSheet.beatList.Add(beatObj);
					}
					beatObj.infos.Add(newInfo);
				}
			}

			musicSheet.SaveData(JsonUtility.ToJson(musicSheet));
			EditorGUIUtility.PingObject(musicSheet);
			Debug.Log("Data saved");
		}

		public void OnClickPlayMusic()
		{
			if (myAudio.isPlaying) PauseMusic();
			else
			{
				myAudio.time = GetProgressTime();
				myAudio.Play();
			}
		}

		public void PauseMusic()
		{
			nowBeatTime = -1;
			myAudio.Pause();
		}

		public void OnClickStopMusic()
		{
			progressBtn.SetXPos(0);
			mapScrollRect.horizontalScrollbar.value = 0;
			myAudio.Stop();
		}

		public void OnClickAddKey()
		{
			beatInfoEditor.OnClickAddBeat(GetProgressTime());
		}

		public void OnClickAddBeatInGroup()
		{
			beatInfoEditor.OnClickAddBeatInGroup(GetProgressTime());
		}

		public void OnClickRemoveKey()
		{
			beatInfoEditor.OnClickRemoveBeat(GetProgressTime());
		}

		public void OnClickFindKey(bool findNext)
		{
			var piste = beatInfoEditor.GetSelectedPiste();
			if (piste == null) return;

			var beat = beatInfoEditor.FindClosestBeat(GetProgressTime(), piste.Value, findNext);
			if (beat == null) return;

			PauseMusic();
			progressBtn.SetXPos(beat.rectTransfom.anchoredPosition.x);
		}

		public void OnClickZoom(bool zoomIn)
		{
			zoomStep += zoomIn ? 1 : -1;
			zoomStep = Mathf.Clamp(zoomStep, 1, 50);

			var progressTime = GetProgressTime();

			mapWidth = defaultMapWidth * zoomStep;
			musicMapContent.sizeDelta = new Vector2(mapWidth + mapWidthOffset * 2, musicMapContent.sizeDelta.y);

			//adjust progress button
			progressBtn.SetXPos(GetPositionByTime(progressTime));

			SetupTimeLineTexts();

			//adjust all object
			beatInfoEditor.AdjustBeatPos();
		}

		public float GetPositionByTime(float time)
		{
			return time / clipLenght * mapWidth;
		}

		public float GetTimeByPosition(float xPos)
		{
			return xPos / mapWidth * clipLenght;
		}

		float GetProgressTime()
		{
			return GetTimeByPosition(progressBtn.rectTransfom.anchoredPosition.x);
		}

		void SetupTimeLineTexts()
		{
			var roundTime = clipLenght.RoundUp(5);
			var roundWidth = roundTime / clipLenght * mapWidth;
			var dist = roundWidth / timeLineTexts.Count;
			for (int i = 0; i < timeLineTexts.Count; i++)
				timeLineTexts[i].anchoredPosition = new Vector2(dist * i, 0);
		}

		public void OnClickActiveMagnet()
		{
			moveMode = moveMode == MoveModes.Free ? MoveModes.Magnet : MoveModes.Free;
			btnMagnet.GetComponent<Image>().color = moveMode == MoveModes.Free ? Color.white : Color.green;
		}

		//magnet mode
		public EditorBeat AttractToBeat(float xPos)
		{
			var closestBeat = beatInfoEditor.FindClosestBeatInAllPiste(GetTimeByPosition(xPos));

			if (closestBeat == null) return null;
			if (Mathf.Abs(xPos - closestBeat.rectTransfom.anchoredPosition.x) < 10)
				return closestBeat;

			return null;
		}

		public void ShowBeatMarkerPos()
		{
			var time = GetProgressTime();

			for (int i = 0; i < beatInfoEditor.beatGroupsList.Count; i++)
			{
				EditorBeat beat = null;
				var group = beatInfoEditor.FindBeatGroupByTime(time, (BeatPiste)i);

				if (group != null)
				{
					var pos = group.GetTimeCurrentPos(time, out beat);
					markerEditor.SetMarkerBeat(i, beat);
					markerEditor.SetMarkerPos(i, pos);
					markerEditor.ActiveMarker(i, true);
				}
				else
					markerEditor.ActiveMarker(i, false);
			}
		}

		#region copy paste func
		BeatGroupInfo copiedGroupInfo;

		public void OnClickCopyBeat()
		{
			var piste = beatInfoEditor.GetSelectedPiste();
			if (piste == null) return;
			var group = beatInfoEditor.FindBeatGroupByTime(GetProgressTime(), piste.Value);
			if (group == null) return;

			var beatInfos = new List<BeatInfo>();
			foreach (var b in group.beatList)
				beatInfos.Add(b.info);

			copiedGroupInfo = new BeatGroupInfo(beatInfos);
			Debug.Log("copied");
		}

		public void OnClickPasteBeat()
		{
			if (copiedGroupInfo == null) return;
			beatInfoEditor.OnClickPasteBeatGroup(copiedGroupInfo, GetProgressTime());
		}
		#endregion
	}
}