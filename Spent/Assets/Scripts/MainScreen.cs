using TMPro;
using UnityEngine;
using UnityEngine.UI;
using StarstruckFramework;
using System;
using System.Collections.Generic;

public class MainScreen : SingletonBehavior<MainScreen>
{
    private const int NULL_INDEX = -1;

    [Header("Display State Template")]
    [SerializeField]
    private Button mAddScreenButton;
    [SerializeField]
    private Button mExpenditureListScreenButton;
    [SerializeField]
    private Button mCostBreakdownScreenButton;

    [Header("List Template")]
    [SerializeField]
    private GameObject ExpenditureListTemplate;
    [SerializeField]
    private GameObject CostBreakdownListTemplate;
    [SerializeField]
    private GameObject CategoryExpenditureListTemplate;

    private bool mIsCostBreakdownExpenseList
    {
        get { return mSelectedCostBreakdownCat.Contains("\t"); }
    }

    private List<ExpenditureItem> mCostBreakdownExpenditureListItems = new List<ExpenditureItem>();
    public List<ExpenditureItem> DisplayedItems
    {
        get
        {
            if (mIsCostBreakdownExpenseList)
            {
                return mCostBreakdownExpenditureListItems;
            }
            else
            {
                return Expenditures.DateRangetems;
            }
        }
    }

    #region Item Edit
    public void StartItemEdit()
    {
        SetDisplayState(DisplayState.EDIT);

        ExpenditureItem item = mIsCostBreakdownExpenseList 
            ? mCostBreakdownExpenditureListItems[CostBreakdownIndex]
            : Expenditures.DateRangetems[EditIndex];

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
        EditIndex = NULL_INDEX;
        CostBreakdownIndex = NULL_INDEX;

        LoadBlankExpenditure();

        mAddExpenditureContainer.SetActive(false);

        if (mIsCostBreakdownExpenseList)
        {
            mCostBreakdownContainer.SetActive(true);
        }
        else
        {
            mExpenditureListContainer.SetActive(true);
        }
    }

    public void ConfirmEdit()
    {
        int day = int.Parse(mDayField.text);
        int month = int.Parse(mMonthField.text);
        int year = int.Parse(mYearField.text);

        ExpenditureItem item = mIsCostBreakdownExpenseList
            ? mCostBreakdownExpenditureListItems[CostBreakdownIndex]
            : Expenditures.DateRangetems[EditIndex];

        DateTime editDateTime = item.Date.DateTime;

        DateTime date = new DateTime(year, month, day, editDateTime.Hour, editDateTime.Minute, editDateTime.Second);
        date = DateTime.SpecifyKind(date, DateTimeKind.Local);

        float amount = float.Parse(mAmountField.text);

        ExpenditureStats backup = ScriptableObject.CreateInstance<ExpenditureStats>();
        backup.CopyExpenditures(Expenditures);

        Expenditures.Remove(item);
        Expenditures.Add(new ExpenditureItem(amount,
                                       mPrimaryCatField.text,
                                       mSecondaryCatField.text,
                                       mDescriptionField.text,
                                       date));

        Expenditures.RefreshDisplayedList();

        EditIndex = NULL_INDEX;
        CostBreakdownIndex = NULL_INDEX;

        if (Expenditures.EditCount > mBackup.EditCount)
        {
            mBackup = backup;
        }
        else if (mBackup.EditCount > Expenditures.EditCount)
        {
            Expenditures.CopyExpenditures(mBackup);
        }

        PlayerPrefs.SetString(SaveString, JsonUtility.ToJson(Expenditures));
        PlayerPrefs.SetString(BackupString, JsonUtility.ToJson(mBackup));

        LoadBlankExpenditure();

        mAddExpenditureContainer.SetActive(false);

        if (mIsCostBreakdownExpenseList)
        {
            mCostBreakdownContainer.SetActive(true);
            LoadCostBreakdown(mSelectedCostBreakdownCat);
        }
        else
        {
            mExpenditureListContainer.SetActive(true);
            LoadExpenditureList();
        }
    }

