using System;
using System.Collections.Generic;
using UnityEngine;
using StarstruckFramework;

[CreateAssetMenu]
public class ExpenditureStats : ScriptableObject
{
    public List<ExpenditureItem> Items = new List<ExpenditureItem>();
    public List<string> PrimaryCategories = new List<string>();
    public StringListDictionary SecondaryCategories = new StringListDictionary();

    public void Add(ExpenditureItem item)
    {
        Items.Add(item);
        LoadCategory(item);
    }

    public void LoadCategories()
    {
        foreach (ExpenditureItem item in Items)
        {
            LoadCategory(item);
        }
    }

    public void LoadCategory(ExpenditureItem item)
    {
        if (!PrimaryCategories.Contains(item.PrimaryCategory))
        {
            PrimaryCategories.Add(item.PrimaryCategory);
            PrimaryCategories.Sort();
            SecondaryCategories[item.PrimaryCategory] = new List<string>();
        }

        if (!SecondaryCategories.ContainsKey(item.PrimaryCategory))
        {
            SecondaryCategories[item.PrimaryCategory] = new List<string> { item.SecondaryCategory };
        }
        else if (!SecondaryCategories[item.PrimaryCategory].Contains(item.SecondaryCategory))
        {
            SecondaryCategories[item.PrimaryCategory].Add(item.SecondaryCategory);
            SecondaryCategories[item.PrimaryCategory].Sort();
        }
    }
}

[Serializable]
public class StringListDictionary : SerializableDictionary<string, List<string>> { }

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