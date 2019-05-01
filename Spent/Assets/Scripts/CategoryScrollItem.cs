using UnityEngine.UI;
using UnityEngine;
using StarstruckFramework;
using TMPro;

public class CategoryScrollItem : PooledObject
{
    [SerializeField]
    private Button mButton;
    [SerializeField]
    private TMP_Text mText;

    private bool mIsPrimaryCat;
    private int mIndex;

    public void Init(int index, string text, bool isPrimaryCat)
    {
        gameObject.SetActive(true);

        mButton.onClick.RemoveAllListeners();

        if (isPrimaryCat)
        {
            mButton.onClick.AddListener(() => MainScreen.Instance.OnPrimaryCatSelect(index));
        }
        else
        {
            mButton.onClick.AddListener(() => MainScreen.Instance.OnSecondaryCatSelect(index));
        }

        mIsPrimaryCat = isPrimaryCat;
        mIndex = index;

        mText.text = text;

        RectTransform rectTrans = GetComponent<RectTransform>();
        rectTrans.anchoredPosition = new Vector2(0.0f, 0.0f + (rectTrans.rect.height * index));
    }
}
