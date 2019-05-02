using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine;
using StarstruckFramework;
using UnityEngine.EventSystems;

public class DailyExpenditureScrollList : MonoBehaviour, IEndDragHandler, IBeginDragHandler
{
    [SerializeField]
    private ScrollRect mScrollRect;
    [SerializeField]
    private RectTransform mContainerRect;

    [SerializeField]
    private ObjectPoolType mListItemPoolType;

    [SerializeField]
    private float mTopBorder;
    [SerializeField]
    private float mBottomBorder;

    private float mViewportSize;

    private List<DailyExpenditureListItem> mItemList = new List<DailyExpenditureListItem>();
    public List<DailyExpenditureListItem> ItemList
    {
        get { return mItemList; }
    }

    private List<DailyExpenditureSetItem> mCurrDisplay = new List<DailyExpenditureSetItem>();

    private int mFirstIndex;
    private int mLastIndex;

    private float mTopPos;
    private float mBottomPos;

    private float mContainerSize;

    private List<ExpenditureItem> mItems;
    private List<GameObject> mSetItems;

    private float mCurrTotalOffset;

    private bool mIsDragging;
    private float mDragOffset;
    private bool mWasPositionUpdated;

    private bool mIsInit;

    private PoolMgr mPoolMgr;

    private void Start()
    {
        if (mIsInit) return;

        mViewportSize = GetComponent<RectTransform>().rect.height;
        mScrollRect.onValueChanged.AddListener(OnScrollValueChanged);

        mPoolMgr = PoolMgr.Instance;

        mIsInit = true;
    }

