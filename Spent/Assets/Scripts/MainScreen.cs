﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;
using StarstruckFramework;
using System;
using System.Collections.Generic;

public class MainScreen : SingletonBehavior<MainScreen>
{
    private void Update()
    {
        if (mAddExpenditureContainer.activeSelf)
        {
            if (EditIndex == -1 && !mAddExpenditureButton.activeSelf)
            {
                mEditExpenditureButtonContainer.SetActive(false);
                mAddExpenditureButton.SetActive(true);
            }
            else if (EditIndex != -1 && !mEditExpenditureButtonContainer.activeSelf)
            {
                mEditExpenditureButtonContainer.SetActive(true);
                mAddExpenditureButton.SetActive(false);
            }
        }
    }

    #region Add Expenditure
    [SerializeField]
    private GameObject mAddExpenditureContainer;
    [SerializeField]
    private GameObject mAddExpenditureButton;
    [SerializeField]
    private TMP_InputField mDayField;
    [SerializeField]
    private TMP_InputField mMonthField;
    [SerializeField]
    private TMP_InputField mYearField;
    [SerializeField]
    private TMP_InputField mAmountField;
    [SerializeField]
    private TMP_InputField mPrimaryCatField;
    [SerializeField]
    private TMP_InputField mSecondaryCatField;
    [SerializeField]
    private TMP_InputField mDescriptionField;

    [SerializeField]
    private CategoryScrollview mPrimaryCatDropdown;
    [SerializeField]
    private CategoryScrollview mSecondaryCatDropdown;

    [SerializeField]
    private TMP_Text mDayText;

    private List<string> mPrimaryCatOptions = new List<string>();
    private List<string> mSecondaryCatOptions = new List<string>();

    private bool mIsStallingForChoiceInput;

    [SerializeField]
    private ExpenditureStats mStats;

    private void Start()
    {
        LoadBlankExpenditure();
    }

	private void OnApplicationPause(bool pause)
	{
        if (!pause)
        {
            try
            {
                int day = int.Parse(mDayField.text);
                int month = int.Parse(mMonthField.text);
                int year = int.Parse(mYearField.text);

                DateTime date = new DateTime(year, month, day);

                if (!mAddExpenditureContainer.activeSelf
                    || DateTime.Now.Date != date)
                {
                    LoadBlankExpenditure();
                }
            }
            catch
            {
                
            }
        }
	}

	private void LoadBlankExpenditure()
    {
        mAddExpenditureContainer.SetActive(true);
        mExpenditureListContainer.SetActive(false);
        mCostBreakdownContainer.SetActive(false);

        mDayField.text = DateTime.Now.ToString("dd");
        mMonthField.text = DateTime.Now.ToString("MM");
        mYearField.text = DateTime.Now.ToString("yyyy");

        mAmountField.text = string.Empty;
        mPrimaryCatField.text = string.Empty;
        mSecondaryCatField.text = string.Empty;
        mDescriptionField.text = string.Empty;

        OnDateEndEdit();
    }

    private void LateUpdate()
    {
        if (!mPrimaryCatField.isFocused
            && !mIsStallingForChoiceInput
            && mPrimaryCatDropdown.gameObject.activeSelf)
        {
            mPrimaryCatDropdown.gameObject.SetActive(false);
        }
        else if (mPrimaryCatField.isFocused
                 && !mPrimaryCatDropdown.gameObject.activeSelf)
        {
            mPrimaryCatDropdown.gameObject.SetActive(true);
        }

        if (!mSecondaryCatField.isFocused
            && !mIsStallingForChoiceInput
            && mSecondaryCatDropdown.gameObject.activeSelf)
        {
            mSecondaryCatDropdown.gameObject.SetActive(false);
        }
        else if (mSecondaryCatField.isFocused
                 && !mSecondaryCatDropdown.gameObject.activeSelf)
        {
            mSecondaryCatDropdown.gameObject.SetActive(true);
        }
    }

