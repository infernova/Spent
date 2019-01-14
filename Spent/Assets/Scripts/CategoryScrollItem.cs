using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CategoryScrollItem : MonoBehaviour
{
    [SerializeField]
    private Button mButton;
    [SerializeField]
    private TMP_Text mText;

    private int mIndex;
    private bool mIsPrimaryCat;

    public void Init(int index, string text, bool isPrimaryCat)
    {
        mIndex = index;
        mIsPrimaryCat = isPrimaryCat;

        gameObject.SetActive(true);

        if (isPrimaryCat)
        {
            mButton.onClick.AddListener(() => MainScreen.Instance.OnPrimaryCatSelect(index));
        }
        else
        {
            mButton.onClick.AddListener(() => MainScreen.Instance.OnSecondaryCatSelect(index));
        }

        mText.text = text;

        RectTransform rectTrans = GetComponent<RectTransform>();
        rectTrans.anchoredPosition = new Vector2(0.0f, 0.0f + (rectTrans.rect.height * index));
    }
}
