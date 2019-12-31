using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RythhmMagic.MusicEditor
{
	public class EditorMarker : MonoBehaviour
	{
		public EditorBeat currentBeat { get; private set; }

		TextMeshPro textPos;
		// Start is called before the first frame update
		void Awake()
		{
			textPos = GetComponentInChildren<TextMeshPro>();
		}

		public void SetCurrentBeat(EditorBeat beat)
		{
			currentBeat = beat;
		}

		public void SetPosition(Vector3 pos)
		{
			transform.localPosition = new Vector3(pos.x, pos.y);
			textPos.text = "X:" + transform.localPosition.x.ToString("F2") + "  Y:" + transform.localPosition.y.ToString("F2");
		}

		public void DragEnd()
		{
			//auto refresh beat postion
			if (currentBeat != null) currentBeat.pos = transform.localPosition;
		}
	}
}