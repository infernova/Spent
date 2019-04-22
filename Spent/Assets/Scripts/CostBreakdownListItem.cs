﻿using StarstruckFramework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CostBreakdownListItem : SelectableListItem
{
    [SerializeField]
    private Button mButton;
    [SerializeField]
    private TextMeshProUGUI mPrimaryCat;
    [SerializeField]
    private TextMeshProUGUI mAmount;
    [SerializeField]
    private TextMeshProUGUI mPercentage;

    private void Start()
    {
        mButton.onClick.AddListener(() => MainScreen.Instance.SelectCostBreakdownItem(mIndex));
    }

    public override void ResetPosition(int index)
    {
        base.ResetPosition(index);

        CostBreakdownItem item = MainScreen.Instance.CostBreakdownItems[index];
        mAmount.text = "$" + item.Amount.ToString("0.00");
        mPrimaryCat.text = item.Category;
        mPercentage.text = item.Percentage.ToString("0.0") + "%";

        SetAsSelected(index == MainScreen.Instance.CostBreakdownIndex);
    }
}
