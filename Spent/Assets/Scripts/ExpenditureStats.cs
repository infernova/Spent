using System;
using System.Collections.Generic;
using UnityEngine;
using StarstruckFramework;

[CreateAssetMenu]
public class ExpenditureStats : ScriptableObject
{
    public List<ExpenditureItem> Items = new List<ExpenditureItem>();

    public long EditCount;

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

    public void CopyExpenditures(ExpenditureStats reference)
    {
        Items = new List<ExpenditureItem>();

        PrimaryCategories = new List<string>();
        LastUsedPrimaryCategories = new List<string>();
        PrimaryCatCount = new StringIntDictionary();

        SecondaryCategories = new Dictionary<string, List<string>>();
        LastUsedSecondaryCategories = new Dictionary<string, List<string>>();
        SecondaryCatCount = new Dictionary<string, Dictionary<string, int>>();

        EditCount = reference.EditCount;

        foreach(ExpenditureItem item in reference.Items)
        {
            Items.Add(item.Copy());
        }

        PrimaryCategories = new List<string>(reference.PrimaryCategories);
        LastUsedPrimaryCategories = new List<string>(reference.LastUsedPrimaryCategories);

        if (reference.PrimaryCatCount.Count > 0)
        {
            foreach (KeyValuePair<string, int> pair in reference.PrimaryCatCount)
            {
                PrimaryCatCount.Add(pair.Key, pair.Value);
            }
        }

        if (reference.SecondaryCategories.Count > 0)
        {
            foreach (KeyValuePair<string, List<string>> pair in reference.SecondaryCategories)
            {
                SecondaryCategories.Add(pair.Key, new List<string>(pair.Value));
            }
        }

        if (reference.LastUsedSecondaryCategories.Count > 0)
        {
            foreach (KeyValuePair<string, List<string>> pair in reference.LastUsedSecondaryCategories)
            {
                LastUsedSecondaryCategories.Add(pair.Key, new List<string>(pair.Value));
            }
        }

        if (reference.SecondaryCatCount.Count > 0)
        {
            foreach (KeyValuePair<string, Dictionary<string, int>> pair in reference.SecondaryCatCount)
            {
                SecondaryCatCount.Add(pair.Key, new Dictionary<string, int>(pair.Value));
            }
        }

        SerializeCategories();
    }

    public void LoadCategories()
    {
        SecondaryCategories = new Dictionary<string, List<string>>();
        if (SerializedSecondaryCategories.Count > 0)
        {
            foreach (KeyValuePair<string, string> pair in SerializedSecondaryCategories)
            {
                SecondaryCategories.Add(pair.Key, new List<string>(pair.Value.Split('\t')));
            }
        }

        LastUsedSecondaryCategories = new Dictionary<string, List<string>>();
        if (SerializedLastUsedSecondaryCategories.Count > 0)
        {
            foreach (KeyValuePair<string, string> pair in SerializedLastUsedSecondaryCategories)
            {
                LastUsedSecondaryCategories.Add(pair.Key, new List<string>(pair.Value.Split('\t')));
            }
        }

        if (SerializedSecondaryCatCount.Count > 0)
        {
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
    }

    private void SerializeCategories()
    {
        SerializedSecondaryCategories = new StringListDictionary();
        SerializedLastUsedSecondaryCategories = new StringListDictionary();
        SerializedSecondaryCatCount = new StringIntDictionary();

        foreach (KeyValuePair<string, List<string>> pair in SecondaryCategories)
        {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            for (int i = 0; i < pair.Value.Count; i++)
            {
                stringBuilder.Append(pair.Value[i]);
                if (i < pair.Value.Count - 1)
                {
                    stringBuilder.Append("\t");
                }
            }

            SerializedSecondaryCategories.Add(pair.Key, stringBuilder.ToString());
        }

        foreach (KeyValuePair<string, List<string>> pair in LastUsedSecondaryCategories)
        {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            for (int i = 0; i < pair.Value.Count; i++)
            {
                stringBuilder.Append(pair.Value[i]);
                if (i < pair.Value.Count - 1)
                {
                    stringBuilder.Append("\t");
                }
            }

            SerializedLastUsedSecondaryCategories.Add(pair.Key, stringBuilder.ToString());
        }

        foreach (KeyValuePair<string, Dictionary<string, int>> pair in SecondaryCatCount)
        {
            foreach (KeyValuePair<string, int> dictPair in pair.Value)
            {
                SerializedSecondaryCatCount.Add(pair.Key + "\t" + dictPair.Key, dictPair.Value);
            }
        }
    }

    private void SaveSecondaryCategory(string priCat)
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

        EditCount++;
    }

    private void AddCategory(ExpenditureItem item)
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

        EditCount++;
    }

    private void RemoveCategory(ExpenditureItem item)
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
    public List<ExpenditureItem> List = new List<ExpenditureItem>();

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

    public ExpenditureItem Copy()
    {
        return new ExpenditureItem(Amount,
                                   PrimaryCategory,
                                   SecondaryCategory,
                                   Description,
                                   Date.DateTime);
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
}