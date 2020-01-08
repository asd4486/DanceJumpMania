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
		[SerializeField] string markerTag;
		[SerializeField] InputField enterX;
		[SerializeField] InputField enterY;
		[SerializeField] Dropdown markerTypeDp;
		EditorBeatGroup currentBeatGroup;
		public EditorBeat currentBeat { get; private set; }

		[SerializeField] LineRenderer lineRenderer;

		[SerializeField] MeshRenderer markerTypeRenderer;
		[SerializeField] Material defaultMat;
		[SerializeField] Material triggerMat;
		[SerializeField] Material twoHandMat;

		TextMeshPro textPos;
		// Start is called before the first frame update
		void Awake()
		{
			textPos = GetComponentInChildren<TextMeshPro>();
			enterX.onValueChanged.AddListener(EnterXPos);
			enterY.onValueChanged.AddListener(EnterYPos);

			enterX.interactable = enterY.interactable = false;
			markerTypeRenderer.material = defaultMat;

			markerTypeDp.onValueChanged.AddListener(MarkerTypeChanged);
		}

		public void SetActive(bool active)
		{
			if (!active) SetCurrentBeat(null);
			gameObject.SetActive(active);
		}

		public void SetCurrentBeatGroup(EditorBeatGroup group)
		{
			currentBeatGroup = group;
			markerTypeDp.enabled = currentBeatGroup != null;

			var posList = new List<Vector3>();
			posList.Add(Vector3.zero);
			if (currentBeatGroup != null)
			{
				markerTypeDp.onValueChanged.RemoveAllListeners();
				markerTypeDp.value = (int)currentBeatGroup.markerType;
				markerTypeDp.onValueChanged.AddListener(MarkerTypeChanged);

				if (currentBeatGroup.beatList.Count > 1)
				{
					posList.Clear();
					foreach (var b in currentBeatGroup.beatList)
						posList.Add(b.info.pos);
				}
				ChangeMarkerTypeMat();
			}

			lineRenderer.positionCount = posList.Count;
			lineRenderer.SetPositions(posList.ToArray());
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

			textPos.text = markerTag + " X:" + strX + "  Y:" + strY;
			RefreshMarkerLinePos();

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
			if (currentBeat != null) currentBeat.info.pos = transform.localPosition;
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
			textPos.text = markerTag + "X:" + strX + "  Y:" + strY;
			RefreshMarkerLinePos();

			currentBeat.info.pos = transform.localPosition;
		}

		void MarkerTypeChanged(int index)
		{
			if (currentBeatGroup == null) return;
			currentBeatGroup.markerType = (MarkerType)index;
			ChangeMarkerTypeMat();
		}

		void RefreshMarkerLinePos()
		{
			if (currentBeatGroup == null || currentBeatGroup.beatList.Count < 2) return;
			var posList = new List<Vector3>();

			foreach (var b in currentBeatGroup.beatList)
				posList.Add(b.info.pos);
			lineRenderer.SetPositions(posList.ToArray());
		}

		void ChangeMarkerTypeMat()
		{
			switch (currentBeatGroup.markerType)
			{
				case MarkerType.Default:
					markerTypeRenderer.material = lineRenderer.material = defaultMat;
					break;
				case MarkerType.Trigger:
					markerTypeRenderer.material = lineRenderer.material = triggerMat;
					break;
				case MarkerType.TwoHand:
					markerTypeRenderer.material = lineRenderer.material = twoHandMat;
					break;
			}
		}
	}
}