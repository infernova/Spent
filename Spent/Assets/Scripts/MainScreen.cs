using TMPro;
using UnityEngine;
using UnityEngine.UI;
using StarstruckFramework;
using System;
using System.Collections.Generic;

public class MainScreen : SingletonBehavior<MainScreen>
{
    public const string DateTimeFormatString = "yyyy-MM-dd HH:mm:ss K";

    public void IncrementDateRange()
    {
        mStats.IncrementDateRange();
        RefreshDateRange();
    }

    public void DecrementDateRange()
    {
        mStats.DecrementDateRange();
        RefreshDateRange();
    }

    private void RefreshDateRange()
    {
        if (mStats.DateRange.Year == 1)
        {
            mExpenditureListDateRangeText.SetText("All Time");
            mCostBreakdownDateRangeText.SetText("All Time");
        }
        else
        {
            mExpenditureListDateRangeText.SetText(mStats.DateRange.ToString("MMM yyyy"));
            mCostBreakdownDateRangeText.SetText(mStats.DateRange.ToString("MMM yyyy"));
        }

        if (mExpenditureListContainer.activeSelf)
        {
            mExpenditureList.Init(mStats.DisplayedItems.Count);
        }
        else if (mCostBreakdownContainer.activeSelf)
        {
            LoadCostBreakdown(string.Empty);
        }
    }

    private void Start()
    {
        LoadBlankExpenditure();

        if (PlayerPrefs.HasKey(SaveString))
        {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(SaveString), mStats);
        }

        mStats.LoadCategories();
        mStats.LoadRecurringExpenditureList();

        mStats.RefreshDateRange();
        RefreshDateRange();

