using TMPro;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DailyExpenditureSetItem : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI mDateTitle;

    [SerializeField]
    private GameObject mItemTemplate;

    public void LoadExpenditures(List<ExpenditureItem> list,
        ref int index,
        out float size,
        out List<DailyExpenditureListItem> items)
    {
        if (index >= list.Count)
        {
            size = 0.0f;
            items = new List<DailyExpenditureListItem>();
            return;
        }

        DateTime selectedDate = list[index].Date;
        float itemHeight = mItemTemplate.GetComponent<RectTransform>().rect.height;
        int offset = 0;
        mDateTitle.SetText(selectedDate.ToString("dddd, d MMM yyyy"));

        items = new List<DailyExpenditureListItem>();
   
        while (index + offset < list.Count && list[index + offset].Date.DateTime.Date == selectedDate.Date)
        {
            GameObject item = Instantiate(mItemTemplate, transform);
            item.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, -100.0f - (itemHeight * offset));
            item.GetComponent<DailyExpenditureListItem>().Init(list[index + offset], index + offset);

            items.Add(item.GetComponent<DailyExpenditureListItem>());
            offset++;
        }

        index += offset;
        size = 100 + itemHeight * offset;
    }
}
