using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine;
using StarstruckFramework;

public class DailyExpenditureScrollList : MonoBehaviour
{
    [SerializeField]
    private ScrollRect mScrollRect;
    [SerializeField]
    private RectTransform mContainerRect;

    [SerializeField]
    private GameObject mListItemTemplate;

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

    private List<ExpenditureItem> mItems;

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

    public void Init (List<ExpenditureItem> items)
    {
        if (!mIsInit)
        {
            Start();
        }

        mItems = items;

        mItemList = new List<DailyExpenditureListItem>();

        foreach(DailyExpenditureSetItem item in mCurrDisplay)
        {
            PoolMgr.Instance.DestroyObj(item.gameObject);
        }

        mCurrDisplay = new List<DailyExpenditureSetItem>();

        mContainerRect.anchoredPosition = Vector2.zero;

        mFirstIndex = 0;
        mTopPos = 0.0f;

        mLastIndex = 0;
        mBottomPos = 0.0f;

        while (mLastIndex < items.Count && -mBottomPos < mViewportSize)
        {
            GameObject setItem = PoolMgr.Instance.InstantiateObj(mListItemPoolType, mContainerRect);
            float itemHeight = 0.0f;
            List<DailyExpenditureListItem> additionalItems = null;
            setItem.GetComponent<DailyExpenditureSetItem>().LoadExpenditures(items,
                mBottomPos,
                ref mLastIndex,
                out itemHeight,
                out additionalItems);

            mItemList.AddRange(additionalItems);
            mBottomPos -= itemHeight;

            mCurrDisplay.Add(setItem.GetComponent<DailyExpenditureSetItem>());
        }

        int sizeIndex = 0;
        float containerSize = 0.0f;
        
        while (sizeIndex < items.Count)
        {
            containerSize += mListItemTemplate.GetComponent<DailyExpenditureSetItem>().GetSize(items,
                ref sizeIndex);
        }

        mScrollRect.vertical = containerSize > GetComponent<RectTransform>().rect.height;
        mContainerRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, containerSize);
    }

    public void OnScrollValueChanged(Vector2 pos)
    {
        float containerPos = mContainerRect.anchoredPosition.y;
        mTopBorder = -containerPos;
        mBottomBorder = -containerPos - mViewportSize;

        while (mLastIndex < mItems.Count && mBottomBorder < mBottomPos)
        {
            GameObject setItem = mPoolMgr.InstantiateObj(mListItemPoolType, mContainerRect);
            float itemHeight = 0.0f;
            List<DailyExpenditureListItem> additionalItems = null;
            setItem.GetComponent<DailyExpenditureSetItem>().LoadExpenditures(mItems,
                mBottomPos,
                ref mLastIndex,
                out itemHeight,
                out additionalItems);

            mItemList.AddRange(additionalItems);
            mBottomPos -= itemHeight;

            mCurrDisplay.Add(setItem.GetComponent<DailyExpenditureSetItem>());
        }

        while (mLastIndex > 0 && mCurrDisplay.Count > 0 && mBottomBorder > mCurrDisplay[mCurrDisplay.Count - 1].TopPos)
        {
            DailyExpenditureSetItem set = mCurrDisplay[mCurrDisplay.Count - 1];
            mLastIndex -= set.NumItems;
            mBottomPos += set.Size;
            mPoolMgr.DestroyObj(set.gameObject);
            mCurrDisplay.RemoveAt(mCurrDisplay.Count - 1);

            while (mItemList.Count > mLastIndex)
            {
                mItemList.RemoveAt(mItemList.Count - 1);
            }
        }

        while (mFirstIndex < mItems.Count && mCurrDisplay.Count > 0 && mTopBorder < mCurrDisplay[0].BottomPos)
        {
            DailyExpenditureSetItem set = mCurrDisplay[0];
            mFirstIndex += set.NumItems;
            mTopPos -= set.Size;
            mPoolMgr.DestroyObj(set.gameObject);
            mCurrDisplay.RemoveAt(0);
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
        }

        foreach (DailyExpenditureSetItem set in mCurrDisplay)
        {
            set.DoUpdate(mTopBorder);
        }
    }
}
