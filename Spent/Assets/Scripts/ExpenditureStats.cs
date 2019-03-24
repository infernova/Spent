using System;
using System.Collections.Generic;
using UnityEngine;
using StarstruckFramework;

[CreateAssetMenu]
public class ExpenditureStats : ScriptableObject
{
    public ExpenditureItemList[] RecurringItems = new ExpenditureItemList[32];
    public List<ExpenditureItem> RecurringItemList = new List<ExpenditureItem>();
    public List<ExpenditureItem> Items = new List<ExpenditureItem>();

    public List<string> PrimaryCategories = new List<string>();
    public List<string> LastUsedPrimaryCategories = new List<string>();
    public StringIntDictionary PrimaryCatCount = new StringIntDictionary();

    public Dictionary<string, List<string>> SecondaryCategories = new Dictionary<string, List<string>>();
    public Dictionary<string, List<string>> LastUsedSecondaryCategories = new Dictionary<string, List<string>>();
    public Dictionary<string, Dictionary<string, int>> SecondaryCatCount = new Dictionary<string, Dictionary<string, int>>();

    [SerializeField]
    private StringListDictionary SerializedSecondaryCategories = new StringListDictionary();
    [SerializeField]
    private StringListDictionary SerializedLastUsedSecondaryCategories = new StringListDictionary();
    [SerializeField]
    private StringIntDictionary SerializedSecondaryCatCount = new StringIntDictionary();

    private List<ExpenditureItem> mDisplayedItems = new List<ExpenditureItem>();
    public List<ExpenditureItem> DisplayedItems
    {
        get { return mDisplayedItems; }
    }

    private DateTime mDateRange;
    public DateTime DateRange
    {
        get { return mDateRange; }
        private set
        {
            mDateRange = value;
            RefreshDisplayedItems();
        }
    }

    public void RefreshDisplayedItems()
    {
        mDisplayedItems = GetDisplayedItems();
    }

    private List<ExpenditureItem> GetDisplayedItems()
    {
        if (DateRange.Year == 1)
        {
            return new List<ExpenditureItem>(Items);
        }

        DateTime dateRangeStart = new DateTime(DateRange.Year, DateRange.Month, 1);
        DateTime dateRangeEnd = dateRangeStart.AddMonths(1).AddDays(-1).Date;

        int startIndex = 0;
        int endIndex = Items.Count - 1;

        int midPoint = 0;
        DateTime midItemDate = DateTime.MinValue;

        do
        {
            midPoint = (endIndex + startIndex) / 2;

            if (midPoint < 0 || midPoint >= Items.Count)
            {
                return new List<ExpenditureItem>();
            }

            midItemDate = Items[midPoint].Date.DateTime.Date;

            if (endIndex != midPoint && midItemDate < dateRangeStart)
            {
                endIndex = midPoint;
            }
            else if (startIndex != midPoint && midItemDate > dateRangeEnd)
            {
                startIndex = midPoint;
            }
        }
        while ((endIndex != midPoint && midItemDate < dateRangeStart)
        || (startIndex != midPoint && midItemDate > dateRangeEnd));

        while (Items[startIndex].Date.DateTime.Date > dateRangeEnd)
        {
            int firstQuater = (startIndex + midPoint) / 2;
            DateTime firstQuarterDate = Items[firstQuater].Date.DateTime.Date;

            if (startIndex != firstQuater && firstQuarterDate > dateRangeEnd)
            {
                startIndex = firstQuater;
            }
            else
            {
                while (startIndex < endIndex && Items[startIndex].Date.DateTime.Date > dateRangeEnd)
                {
                    startIndex++;
                }
            }
        }

        while (Items[endIndex].Date.DateTime.Date < dateRangeStart)
        {
            int thirdQuater = (endIndex + midPoint) / 2;
            DateTime thirdQuarterDate = Items[thirdQuater].Date.DateTime.Date;

            if (endIndex != thirdQuater && thirdQuarterDate < dateRangeStart)
            {
                endIndex = thirdQuater;
            }
            else
            {
                while (startIndex < endIndex && Items[endIndex].Date.DateTime.Date < dateRangeStart)
                {
                    endIndex--;
                }
            }
        }

        List<ExpenditureItem> resultList = new List<ExpenditureItem>();

        if (startIndex == endIndex)
        {
            DateTime singleDate = Items[endIndex].Date.DateTime.Date;

            if (singleDate < dateRangeStart || singleDate > dateRangeEnd)
            {
                return resultList;
            }
        }

        for (int i = startIndex; i <= endIndex; i++)
        {
            resultList.Add(Items[i]);
        }

        return resultList;
    }

