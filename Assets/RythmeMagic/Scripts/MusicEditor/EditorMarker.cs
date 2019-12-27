using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RythhmMagic.MusicEditor
{
	public class EditorMarker : MonoBehaviour
	{
		EditorBeat currentBeat;

		TextMeshPro textPos;
		// Start is called before the first frame update
		void Awake()
		{
			textPos = GetComponentInChildren<TextMeshPro>();
		}

		public void SetCurrentBeat(EditorBeat beat)
		{
			currentBeat = beat;
			SetPosition(beat.pos);
		}

		public void SetPosition(Vector3 pos, bool isLocal = true)
		{
			if (!isLocal) transform.position = pos;
			else transform.localPosition = pos;

			textPos.text = "X:" + transform.localPosition.x.ToString("F2") + "  Y:" + transform.localPosition.y.ToString("F2");
		}

		public void DragEnd()
		{
			//auto refresh beat postion
			if (currentBeat != null) currentBeat.pos = transform.localPosition;
		}
	}
}