    public void RemoveExpenditure()
    {
        ExpenditureStats backup = ScriptableObject.CreateInstance<ExpenditureStats>();
        backup.CopyExpenditures(Expenditures);

        if (!mIsCostBreakdownExpenseList)
        {
            Expenditures.Remove(EditIndex);
        }
        else
        {
            Expenditures.Remove(DisplayedItems[CostBreakdownIndex]);
        }

        Expenditures.RefreshDisplayedList();

        if (Expenditures.EditCount > mBackup.EditCount)
        {
            mBackup = backup;
        }
        else if (mBackup.EditCount > Expenditures.EditCount)
        {
            Expenditures.CopyExpenditures(mBackup);
        }

        PlayerPrefs.SetString(SaveString, JsonUtility.ToJson(Expenditures));
        PlayerPrefs.SetString(BackupString, JsonUtility.ToJson(mBackup));

        if (!mIsCostBreakdownExpenseList)
        {
            LoadExpenditureList();
        }
        else
        {
            LoadCostBreakdown(mSelectedCostBreakdownCat);
        }

        EditIndex = NULL_INDEX;
        CostBreakdownIndex = NULL_INDEX;
    }
    #endregion

#region Date Range
    public const string DateTimeFormatString = "yyyy-MM-dd HH:mm:ss K";

    public void IncrementDateRange()
    {
        Expenditures.IncrementDateRange();
        RefreshDateRange();
    }

    public void DecrementDateRange()
    {
        Expenditures.DecrementDateRange();
        RefreshDateRange();
    }

    private void RefreshDateRange()
    {
        if (Expenditures.DateRange.Year == 1)
        {
            mExpenditureListDateRangeText.SetText("All Time");
            mCostBreakdownDateRangeText.SetText("All Time");
        }
        else
        {
            mExpenditureListDateRangeText.SetText(Expenditures.DateRange.ToString("MMM yyyy"));
            mCostBreakdownDateRangeText.SetText(Expenditures.DateRange.ToString("MMM yyyy"));
        }

        if (mExpenditureListContainer.activeSelf)
        {
            mExpenditureList.Init(Expenditures.DateRangetems.Count);
        }
        else if (mCostBreakdownContainer.activeSelf)
        {
            LoadCostBreakdown(mSelectedCostBreakdownCat);
        }
    }
    #endregion

    private DateTime pauseTime;
    private bool mIsInit;

    private void Start()
    {
        SetDisplayState(DisplayState.ADD);

        Expenditures = ScriptableObject.CreateInstance<ExpenditureStats>();
        mBackup = ScriptableObject.CreateInstance<ExpenditureStats>();

        RecurringExpense = ScriptableObject.CreateInstance<RecurringExpenditureStats>();

        if (PlayerPrefs.HasKey(SaveString))
        {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(SaveString), Expenditures);
        }

