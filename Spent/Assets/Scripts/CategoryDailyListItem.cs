using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class CategoryDailyListItem : MonoBehaviour
{
    [SerializeField]
    private Button mButton;
    [SerializeField]
    private TextMeshProUGUI mAmount;
    [SerializeField]
    private TextMeshProUGUI mDesc;

    [SerializeField]
    private Image mBackground;

    private int mIndex;

    public void Init(ExpenditureItem item, int index)
    {
        mButton.onClick.AddListener(() => MainScreen.Instance.SelectExpenditureListItem(index));

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
            mBackground.color = new Color(0.8f, 0.8f, 0.8f, 1.0f);
        }
        else
        {
            mBackground.color = new Color(0.96f, 0.96f, 0.96f, 1.0f);
        }
    }
}