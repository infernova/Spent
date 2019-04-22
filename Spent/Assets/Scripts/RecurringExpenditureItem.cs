using StarstruckFramework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class RecurringExpenditureItem : SelectableListItem
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

    private void Start()
    {
        mButton.onClick.AddListener(() => MainScreen.Instance.SelectRecurringExpenditureItem(mIndex));
    }

    public override void ResetPosition(int index)
    {
        base.ResetPosition(index);

        ExpenditureItem item = MainScreen.Instance.RecurringExpense.RecurringItemList[index];
        mDate.text = "Day " + item.Date.ToString("dd");
        mAmount.text = "$" + item.Amount.ToString("0.00");
        mPrimaryCat.text = item.PrimaryCategory;
        mSecondaryCat.text = item.SecondaryCategory;

        SetAsSelected(index == MainScreen.Instance.EditIndex);
    }
}
