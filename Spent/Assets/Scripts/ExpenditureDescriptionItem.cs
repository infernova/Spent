using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarstruckFramework;
using TMPro;

public class ExpenditureDescriptionItem : PooledObject
{
    [SerializeField]
    private TextMeshProUGUI mDescription;

    public void Init(GameObject refItem, ExpenditureItem refExpense)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0.0f, refItem.GetComponent<RectTransform>().rect.yMin - rectTransform.rect.height);

        if (string.IsNullOrEmpty(refExpense.Description))
        {
            mDescription.text = "No description";
        }
        else
        {
            mDescription.text = refExpense.Description;
        }
    }
}
