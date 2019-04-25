using TMPro;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DailyExpenditureSetItem : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI mDateTitle;
    [SerializeField]
    private RectTransform mDateTitleContainer;

    [SerializeField]
    private GameObject mItemTemplate;

    public float TopPos { get; private set; }
    public float BottomPos
    {
        get { return TopPos - Size; }
    }
    public float Size { get; private set; }
    public int NumItems { get; private set; }

    private float mTitleContainerSize;

    private bool mIsInit;

    private void Start()
    {
        if (mIsInit) return;

        mTitleContainerSize = mDateTitleContainer.rect.height;
        mIsInit = true;
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
   
        while (index + NumItems < list.Count && list[index + NumItems].Date.DateTime.Date == selectedDate.Date)
        {
            GameObject item = Instantiate(mItemTemplate, transform);
            item.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, -mTitleContainerSize - (itemHeight * NumItems));
            item.GetComponent<DailyExpenditureListItem>().Init(list[index + NumItems], index + NumItems);

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
