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
    private GameObject mItemTemplate;

    [SerializeField]
    private ObjectPoolType mItemPoolType;

    private List<GameObject> mListItems = new List<GameObject>();

    public float TopPos { get; private set; }
    public float BottomPos
    {
        get { return TopPos - Size; }
    }
    public float Size { get; private set; }
    public int NumItems { get; private set; }

    private float mTitleContainerSize;

    private bool mIsInit;

    private PoolMgr mPoolMgr;

    public override void Reinit()
    {
        base.Reinit();

        foreach(GameObject item in mListItems)
        {
            mPoolMgr.DestroyObj(item);
        }

        mListItems.Clear();
    }

    private void Start()
    {
        if (mIsInit) return;

        mPoolMgr = PoolMgr.Instance;

        mTitleContainerSize = mDateTitleContainer.rect.height;
        mIsInit = true;
    }

    public float GetSize(List<ExpenditureItem> list, ref int index)
    {
        if (index >= list.Count)
        {
            return 0.0f;
        }

        DateTime selectedDate = list[index].Date;
        float itemHeight = mItemTemplate.GetComponent<RectTransform>().rect.height;
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
        ref int index,
        out float size,
        out List<DailyExpenditureListItem> items,
        bool isReversePos = false)
    {
        if (!mIsInit)
        {
            Start();
        }

        if (index >= list.Count)
        {
            size = 0.0f;
            items = new List<DailyExpenditureListItem>();
            return;
        }

        DateTime selectedDate = list[index].Date;
        float itemHeight = mItemTemplate.GetComponent<RectTransform>().rect.height;
        NumItems = 0;
        mDateTitle.SetText(selectedDate.ToString("dddd, d MMM yyyy"));

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
            item.GetComponent<DailyExpenditureListItem>().Init(list[index + NumItems], index + NumItems);

            mListItems.Add(item);

            items.Add(item.GetComponent<DailyExpenditureListItem>());
            NumItems++;
        }

        index += NumItems;
        size = mTitleContainerSize + itemHeight * NumItems;

        Size = size;

        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);

        TopPos = topPos;

        if (isReversePos)
        {
            TopPos += size;
        }

        GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, TopPos);

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
            titleContainerPos = -Size + mTitleContainerSize;
        }
        else
        {
            titleContainerPos = topBorder - TopPos;
        }

        mDateTitleContainer.anchoredPosition = new Vector2(0.0f, titleContainerPos);
    }
}
