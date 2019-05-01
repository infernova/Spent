using UnityEngine.UI;
using UnityEngine;
using StarstruckFramework;

public class PieChartItem : PooledObject
{
    [SerializeField]
    private Image[] mPieSlices;

    public void SetPercentage(float percent)
    {
        for (int i = 0; i < 4; i++)
        {
            if (percent > 0.25f * (i + 1))
            {
                mPieSlices[i].fillAmount = 1.0f;
            }
            else
            {
                mPieSlices[i].fillAmount = (percent - (0.25f * i)) * 4.0f;
            }
        }
    }

    public void SetColour(Color color)
    {
        foreach(Image slice in mPieSlices)
        {
            slice.color = color;
        }
    }
}
