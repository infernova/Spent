using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CostBreakdownBarItem : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI mDate;

    [SerializeField]
    private RectTransform mBarRectTrans;

    public float MaxBarHeight;

    public void Init(DateTime date, float percentage, bool isHighlight)
    {
        mDate.SetText(date.ToString("MMM yyyy"));
        mBarRectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, MaxBarHeight * percentage);

        if (isHighlight)
        {
            mBarRectTrans.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f);
        }
        else
        {
            mBarRectTrans.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
        }
    }
}
