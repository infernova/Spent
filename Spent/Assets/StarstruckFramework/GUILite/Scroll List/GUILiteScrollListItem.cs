using UnityEngine;
using UnityEngine.UI;

namespace StarstruckFramework
{
	public class GUILiteScrollListItem : MonoBehaviour
	{
		private RectTransform.Axis mAxis;
		private float mItemSize;
		private float mSpacing;
		private RectTransform mRectTransform;

        protected int mCurrIndex;

		public void Init(RectTransform.Axis axis, float spacing)
		{
			mRectTransform = GetComponent<RectTransform>();
			mAxis = axis;
			mSpacing = spacing;

			if (axis == RectTransform.Axis.Vertical)
			{
				mItemSize = mRectTransform.rect.height;
				mRectTransform.pivot = new Vector2(0.5f, 1.0f);
			}
			else if (axis == RectTransform.Axis.Horizontal)
			{
				mItemSize = mRectTransform.rect.width;
				mRectTransform.pivot = new Vector2(0.0f, 0.5f);
			}
		}

		public virtual void ResetPosition(int index)
		{
			if (mAxis == RectTransform.Axis.Vertical)
			{
				mRectTransform.anchoredPosition = new Vector2(0.0f, -(index * (mItemSize + mSpacing) + mSpacing));
			}
			else if (mAxis == RectTransform.Axis.Horizontal)
			{
				mRectTransform.anchoredPosition = new Vector2((index * (mItemSize + mSpacing) + mSpacing), 0.0f);
			}

            mCurrIndex = index;
		}

        public void ResetPosition()
        {
            ResetPosition(mCurrIndex);
        }
	}
}