    public void OnDateEndEdit()
    {
        try
        {
            int day = int.Parse(mDayField.text);
            int month = int.Parse(mMonthField.text);
            int year = int.Parse(mYearField.text);

            DateTime date = new DateTime(year, month, day);

            switch (date.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    mDayText.text = "(Sun)";
                    break;
                case DayOfWeek.Monday:
                    mDayText.text = "(Mon)";
                    break;
                case DayOfWeek.Tuesday:
                    mDayText.text = "(Tue)";
                    break;
                case DayOfWeek.Wednesday:
                    mDayText.text = "(Wed)";
                    break;
                case DayOfWeek.Thursday:
                    mDayText.text = "(Thu)";
                    break;
                case DayOfWeek.Friday:
                    mDayText.text = "(Fri)";
                    break;
                case DayOfWeek.Saturday:
                    mDayText.text = "(Sat)";
                    break;
            }
        }
        catch
        {
            mDayText.text = "(Invalid)";
        }
    }

    public void OnAmountEndEdit(string s)
    {
        if (s.Length > 0)
        {
            float amount = float.Parse(mAmountField.text);
            mAmountField.text = amount.ToString("0.00");
        }
    }

    public void OnPrimaryCatEndEdit(string s)
    {
        mPrimaryCatField.text = s.ToUpper();
        mPrimaryCatDropdown.gameObject.SetActive(false);
    }

    public void OnSecondaryCatEndEdit(string s)
    {
        mSecondaryCatField.text = s.ToUpper();
        mSecondaryCatDropdown.gameObject.SetActive(false);
    }

    public void OnPrimaryCatSelect(int value)
    {
        mPrimaryCatField.text = mPrimaryCatOptions[value].ToUpper();
        mPrimaryCatDropdown.gameObject.SetActive(false);

        mPrimaryCatField.DeactivateInputField();
        mIsStallingForChoiceInput = false;
    }

    public void OnSecondaryCatSelect(int value)
    {
        mSecondaryCatField.text = mSecondaryCatOptions[value].ToUpper();
        mSecondaryCatDropdown.gameObject.SetActive(false);

        mSecondaryCatField.DeactivateInputField();
        mIsStallingForChoiceInput = false;
    }

    public void StallForChoiceInput()
    {
        mIsStallingForChoiceInput = true;
    }

    public void RefocusCategoryInput(bool isPrimaryCat)
    {
        if (isPrimaryCat)
        {
            mPrimaryCatField.ActivateInputField();
            mSecondaryCatDropdown.gameObject.SetActive(false);
        }
        else
        {
            mSecondaryCatField.ActivateInputField();
            mPrimaryCatDropdown.gameObject.SetActive(false);
        }
    }

    public void AddExpenditure()
    {
        if (mAmountField.text.Length > 0
            && mPrimaryCatField.text.Length > 0
            && mSecondaryCatField.text.Length > 0
            && !mDayText.text.Equals("(Invalid)", StringComparison.InvariantCulture))
        {
            int day = int.Parse(mDayField.text);
            int month = int.Parse(mMonthField.text);
            int year = int.Parse(mYearField.text);

            DateTime date = new DateTime(year, month, day);
            float amount = float.Parse(mAmountField.text);

            mStats.Add(new ExpenditureItem(amount,
                mPrimaryCatField.text,
                mSecondaryCatField.text,
                mDescriptionField.text,
                date));

            mAmountField.text = string.Empty;
            mPrimaryCatField.text = string.Empty;
            mSecondaryCatField.text = string.Empty;
            mDescriptionField.text = string.Empty;

            mPrimaryCatDropdown.gameObject.SetActive(false);
            mSecondaryCatDropdown.gameObject.SetActive(false);

            mDayField.text = DateTime.Now.ToString("dd");
            mMonthField.text = DateTime.Now.ToString("MM");
            mYearField.text = DateTime.Now.ToString("yyyy");

            OnDateEndEdit();
        }
    }

