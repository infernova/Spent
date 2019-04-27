using StarstruckFramework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CostBreakdownListItem : SelectableListItem
{
    [SerializeField]
    private Image mColour;
    [SerializeField]
    private Button mButton;
    [SerializeField]
    private TextMeshProUGUI mPrimaryCat;
    [SerializeField]
    private TextMeshProUGUI mAmount;
    [SerializeField]
    private TextMeshProUGUI mPercentage;

    [SerializeField]
    private RectTransform mColourRect;
    [SerializeField]
    private RectTransform mPriCatRect;
    [SerializeField]
    private RectTransform mPriCatTextRect;
    [SerializeField]
    private RectTransform mAmountRect;
    [SerializeField]
    private RectTransform mAmountTextRect;
    [SerializeField]
    private RectTransform mPercentRect;
    [SerializeField]
    private RectTransform mPercentTextRect;

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

        mColour.color = MainScreen.Instance.CostBreakdownItemColours[index];

        SetAsSelected(index == MainScreen.Instance.CostBreakdownIndex);
    }

    public void IsColoursVisibile(bool isVisible)
    {
        if (isVisible)
        {
            mColourRect.anchorMin = new Vector2(0.0f, 0.0f);
            mColourRect.anchorMax = new Vector2(0.1f, 1.0f);

            mPriCatRect.anchorMin = new Vector2(0.1f, 0.0f);
            mPriCatRect.anchorMax = new Vector2(0.55f, 1.0f);
            mPriCatTextRect.anchorMin = new Vector2(0.1f, 0.0f);
            mPriCatTextRect.anchorMax = new Vector2(0.55f, 1.0f);

            mAmountRect.anchorMin = new Vector2(0.55f, 0.0f);
            mAmountRect.anchorMax = new Vector2(0.8f, 1.0f);
            mAmountTextRect.anchorMin = new Vector2(0.55f, 0.0f);
            mAmountTextRect.anchorMax = new Vector2(0.8f, 1.0f);

            mPercentRect.anchorMin = new Vector2(0.8f, 0.0f);
            mPercentRect.anchorMax = new Vector2(1.0f, 1.0f);
            mPercentTextRect.anchorMin = new Vector2(0.8f, 0.0f);
            mPercentTextRect.anchorMax = new Vector2(1.0f, 1.0f);
        }
        else
        {
            mColourRect.anchorMin = new Vector2(0.0f, 0.0f);
            mColourRect.anchorMax = new Vector2(0.0f, 1.0f);

            mPriCatRect.anchorMin = new Vector2(0.0f, 0.0f);
            mPriCatRect.anchorMax = new Vector2(0.5f, 1.0f);
            mPriCatTextRect.anchorMin = new Vector2(0.0f, 0.0f);
            mPriCatTextRect.anchorMax = new Vector2(0.5f, 1.0f);

            mAmountRect.anchorMin = new Vector2(0.5f, 0.0f);
            mAmountRect.anchorMax = new Vector2(0.75f, 1.0f);
            mAmountTextRect.anchorMin = new Vector2(0.5f, 0.0f);
            mAmountTextRect.anchorMax = new Vector2(0.75f, 1.0f);

            mPercentRect.anchorMin = new Vector2(0.75f, 0.0f);
            mPercentRect.anchorMax = new Vector2(1.0f, 1.0f);
            mPercentTextRect.anchorMin = new Vector2(0.75f, 0.0f);
            mPercentTextRect.anchorMax = new Vector2(1.0f, 1.0f);
        }
    }
}