    public void IncrementDateRange()
    {
        if (DateRange.Year == 1)
        {
            ExpenditureItem lastItem = Items[Items.Count - 1];
            DateTime lastItemDate = lastItem.Date;
            DateRange = new DateTime(lastItemDate.Year, lastItemDate.Month, 1);
        }
        else
        {
            DateTime newDateTime = DateRange.AddMonths(1);
            if (newDateTime.Date > DateTime.Now.Date)
            {
                newDateTime = new DateTime(1, 1, 1);
            }

            DateRange = new DateTime(newDateTime.Year, newDateTime.Month, 1);
        }
    }

    public void DecrementDateRange()
    {
        if (DateRange.Year == 1)
        {
            DateRange = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        }
        else
        {
            DateTime newDateTime = DateRange.AddMonths(-1);
            newDateTime = new DateTime(newDateTime.Year, newDateTime.Month, 1);

            ExpenditureItem lastItem = Items[Items.Count - 1];
            DateTime lastItemDate = lastItem.Date;
            lastItemDate = new DateTime(lastItemDate.Year, lastItemDate.Month, 1);

            if (newDateTime.Date < lastItemDate.Date)
            {
                newDateTime = new DateTime(1, 1, 1);
            }

            DateRange = newDateTime;
        }
    }

    public void RefreshDateRange()
    {
        DateRange = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
    }

    public void LoadCategories()
    {
        SecondaryCategories = new Dictionary<string, List<string>>();
        foreach (KeyValuePair<string, string> pair in SerializedSecondaryCategories)
        {
            SecondaryCategories.Add(pair.Key, new List<string>(pair.Value.Split('\t')));
        }

        LastUsedSecondaryCategories = new Dictionary<string, List<string>>();
        foreach (KeyValuePair<string, string> pair in SerializedLastUsedSecondaryCategories)
        {
            LastUsedSecondaryCategories.Add(pair.Key, new List<string>(pair.Value.Split('\t')));
        }

        foreach (KeyValuePair<string, int> pair in SerializedSecondaryCatCount)
        {
            string[] idArray = pair.Key.Split('\t');
            if (!SecondaryCatCount.ContainsKey(idArray[0]))
            {
                SecondaryCatCount.Add(idArray[0], new Dictionary<string, int>());
            }

            SecondaryCatCount[idArray[0]].Add(idArray[1], pair.Value);
        }
    }

    public void LoadRecurringExpenditureList()
    {
        RecurringItemList = new List<ExpenditureItem>();
        foreach(ExpenditureItemList list in RecurringItems)
        {
            foreach(ExpenditureItem item in list.List)
            {
                RecurringItemList.Add(item);
            }
        }
    }

