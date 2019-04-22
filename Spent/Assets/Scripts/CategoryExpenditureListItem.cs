using StarstruckFramework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CategoryExpenditureListItem : SelectableListItem
{
    [SerializeField]
    private Button mButton;
    [SerializeField]
    private TextMeshProUGUI mDate;
    [SerializeField]
    private TextMeshProUGUI mAmount;
    [SerializeField]
    private TextMeshProUGUI mDesc;

    private void Start()
    {
        mButton.onClick.AddListener(() => MainScreen.Instance.SelectCostBreakdownItem(mIndex));
    }

    public override void ResetPosition(int index)
    {
        base.ResetPosition(index);

        ExpenditureItem item = MainScreen.Instance.DisplayedItems[index];
        mDate.text = item.Date.ToString("dd/MM/yy");
        mAmount.text = "$" + item.Amount.ToString("0.00");
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

        SetAsSelected(index == MainScreen.Instance.CostBreakdownIndex);
    }
}