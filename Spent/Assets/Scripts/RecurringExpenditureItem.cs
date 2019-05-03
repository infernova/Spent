using StarstruckFramework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class RecurringExpenditureItem : DailyExpenditureListItem
{
    [SerializeField]
    private TextMeshProUGUI mDate;
    public override void Init(DailyExpenditureSetItem parent, ExpenditureItem item, int index)
    {
        base.Init(parent, item, index);
        mDate.text = "Day " + item.Date.ToString("dd");
    }
}