        if (PlayerPrefs.HasKey(RecurringString))
        {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(RecurringString), RecurringExpense);
        }

        if (PlayerPrefs.HasKey(BackupString))
        {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(BackupString), mBackup);
        }

        if (mBackup.EditCount > Expenditures.EditCount)
        {
            Expenditures.CopyExpenditures(mBackup);
        }

        Expenditures.LoadCategories();
        mBackup.LoadCategories();

        RecurringExpense.LoadRecurringExpenditureList();

        Expenditures.RefreshDateRange();
        RefreshDateRange();

        CheckRecurringExpenditure();

        pauseTime = DateTime.Now.Date;
        mIsInit = true;
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            if (!mAddExpenditureContainer.activeSelf
                && mIsInit 
                && DateTime.Now > pauseTime.AddMinutes(3))
            {
                LoadBlankExpenditure();
            }

            if (mIsInit && DateTime.Now.Date > pauseTime.Date)
            {
                CheckRecurringExpenditure();
            }
        }
        else
        {
            pauseTime = DateTime.Now;
        }
    }

	private void Update()
    {
        if (mAddExpenditureContainer.activeSelf)
        {
            if (EditIndex == NULL_INDEX
                && RecurringEditIndex == NULL_INDEX
                && CostBreakdownIndex == NULL_INDEX
                && !mAddExpenditureButtonContainer.activeSelf)
            {
                mEditExpenditureButtonContainer.SetActive(false);
                mEditRecurringExpenditureButtonContainer.SetActive(false);
                mAddExpenditureButtonContainer.SetActive(true);
            }
            else if ((EditIndex != NULL_INDEX || CostBreakdownIndex != NULL_INDEX) && !mEditExpenditureButtonContainer.activeSelf)
            {
                mEditExpenditureButtonContainer.SetActive(true);
                mEditRecurringExpenditureButtonContainer.SetActive(false);
                mAddExpenditureButtonContainer.SetActive(false);
            }
            else if (RecurringEditIndex != NULL_INDEX && !mEditRecurringExpenditureButtonContainer.activeSelf)
            {
                mEditExpenditureButtonContainer.SetActive(false);
                mEditRecurringExpenditureButtonContainer.SetActive(true);
                mAddExpenditureButtonContainer.SetActive(false);
            }
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
            if (mCostBreakdownContainer.activeSelf && !string.IsNullOrEmpty(mSelectedCostBreakdownCat))
            {
                SelectPreviousCostBreakdown();
            }
            else if (mAddExpenditureContainer.activeSelf
                     && !(EditIndex == NULL_INDEX
                          && RecurringEditIndex == NULL_INDEX
                          && CostBreakdownIndex == NULL_INDEX))
            {
                CancelEdit();
            }
            else
            {
                SetDisplayState(DisplayState.ADD, false);
            }
        }
    }

    public enum DisplayState
    {
        ADD,
        LIST,
        BREAKDOWN,
        EDIT,
        RECURRING
    }

    public void SetDisplayState(int state)
    {
        SetDisplayState((DisplayState)state);
    }

    public void SetDisplayState(DisplayState state, bool requiresUpdate = true)
    {
        switch (state)
        {
            case DisplayState.ADD:
                if (EditIndex != NULL_INDEX || CostBreakdownIndex != NULL_INDEX)
                {
                    CancelEdit();
                }
                else if (RecurringEditIndex != NULL_INDEX)
                {
                    CancelRecurringEdit();
                }

                LoadBlankExpenditure();

                mAddExpenditureContainer.SetActive(true);
                mExpenditureListContainer.SetActive(false);
                mCostBreakdownContainer.SetActive(false);
                mRecurringExpenditureListContainer.SetActive(false);

                mAddScreenButton.interactable = false;
                mExpenditureListScreenButton.interactable = true;
                mCostBreakdownScreenButton.interactable = true;
                break;

            case DisplayState.LIST:
                LoadExpenditureList();

                mAddExpenditureContainer.SetActive(false);
                mExpenditureListContainer.SetActive(true);
                mCostBreakdownContainer.SetActive(false);
                mRecurringExpenditureListContainer.SetActive(false);

                mAddScreenButton.interactable = true;
                mExpenditureListScreenButton.interactable = false;
                mCostBreakdownScreenButton.interactable = true;
                break;

            case DisplayState.BREAKDOWN:
                LoadCostBreakdown(string.Empty);

                mAddExpenditureContainer.SetActive(false);
                mExpenditureListContainer.SetActive(false);
                mCostBreakdownContainer.SetActive(true);
                mRecurringExpenditureListContainer.SetActive(false);

                mAddScreenButton.interactable = true;
                mExpenditureListScreenButton.interactable = true;
                mCostBreakdownScreenButton.interactable = false;
                break;

            case DisplayState.EDIT:
                mAddExpenditureContainer.SetActive(true);
                mExpenditureListContainer.SetActive(false);
                mCostBreakdownContainer.SetActive(false);
                mRecurringExpenditureListContainer.SetActive(false);

                mAddScreenButton.interactable = true;
                mExpenditureListScreenButton.interactable = true;
                mCostBreakdownScreenButton.interactable = true;
                break;

            case DisplayState.RECURRING:
                LoadRecurringExpenditureList();

                mAddExpenditureContainer.SetActive(false);
                mExpenditureListContainer.SetActive(false);
                mCostBreakdownContainer.SetActive(false);
                mRecurringExpenditureListContainer.SetActive(true);

                mAddScreenButton.interactable = true;
                mExpenditureListScreenButton.interactable = true;
                mCostBreakdownScreenButton.interactable = true;
                break;
        }

        if (requiresUpdate)
        {
            Update();
        }
    }

    #region Add Expenditure
    [Header("Add Expenditure")]
    [SerializeField]
    private GameObject mAddExpenditureContainer;
    [SerializeField]
    private GameObject mAddExpenditureButtonContainer;
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
    private const string BackupString = "Backup";
    private const string RecurringString = "Recurring";

    private const string CurrDateSaveString = "CurrDate";

    [SerializeField]
    private ExpenditureStats mBackup;
    public ExpenditureStats Expenditures;

    public RecurringExpenditureStats RecurringExpense;

    private void CheckRecurringExpenditure()
    {
        if (!PlayerPrefs.HasKey(CurrDateSaveString))
        {
            PlayerPrefs.SetString(CurrDateSaveString,
                                  DateTime.UtcNow.Date.ToString(DateTimeFormatString,
                                                                System.Globalization.CultureInfo.InvariantCulture));
            return;
        }

        ExpenditureStats backup = ScriptableObject.CreateInstance<ExpenditureStats>();
        backup.CopyExpenditures(Expenditures);

        DateTime lastLoad = DateTime.ParseExact(PlayerPrefs.GetString(CurrDateSaveString),
                                                DateTimeFormatString,
                                                System.Globalization.CultureInfo.InvariantCulture,
                                                System.Globalization.DateTimeStyles.AdjustToUniversal).Date;

        bool isNewDay = false;

        while (lastLoad < DateTime.UtcNow.Date)
        {
            DateTime newDate = new DateTime(lastLoad.Year, lastLoad.Month, lastLoad.Day);
            newDate = DateTime.SpecifyKind(newDate, DateTimeKind.Local);

            foreach(ExpenditureItem item in RecurringExpense.RecurringItems[lastLoad.Day].List)
            {
                Expenditures.Add(new ExpenditureItem(item.Amount,
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
            Expenditures.RefreshDateRange();
        }

        PlayerPrefs.SetString(CurrDateSaveString,
                              DateTime.UtcNow.Date.ToString(DateTimeFormatString,
                                                            System.Globalization.CultureInfo.InvariantCulture));

        if (Expenditures.EditCount > mBackup.EditCount)
        {
            mBackup = backup;
        }
        else if (mBackup.EditCount > Expenditures.EditCount)
        {
            Expenditures.CopyExpenditures(mBackup);
        }

        PlayerPrefs.SetString(SaveString, JsonUtility.ToJson(Expenditures));
        PlayerPrefs.SetString(BackupString, JsonUtility.ToJson(mBackup));
    }

	private void LoadBlankExpenditure()
    {
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
                RecurringExpense.AddRecurringExpenditure(new ExpenditureItem(amount,
                                                                       mPrimaryCatField.text,
                                                                       mSecondaryCatField.text,
                                                                       mDescriptionField.text,
                                                                       new DateTime(2000, 1, day)));

                PlayerPrefs.SetString(RecurringString, JsonUtility.ToJson(RecurringExpense));
            }
            else
            {
                ExpenditureStats backup = ScriptableObject.CreateInstance<ExpenditureStats>();
                backup.CopyExpenditures(Expenditures);

                DateTime date = new DateTime(year, month, day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                date = DateTime.SpecifyKind(date, DateTimeKind.Local);

                Expenditures.Add(new ExpenditureItem(amount,
                                               mPrimaryCatField.text,
                                               mSecondaryCatField.text,
                                               mDescriptionField.text,
                                               date));

                if (Expenditures.EditCount > mBackup.EditCount)
                {
                    mBackup = backup;
                }
                else if (mBackup.EditCount > Expenditures.EditCount)
                {
                    Expenditures.CopyExpenditures(mBackup);
                }

                PlayerPrefs.SetString(SaveString, JsonUtility.ToJson(Expenditures));
                PlayerPrefs.SetString(BackupString, JsonUtility.ToJson(mBackup));

                Expenditures.RefreshDisplayedList();
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

    public void OnPrimaryCatSelect(string text)
    {
        LoadPrimaryCatOptions(text, true);
    }

    public void OnPrimaryCatChange(string text)
    {
        LoadPrimaryCatOptions(text, false);
    }

    private void LoadPrimaryCatOptions(string text, bool isSelect)
    {
        mPrimaryCatOptions = new List<string>();
        if (text.Length > 0)
        {
            bool hasFoundMatch = false;
            for (int i = 0; i < Expenditures.PrimaryCategories.Count; i++)
            {
                if (Expenditures.PrimaryCategories[i].StartsWith(text, StringComparison.InvariantCultureIgnoreCase))
                {
                    mPrimaryCatOptions.Add(Expenditures.PrimaryCategories[i]);
                    hasFoundMatch = true;
                }
                else if (hasFoundMatch)
                {
                    break;
                }
            }

            if (!Expenditures.PrimaryCategories.Contains(text.ToUpper()))
            {
                mPrimaryCatOptions.Add(text.ToUpper());
            }
        }

        if (text.Length == 0)
        {
            mPrimaryCatOptions = Expenditures.LastUsedPrimaryCategories;
        }
        else if (isSelect)
        {
            List<string> lastUsedOptions = new List<string>(Expenditures.LastUsedPrimaryCategories);
            if (lastUsedOptions.Contains(text))
            {
                lastUsedOptions.Remove(text);
            }

            mPrimaryCatOptions.AddRange(lastUsedOptions);
        }

        mPrimaryCatDropdown.UpdateOptions(mPrimaryCatOptions, true);
    }

    public void OnSecondaryCatSelect(string text)
    {
        LoadSecondaryCatOptions(text, true);
    }

    public void OnSecondaryCatChange(string text)
    {
        LoadSecondaryCatOptions(text, false);
    }

    public void LoadSecondaryCatOptions(string text, bool isSelect)
    {
        mSecondaryCatOptions = new List<string>();
        if (text.Length > 0
            && Expenditures.SecondaryCategories.ContainsKey(mPrimaryCatField.text))
        {
            bool hasFoundMatch = false;
            string primaryCat = mPrimaryCatField.text;
            for (int i = 0; i < Expenditures.SecondaryCategories[primaryCat].Count; i++)
            {
                if (Expenditures.SecondaryCategories[primaryCat][i].StartsWith(text,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    mSecondaryCatOptions.Add(Expenditures.SecondaryCategories[primaryCat][i]);
                    hasFoundMatch = true;
                }
                else if (hasFoundMatch)
                {
                    break;
                }
            }
        }

        if (text.Length > 0
            && (!Expenditures.SecondaryCategories.ContainsKey(mPrimaryCatField.text)
                || !Expenditures.SecondaryCategories[mPrimaryCatField.text].Contains(text.ToUpper().Trim())))
        {
            mSecondaryCatOptions.Add(text.ToUpper().Trim());
        }

        if (Expenditures.LastUsedSecondaryCategories.ContainsKey(mPrimaryCatField.text.ToUpper()))
        {
            if (text.Length == 0)
            {
                mSecondaryCatOptions = Expenditures.LastUsedSecondaryCategories[mPrimaryCatField.text.ToUpper()];
            }
            else if (isSelect)
            {
                List<string> lastUsedOptions = new List<string>(Expenditures.LastUsedSecondaryCategories[mPrimaryCatField.text.ToUpper()]);
                if (lastUsedOptions.Contains(text))
                {
                    lastUsedOptions.Remove(text);
                }

                mSecondaryCatOptions.AddRange(lastUsedOptions);
            }
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
    private int mEditIndex = NULL_INDEX;
    public int EditIndex
    {
        get { return mEditIndex; }
        set
        {
            if (value != NULL_INDEX && mEditIndex == value)
            {
                StartItemEdit();
                return;
            }
            else
            {
                mEditIndex = value;
            }

            for (int i = 0; i < mExpenditureList.ItemList.Count; i++)
            {
                ((SelectableListItem)mExpenditureList.ItemList[i]).CheckIfSelected(mEditIndex);
            }

            mEditExpenditureButton.interactable = EditIndex != NULL_INDEX;
            mRemoveExpenditureButton.interactable = EditIndex != NULL_INDEX;
        }
    }

    public void LoadExpenditureList()
    {
        mSelectedCostBreakdownCat = string.Empty;

        EditIndex = NULL_INDEX;
        CostBreakdownIndex = NULL_INDEX;
        RecurringEditIndex = NULL_INDEX;

        mExpenditureListContainer.SetActive(true);
        mExpenditureList.Init(Expenditures.DateRangetems.Count);
    }

    public void SelectExpenditureItem(int index)
    {
        EditIndex = index;
    }
    #endregion

    #region Cost Breakdown
    [Header("Cost Breakdown")]
    [SerializeField]
    private GameObject mCostBreakdownContainer;
    [SerializeField]
    private TextMeshProUGUI mCostBreakdownTitle;
    [SerializeField]
    private TextMeshProUGUI mTotalSpendAmount;
    [SerializeField]
    private GUILiteScrollList mCostBreakdownList;
    [SerializeField]
    private TextMeshProUGUI mCostBreakdownDateRangeText;
    [SerializeField]
    private GameObject mCostBreakdownBackButton;
    [SerializeField]
    private GameObject mCostBreakdownEditButton;
    [SerializeField]
    private GameObject mCostBreakdownRemoveButton;
    public List<CostBreakdownItem> CostBreakdownItems;
    private string mSelectedCostBreakdownCat = string.Empty;
    private int mCostBreakdownIndex = NULL_INDEX;
    [SerializeField]
    private int mCostBreakdownScrollListMinSize;
    public int CostBreakdownIndex
    {
        get { return mCostBreakdownIndex; }
        private set
        {
            if (value != NULL_INDEX && mCostBreakdownIndex == value)
            {
                StartItemEdit();
                return;
            }
            else
            {
                mCostBreakdownIndex = value;
            }

            for (int i = 0; i < mCostBreakdownList.ItemList.Count; i++)
            {
                ((SelectableListItem)mCostBreakdownList.ItemList[i]).CheckIfSelected(mCostBreakdownIndex);
            }

            mCostBreakdownEditButton.GetComponent<Button>().interactable = mCostBreakdownIndex != NULL_INDEX;
            mCostBreakdownRemoveButton.GetComponent<Button>().interactable = mCostBreakdownIndex != NULL_INDEX;
        }
    }

    public void LoadCostBreakdown(string s)
    {
        LoadCostBreakdown(s, false);
    }

    public void LoadCostBreakdown(string s, bool overrideInput)
    {
        EditIndex = NULL_INDEX;
        CostBreakdownIndex = NULL_INDEX;
        RecurringEditIndex = NULL_INDEX;

        mCostBreakdownContainer.SetActive(true);

        float totalAmount = 0.0f;
        Dictionary<string, float> categoryAmounts = new Dictionary<string, float>();

        mCostBreakdownEditButton.SetActive(false);
        mCostBreakdownRemoveButton.SetActive(false);

        if (string.IsNullOrEmpty(s))
        {
            mSelectedCostBreakdownCat = string.Empty;
            mCostBreakdownTitle.SetText("TOTAL SPENDING");
            mCostBreakdownBackButton.SetActive(false);

            mCostBreakdownList.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, 300.0f);
            mCostBreakdownList.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                mCostBreakdownScrollListMinSize + 300.0f);

            foreach (ExpenditureItem item in Expenditures.DateRangetems)
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

            mCostBreakdownList.ListItemTemplate = CostBreakdownListTemplate;
        }
        else
        {
            mCostBreakdownBackButton.SetActive(true);

            mCostBreakdownList.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, 450.0f);
            mCostBreakdownList.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                mCostBreakdownScrollListMinSize + 150.0f);

            if (!s.Contains("\t")
                && (string.IsNullOrEmpty(mSelectedCostBreakdownCat)
                    || mSelectedCostBreakdownCat.Contains("\t")
                    || (mSelectedCostBreakdownCat.Equals(s) && !overrideInput)))
            {
                mCostBreakdownTitle.SetText((s + " SPENDING").ToUpper());

                mSelectedCostBreakdownCat = s;

                foreach (ExpenditureItem item in Expenditures.DateRangetems)
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

                mCostBreakdownList.ListItemTemplate = CostBreakdownListTemplate;
            }
            else
            {
                if (!mSelectedCostBreakdownCat.Contains("\t"))
                {
                    mSelectedCostBreakdownCat = mSelectedCostBreakdownCat + "\t" + s;
                }
                else
                {
                    mSelectedCostBreakdownCat = s;
                }

                mCostBreakdownTitle.SetText((mSelectedCostBreakdownCat.Split('\t')[1] + " SPENDING").ToUpper());

                string[] catArray = mSelectedCostBreakdownCat.Split('\t');
                string priCat = catArray[0];
                string secCat = catArray[1];

                mCostBreakdownExpenditureListItems = new List<ExpenditureItem>();

                foreach (ExpenditureItem item in Expenditures.DateRangetems)
                {
                    if (!item.PrimaryCategory.Equals(priCat, StringComparison.InvariantCultureIgnoreCase)
                        || !item.SecondaryCategory.Equals(secCat, StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }

                    totalAmount += item.Amount;

                    mCostBreakdownExpenditureListItems.Add(item);
                }

                mCostBreakdownList.ListItemTemplate = CategoryExpenditureListTemplate;

                mCostBreakdownEditButton.SetActive(true);
                mCostBreakdownRemoveButton.SetActive(true);

                mCostBreakdownList.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, 600.0f);
                mCostBreakdownList.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                    mCostBreakdownScrollListMinSize + 0.0f);
            }
        }

        if (mSelectedCostBreakdownCat.Contains("\t"))
        {
            mCostBreakdownList.Init(mCostBreakdownExpenditureListItems.Count);
        }
        else
        {
            CostBreakdownItems = new List<CostBreakdownItem>();
            foreach (KeyValuePair<string, float> pair in categoryAmounts)
            {
                CostBreakdownItems.Add(new CostBreakdownItem(pair.Key, pair.Value, (pair.Value / totalAmount) * 100.0f));
            }

            CostBreakdownItems.Sort();
            mCostBreakdownList.Init(CostBreakdownItems.Count);
        }

        mTotalSpendAmount.text = "$" + totalAmount.ToString("0.00");
    }

    public void SelectPreviousCostBreakdown()
    {
        if (mSelectedCostBreakdownCat.Contains("\t"))
        {
            LoadCostBreakdown(mSelectedCostBreakdownCat.Split('\t')[0]);
        }
        else
        {
            LoadCostBreakdown(string.Empty);
        }
    }

    public void SelectCostBreakdownItem(int index)
    {
        if (CostBreakdownIndex == index)
        {
            if (mSelectedCostBreakdownCat.Contains("\t"))
            {
                StartItemEdit();
            }
            else
            {
                LoadCostBreakdown(CostBreakdownItems[index].Category, true);
            }
        }
        else
        {
            CostBreakdownIndex = index;   
        }
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
    private int mRecurringEditIndex = NULL_INDEX;
    public int RecurringEditIndex
    {
        get { return mRecurringEditIndex; }
        set
        {
            if (value != NULL_INDEX && mRecurringEditIndex == value)
            {
                StartRecurringItemEdit();
                return;
            }
            else
            {
                mRecurringEditIndex = value;
            }

            for (int i = 0; i < mRecurringExpenditureList.ItemList.Count; i++)
            {
                ((SelectableListItem)mRecurringExpenditureList.ItemList[i]).CheckIfSelected(mRecurringEditIndex);
            }

            mEditRecurringExpenditureButton.interactable = mRecurringEditIndex != NULL_INDEX;
            mRemoveRecurringExpenditureButton.interactable = mRecurringEditIndex != NULL_INDEX;
        }
    }

    public void LoadRecurringExpenditureList()
    {
        mSelectedCostBreakdownCat = string.Empty;

        EditIndex = NULL_INDEX;
        CostBreakdownIndex = NULL_INDEX;
        RecurringEditIndex = NULL_INDEX;

        mRecurringExpenditureListContainer.SetActive(true);
        mRecurringExpenditureList.Init(RecurringExpense.RecurringItemList.Count);
    }

    public void SelectRecurringExpenditureItem(int index)
    {
        RecurringEditIndex = index;
    }

    public void StartRecurringItemEdit()
    {
        SetDisplayState(DisplayState.EDIT);

        ExpenditureItem item = RecurringExpense.RecurringItemList[RecurringEditIndex];

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
        RecurringEditIndex = NULL_INDEX;

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

        RecurringExpense.RemoveRecurringExpenditure(RecurringEditIndex);
        RecurringExpense.AddRecurringExpenditure(new ExpenditureItem(amount,
                                                               mPrimaryCatField.text,
                                                               mSecondaryCatField.text,
                                                               mDescriptionField.text,
                                                               date));

        PlayerPrefs.SetString(RecurringString, JsonUtility.ToJson(RecurringExpense));

        RecurringEditIndex = NULL_INDEX;

        LoadBlankExpenditure();

        mRecurringExpenditureListContainer.SetActive(true);
        mAddExpenditureContainer.SetActive(false);
        LoadRecurringExpenditureList();
    }

    public void RemoveRecurringExpenditure()
    {
        RecurringExpense.RemoveRecurringExpenditure(RecurringEditIndex);
        LoadRecurringExpenditureList();

        PlayerPrefs.SetString(RecurringString, JsonUtility.ToJson(RecurringExpense));
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