    public void LoadPrimaryCatOptions(string text)
    {
        mPrimaryCatOptions = new List<string>();
        if (text.Length > 0)
        {
            bool hasFoundMatch = false;
            for (int i = 0; i < mStats.PrimaryCategories.Count; i++)
            {
                if (mStats.PrimaryCategories[i].StartsWith(text, StringComparison.InvariantCultureIgnoreCase))
                {
                    mPrimaryCatOptions.Add(mStats.PrimaryCategories[i]);
                    hasFoundMatch = true;
                }
                else if (hasFoundMatch)
                {
                    break;
                }
            }

            if (!mStats.PrimaryCategories.Contains(text.ToUpper()))
            {
                mPrimaryCatOptions.Add(text.ToUpper());
            }
        }

        if (text.Length == 0)
        {
            mPrimaryCatOptions = mStats.LastUsedPrimaryCategories;
        }

        mPrimaryCatDropdown.UpdateOptions(mPrimaryCatOptions, true);
        mPrimaryCatDropdown.gameObject.SetActive(true);
    }

    public void LoadSecondaryCatOptions(string text)
    {
        mSecondaryCatOptions = new List<string>();
        if (text.Length > 0
            && mStats.SecondaryCategories.ContainsKey(mPrimaryCatField.text))
        {
            bool hasFoundMatch = false;
            string primaryCat = mPrimaryCatField.text;
            for (int i = 0; i < mStats.SecondaryCategories[primaryCat].Count; i++)
            {
                if (mStats.SecondaryCategories[primaryCat][i].StartsWith(text,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    mSecondaryCatOptions.Add(mStats.SecondaryCategories[primaryCat][i]);
                    hasFoundMatch = true;
                }
                else if (hasFoundMatch)
                {
                    break;
                }
            }
        }

        if (text.Length > 0
            && (!mStats.SecondaryCategories.ContainsKey(mPrimaryCatField.text)
            || !mStats.SecondaryCategories[mPrimaryCatField.text].Contains(text.ToUpper())))
        {
            mSecondaryCatOptions.Add(text.ToUpper());
        }

        if (text.Length == 0 && mStats.LastUsedSecondaryCategories.ContainsKey(mPrimaryCatField.text.ToUpper()))
        {
            mSecondaryCatOptions = mStats.LastUsedSecondaryCategories[mPrimaryCatField.text.ToUpper()].List;
        }

        mSecondaryCatDropdown.UpdateOptions(mSecondaryCatOptions, false);
        mSecondaryCatDropdown.gameObject.SetActive(true);
    }
    #endregion

    #region Expenditure List
    [SerializeField]
    private GameObject mExpenditureListContainer;
    [SerializeField]
    private GameObject mEditExpenditureButtonContainer;
    [SerializeField]
    private GUILiteScrollList mExpenditureList;
    [SerializeField]
    private Button mEditExpenditureButton;
    [SerializeField]
    private Button mRemoveExpenditureButton;
    private int mEditIndex = -1;
    private int EditIndex
    {
        get { return mEditIndex; }
        set
        {
            if (mEditIndex == value)
            {
                mEditIndex = -1;
            }
            else
            {
                mEditIndex = value;
            }

            for (int i = 0; i < mExpenditureList.ItemList.Count; i++)
            {
                ((ExpenditureListItem)mExpenditureList.ItemList[i]).SetAsSelected(i == mEditIndex);
            }

            mEditExpenditureButton.interactable = EditIndex != -1;
            mRemoveExpenditureButton.interactable = EditIndex != -1;
        }
    }

    public void LoadExpenditureList()
    {
        mExpenditureListContainer.SetActive(true);
        mExpenditureList.Init(mStats.Items.Count);
        EditIndex = -1;
    }

    public void SelectExpenditureItem(int index)
    {
        EditIndex = index;
    }

    public void StartItemEdit()
    {
        mExpenditureListContainer.SetActive(false);
        mAddExpenditureContainer.SetActive(true);

        ExpenditureItem item = mStats.Items[EditIndex];

        mDayField.text = item.Date.ToString("dd");
        mMonthField.text = item.Date.ToString("MM");
        mYearField.text = item.Date.ToString("yyyy");

        mAmountField.text = item.Amount.ToString("0.00");
        mPrimaryCatField.text = item.PrimaryCategory;
        mSecondaryCatField.text = item.SecondaryCategory;
        mDescriptionField.text = item.Description;
    }

    public void CancelEdit()
    {
        EditIndex = -1;
        mExpenditureListContainer.SetActive(true);
        mAddExpenditureContainer.SetActive(false);
    }

    public void ConfirmEdit()
    {
        int day = int.Parse(mDayField.text);
        int month = int.Parse(mMonthField.text);
        int year = int.Parse(mYearField.text);

        DateTime date = new DateTime(year, month, day);
        float amount = float.Parse(mAmountField.text);

        mStats.Edit(mStats.Items[EditIndex],
                    new ExpenditureItem(amount,
                                        mPrimaryCatField.text,
                                        mSecondaryCatField.text,
                                        mDescriptionField.text,
                                        date));

        EditIndex = -1;
        mExpenditureListContainer.SetActive(true);
        mAddExpenditureContainer.SetActive(false);
    }

    public void RemoveExpenditure()
    {
        mStats.Remove(EditIndex);
        LoadExpenditureList();
    }
    #endregion

    #region Cost Breakdown
    [SerializeField]
    private GameObject mCostBreakdownContainer;
    [SerializeField]
    private TextMeshProUGUI mTotalSpendAmount;
    [SerializeField]
    private GUILiteScrollList mCostBreakdownList;
    public List<CostBreakdownItem> CostBreakdownItems;
    private int mCostBreakdownIndex = -1;
    private int CostBreakdownIndex
    {
        get { return mCostBreakdownIndex; }
        set
        {
            if (mCostBreakdownIndex == value)
            {
                mCostBreakdownIndex = -1;
            }
            else
            {
                mCostBreakdownIndex = value;
            }

            for (int i = 0; i < mCostBreakdownList.ItemList.Count; i++)
            {
                ((CostBreakdownListItem)mCostBreakdownList.ItemList[i]).SetAsSelected(i == mCostBreakdownIndex);
            }
        }
    }

    public void LoadCostBreakdown(string s)
    {
        mCostBreakdownContainer.SetActive(true);
        float totalAmount = 0.0f;
        Dictionary<string, float> categoryAmounts = new Dictionary<string, float>();

        if (string.IsNullOrEmpty(s))
        {
            foreach (ExpenditureItem item in mStats.Items)
            {
                totalAmount += item.Amount;
                if (!categoryAmounts.ContainsKey(item.PrimaryCategory))
                {
                    categoryAmounts.Add(item.PrimaryCategory, item.Amount);
                }
                else
                {
                    categoryAmounts[item.PrimaryCategory] += item.Amount;
                }
            }
        }
        else
        {
            foreach (ExpenditureItem item in mStats.Items)
            {
                if (!item.PrimaryCategory.Equals(s, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                totalAmount += item.Amount;
                if (!categoryAmounts.ContainsKey(item.SecondaryCategory))
                {
                    categoryAmounts.Add(item.SecondaryCategory, item.Amount);
                }
                else
                {
                    categoryAmounts[item.SecondaryCategory] += item.Amount;
                }
            }
        }

        CostBreakdownItems = new List<CostBreakdownItem>();
        foreach(KeyValuePair<string, float> pair in categoryAmounts)
        {
            CostBreakdownItems.Add(new CostBreakdownItem(pair.Key, pair.Value, (pair.Value / totalAmount) * 100.0f));
        }

        CostBreakdownItems.Sort();

        mTotalSpendAmount.text = "$" + totalAmount.ToString("0.00");
        mCostBreakdownList.Init(CostBreakdownItems.Count);
        CostBreakdownIndex = -1;
    }

    public void SelectCostBreakdownItem(int index)
    {
        CostBreakdownIndex = index;
    }
    #endregion
}

public class CostBreakdownItem : IComparable
{
    public string Category;
    public float Amount;
    public float Percentage;

    public CostBreakdownItem(string cat, float amount, float percentage)
    {
        Category = cat;
        Amount = amount;
        Percentage = percentage;
    }

    int IComparable.CompareTo (object other)
    {
        if (other.GetType() != typeof(CostBreakdownItem))
        {
            return 0;
        }
        if (((CostBreakdownItem)other).Amount > Amount)
        {
            return 1;
        }
        else if (((CostBreakdownItem)other).Amount < Amount)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }
}