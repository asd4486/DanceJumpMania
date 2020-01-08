using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RythhmMagic.MusicEditor
{
	[RequireComponent(typeof(BoxCollider2D))]
	public class EditorKeyBase : MonoBehaviour
	{
		public event System.Action onDragAction;

		protected MusicEditorMain main;
		public RectTransform rectTransfom { get; private set; }
		protected BoxCollider2D col;

		private void Awake()
		{
			main = FindObjectOfType<MusicEditorMain>();
			rectTransfom = GetComponent<RectTransform>();
			col = GetComponent<BoxCollider2D>();
		}

		public virtual void OnDragStart(float xPos)
		{

		}

		public virtual void OnDragSetPos(float xPos)
		{
			if (onDragAction != null) onDragAction();
		}

		public virtual void OnDragEnd() { }
	}
}