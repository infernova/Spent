using TMPro;
using UnityEngine.UI;
using StarstruckFramework;
using UnityEngine;

public class DailyExpenditureListItem : PooledObject
{
    [SerializeField]
    private Button mButton;
    [SerializeField]
    private TextMeshProUGUI mAmount;
    [SerializeField]
    private TextMeshProUGUI mPrimaryCat;
    [SerializeField]
    private TextMeshProUGUI mSecondaryCat;
    [SerializeField]
    private TextMeshProUGUI mDesc;

    [SerializeField]
    private Image mBackground;

    private int mIndex;
    public GameObject mDescriptionItem;

    private ExpenditureItem mRefItem;
    private DailyExpenditureSetItem mParent;

    private float mItemHeight;
    public float ItemHeight
    {
        get { return mItemHeight; }
    }

    public float mOffset;
    public float Offset
    {
        get { return mOffset; }
        set
        {
            mOffset = value;
            mParent.RepositionItems();
        }
    }

    private void Start()
    {
        mButton.onClick.AddListener(() => MainScreen.Instance.SelectExpenditure(mIndex));
        mItemHeight = GetComponent<RectTransform>().rect.height;
        Offset = 0.0f;
    }

    public void Init(DailyExpenditureSetItem parent, ExpenditureItem item, int index)
    {
        mParent = parent;

        mRefItem = item;

        mAmount.text = "$" + item.Amount.ToString("0.00");
        if (mPrimaryCat != null) mPrimaryCat.text = item.PrimaryCategory; 
        if (mSecondaryCat != null) mSecondaryCat.text = item.SecondaryCategory;
        if (mDesc != null)
        {
            if (string.IsNullOrEmpty(item.Description))
            {
                mDesc.text = "No description";
                mDesc.fontStyle = FontStyles.Italic;
            }
            else
            {
                mDesc.text = item.Description;
                mDesc.fontStyle = FontStyles.Normal;
            }
        }

        SetAsSelected(index == MainScreen.Instance.EditIndex);

        mIndex = index;
    }

    public void CheckIfSelected(int index)
    {
        SetAsSelected(mIndex == index);
    }

    protected void SetAsSelected(bool isSelected)
    {
        if (isSelected)
        {
            mBackground.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }
        else
        {
            mBackground.color = new Color(0.5f, 0.5f, 0.5f, 0.0f);
        }

        if (mDesc == null && isSelected && mDescriptionItem == null)
        {
            mDescriptionItem = PoolMgr.Instance.InstantiateObj(ObjectPoolType.DESCRIPTION_ITEM, transform.parent);
            mDescriptionItem.transform.SetAsFirstSibling();
            mDescriptionItem.GetComponent<ExpenditureDescriptionItem>().Init(gameObject, mRefItem);
        }
        else if (mDesc == null && !isSelected && mDescriptionItem != null)
        {
            mDescriptionItem.GetComponent<ExpenditureDescriptionItem>().StartScrollOut();
        }
    }
}