    public void RepositionItems()
    {
        float currPosition = mSetItems[0].GetComponent<RectTransform>().anchoredPosition.y;
        mCurrTotalOffset = 0.0f;
        foreach (DailyExpenditureSetItem setItem in mCurrDisplay)
        {
            RectTransform setTrans = setItem.GetComponent<RectTransform>();
            setTrans.anchoredPosition = new Vector2(0.0f, currPosition);
            currPosition -= setTrans.rect.height;

            setItem.PreviousOffset = mCurrTotalOffset;

            mCurrTotalOffset += setItem.Offset;
        }

        if (mContainerSize + mCurrTotalOffset > GetComponent<RectTransform>().rect.height)
        {
            mScrollRect.movementType = ScrollRect.MovementType.Elastic;
        }
        else
        {
            mScrollRect.movementType = ScrollRect.MovementType.Clamped;
        }

        mContainerRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mContainerSize + mCurrTotalOffset);
    }

    public void Init (List<ExpenditureItem> items)
    {
        if (!mIsInit)
        {
            Start();
        }

        mItems = items;

        mItemList = new List<DailyExpenditureListItem>();

        mSetItems = new List<GameObject>();

        foreach (DailyExpenditureSetItem item in mCurrDisplay)
        {
            PoolMgr.Instance.DestroyObj(item.gameObject);
        }

        mCurrDisplay = new List<DailyExpenditureSetItem>();

        mContainerRect.anchoredPosition = Vector2.zero;

        mFirstIndex = 0;
        mTopPos = 0.0f;

        mLastIndex = 0;
        mBottomPos = 0.0f;

        mContainerSize = 0.0f;

        while (mLastIndex < items.Count && -mBottomPos < mViewportSize)
        {
            GameObject setItem = PoolMgr.Instance.InstantiateObj(mListItemPoolType, mContainerRect);
            float itemHeight = 0.0f;
            List<DailyExpenditureListItem> additionalItems = null;
            setItem.GetComponent<DailyExpenditureSetItem>().LoadExpenditures(items,
                mBottomPos,
                this,
                ref mLastIndex,
                out itemHeight,
                out additionalItems);

            mItemList.AddRange(additionalItems);
            mBottomPos -= itemHeight;

            mCurrDisplay.Add(setItem.GetComponent<DailyExpenditureSetItem>());
            mSetItems.Add(setItem);
        }

        int sizeIndex = 0;
        
        while (sizeIndex < items.Count)
        {
            mContainerSize += mPoolMgr.GetPooledObjRef(mListItemPoolType).GetComponent<DailyExpenditureSetItem>().GetSize(items,
                ref sizeIndex);
        }

        if (mContainerSize > GetComponent<RectTransform>().rect.height)
        {
            mScrollRect.movementType = ScrollRect.MovementType.Elastic;
        }
        else
        {
            mScrollRect.movementType = ScrollRect.MovementType.Clamped;
        }

        mContainerRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mContainerSize);
    }

    public void OnScrollValueChanged(Vector2 pos)
    {
        if (mIsDragging)
        {
            if (!mWasPositionUpdated)
            {
                pos.y += mDragOffset;
                mScrollRect.verticalNormalizedPosition = pos.y;
            }
            else
            {
                mWasPositionUpdated = false;
            }
        }

        float containerPos = mContainerRect.anchoredPosition.y;
        mTopBorder = -containerPos;
        mBottomBorder = -containerPos - mViewportSize;

        foreach (DailyExpenditureSetItem set in mCurrDisplay)
        {
            set.DoUpdate(mTopBorder);
        }

        while (mLastIndex < mItems.Count && mBottomBorder < mBottomPos - mCurrTotalOffset)
        {
            GameObject setItem = mPoolMgr.InstantiateObj(mListItemPoolType, mContainerRect);
            float itemHeight = 0.0f;
            List<DailyExpenditureListItem> additionalItems = null;
            setItem.GetComponent<DailyExpenditureSetItem>().LoadExpenditures(mItems,
                mBottomPos - mCurrTotalOffset,
                this,
                ref mLastIndex,
                out itemHeight,
                out additionalItems);

            mItemList.AddRange(additionalItems);
            mBottomPos -= itemHeight;

            mCurrDisplay.Add(setItem.GetComponent<DailyExpenditureSetItem>());
            mSetItems.Add(setItem);
        }

        while (mLastIndex > 0 && mCurrDisplay.Count > 0 && mBottomBorder > mCurrDisplay[mCurrDisplay.Count - 1].TopPos)
        {
            DailyExpenditureSetItem set = mCurrDisplay[mCurrDisplay.Count - 1];

            if (set.ContainsIndex(MainScreen.Instance.EditIndex)
                || set.ContainsIndex(MainScreen.Instance.CostBreakdownIndex))
            {
                MainScreen.Instance.ResetEditIndices();
            }

            mLastIndex -= set.NumItems;
            mBottomPos += set.Size;
            mPoolMgr.DestroyObj(set.gameObject);
            mCurrDisplay.RemoveAt(mCurrDisplay.Count - 1);
            mSetItems.RemoveAt(mSetItems.Count - 1);

            while (mItemList.Count > mLastIndex)
            {
                mItemList.RemoveAt(mItemList.Count - 1);
            }
        }

        while (mFirstIndex < mItems.Count && mCurrDisplay.Count > 0 && mTopBorder < mCurrDisplay[0].BottomPos)
        {
            DailyExpenditureSetItem set = mCurrDisplay[0];
            float setOffset = set.Offset;

            if (set.ContainsIndex(MainScreen.Instance.EditIndex)
                || set.ContainsIndex(MainScreen.Instance.CostBreakdownIndex))
            {
                MainScreen.Instance.ResetEditIndices();
            }

            mFirstIndex += set.NumItems;
            mTopPos -= set.Size;
            mPoolMgr.DestroyObj(set.gameObject);
            mCurrDisplay.RemoveAt(0);
            mSetItems.RemoveAt(0);

            if (setOffset > 0.0f)
            {
                Vector2 currPos = mContainerRect.anchoredPosition;
                currPos.y -= setOffset;
                mContainerRect.anchoredPosition = currPos;

                if (mIsDragging)
                {
                    mDragOffset += setOffset / mViewportSize;
                    mWasPositionUpdated = true;
                }
            }
        }

        while (mFirstIndex > 0 && mTopBorder > mTopPos)
        {
            int prevIndex = mFirstIndex - 1;
            DateTime prevDate = mItems[prevIndex].Date.DateTime.Date;

            while (prevIndex > 0 && mItems[prevIndex - 1].Date.DateTime.Date == prevDate)
            {
                prevIndex--;
            }

            mFirstIndex = prevIndex;

            GameObject setItem = mPoolMgr.InstantiateObj(mListItemPoolType, mContainerRect);
            float itemHeight = 0.0f;
            List<DailyExpenditureListItem> additionalItems = null;
            setItem.GetComponent<DailyExpenditureSetItem>().LoadExpenditures(mItems,
                mTopPos,
                this,
                ref prevIndex,
                out itemHeight,
                out additionalItems,
                true);

            for (int i = 0; i < additionalItems.Count; i++)
            {
                mItemList[mFirstIndex + i] = additionalItems[i];
            }

            mTopPos += itemHeight;

            mCurrDisplay.Insert(0, setItem.GetComponent<DailyExpenditureSetItem>());
            mSetItems.Insert(0, setItem);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        mIsDragging = false;
        mDragOffset = 0.0f;
        mWasPositionUpdated = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        mIsDragging = true;
    }
}
