using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class DailyExpenditureListItem : MonoBehaviour
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

    public void Init(ExpenditureItem item, int index)
    {
        mButton.onClick.AddListener(() => MainScreen.Instance.SelectExpenditure(index));

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
    }
}
