using UnityEngine.UI;
using UnityEngine;

public class PieChartItem : MonoBehaviour
{
    [SerializeField]
    private Image[] mPieSlices;

    public void SetPercentage(float percent)
    {
        for (int i = 0; i < 4; i++)
        {
            if (percent > 0.25f * (i + 1))
            {
                mPieSlices[0].fillAmount = 1.0f;
            }
            else
            {
                mPieSlices[0].fillAmount = percent - (0.25f * i);
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
