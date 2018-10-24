using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CategoryScrollItem : MonoBehaviour, IPointerDownHandler, IPointerExitHandler
{
    [SerializeField]
    private Button mButton;
    [SerializeField]
    private TMP_Text mText;
    [SerializeField]
    private MainScreen mMain;

    private int mIndex;
    private bool mIsPrimaryCat;

    public void OnPointerDown(PointerEventData eventData)
    {
        mMain.StallForChoiceInput();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mMain.RefocusCategoryInput(mIsPrimaryCat);
    }

    public void Init(int index, string text, bool isPrimaryCat)
    {
        gameObject.SetActive(true);

        if (isPrimaryCat)
        {
            mButton.onClick.AddListener(() => mMain.OnPrimaryCatSelect(index));
        }
        else
        {
            mButton.onClick.AddListener(() => mMain.OnSecondaryCatSelect(index));
        }

        mText.text = text;

        RectTransform rectTrans = GetComponent<RectTransform>();
        rectTrans.anchoredPosition = new Vector2(0.0f, 0.0f - (rectTrans.rect.height * index));
    }
}