        CheckRecurringExpenditure();
    }

    private void OnApplicationQuit()
    {
        if (Application.isEditor)
        {
            PlayerPrefs.SetString(SaveString, JsonUtility.ToJson(mStats));
            mStats.ClearStats();
        }
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

                CheckRecurringExpenditure();
            }
            catch
            {

            }
        }
        else
        {
            PlayerPrefs.SetString(SaveString, JsonUtility.ToJson(mStats));
        }
    }

    private void Update()
    {
        if (mAddExpenditureContainer.activeSelf)
        {
            if (EditIndex == -1 && RecurringEditIndex == -1 && !mAddExpenditureButton.activeSelf)
            {
                mEditExpenditureButtonContainer.SetActive(false);
                mEditRecurringExpenditureButtonContainer.SetActive(false);
                mAddExpenditureButton.SetActive(true);
            }
            else if (EditIndex != -1 && !mEditExpenditureButtonContainer.activeSelf)
            {
                mEditExpenditureButtonContainer.SetActive(true);
                mEditRecurringExpenditureButtonContainer.SetActive(false);
                mAddExpenditureButton.SetActive(false);
            }
            else if (RecurringEditIndex != -1 && !mEditRecurringExpenditureButtonContainer.activeSelf)
            {
                mEditExpenditureButtonContainer.SetActive(false);
                mEditRecurringExpenditureButtonContainer.SetActive(true);
                mAddExpenditureButton.SetActive(false);
            }
        }

        if (mAddExpenditureContainer.activeSelf)
        {
            
        }

        if (mPrimaryCatField.isFocused)
        {
            mPrimaryCatDropdown.gameObject.SetActive(true);
            mSecondaryCatDropdown.gameObject.SetActive(false);
            mCategoryBlackout.gameObject.SetActive(true);
        }

        if (mSecondaryCatField.isFocused)
        {
            mPrimaryCatDropdown.gameObject.SetActive(false);
            mSecondaryCatDropdown.gameObject.SetActive(true);
            mCategoryBlackout.gameObject.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            mAddExpenditureContainer.SetActive(true);
            mExpenditureListContainer.SetActive(false);
            mCostBreakdownContainer.SetActive(false);
            mRecurringExpenditureListContainer.SetActive(false);
        }
    }

    #region Add Expenditure
    [Header("Add Expenditure")]
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
    private Toggle mIsRecurring;


    [SerializeField]
    private GameObject DaySeperatorGob;
    [SerializeField]
    private GameObject OfTheMonthGob;

    [SerializeField]
    private GameObject mCategoryBlackout;
    [SerializeField]
    private CategoryScrollview mPrimaryCatDropdown;
    [SerializeField]
    private CategoryScrollview mSecondaryCatDropdown;

    [SerializeField]
    private TMP_Text mDayText;

    private List<string> mPrimaryCatOptions = new List<string>();
    private List<string> mSecondaryCatOptions = new List<string>();

    private const string SaveString = "Save";
    private const string CurrDateSaveString = "CurrDate";

    [SerializeField]
    private ExpenditureStats mStats;

    private void CheckRecurringExpenditure()
    {
        if (!PlayerPrefs.HasKey(CurrDateSaveString))
        {
            PlayerPrefs.SetString(CurrDateSaveString,
                                  DateTime.UtcNow.Date.ToString(DateTimeFormatString,
                                                                System.Globalization.CultureInfo.InvariantCulture));
            return;
        }

        DateTime lastLoad = DateTime.ParseExact(PlayerPrefs.GetString(CurrDateSaveString),
                                                DateTimeFormatString,
                                                System.Globalization.CultureInfo.InvariantCulture,
                                                System.Globalization.DateTimeStyles.AdjustToUniversal).Date;

        bool isNewDay = false;

        while (lastLoad < DateTime.UtcNow.Date)
        {
            DateTime newDate = new DateTime(lastLoad.Year, lastLoad.Month, lastLoad.Day);
            newDate = DateTime.SpecifyKind(newDate, DateTimeKind.Local);

            foreach(ExpenditureItem item in mStats.RecurringItems[lastLoad.Day].List)
            {
                mStats.Add(new ExpenditureItem(item.Amount,
                                               item.PrimaryCategory,
                                               item.SecondaryCategory,
                                               item.Description,
                                               newDate.Date));
            }

            lastLoad = lastLoad.AddDays(1.0f).Date;

            isNewDay = true;
        }

        if (isNewDay)
        {
            mStats.RefreshDateRange();
        }

        PlayerPrefs.SetString(CurrDateSaveString,
                              DateTime.UtcNow.Date.ToString(DateTimeFormatString,
                                                            System.Globalization.CultureInfo.InvariantCulture));
    }

	private void LoadBlankExpenditure()
    {
        mAddExpenditureContainer.SetActive(true);
        mExpenditureListContainer.SetActive(false);
        mCostBreakdownContainer.SetActive(false);
        mRecurringExpenditureListContainer.SetActive(false);

        mDayField.text = DateTime.Now.ToString("dd");
        mMonthField.text = DateTime.Now.ToString("MM");
        mYearField.text = DateTime.Now.ToString("yyyy");

        mAmountField.text = string.Empty;
        mPrimaryCatField.text = string.Empty;
        mSecondaryCatField.text = string.Empty;
        mDescriptionField.text = string.Empty;

        mIsRecurring.isOn = false;
        mIsRecurring.interactable = true;
        OnRecurringValueChange(mIsRecurring.isOn);

        OnDateEndEdit();
    }

    public void OnRecurringValueChange (bool isRecurring)
    {
        OfTheMonthGob.SetActive(isRecurring);
        DaySeperatorGob.SetActive(!isRecurring);

        OnDateEndEdit();
    }

    public void OnDateEndEdit()
    {
        try
        {
            if (mIsRecurring.isOn)
            {
                int day = int.Parse(mDayField.text);
                if (day < 0 || day > 31)
                {
                    mDayText.text = "(Invalid)";
                }
                else
                {
                    mDayText.text = "(Sun)";
                }
            }
            else
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

    public void OnCategoryEditEnd()
    {
        mPrimaryCatField.text = mPrimaryCatField.text.ToUpper();
        mPrimaryCatDropdown.gameObject.SetActive(false);

        mSecondaryCatField.text = mSecondaryCatField.text.ToUpper();
        mSecondaryCatDropdown.gameObject.SetActive(false);

        mCategoryBlackout.SetActive(false);
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
        mCategoryBlackout.SetActive(false);
    }

    public void OnSecondaryCatSelect(int value)
    {
        mSecondaryCatField.text = mSecondaryCatOptions[value].ToUpper();
        mSecondaryCatDropdown.gameObject.SetActive(false);
        mCategoryBlackout.SetActive(false);
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

            float amount = float.Parse(mAmountField.text);

            if (mIsRecurring.isOn)
            {
                mStats.AddRecurringExpenditure(new ExpenditureItem(amount,
                                                                   mPrimaryCatField.text,
                                                                   mSecondaryCatField.text,
                                                                   mDescriptionField.text,
                                                                   new DateTime(2000, 1, day)));
            }
            else
            {
                DateTime date = new DateTime(year, month, day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                date = DateTime.SpecifyKind(date, DateTimeKind.Local);

                mStats.Add(new ExpenditureItem(amount,
                                               mPrimaryCatField.text,
                                               mSecondaryCatField.text,
                                               mDescriptionField.text,
                                               date));

                mStats.RefreshDisplayedItems();
            }

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
                || !mStats.SecondaryCategories[mPrimaryCatField.text].Contains(text.ToUpper().Trim())))
        {
            mSecondaryCatOptions.Add(text.ToUpper().Trim());
        }

        if (text.Length == 0 && mStats.LastUsedSecondaryCategories.ContainsKey(mPrimaryCatField.text.ToUpper()))
        {
            mSecondaryCatOptions = mStats.LastUsedSecondaryCategories[mPrimaryCatField.text.ToUpper()];
        }

        mSecondaryCatDropdown.UpdateOptions(mSecondaryCatOptions, false);
    }
    #endregion

    #region Expenditure List
    [Header("Expenditure List")]
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
    [SerializeField]
    private TextMeshProUGUI mExpenditureListDateRangeText;
    private int mEditIndex = -1;
    public int EditIndex
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
                ((ExpenditureListItem)mExpenditureList.ItemList[i]).CheckIfSelected(mEditIndex);
            }

            mEditExpenditureButton.interactable = EditIndex != -1;
            mRemoveExpenditureButton.interactable = EditIndex != -1;
        }
    }

    public void LoadExpenditureList()
    {
        mExpenditureListContainer.SetActive(true);
        mExpenditureList.Init(mStats.DisplayedItems.Count);
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

        ExpenditureItem item = mStats.DisplayedItems[EditIndex];

        mDayField.text = item.Date.ToString("dd");
        mMonthField.text = item.Date.ToString("MM");
        mYearField.text = item.Date.ToString("yyyy");

        mAmountField.text = item.Amount.ToString("0.00");
        mPrimaryCatField.text = item.PrimaryCategory;
        mSecondaryCatField.text = item.SecondaryCategory;
        mDescriptionField.text = item.Description;

        mIsRecurring.isOn = false;
        mIsRecurring.interactable = false;
    }

    public void CancelEdit()
    {
        EditIndex = -1;

        LoadBlankExpenditure();

        mExpenditureListContainer.SetActive(true);
        mAddExpenditureContainer.SetActive(false);
    }

    public void ConfirmEdit()
    {
        int day = int.Parse(mDayField.text);
        int month = int.Parse(mMonthField.text);
        int year = int.Parse(mYearField.text);

        DateTime editDateTime = mStats.DisplayedItems[EditIndex].Date.DateTime;

        DateTime date = new DateTime(year, month, day, editDateTime.Hour, editDateTime.Minute, editDateTime.Second);
        date = DateTime.SpecifyKind(date, DateTimeKind.Local);

        float amount = float.Parse(mAmountField.text);

        mStats.Remove(EditIndex);
        mStats.Add(new ExpenditureItem(amount,
                                       mPrimaryCatField.text,
                                       mSecondaryCatField.text,
                                       mDescriptionField.text,
                                       date));

        mStats.RefreshDisplayedItems();

        EditIndex = -1;

        LoadBlankExpenditure();

        mExpenditureListContainer.SetActive(true);
        mAddExpenditureContainer.SetActive(false);
        LoadExpenditureList();
    }

    public void RemoveExpenditure()
    {
        mStats.Remove(EditIndex);
        LoadExpenditureList();
    }
    #endregion

    #region Cost Breakdown
    [Header("Cost Breakdown")]
    [SerializeField]
    private GameObject mCostBreakdownContainer;
    [SerializeField]
    private TextMeshProUGUI mTotalSpendAmount;
    [SerializeField]
    private GUILiteScrollList mCostBreakdownList;
    [SerializeField]
    private TextMeshProUGUI mCostBreakdownDateRangeText;
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
            foreach (ExpenditureItem item in mStats.DisplayedItems)
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
            foreach (ExpenditureItem item in mStats.DisplayedItems)
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

    #region Recurring Expenditure List
    [Header("Recurring Expenditure")]
    [SerializeField]
    private GameObject mRecurringExpenditureListContainer;
    [SerializeField]
    private GameObject mEditRecurringExpenditureButtonContainer;
    [SerializeField]
    private GUILiteScrollList mRecurringExpenditureList;
    [SerializeField]
    private Button mEditRecurringExpenditureButton;
    [SerializeField]
    private Button mRemoveRecurringExpenditureButton;
    private int mRecurringEditIndex = -1;
    public int RecurringEditIndex
    {
        get { return mRecurringEditIndex; }
        set
        {
            if (mRecurringEditIndex == value)
            {
                mRecurringEditIndex = -1;
            }
            else
            {
                mRecurringEditIndex = value;
            }

            for (int i = 0; i < mRecurringExpenditureList.ItemList.Count; i++)
            {
                ((RecurringExpenditureItem)mRecurringExpenditureList.ItemList[i]).CheckIfSelected(mRecurringEditIndex);
            }

            mEditRecurringExpenditureButton.interactable = mRecurringEditIndex != -1;
            mRemoveRecurringExpenditureButton.interactable = mRecurringEditIndex != -1;
        }
    }

    public void LoadRecurringExpenditureList()
    {
        mRecurringExpenditureListContainer.SetActive(true);
        mRecurringExpenditureList.Init(mStats.RecurringItemList.Count);
        RecurringEditIndex = -1;
    }

    public void SelectRecurringExpenditureItem(int index)
    {
        RecurringEditIndex = index;
    }

    public void StartRecurringItemEdit()
    {
        mRecurringExpenditureListContainer.SetActive(false);
        mAddExpenditureContainer.SetActive(true);

        ExpenditureItem item = mStats.RecurringItemList[RecurringEditIndex];

        mDayField.text = item.Date.ToString("dd");
        mMonthField.text = item.Date.ToString("MM");
        mYearField.text = item.Date.ToString("yyyy");

        mAmountField.text = item.Amount.ToString("0.00");
        mPrimaryCatField.text = item.PrimaryCategory;
        mSecondaryCatField.text = item.SecondaryCategory;
        mDescriptionField.text = item.Description;

        mIsRecurring.isOn = true;
        mIsRecurring.interactable = false;
    }

    public void CancelRecurringEdit()
    {
        RecurringEditIndex = -1;

        LoadBlankExpenditure();

        mRecurringExpenditureListContainer.SetActive(true);
        mAddExpenditureContainer.SetActive(false);
    }

    public void ConfirmRecurringEdit()
    {
        int day = int.Parse(mDayField.text);

        DateTime date = new DateTime(2000, 1, day);
        date = DateTime.SpecifyKind(date, DateTimeKind.Local);

        float amount = float.Parse(mAmountField.text);

        mStats.RemoveRecurringExpenditure(RecurringEditIndex);
        mStats.AddRecurringExpenditure(new ExpenditureItem(amount,
                                       mPrimaryCatField.text,
                                       mSecondaryCatField.text,
                                       mDescriptionField.text,
                                       date));
        RecurringEditIndex = -1;

        LoadBlankExpenditure();

        mRecurringExpenditureListContainer.SetActive(true);
        mAddExpenditureContainer.SetActive(false);
        LoadRecurringExpenditureList();
    }

    public void RemoveRecurringExpenditure()
    {
        mStats.RemoveRecurringExpenditure(RecurringEditIndex);
        LoadRecurringExpenditureList();
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

    public int CompareTo(object obj)
    {
        if (obj == null) return 1;

        CostBreakdownItem other = obj as CostBreakdownItem;
        if (other != null)
        {
            if (other.Amount > Amount)
            {
                return 1;
            }
            else if (other.Amount < Amount)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
        else
        {
            return 1;
        }
    }
}