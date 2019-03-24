using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RecurringExpenditureStats : ScriptableObject
{
    public ExpenditureItemList[] RecurringItems = new ExpenditureItemList[32];
    public List<ExpenditureItem> RecurringItemList = new List<ExpenditureItem>();

    public void LoadRecurringExpenditureList()
    {
        RecurringItemList = new List<ExpenditureItem>();
        for (int i = 0; i < RecurringItems.Length; i++)
        {
            if (RecurringItems[i] == null)
            {
                RecurringItems[i] = new ExpenditureItemList();
            }
            else
            {
                foreach (ExpenditureItem item in RecurringItems[i].List)
                {
                    RecurringItemList.Add(item);
                }
            }
        }
    }

    public void AddRecurringExpenditure(ExpenditureItem item)
    {
        RecurringItemList.Add(item);
        RecurringItems[item.Date.DateTime.Day].Add(item);
    }

    public void RemoveRecurringExpenditure(int index)
    {
        ExpenditureItem toRemove = RecurringItemList[index];
        List<ExpenditureItem> list = RecurringItems[toRemove.Date.DateTime.Day].List;
        for (int i = list.Count - 1; i >= 0; i--)
        {
            ExpenditureItem currItem = list[i];
            if (System.Math.Abs(currItem.Amount - toRemove.Amount) < Mathf.Epsilon 
                && currItem.PrimaryCategory == toRemove.PrimaryCategory
                && currItem.SecondaryCategory == toRemove.SecondaryCategory)
            {
                list.RemoveAt(i);
                break;
            }
        }

        RecurringItemList.RemoveAt(index);
    }
}