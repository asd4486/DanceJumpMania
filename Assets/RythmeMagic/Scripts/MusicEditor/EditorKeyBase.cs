using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RythhmMagic.MusicEditor
{
	[RequireComponent(typeof(EventTrigger))]
	public class EditorKeyBase : MonoBehaviour
	{
		public event System.Action onDragAction;

		protected MusicEditorMain main;
		public RectTransform rectTransfom { get; private set; }

		private void Awake()
		{
			main = FindObjectOfType<MusicEditorMain>();
			rectTransfom = GetComponent<RectTransform>();

			EventTrigger trigger = GetComponent<EventTrigger>();
			EventTrigger.Entry dragEntry = new EventTrigger.Entry();
			dragEntry.eventID = EventTriggerType.Drag;
			dragEntry.callback.AddListener(OnDragSetPos);
			trigger.triggers.Add(dragEntry);

			EventTrigger.Entry dragEndEntry = new EventTrigger.Entry();
			dragEndEntry.eventID = EventTriggerType.EndDrag;
			dragEndEntry.callback.AddListener(OnDragEnd);
			trigger.triggers.Add(dragEndEntry);
		}

		public virtual void OnDragSetPos(BaseEventData arg0)
		{
			if (onDragAction != null) onDragAction();

			Vector2 movePos;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(main.RectRefPoint, Input.mousePosition, main.GetComponent<Canvas>().worldCamera, out movePos);
			//var xPos = moveMode == MoveModes.Free ? movePos.x : GetClosestBeatPos(movePos.x);
			var xPos = movePos.x;
			xPos = Mathf.Clamp(xPos, 0, main.mapWidth);
			rectTransfom.anchoredPosition = new Vector2(xPos, 0);
		}

		protected virtual void OnDragEnd(BaseEventData arg0) { }
	}
}