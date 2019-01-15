using StarstruckFramework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class RecurringExpenditureItem : GUILiteScrollListItem
{
    [SerializeField]
    private Button mButton;
    [SerializeField]
    private TextMeshProUGUI mDate;
    [SerializeField]
    private TextMeshProUGUI mAmount;
    [SerializeField]
    private TextMeshProUGUI mPrimaryCat;
    [SerializeField]
    private TextMeshProUGUI mSecondaryCat;
    [SerializeField]
    private Image mBackground;

    [SerializeField]
    private ExpenditureStats mStats;

    private int mIndex;

    private void Start()
    {
        mButton.onClick.AddListener(() => MainScreen.Instance.SelectRecurringExpenditureItem(mIndex));
    }

    public void CheckIfSelected(int index)
    {
        SetAsSelected(mIndex == index);
    }

    private void SetAsSelected(bool isSelected)
    {
        if (isSelected)
        {
            mBackground.color = new Color(0.8f, 0.8f, 0.8f, 1.0f);
        }
        else
        {
            mBackground.color = new Color(0.96f, 0.96f, 0.96f, 1.0f);
        }
    }

    public override void ResetPosition(int index)
    {
        base.ResetPosition(index);
        mIndex = index;

        ExpenditureItem item = mStats.RecurringItemList[index];
        mDate.text = "Day " + item.Date.ToString("dd");
        mAmount.text = "$" + item.Amount.ToString("0.00");
        mPrimaryCat.text = item.PrimaryCategory;
        mSecondaryCat.text = item.SecondaryCategory;

        SetAsSelected(index == MainScreen.Instance.EditIndex);
    }
}
