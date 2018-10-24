using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace StarstruckFramework
{
	public enum GUIScrollAxis
	{
		HORIZONTAL_ONLY,
		VERTICAL_ONLY,
		BOTH
	}

	public class GUILiteScrollRect : GUILiteImage, IBeginDragHandler, IEndDragHandler
	{
		[SerializeField]
		private GameObject m_container;

		public GameObject Container
		{
			get { return m_container; }
		}

		[SerializeField]
		private GameObject m_scrollView;

		public GameObject ScrollView
		{
			get { return m_scrollView; }
		}

		[SerializeField]
		private GUIScrollAxis m_axis;
		[SerializeField]
		private Scrollbar m_vertScrollbar;
		[SerializeField]
		private Scrollbar m_horiScrollbar;
		private float m_height = 0;
		private float m_width = 0;

		public delegate void DraggingStartDelegate ();

		private DraggingStartDelegate m_onDraggingStart;

		public delegate void DraggingEndDelegate ();

		private DraggingEndDelegate m_onDraggingEnd;

		private Sprite m_scrollBarFront;

		public override void Awake ()
		{
			base.Awake ();
			ResetContainerSize ();
		}

		public void AddChild (GameObject child, bool requiresReset = true)
		{
			Vector3 origScale = child.transform.localScale;
			Vector3 origPos = child.transform.localPosition;

			child.transform.SetParent (m_container.transform);
			child.transform.localScale = origScale;
			child.transform.localPosition = origPos;

			if (requiresReset)
			{
				ResetContainerSize ();
			}
		}

		public GameObject CloneAsChild (GameObject template, bool requiresReset = true)
		{
			GameObject clone = GameObject.Instantiate<GameObject> (template,
				                  m_container.transform);

			if (requiresReset)
			{
				ResetContainerSize ();
			}

			return clone;
		}

		public void ResetContainerSize (float adjustment = 0)
		{
			m_container.GetComponent<RectTransform> ().SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, 0.0f);
			m_container.GetComponent<RectTransform> ().SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, 0.0f);

			Rect boundingContainer = BoundingContainer (m_container, m_container.transform.lossyScale);

			m_container.GetComponent<RectTransform> ().SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal,
				(boundingContainer.width));

			m_container.GetComponent<RectTransform> ().SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical,
				(boundingContainer.height));

			if (m_vertScrollbar != null
			   && (m_axis == GUIScrollAxis.BOTH || m_axis == GUIScrollAxis.VERTICAL_ONLY))
			{
				m_vertScrollbar.size = m_height /
				m_container.GetComponent<RectTransform> ().rect.height;
			}

			if (m_horiScrollbar != null
			   && (m_axis == GUIScrollAxis.BOTH || m_axis == GUIScrollAxis.HORIZONTAL_ONLY))
			{
				m_horiScrollbar.size = m_width /
				m_container.GetComponent<RectTransform> ().rect.width;
			}

			if (adjustment != 0)
			{
				AdjustScrollViewHeight (adjustment);
			}
		}

		private Rect BoundingContainer (GameObject gob, Vector3 scale)
		{
			Vector2 bottomLeft = Vector2.zero;
			Vector2 topRight = Vector2.zero;
			RectTransform rectTrans = gob.GetComponent<RectTransform> ();
			Transform trans = gob.transform;
			Vector2 pivot = rectTrans.pivot;

			Vector3 pos = trans.position;
			pos.x /= scale.x;
			pos.y /= scale.y;

			if (pivot.x == 0.0f)
			{
				bottomLeft.x = pos.x;
				topRight.x = pos.x + (rectTrans.rect.width * trans.localScale.x);
			}
			else if (pivot.x == 0.5f)
			{
				bottomLeft.x = pos.x - (rectTrans.rect.width * trans.localScale.x) / 2.0f;
				topRight.x = pos.x + (rectTrans.rect.width * trans.localScale.x) / 2.0f;
			}
			else if (pivot.x == 1.0f)
			{
				bottomLeft.x = pos.x - (rectTrans.rect.width * trans.localScale.x);
				topRight.x = pos.x;
			}

			if (pivot.y == 0.0f)
			{
				bottomLeft.y = pos.y;
				topRight.y = pos.y + (rectTrans.rect.height * trans.localScale.y);
			}
			else if (pivot.y == 0.5f)
			{
				bottomLeft.y = pos.y - (rectTrans.rect.height * trans.localScale.y) / 2.0f;
				topRight.y = pos.y + (rectTrans.rect.height * trans.localScale.y) / 2.0f;
			}
			else if (pivot.y == 1.0f)
			{
				bottomLeft.y = pos.y - (rectTrans.rect.height * trans.localScale.y);
				topRight.y = pos.y;
			}

			foreach (Transform childTrans in gob.transform)
			{
				if (childTrans.gameObject.activeSelf && childTrans.gameObject.GetComponent<RectTransform> () != null)
				{
					Rect childBox = BoundingContainer (childTrans.gameObject, scale);

					bottomLeft.x = Mathf.Min (bottomLeft.x, childBox.x);
					bottomLeft.y = Mathf.Min (bottomLeft.y, childBox.y);
					topRight.x = Mathf.Max (topRight.x, childBox.x + childBox.width);
					topRight.y = Mathf.Max (topRight.y, childBox.y + childBox.height);
				}
			}

			float width = topRight.x - bottomLeft.x;
			float height = topRight.y - bottomLeft.y;

			return new Rect (bottomLeft.x, bottomLeft.y, width, height);
		}

		public void ClearScrollView (bool hasPosReset = true)
		{
			while (m_container.transform.childCount > 0)
			{
				GameObject child = m_container.transform.GetChild (0).gameObject;
				child.transform.SetParent (null);
				Destroy (child);
			}

			m_container.GetComponent<RectTransform> ().SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, 0.0f);
			m_container.GetComponent<RectTransform> ().SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, 0.0f);

			if (hasPosReset)
			{
				SetContainerYPos (0.0f);
			}
		}

		public void ClearScrollView (List<GameObject> gobsToIgnore)
		{
			List<GameObject> gobsToDestroy = new List<GameObject> ();

			foreach (Transform child in m_container.transform)
			{
				if (!gobsToIgnore.Contains (child.gameObject))
				{
					gobsToDestroy.Add (child.gameObject);
				}
			}

			foreach (GameObject gob in gobsToDestroy)
			{
				gob.transform.SetParent (null);
				Destroy (gob);
			}

			ResetContainerSize ();
		}

		public void SetContainerNormalisedYPos (float t)
		{
			GetComponent<ScrollRect> ().verticalNormalizedPosition = t;
		}

		public void SetContainerYPos (float yPos)
		{
			m_container.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0.0f, yPos);
		}

		public float GetScrollViewYPos ()
		{
			return m_container.GetComponent<RectTransform> ().anchoredPosition.y;
		}

		public float GetScrollViewHeight ()
		{
			return m_container.GetComponent<RectTransform> ().rect.height;
		}

		public void AdjustScrollViewHeight (float height)
		{
			m_container.GetComponent<RectTransform> ().SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical,
				m_container.GetComponent<RectTransform> ().rect.height + height);
		}

		public void OnBeginDrag (PointerEventData eventData)
		{
			if (m_onDraggingStart != null) m_onDraggingStart ();
		}

		public void OnEndDrag (PointerEventData eventData)
		{
			if (m_onDraggingEnd != null) m_onDraggingEnd ();
		}

		public void OnDragStart (DraggingStartDelegate onDragStrat)
		{
			m_onDraggingStart = onDragStrat;
		}

		public void OnDragEnd (DraggingEndDelegate onDragEnd)
		{
			m_onDraggingEnd = onDragEnd;
		}
	}
}