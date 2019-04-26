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
            mBackground.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }
        else
        {
            mBackground.color = new Color(0.5f, 0.5f, 0.5f, 0.0f);
        }
    }

    public override void ResetPosition(int index)
    {
        base.ResetPosition(index);
        mIndex = index;
    }
}
