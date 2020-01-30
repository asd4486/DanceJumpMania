using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.Extras;

namespace RythhmMagic
{
	public class MainMenu : MonoBehaviour
	{
		RythmMagicMain main;

		[SerializeField] MusicSelectorUI selectorUI;
		[SerializeField] ResultUI resultUI;

		[SerializeField] SongMusicSheets[] songMusicSheetsList;
		MusicSheetObject selectedSheet;
		[SerializeField] Animator menuAnimator;

		// Start is called before the first frame update
		void Start()
		{
			main = FindObjectOfType<RythmMagicMain>();

			ShowMusicSelector();
			selectorUI.Init(this, songMusicSheetsList);
		}

		public void ActiveUI(bool active)
		{
			gameObject.SetActive(active);
		}

		void ShowMusicSelector()
		{
			menuAnimator.SetTrigger("show");
			resultUI.gameObject.SetActive(false);
			selectorUI.gameObject.SetActive(true);
		}

		public void ShowResult(int goodCount, int missCount, int maxCombo, int score)
		{
			menuAnimator.SetTrigger("show");
			resultUI.gameObject.SetActive(true);
			selectorUI.gameObject.SetActive(false);

			resultUI.SetInfos(selectedSheet, goodCount, missCount, maxCombo, score);
		}

		public void LaunchGame(MusicSheetObject sheet)
		{
			menuAnimator.SetTrigger("hide");
			selectedSheet = sheet;
			main.StartGame(selectedSheet);
		}

		Coroutine switchWindowCoroutine;
		public void OnClickSwitchWindow()
		{
			if (switchWindowCoroutine != null)
				return;
			switchWindowCoroutine = StartCoroutine(SwitchWindowCorou());
		}

		IEnumerator SwitchWindowCorou()
		{
			menuAnimator.SetTrigger("hide");
			yield return new WaitForSeconds(.2f);

			ShowMusicSelector();
			switchWindowCoroutine = null;
		}
	}

	[System.Serializable]
	public class SongMusicSheets
	{
		public List<MusicSheetObject> sheets;
	}
}