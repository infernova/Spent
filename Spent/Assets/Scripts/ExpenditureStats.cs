using System;
using System.Collections.Generic;
using UnityEngine;
using StarstruckFramework;

[CreateAssetMenu]
public class ExpenditureStats : ScriptableObject
{
    public ExpenditureItemList[] RecurringItems = new ExpenditureItemList[32];
    public List<ExpenditureItem> Items = new List<ExpenditureItem>();
    public List<string> PrimaryCategories = new List<string>();
    public StringListDictionary SecondaryCategories = new StringListDictionary();
    public List<string> LastUsedPrimaryCategories = new List<string>();
    public StringListDictionary LastUsedSecondaryCategories = new StringListDictionary();

    public StringIntDictionary PrimaryCatCount = new StringIntDictionary();
    public StringIntDictionary SecondaryCatCount = new StringIntDictionary();

    public void ClearStats()
    {
        RecurringItems = new ExpenditureItemList[32];
        Items = new List<ExpenditureItem>();
        PrimaryCategories = new List<string>();
        SecondaryCategories = new StringListDictionary();
        LastUsedPrimaryCategories = new List<string>();
        LastUsedSecondaryCategories = new StringListDictionary();

        PrimaryCatCount = new StringIntDictionary();
        SecondaryCatCount = new StringIntDictionary();
    }

    public void AddRecurringExpenditure(ExpenditureItem item, int day)
    {
        RecurringItems[day].Add(item);
    }

    public void Add(ExpenditureItem item)
    {
        Items.Insert(0, item);
        AddCategory(item);

        if (LastUsedPrimaryCategories.Contains(item.PrimaryCategory))
        {
            LastUsedPrimaryCategories.Remove(item.PrimaryCategory);
        }

        LastUsedPrimaryCategories.Insert(0, item.PrimaryCategory);

        while (LastUsedPrimaryCategories.Count > 5)
        {
            LastUsedPrimaryCategories.RemoveAt(LastUsedPrimaryCategories.Count - 1);
        }

        if (!LastUsedSecondaryCategories.ContainsKey(item.PrimaryCategory))
        {
            LastUsedSecondaryCategories.Add(item.PrimaryCategory, new StringListItem(item.SecondaryCategory));
        }
        else
        {
            LastUsedSecondaryCategories[item.PrimaryCategory].Insert(0, item.PrimaryCategory);

            while (LastUsedSecondaryCategories[item.PrimaryCategory].Count > 5)
            {
                LastUsedSecondaryCategories[item.PrimaryCategory].RemoveAt(LastUsedPrimaryCategories.Count - 1);
            }
        }
    }

    public void AddCategory(ExpenditureItem item)
    {
        if (!PrimaryCategories.Contains(item.PrimaryCategory))
        {
            PrimaryCategories.Add(item.PrimaryCategory);
            PrimaryCategories.Sort();

            PrimaryCatCount.Add(item.PrimaryCategory, 1);

            SecondaryCategories.Add(item.PrimaryCategory, new StringListItem(item.SecondaryCategory));
        }
        else
        {
            PrimaryCatCount[item.PrimaryCategory]++;
        }

        if (!SecondaryCategories[item.PrimaryCategory].Contains(item.SecondaryCategory))
        {
            SecondaryCategories[item.PrimaryCategory].Add(item.SecondaryCategory);
        }

        if (!SecondaryCatCount.ContainsKey(item.PrimaryCategory + "-" + item.SecondaryCategory))
        {
            SecondaryCatCount.Add(item.PrimaryCategory + "-" + item.SecondaryCategory, 1);
        }
        else
        {
            SecondaryCatCount[item.PrimaryCategory + "-" + item.SecondaryCategory]++;
        }
    }

    public void Remove(int index)
    {
        RemoveCategory(Items[index]);
        Items.RemoveAt(index);
    }

    public void Remove(ExpenditureItem item)
    {
        Items.Remove(item);
        RemoveCategory(item);
    }

    public void Edit(ExpenditureItem oldItem, ExpenditureItem newItem)
    {
        Remove(oldItem);
        Add(newItem);
    }

    public void RemoveCategory(ExpenditureItem item)
    {
        PrimaryCatCount[item.PrimaryCategory]--;

        if (PrimaryCatCount[item.PrimaryCategory] == 0)
        {
            PrimaryCategories.Remove(item.PrimaryCategory);
            PrimaryCatCount.Remove(item.PrimaryCategory);

            SecondaryCategories.Remove(item.PrimaryCategory);
            SecondaryCatCount.Remove(item.PrimaryCategory + "-" + item.SecondaryCategory);

            LastUsedSecondaryCategories.Remove(item.PrimaryCategory);
        }
        else
        {
            SecondaryCatCount[item.PrimaryCategory + "-" + item.SecondaryCategory]--;
            if (SecondaryCatCount[item.PrimaryCategory + "-" + item.SecondaryCategory] == 0)
            {
                SecondaryCategories[item.PrimaryCategory].Remove(item.SecondaryCategory);
                SecondaryCatCount.Remove(item.PrimaryCategory + "-" + item.SecondaryCategory);
            }
        }
    }
}

[Serializable]
public class ExpenditureItemList
{
    public List<ExpenditureItem> List;

    public bool Contains(ExpenditureItem s)
    {
        return List.Contains(s);
    }

    public void Add(ExpenditureItem s)
    {
        List.Add(s);
    }

    public void Insert(int index, ExpenditureItem s)
    {
        List.Insert(index, s);
    }

    public void Remove(ExpenditureItem s)
    {
        List.Remove(s);
    }

    public void RemoveAt(int index)
    {
        List.RemoveAt(index);
    }

    public int Count
    {
        get { return List.Count; }
    }

    public ExpenditureItem this[int i]
    {
        get { return List[i]; }
        set { List[i] = value; }
    }
}

[Serializable]
public class StringListDictionary : SerializableDictionary<string, StringListItem> { }

[Serializable]
public class StringIntDictionary : SerializableDictionary<string, int> { }

[Serializable]
public struct StringListItem
{
    private List<string> mList;
    public List<string> List
    {
        get { return mList; }
    }

    public StringListItem(string s)
    {
        mList = new List<string> { s };
    }

    public bool Contains(string s)
    {
        return mList.Contains(s);
    }

    public void Add(string s)
    {
        mList.Add(s);
        mList.Sort();
    }

    public void Insert(int index, string s)
    {
        mList.Insert(index, s);
    }

    public void Remove(string s)
    {
        mList.Remove(s);
    }

    public void RemoveAt(int index)
    {
        mList.RemoveAt(index);
    }

    public int Count
    {
        get { return mList.Count; }
    }

    public string this[int i]
    {
        get { return mList[i]; }
        set { mList[i] = value; }
    }
}

[Serializable]
public struct ExpenditureItem
{
    public float Amount;
    public string PrimaryCategory;
    public string SecondaryCategory;
    public string Description;
    public SerializableDatetime Date;

    public ExpenditureItem(float amount,
                           string primaryCat,
                           string secondaryCat,
                           string desc,
                           DateTime date)
    {
        Amount = amount;
        PrimaryCategory = primaryCat.ToUpper();
        SecondaryCategory = secondaryCat.ToUpper();
        Description = desc;
        Date = new SerializableDatetime(date);
    }
}