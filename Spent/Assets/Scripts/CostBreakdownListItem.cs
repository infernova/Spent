using StarstruckFramework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CostBreakdownListItem : GUILiteScrollListItem
{
    [SerializeField]
    private Button mButton;
    [SerializeField]
    private TextMeshProUGUI mPrimaryCat;
    [SerializeField]
    private TextMeshProUGUI mAmount;
    [SerializeField]
    private TextMeshProUGUI mPercentage;
    [SerializeField]
    private Image mBackground;

    private int mIndex;

    private void Start()
    {
        mButton.onClick.AddListener(() => MainScreen.Instance.SelectCostBreakdownItem(mIndex));
    }

    public void SetAsSelected(bool isSelected)
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

        CostBreakdownItem item = MainScreen.Instance.CostBreakdownItems[index];
        mAmount.text = "$" + item.Amount.ToString("0.00");
        mPrimaryCat.text = item.Category;
        mPercentage.text = item.Percentage.ToString("0.0") + "%";
    }
}
