using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarstruckFramework;
using TMPro;

public class ExpenditureDescriptionItem : PooledObject
{
    [SerializeField]
    private TextMeshProUGUI mDescription;
    private float mOffset;
    private float mItemHeight;
    private bool mIsScrollingIn;
    private bool mIsScrollingOut;
    private RectTransform mRectTrans;

    private DailyExpenditureListItem mParent;
    private RectTransform mParentRectTrans;

    private const float REVEAL_SPEED = 1000.0f;

    public void Init(GameObject refItem, ExpenditureItem refExpense)
    {
        mIsScrollingOut = false;

        mOffset = 0.0f;
        mRectTrans = GetComponent<RectTransform>();
        mItemHeight = mRectTrans.rect.height;
        mParentRectTrans = refItem.GetComponent<RectTransform>();
        mRectTrans.anchoredPosition = new Vector2(0.0f, mParentRectTrans.anchoredPosition.y - mParentRectTrans.rect.height + mItemHeight);

        if (string.IsNullOrEmpty(refExpense.Description))
        {
            mDescription.text = "No description";
        }
        else
        {
            mDescription.text = refExpense.Description;
        }

        mParent = refItem.GetComponent<DailyExpenditureListItem>();
        mParent.Offset = mOffset;
        mIsScrollingIn = true;
    }

    public void StartScrollOut()
    {
        mIsScrollingOut = true;
    }

    private void Update()
    {
        if (mIsScrollingIn)
        {
            mOffset += Time.deltaTime * REVEAL_SPEED;

            if (mOffset >= mItemHeight)
            {
                mOffset = mItemHeight;
                mIsScrollingIn = false;
            }

            mRectTrans.anchoredPosition = new Vector2(0.0f, mParentRectTrans.anchoredPosition.y - mParentRectTrans.rect.height + mItemHeight - mOffset);
            mParent.Offset = mOffset;
        }
        else if (mIsScrollingOut)
        {
            mOffset -= Time.deltaTime * REVEAL_SPEED;

            if (mOffset <= 0.0f)
            {
                mOffset = 0.0f;
                mIsScrollingOut = false;
                PoolMgr.Instance.DestroyObj(gameObject);
                mParent.mDescriptionItem = null;
            }

            mRectTrans.anchoredPosition = new Vector2(0.0f, mParentRectTrans.anchoredPosition.y - mParentRectTrans.rect.height + mItemHeight - mOffset);
            mParent.Offset = mOffset;
        }
    }
}