    public void SaveSecondaryCategory(string priCat)
    {
        if (!SecondaryCategories.ContainsKey(priCat) && SerializedSecondaryCategories.ContainsKey(priCat))
        {
            SerializedSecondaryCategories.Remove(priCat);
        }
        else if (SecondaryCategories.ContainsKey(priCat))
        {
            List<string> secondaryCats = SecondaryCategories[priCat];
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            for (int i = 0; i < secondaryCats.Count; i++)
            {
                stringBuilder.Append(secondaryCats[i]);
                if (i < secondaryCats.Count - 1)
                {
                    stringBuilder.Append("\t");
                }
            }

            if (!SerializedSecondaryCategories.ContainsKey(priCat))
            {
                SerializedSecondaryCategories.Add(priCat, stringBuilder.ToString());
            }
            else
            {
                SerializedSecondaryCategories[priCat] = stringBuilder.ToString();
            }
        }

        if (!LastUsedSecondaryCategories.ContainsKey(priCat) && SerializedLastUsedSecondaryCategories.ContainsKey(priCat))
        {
            SerializedLastUsedSecondaryCategories.Remove(priCat);
        }
        else if (LastUsedSecondaryCategories.ContainsKey(priCat))
        {
            List<string> lastUsedCats = LastUsedSecondaryCategories[priCat];
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            for (int i = 0; i < lastUsedCats.Count; i++)
            {
                stringBuilder.Append(lastUsedCats[i]);
                if (i < lastUsedCats.Count - 1)
                {
                    stringBuilder.Append("\t");
                }
            }

            if (!SerializedLastUsedSecondaryCategories.ContainsKey(priCat))
            {
                SerializedLastUsedSecondaryCategories.Add(priCat, stringBuilder.ToString());
            }
            else
            {
                SerializedLastUsedSecondaryCategories[priCat] = stringBuilder.ToString();
            }
        }
    }

    public void ClearStats()
    {
        RecurringItems = new ExpenditureItemList[32];
        Items = new List<ExpenditureItem>();

        PrimaryCategories = new List<string>();
        LastUsedPrimaryCategories = new List<string>();
        PrimaryCatCount = new StringIntDictionary();

        SecondaryCategories = new Dictionary<string, List<string>>();
        LastUsedSecondaryCategories = new Dictionary<string, List<string>>();
        SecondaryCatCount = new Dictionary<string, Dictionary<string, int>>();

        SerializedSecondaryCategories = new StringListDictionary();
        SerializedLastUsedSecondaryCategories = new StringListDictionary();
        SerializedSecondaryCatCount = new StringIntDictionary();
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
            if (currItem.Amount == toRemove.Amount
                && currItem.PrimaryCategory == toRemove.PrimaryCategory
                && currItem.SecondaryCategory == toRemove.SecondaryCategory)
            {
                list.RemoveAt(i);
                break;
            }
        }

