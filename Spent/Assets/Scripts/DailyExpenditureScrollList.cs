using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class DailyExpenditureScrollList : MonoBehaviour
{
    [SerializeField]
    private ScrollRect mScrollRect;
    [SerializeField]
    private RectTransform mContainerRect;

    [SerializeField]
    private GameObject mListItemTemplate;

    private List<DailyExpenditureListItem> mItemList = new List<DailyExpenditureListItem>();
    public List<DailyExpenditureListItem> ItemList
    {
        get { return mItemList; }
    }

    private List<GameObject> mCurrDisplay = new List<GameObject>();

    private int mLastIndex;
    private float mBottomPos;

    // Use this for initialization
    public void Init (List<ExpenditureItem> items)
    {
        mItemList = new List<DailyExpenditureListItem>();

        foreach(GameObject item in mCurrDisplay)
        {
            Destroy(item);
        }

        mLastIndex = 0;
        mBottomPos = 0.0f;

        while (mLastIndex < items.Count)
        {
            GameObject setItem = Instantiate(mListItemTemplate, mContainerRect);
            setItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, mBottomPos);
            float itemHeight = 0.0f;
            List<DailyExpenditureListItem> additionalItems = null;
            setItem.GetComponent<DailyExpenditureSetItem>().LoadExpenditures(items,
                ref mLastIndex,
                out itemHeight,
                out additionalItems);

            mItemList.AddRange(additionalItems);
            mBottomPos -= itemHeight;

            mCurrDisplay.Add(setItem);
        }

        mContainerRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, -mBottomPos);
	}
}
