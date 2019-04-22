using StarstruckFramework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectableListItem : GUILiteScrollListItem
{
    [SerializeField]
    private Image mBackground;

    protected int mIndex;

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

    public override void ResetPosition(int index)
    {
        base.ResetPosition(index);
        mIndex = index;
    }
}