        RecurringItemList.RemoveAt(index);
    }

    public void Add(ExpenditureItem item)
    {
        Items.Insert(0, item);
        Items.Sort();

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
            LastUsedSecondaryCategories.Add(item.PrimaryCategory, new List<string> { item.SecondaryCategory });
        }
        else
        {
            if (LastUsedSecondaryCategories[item.PrimaryCategory].Contains(item.SecondaryCategory))
            {
                LastUsedSecondaryCategories[item.PrimaryCategory].Remove(item.SecondaryCategory);    
            }

            LastUsedSecondaryCategories[item.PrimaryCategory].Insert(0, item.SecondaryCategory);

            while (LastUsedSecondaryCategories[item.PrimaryCategory].Count > 5)
            {
                LastUsedSecondaryCategories[item.PrimaryCategory].RemoveAt(LastUsedPrimaryCategories.Count - 1);
            }
        }

        AddCategory(item);
    }

    public void AddCategory(ExpenditureItem item)
    {
        if (!PrimaryCategories.Contains(item.PrimaryCategory))
        {
            PrimaryCategories.Add(item.PrimaryCategory);
            PrimaryCategories.Sort();

            PrimaryCatCount.Add(item.PrimaryCategory, 1);

            SecondaryCategories.Add(item.PrimaryCategory, new List<string> { item.SecondaryCategory });
        }
        else
        {
            PrimaryCatCount[item.PrimaryCategory]++;
        }

        if (!SecondaryCategories[item.PrimaryCategory].Contains(item.SecondaryCategory))
        {
            SecondaryCategories[item.PrimaryCategory].Add(item.SecondaryCategory);
            SecondaryCategories[item.PrimaryCategory].Sort();
        }

        if (!SecondaryCatCount.ContainsKey(item.PrimaryCategory))
        {
            SecondaryCatCount.Add(item.PrimaryCategory, new Dictionary<string, int>());
        }

        if (!SecondaryCatCount[item.PrimaryCategory].ContainsKey(item.SecondaryCategory))
        {
            SecondaryCatCount[item.PrimaryCategory].Add(item.SecondaryCategory, 1);
            SerializedSecondaryCatCount.Add(item.PrimaryCategory + "\t" + item.SecondaryCategory, 1);
        }
        else
        {
            SecondaryCatCount[item.PrimaryCategory][item.SecondaryCategory]++;
            SerializedSecondaryCatCount[item.PrimaryCategory + "\t" + item.SecondaryCategory]++;
        }

        SaveSecondaryCategory(item.PrimaryCategory);
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

    public void RemoveCategory(ExpenditureItem item)
    {
        PrimaryCatCount[item.PrimaryCategory]--;

        if (PrimaryCatCount[item.PrimaryCategory] == 0)
        {
            PrimaryCategories.Remove(item.PrimaryCategory);
            PrimaryCatCount.Remove(item.PrimaryCategory);

            SecondaryCategories.Remove(item.PrimaryCategory);
            SerializedSecondaryCategories.Remove(item.PrimaryCategory);
            SecondaryCatCount.Remove(item.PrimaryCategory);

            List<string> secondCatToRemove = new List<string>();

            foreach (string key in SerializedSecondaryCatCount.Keys)
            {
                if (key.StartsWith(item.PrimaryCategory + "\t", StringComparison.InvariantCultureIgnoreCase))
                {
                    secondCatToRemove.Add(key);
                }
            }

            foreach (string key in secondCatToRemove)
            {
                SerializedSecondaryCatCount.Remove(key);
            }

            LastUsedSecondaryCategories.Remove(item.PrimaryCategory);
            SerializedLastUsedSecondaryCategories.Remove(item.PrimaryCategory);
        }
        else
        {
            SecondaryCatCount[item.PrimaryCategory][item.SecondaryCategory]--;
            if (SecondaryCatCount[item.PrimaryCategory][item.SecondaryCategory] == 0)
            {
                SecondaryCategories[item.PrimaryCategory].Remove(item.SecondaryCategory);
                SecondaryCatCount[item.PrimaryCategory].Remove(item.SecondaryCategory);
                SerializedSecondaryCatCount.Remove(item.PrimaryCategory + "\t" + item.SecondaryCategory);
            }
        }

        SaveSecondaryCategory(item.PrimaryCategory);
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
public class StringListDictionary : SerializableDictionary<string, string> { }

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
public struct ExpenditureItem : IComparable
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

    public int CompareTo(object obj)
    {
        try
        {
            ExpenditureItem other = (ExpenditureItem)obj;
            if (other.Date.DateTime > Date.DateTime)
            {
                return 1;
            }
            else if (other.Date.DateTime < Date.DateTime)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
        catch
        {
            return 1;
        }
    }

    public override bool Equals(object obj)
    {
        if (!(obj is ExpenditureItem))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        return GetHashCode() == obj.GetHashCode();
    }

    public override int GetHashCode()
    {
        unchecked // Overflow is fine, just wrap
        {
            int hash = 597581;
            // Suitable nullity checks etc, of course :)
            hash = hash * 314263 + Amount.GetHashCode();
            hash = hash * 314263 + PrimaryCategory.GetHashCode();
            hash = hash * 314263 + SecondaryCategory.GetHashCode();
            hash = hash * 314263 + Description.GetHashCode();
            hash = hash * 314263 + Date.GetHashCode();

            return hash;
        }
    }

    public static bool operator ==(ExpenditureItem a, ExpenditureItem b)
    {
        if (ReferenceEquals(a, b))
        {
            return true;
        }

        return a.Equals(b);
    }

    public static bool operator !=(ExpenditureItem a, ExpenditureItem b)
    {
        if (ReferenceEquals(a, b))
        {
            return false;
        }

        return !a.Equals(b);
    }
}