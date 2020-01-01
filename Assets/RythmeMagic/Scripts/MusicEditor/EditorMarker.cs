using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Globalization;

namespace RythhmMagic.MusicEditor
{
	public class EditorMarker : MonoBehaviour
	{
		[SerializeField] InputField enterX;
		[SerializeField] InputField enterY;
		public EditorBeat currentBeat { get; private set; }

		TextMeshPro textPos;
		// Start is called before the first frame update
		void Awake()
		{
			textPos = GetComponentInChildren<TextMeshPro>();
			enterX.onValueChanged.AddListener(EnterXPos);
			enterY.onValueChanged.AddListener(EnterYPos);

			enterX.interactable = enterY.interactable = false;
		}

		public void SetActive(bool active)
		{
			if (!active) SetCurrentBeat(null);
			gameObject.SetActive(active);
		}

		public void SetCurrentBeat(EditorBeat beat)
		{
			currentBeat = beat;
			enterX.interactable = enterY.interactable = currentBeat != null;
		}

		public void DragSetPosition(Vector3 pos)
		{
			transform.localPosition = new Vector3(pos.x, pos.y);
			var strX = transform.localPosition.x.ToString("F2").Replace(',', '.');
			var strY = transform.localPosition.y.ToString("F2").Replace(',', '.');

			textPos.text = "X:" + strX + "  Y:" + strY;

			enterX.onValueChanged.RemoveAllListeners();
			enterY.onValueChanged.RemoveAllListeners();
			enterX.text = strX;
			enterY.text = strY;
			enterX.onValueChanged.AddListener(EnterXPos);
			enterY.onValueChanged.AddListener(EnterYPos);
		}

		public void DragEnd()
		{
			//auto refresh beat postion
			if (currentBeat != null) currentBeat.pos = transform.localPosition;
		}

		void EnterXPos(string value)
		{
			float xPos;
			if (float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out xPos))
			{
				transform.localPosition = new Vector3(xPos, transform.localPosition.y);
				EnterValueChanged();
			}
		}

		void EnterYPos(string value)
		{
			float yPos;
			if (float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out yPos))
			{
				transform.localPosition = new Vector3(transform.localPosition.x, yPos);
				EnterValueChanged();
			}
		}

		void EnterValueChanged()
		{
			var strX = transform.localPosition.x.ToString("F2").Replace(',', '.');
			var strY = transform.localPosition.y.ToString("F2").Replace(',', '.');
			textPos.text = "X:" + strX + "  Y:" + strY;
			currentBeat.pos = transform.localPosition;
		}
	}
}