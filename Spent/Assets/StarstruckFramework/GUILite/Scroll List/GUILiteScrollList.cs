using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace StarstruckFramework
{
	public class GUILiteScrollList : MonoBehaviour
	{
		[SerializeField]
		private ScrollRect mScrollRect;
		[SerializeField]
		private RectTransform mContainerRect;

		[SerializeField]
		private GameObject mListItem;
		[SerializeField]
		private float mSpacing;
		[SerializeField]
		private RectTransform.Axis mAxis;

		private float mTemplateSize;
		private int mMaxItems;

		private int mCurrMinIndex;
		private int mCurrMaxIndex;

		private float mCurrMinCutoff;
		private float mCurrMaxCutoff;

		protected List<GUILiteScrollListItem> mItemList;

		public List<GUILiteScrollListItem> ItemList
		{
			get { return mItemList; }
		}

		public void Init(int numItems)
		{
			mScrollRect.StopMovement();

			mMaxItems = numItems;

			mScrollRect.horizontal = mAxis == RectTransform.Axis.Horizontal;
			mScrollRect.vertical = mAxis == RectTransform.Axis.Vertical;

			mTemplateSize = 0;
			float viewportSize = 0;

			int numVisible = 0;

			if (mAxis == RectTransform.Axis.Vertical)
			{
				mTemplateSize = mListItem.GetComponent<RectTransform>().rect.height;
				viewportSize = mScrollRect.GetComponent<RectTransform>().rect.height;

				mContainerRect.anchorMin = new Vector2(0.0f, 1.0f);
				mContainerRect.anchorMax = new Vector2(1.0f, 1.0f);
				mContainerRect.pivot = new Vector2(0.5f, 1.0f);

				mContainerRect.offsetMin = new Vector2(mContainerRect.offsetMin.x, 0.0f);
				mContainerRect.offsetMax = new Vector2(mContainerRect.offsetMax.x, 0.0f);
			}
			else if (mAxis == RectTransform.Axis.Horizontal)
			{
				mTemplateSize = mListItem.GetComponent<RectTransform>().rect.width;
				viewportSize = mScrollRect.GetComponent<RectTransform>().rect.width;

				mContainerRect.anchorMin = new Vector2(0.0f, 0.0f);
				mContainerRect.anchorMax = new Vector2(0.0f, 1.0f);
				mContainerRect.pivot = new Vector2(0.0f, 0.5f);

				mContainerRect.offsetMin = new Vector2(0.0f, mContainerRect.offsetMin.y);
				mContainerRect.offsetMax = new Vector2(0.0f, mContainerRect.offsetMax.y);
			}

			mContainerRect.SetSizeWithCurrentAnchors(mAxis,
				(numItems * (mTemplateSize + mSpacing)) + mSpacing);

			mContainerRect.anchoredPosition = new Vector2(0.0f, 0.0f);

			numVisible = (int)System.Math.Ceiling((viewportSize - (mSpacing * 2.0f)) / (mTemplateSize + mSpacing)) + 1;
			numVisible = System.Math.Min(mMaxItems, numVisible);

			mScrollRect.enabled = viewportSize < ((mTemplateSize + mSpacing) * numItems) + mSpacing;

			mCurrMinIndex = 0;
			mCurrMaxIndex = numVisible - 1;

			mCurrMinCutoff = mSpacing / 2.0f;
			mCurrMaxCutoff = mCurrMinCutoff + mTemplateSize + mSpacing;

			if (mItemList == null)
			{
				mItemList = new List<GUILiteScrollListItem>();
			}
			else
			{
				for (int i = mItemList.Count - 1; i >= mMaxItems; i--)
				{
					Destroy(mItemList[i].gameObject);
					mItemList.RemoveAt(i);
				}
			}

			InitItems(numVisible);
		}

		protected virtual void InitItems(int numVisible)
		{
			for (int i = 0; i < numVisible; i++)
			{
				GameObject gob = null;

				if (i >= mItemList.Count)
				{
					gob = GameObject.Instantiate<GameObject>(mListItem, mContainerRect);
					mItemList.Add(gob.GetComponent<GUILiteScrollListItem>());
				}
				else
				{
					gob = mItemList[i].gameObject;
				}

				gob.GetComponent<GUILiteScrollListItem>().Init(mAxis, mSpacing);
				gob.GetComponent<GUILiteScrollListItem>().ResetPosition(i);
			}
		}

		void Update()
		{
			float anchorPos = 0.0f;
			if (mAxis == RectTransform.Axis.Vertical)
			{
				anchorPos = mContainerRect.anchoredPosition.y;
			}
			else if (mAxis == RectTransform.Axis.Horizontal)
			{
				anchorPos = -mContainerRect.anchoredPosition.x;
			}

			while (anchorPos > mCurrMaxCutoff
			       && mCurrMaxIndex + 1 < mMaxItems)
			{
				mCurrMinIndex++;
				mCurrMaxIndex++;

				mCurrMinCutoff += mTemplateSize + mSpacing;
				mCurrMaxCutoff += mTemplateSize + mSpacing;

				GUILiteScrollListItem movedItem = mItemList[0];
				movedItem.ResetPosition(mCurrMaxIndex);

				mItemList.RemoveAt(0);
				mItemList.Add(movedItem);
			}

			while (anchorPos < mCurrMinCutoff
			       && mCurrMinIndex > 0)
			{
				mCurrMinIndex--;
				mCurrMaxIndex--;

				mCurrMinCutoff -= mTemplateSize + mSpacing;
				mCurrMaxCutoff -= mTemplateSize + mSpacing;

				GUILiteScrollListItem movedItem = mItemList[mItemList.Count - 1];
				movedItem.ResetPosition(mCurrMinIndex);

				mItemList.RemoveAt(mItemList.Count - 1);
				mItemList.Insert(0, movedItem);
			}
		}
	}
}