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
    public float Offset
    {
        get { return mOffset; }
    }
    private float mItemHeight;
    private bool mIsScrollingIn;
    private bool mIsScrollingOut;
    private RectTransform mRectTrans;

    private DailyExpenditureListItem mParent;
    private RectTransform mParentRectTrans;

    private MainScreen mMainScreen;

    private const float REVEAL_SPEED = 2000.0f;

    public void Init(GameObject refItem, ExpenditureItem refExpense)
    {
        mMainScreen = MainScreen.Instance;

        mMainScreen.ScrollingInDescription = this;

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
        mMainScreen.ScrollingOutDescriptions.Add(this);
    }

    public void ProcessScrollOut(float amount)
    {
        float parentBottom = mParentRectTrans.anchoredPosition.y - mParentRectTrans.rect.height;
        mOffset -= amount;

        if (mOffset <= 0.0f)
        {
            mOffset = 0.0f;
            mIsScrollingOut = false;
            PoolMgr.Instance.DestroyObj(gameObject);
            mParent.mDescriptionItem = null;
            mMainScreen.ScrollingOutDescriptions.Remove(this);
        }

        mRectTrans.anchoredPosition = new Vector2(0.0f, parentBottom + mItemHeight - mOffset);
        mParent.Offset = mOffset;
    }

    private void Update()
    {
        if (mIsScrollingIn)
        {
            mOffset += Time.deltaTime * REVEAL_SPEED;

            for (int i = mMainScreen.ScrollingOutDescriptions.Count - 1; i >= 0; i--)
            {
                mMainScreen.ScrollingOutDescriptions[i].ProcessScrollOut(Time.deltaTime * REVEAL_SPEED);
            }

            float parentBottom = mParentRectTrans.anchoredPosition.y - mParentRectTrans.rect.height;

            if (mOffset >= mItemHeight)
            {
                mOffset = mItemHeight;
                mIsScrollingIn = false;

                if (mMainScreen.ScrollingInDescription == this)
                {
                    mMainScreen.ScrollingInDescription = null;
                }
            }

            mRectTrans.anchoredPosition = new Vector2(0.0f, parentBottom + mItemHeight - mOffset);
            mParent.Offset = mOffset;
        }
        else if (mIsScrollingOut && mMainScreen.ScrollingInDescription == null)
        {
            ProcessScrollOut(Time.deltaTime * REVEAL_SPEED);
        }
    }
}
