using TMPro;
using System;
using StarstruckFramework;
using System.Collections.Generic;
using UnityEngine;

public class DailyExpenditureSetItem : PooledObject
{
    [SerializeField]
    private TextMeshProUGUI mDateTitle;
    [SerializeField]
    private RectTransform mDateTitleContainer;

    [SerializeField]
    private ObjectPoolType mItemPoolType;

    private List<GameObject> mListItems = new List<GameObject>();

    private float mTopPos;
    [HideInInspector]
    public float PreviousOffset;
    public float TopPos
    {
        get { return mTopPos - PreviousOffset; }
        private set { mTopPos = value; }
    }
    public float BottomPos
    {
        get { return TopPos - Size - Offset; }
    }

    private int mStartIndex;
    public float Size { get; private set; }
    public int NumItems { get; private set; }

    private float mTitleContainerSize;
    private RectTransform mRectTrans;

    private List<GameObject> mDailySetItems = new List<GameObject>();

    private float mOffset;
    public float Offset
    {
        get { return mOffset; }
    }

    private bool mIsInit;
    private DailyExpenditureScrollList mParent;

    private PoolMgr mPoolMgr;

    public override void Reinit()
    {
        base.Reinit();

        foreach(GameObject item in mListItems)
        {
            mPoolMgr.DestroyObj(item);
        }

        mListItems.Clear();

        PreviousOffset = 0.0f;
    }

    private void Start()
    {
        if (mIsInit) return;

        mPoolMgr = PoolMgr.Instance;

        mTitleContainerSize = mDateTitleContainer.rect.height;
        mRectTrans = GetComponent<RectTransform>();
        mIsInit = true;
    }

    public bool ContainsIndex(int index)
    {
        return index >= mStartIndex && index < mStartIndex + NumItems;
    }

    public void RepositionItems()
    {
        mOffset = 0.0f;
        
        float currPosition = -mTitleContainerSize;
        foreach (GameObject item in mListItems)
        {
            DailyExpenditureListItem itemComp = item.GetComponent<DailyExpenditureListItem>();
            item.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, currPosition);
            currPosition -= itemComp.ItemHeight + itemComp.Offset;

            mOffset += itemComp.Offset;
        }

        mRectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, -currPosition);

        mParent.RepositionItems();
    }

    public float GetSize(List<ExpenditureItem> list, ref int index)
    {
        if (index >= list.Count)
        {
            return 0.0f;
        }

        if (!mIsInit)
        {
            Start();
        }

        DateTime selectedDate = list[index].Date;
        float itemHeight = mPoolMgr.GetPooledObjRef(mItemPoolType).GetComponent<RectTransform>().rect.height;
        int itemCount = 0;

        while (index + itemCount < list.Count && list[index + itemCount].Date.DateTime.Date == selectedDate.Date)
        {
            itemCount++;
        }

        index += itemCount;
        return mDateTitleContainer.rect.height + itemHeight * itemCount;
    }

    public void LoadExpenditures(List<ExpenditureItem> list,
        float topPos,
        DailyExpenditureScrollList parent,
        ref int index,
        out float size,
        out List<DailyExpenditureListItem> items,
        bool isReversePos = false)
    {
        if (!mIsInit)
        {
            Start();
        }

        mParent = parent;
        mOffset = 0.0f;

        if (index >= list.Count)
        {
            size = 0.0f;
            items = new List<DailyExpenditureListItem>();

            mStartIndex = 0;
            return;
        }

        mStartIndex = index;

        DateTime selectedDate = list[index].Date;
        float itemHeight = mPoolMgr.GetPooledObjRef(mItemPoolType).GetComponent<RectTransform>().rect.height;
        NumItems = 0;
        if (mDateTitle != null)
        {
            mDateTitle.SetText(selectedDate.ToString("dddd, d MMM yyyy"));
        }

        items = new List<DailyExpenditureListItem>();

        foreach (GameObject item in mListItems)
        {
            mPoolMgr.DestroyObj(item);
        }

        mListItems.Clear();

        while (list.Count > 0 
            && index + NumItems < list.Count 
            && list[index + NumItems].Date.DateTime.Date == selectedDate.Date)
        {
            GameObject item = mPoolMgr.InstantiateObj(mItemPoolType, transform);

            item.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, -mTitleContainerSize - (itemHeight * NumItems));
            item.GetComponent<DailyExpenditureListItem>().Init(this, list[index + NumItems], index + NumItems);

            mListItems.Add(item);

            items.Add(item.GetComponent<DailyExpenditureListItem>());
            NumItems++;
        }

        index += NumItems;
        size = mTitleContainerSize + itemHeight * NumItems;

        Size = size;

        mRectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);

        TopPos = topPos;

        if (isReversePos)
        {
            TopPos += size;
        }

        mRectTrans.anchoredPosition = new Vector2(0.0f, TopPos);

        mDateTitleContainer.SetAsLastSibling();
    }

    public void DoUpdate(float topBorder)
    {
        float titleContainerPos = 0.0f;
        if (topBorder >= TopPos)
        {
            titleContainerPos = 0.0f;
        }
        else if (topBorder <= BottomPos + mTitleContainerSize)
        {
            titleContainerPos = -Size - mOffset + mTitleContainerSize;
        }
        else
        {
            titleContainerPos = topBorder - TopPos;
        }

        mDateTitleContainer.anchoredPosition = new Vector2(0.0f, titleContainerPos);
    }
}
