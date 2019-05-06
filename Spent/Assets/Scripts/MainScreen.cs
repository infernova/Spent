using TMPro;
using UnityEngine;
using UnityEngine.UI;
using StarstruckFramework;
using System;
using System.Collections.Generic;

public class MainScreen : SingletonBehavior<MainScreen>
{
    private const int NULL_INDEX = -1;

    [HideInInspector]
    public ExpenditureDescriptionItem ScrollingInDescription;
    [HideInInspector]
    public List<ExpenditureDescriptionItem> ScrollingOutDescriptions = new List<ExpenditureDescriptionItem>();

    [Header("Display State Template")]
    [SerializeField]
    private Button mAddScreenButton;
    [SerializeField]
    private Button mExpenditureListScreenButton;
    [SerializeField]
    private Button mCostBreakdownScreenButton;
    [SerializeField]
    private Button mSettingsScreenButton;

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
                return Expenditures.DateRangeItems;
            }
        }
    }

    #region Item Edit
    [Header("Item Edit")]
    [SerializeField]
    private GameObject mEditExpenditureButtonContainer;

    public void ResetEditIndices()
    {
        EditIndex = NULL_INDEX;
        CostBreakdownIndex = NULL_INDEX;
        RecurringEditIndex = NULL_INDEX;
    }

    public void SelectExpenditure(int index)
    {
        if (mExpenditureListContainer.gameObject.activeSelf)
        {
            SelectExpenditureListItem(index);
        }
        else if (mCostBreakdownContainer.gameObject.activeSelf)
        {
            SelectCostBreakdownItem(index);
        }
        else if (mRecurringExpenditureListContainer.gameObject.activeSelf)
        {
            SelectRecurringExpenditureItem(index);
        }
    }

    public void StartItemEdit()
    {
        SetDisplayState(DisplayState.EDIT);

        ExpenditureItem item = mIsCostBreakdownExpenseList 
            ? mCostBreakdownExpenditureListItems[CostBreakdownIndex]
            : Expenditures.DateRangeItems[EditIndex];

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
        if (CostBreakdownIndex != NULL_INDEX)
        {
            mCostBreakdownContainer.SetActive(true);
        }
        else if (EditIndex != NULL_INDEX)
        {
            mExpenditureListContainer.SetActive(true);
        }
        else if (RecurringEditIndex != NULL_INDEX)
        {
            mRecurringExpenditureListContainer.SetActive(true);
        }

        ResetEditIndices();
        LoadBlankExpenditure();
        mAddExpenditureContainer.SetActive(false);
    }

    public void ConfirmEdit()
    {
        if (RecurringEditIndex != NULL_INDEX)
        {
            ConfirmRecurringEdit();
            return;
        }

        int day = int.Parse(mDayField.text);
        int month = int.Parse(mMonthField.text);
        int year = int.Parse(mYearField.text);

        ExpenditureItem item = mIsCostBreakdownExpenseList
            ? mCostBreakdownExpenditureListItems[CostBreakdownIndex]
            : Expenditures.DateRangeItems[EditIndex];

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

        ResetEditIndices();

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

        ResetEditIndices();
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
            LoadExpenditureList();
        }
        else if (mCostBreakdownContainer.activeSelf)
        {
            LoadCostBreakdown(mSelectedCostBreakdownCat);
        }

        RepositionCostBreakdownBarChart();
    }
    #endregion

    private DateTime pauseTime;
    private bool mIsInit;

    private PoolMgr mPoolMgr;

    private void Awake()
    {
        mPoolMgr = PoolMgr.Instance;

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

        mCostBreakdownBarChartWidth = mCostBreakdownBarChartScrollView.GetComponent<RectTransform>().rect.width;
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            if (mIsInit 
                && DateTime.Now > pauseTime.AddMinutes(3))
            {
                LoadBlankExpenditure();
                SetDisplayState(DisplayState.ADD);
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
                mAddExpenditureButtonContainer.SetActive(true);
                mAddExpenditureRecurringButton.SetActive(true);
            }
            else if ((EditIndex != NULL_INDEX || CostBreakdownIndex != NULL_INDEX) && !mEditExpenditureButtonContainer.activeSelf)
            {
                mEditExpenditureButtonContainer.SetActive(true);
                mAddExpenditureButtonContainer.SetActive(false);
                mAddExpenditureRecurringButton.SetActive(false);
            }
            else if (RecurringEditIndex != NULL_INDEX && !mEditExpenditureButtonContainer.activeSelf)
            {
                mEditExpenditureButtonContainer.SetActive(true);
                mAddExpenditureButtonContainer.SetActive(false);
                mAddExpenditureRecurringButton.SetActive(false);
            }

            if (mAddExpenditureButtonContainer.activeSelf)
            {
                mAddExpenditureButton.interactable = !string.IsNullOrEmpty(mAmountField.text)
                    && !string.IsNullOrEmpty(mPrimaryCatField.text)
                    && !string.IsNullOrEmpty(mSecondaryCatField.text)
                    && !mDayText.text.Equals(INVALID_DATE, StringComparison.InvariantCulture);
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
        RECURRING,
        SETTINGS
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
                if (EditIndex != NULL_INDEX 
                    || CostBreakdownIndex != NULL_INDEX
                    || RecurringEditIndex != NULL_INDEX)
                {
                    CancelEdit();
                }

                LoadBlankExpenditure();

                mAddExpenditureContainer.SetActive(true);
                mExpenditureListContainer.SetActive(false);
                mCostBreakdownContainer.SetActive(false);
                mRecurringExpenditureListContainer.SetActive(false);
                mSettingsContainer.SetActive(false);

                mAddScreenButton.interactable = false;
                mExpenditureListScreenButton.interactable = true;
                mCostBreakdownScreenButton.interactable = true;
                mSettingsScreenButton.interactable = true;
                break;

            case DisplayState.LIST:
                LoadExpenditureList();

                mAddExpenditureContainer.SetActive(false);
                mExpenditureListContainer.SetActive(true);
                mCostBreakdownContainer.SetActive(false);
                mRecurringExpenditureListContainer.SetActive(false);
                mSettingsContainer.SetActive(false);

                mAddScreenButton.interactable = true;
                mExpenditureListScreenButton.interactable = false;
                mCostBreakdownScreenButton.interactable = true;
                mSettingsScreenButton.interactable = true;
                break;

            case DisplayState.BREAKDOWN:
                LoadCostBreakdown(string.Empty);

                mAddExpenditureContainer.SetActive(false);
                mExpenditureListContainer.SetActive(false);
                mCostBreakdownContainer.SetActive(true);
                mRecurringExpenditureListContainer.SetActive(false);
                mSettingsContainer.SetActive(false);

                mAddScreenButton.interactable = true;
                mExpenditureListScreenButton.interactable = true;
                mCostBreakdownScreenButton.interactable = false;
                mSettingsScreenButton.interactable = true;
                break;

            case DisplayState.EDIT:
                mAddExpenditureContainer.SetActive(true);
                mExpenditureListContainer.SetActive(false);
                mCostBreakdownContainer.SetActive(false);
                mRecurringExpenditureListContainer.SetActive(false);
                mSettingsContainer.SetActive(false);

                mAddScreenButton.interactable = true;
                mExpenditureListScreenButton.interactable = true;
                mCostBreakdownScreenButton.interactable = true;
                mSettingsScreenButton.interactable = true;
                break;

            case DisplayState.RECURRING:
                LoadRecurringExpenditureList();

                mAddExpenditureContainer.SetActive(false);
                mExpenditureListContainer.SetActive(false);
                mCostBreakdownContainer.SetActive(false);
                mRecurringExpenditureListContainer.SetActive(true);
                mSettingsContainer.SetActive(false);

                mAddScreenButton.interactable = true;
                mExpenditureListScreenButton.interactable = true;
                mCostBreakdownScreenButton.interactable = true;
                mSettingsScreenButton.interactable = true;
                break;

            case DisplayState.SETTINGS:
                mAddExpenditureContainer.SetActive(false);
                mExpenditureListContainer.SetActive(false);
                mCostBreakdownContainer.SetActive(false);
                mRecurringExpenditureListContainer.SetActive(false);
                mSettingsContainer.SetActive(true);

                mAddScreenButton.interactable = true;
                mExpenditureListScreenButton.interactable = true;
                mCostBreakdownScreenButton.interactable = true;
                mSettingsScreenButton.interactable = false;
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
    private Button mAddExpenditureButton;
    [SerializeField]
    private GameObject mAddExpenditureRecurringButton;
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

    private const string INVALID_DATE = "(Invalid)";

    public void OnDateEndEdit()
    {
        try
        {
            if (mIsRecurring.isOn)
            {
                int day = int.Parse(mDayField.text);
                if (day < 0 || day > 31)
                {
                    mDayText.text = INVALID_DATE;
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
            mDayText.text = INVALID_DATE;
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

            LoadBlankExpenditure();

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
    private GameObject mExpenditureListNilText;
    [SerializeField]
    private DailyExpenditureScrollList mExpenditureList;
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
                mExpenditureList.ItemList[i].CheckIfSelected(mEditIndex);
            }

            mEditExpenditureButton.interactable = EditIndex != NULL_INDEX;
            mRemoveExpenditureButton.interactable = EditIndex != NULL_INDEX;
        }
    }

    public void LoadExpenditureList()
    {
        mSelectedCostBreakdownCat = string.Empty;

        ResetEditIndices();

        mExpenditureListContainer.SetActive(true);
        mExpenditureList.Init(Expenditures.DateRangeItems);
        mExpenditureListNilText.SetActive(Expenditures.DateRangeItems.Count == 0);
    }

    public void SelectExpenditureListItem(int index)
    {
        EditIndex = index;
    }
    #endregion

    #region Cost Breakdown
    [Header("Cost Breakdown")]
    [SerializeField]
    private GameObject mCostBreakdownContainer;
    [SerializeField]
    private GameObject mCostBreakdownNilText;
    [SerializeField]
    private TextMeshProUGUI mCostBreakdownTitle;
    [SerializeField]
    private TextMeshProUGUI mTotalSpendAmount;
    [SerializeField]
    private GUILiteScrollList mCostBreakdownList;
    [SerializeField]
    private DailyExpenditureScrollList mCostBreakdownDailyExpenditureList;
    [SerializeField]
    private TextMeshProUGUI mCostBreakdownDateRangeText;
    [SerializeField]
    private GameObject mCostBreakdownBackButton;
    [SerializeField]
    private GameObject mCostBreakdownEditButton;
    [SerializeField]
    private GameObject mCostBreakdownRemoveButton;
    public List<CostBreakdownItem> CostBreakdownItems;
    [HideInInspector]
    public List<Color> CostBreakdownItemColours;

    private string mSelectedCostBreakdownCat = string.Empty;
    private int mCostBreakdownIndex = NULL_INDEX;

    [SerializeField]
    private int mCostBreakdownScrollListMinSize;

    [SerializeField]
    private GameObject mCostBreakdownPieChartContainer;
    [SerializeField]
    private GameObject mCostBreakdownPieChartButton;
    private List<GameObject> mCostBreakdownPieChartInstances = new List<GameObject>();

    [SerializeField]
    private GameObject mCostBreakdownBarChartContainer;
    [SerializeField]
    private GameObject mCostBreakdownBarChartButton;
    [SerializeField]
    private ScrollRect mCostBreakdownBarChartScrollView;
    [SerializeField]
    private TextMeshProUGUI[] mCostBreakdownBarChartLineAmounts;
    [SerializeField]
    private RectTransform[] mCostBreakdownBarChartLineTrans;
    [SerializeField]
    private RectTransform mCostBreakdownBarChartLeftAxis;
    [SerializeField]
    private RectTransform mCostBreakdownBarChartScrollViewContent;
    [SerializeField]
    private GameObject mCostBreakdownBarChartScrollBar;
    private List<GameObject> mCostBreakdownBarChartInstances = new List<GameObject>();
    private float mCostBreakdownBarChartWidth;
    private int mCostBreakdownNumMonths;
    private const float COST_BREAKDOWN_MAX_BAR_INTERVAL = 4.65f;

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

            if (mCostBreakdownList.gameObject.activeSelf)
            {
                for (int i = 0; i < mCostBreakdownList.ItemList.Count; i++)
                {
                    ((SelectableListItem)mCostBreakdownList.ItemList[i]).CheckIfSelected(mCostBreakdownIndex);
                }
            }

            if (mCostBreakdownDailyExpenditureList.gameObject.activeSelf)
            {
                for (int i = 0; i < mCostBreakdownDailyExpenditureList.ItemList.Count; i++)
                {
                    mCostBreakdownDailyExpenditureList.ItemList[i].CheckIfSelected(mCostBreakdownIndex);
                }
            }

            mCostBreakdownEditButton.GetComponent<Button>().interactable = mCostBreakdownIndex != NULL_INDEX;
            mCostBreakdownRemoveButton.GetComponent<Button>().interactable = mCostBreakdownIndex != NULL_INDEX;
        }
    }

    public void LoadCostBreakdown(string s)
    {
        LoadCostBreakdown(s, false);
    }

    private HSBColor GetRandomHSBColour()
    {
        return new HSBColor(UnityEngine.Random.Range(0.0f, 1.0f),
                            UnityEngine.Random.Range(0.2f, 0.5f),
                            UnityEngine.Random.Range(0.75f, 1.0f));
    }

    public void LoadCostBreakdown(string s, bool overrideInput)
    {
        mCostBreakdownList.gameObject.SetActive(true);
        mCostBreakdownDailyExpenditureList.gameObject.SetActive(false);

        ResetEditIndices();

        mCostBreakdownContainer.SetActive(true);

        float totalAmount = 0.0f;
        List<float> monthlyAmounts = new List<float> { 0.0f };
        DateTime startingMonth = Expenditures.Items[Expenditures.Items.Count - 1].Date;
        startingMonth = new DateTime(startingMonth.Year, startingMonth.Month, 1);
        DateTime currMonth = new DateTime(startingMonth.Year, startingMonth.Month, 1);

        Dictionary<string, float> categoryAmounts = new Dictionary<string, float>();

        mCostBreakdownEditButton.SetActive(false);
        mCostBreakdownRemoveButton.SetActive(false);

        DateTime latestDate = Expenditures.Items[0].Date;
        bool needCurrMonthEntry = false;

        if (latestDate.Year < DateTime.Now.Year 
            || (latestDate.Year == DateTime.Now.Year && latestDate.Month < DateTime.Now.Month))
        {
            needCurrMonthEntry = true;
        }

        if (string.IsNullOrEmpty(s))
        {
            mSelectedCostBreakdownCat = string.Empty;
            mCostBreakdownTitle.SetText("TOTAL SPENDING");
            mCostBreakdownBackButton.SetActive(false);

            mCostBreakdownList.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                mCostBreakdownScrollListMinSize + 150.0f);

            for (int i = Expenditures.Items.Count - 1; i >= 0; i--)
            {
                ExpenditureItem item = Expenditures.Items[i];

                while (!(item.Date.DateTime.Year == currMonth.Year && item.Date.DateTime.Month == currMonth.Month))
                {
                    currMonth = currMonth.AddMonths(1);
                    monthlyAmounts.Add(0.0f);
                }

                monthlyAmounts[monthlyAmounts.Count - 1] += item.Amount;

                DateTime itemDate = item.Date;

                if (Expenditures.DateRange.Year == 1 ||
                    (itemDate.Year == Expenditures.DateRange.Year && itemDate.Month == Expenditures.DateRange.Month))
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

            mCostBreakdownList.gameObject.SetActive(true);
            mCostBreakdownDailyExpenditureList.gameObject.SetActive(false);
        }
        else
        {
            mCostBreakdownBackButton.SetActive(true);

            mCostBreakdownList.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                mCostBreakdownScrollListMinSize + 150.0f);

            if (!s.Contains("\t")
                && (string.IsNullOrEmpty(mSelectedCostBreakdownCat)
                    || mSelectedCostBreakdownCat.Contains("\t")
                    || (mSelectedCostBreakdownCat.Equals(s) && !overrideInput)))
            {
                mCostBreakdownTitle.SetText((s + " SPENDING").ToUpper());

                mSelectedCostBreakdownCat = s;

                for (int i = Expenditures.Items.Count - 1; i >= 0; i--)
                {
                    ExpenditureItem item = Expenditures.Items[i];

                    while (!(item.Date.DateTime.Year == currMonth.Year && item.Date.DateTime.Month == currMonth.Month))
                    {
                        currMonth = currMonth.AddMonths(1);
                        monthlyAmounts.Add(0.0f);
                    }

                    if (!item.PrimaryCategory.Equals(s, StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }

                    monthlyAmounts[monthlyAmounts.Count - 1] += item.Amount;

                    DateTime itemDate = item.Date;

                    if (Expenditures.DateRange.Year == 1 ||
                        (itemDate.Year == Expenditures.DateRange.Year && itemDate.Month == Expenditures.DateRange.Month))
                    {
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

                mCostBreakdownList.gameObject.SetActive(true);
                mCostBreakdownDailyExpenditureList.gameObject.SetActive(false);
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

                for (int i = Expenditures.Items.Count - 1; i >= 0; i--)
                {
                    ExpenditureItem item = Expenditures.Items[i];

                    while (!(item.Date.DateTime.Year == currMonth.Year && item.Date.DateTime.Month == currMonth.Month))
                    {
                        currMonth = currMonth.AddMonths(1);
                        monthlyAmounts.Add(0.0f);
                    }

                    if (!item.PrimaryCategory.Equals(priCat, StringComparison.InvariantCultureIgnoreCase)
                        || !item.SecondaryCategory.Equals(secCat, StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }

                    monthlyAmounts[monthlyAmounts.Count - 1] += item.Amount;

                    DateTime itemDate = item.Date;

                    if (Expenditures.DateRange.Year == 1 ||
                        (itemDate.Year == Expenditures.DateRange.Year && itemDate.Month == Expenditures.DateRange.Month))
                    {
                        totalAmount += item.Amount;

                        mCostBreakdownExpenditureListItems.Add(item);
                    }
                }

                mCostBreakdownList.gameObject.SetActive(false);
                mCostBreakdownDailyExpenditureList.gameObject.SetActive(true);

                mCostBreakdownEditButton.SetActive(true);
                mCostBreakdownRemoveButton.SetActive(true);

                mCostBreakdownList.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                    mCostBreakdownScrollListMinSize + 0.0f);
            }
        }

        if (needCurrMonthEntry)
        {
            monthlyAmounts.Add(0.0f);
        }

        float lineInterval = 65.0f;

        if (mSelectedCostBreakdownCat.Contains("\t"))
        {
            mCostBreakdownExpenditureListItems.Reverse();
            mCostBreakdownDailyExpenditureList.Init(mCostBreakdownExpenditureListItems);

            mCostBreakdownPieChartButton.SetActive(false);
            mCostBreakdownBarChartButton.SetActive(false);

            mCostBreakdownPieChartContainer.SetActive(false);
            mCostBreakdownBarChartContainer.SetActive(true);
        }
        else
        {
            CostBreakdownItems = new List<CostBreakdownItem>();
            CostBreakdownItemColours = new List<Color>();

            foreach (KeyValuePair<string, float> pair in categoryAmounts)
            {
                CostBreakdownItems.Add(new CostBreakdownItem(pair.Key, pair.Value, (pair.Value / totalAmount) * 100.0f));

                /*
                Color newColour = Color.black;
                Color prevColour = CostBreakdownItemColours.Count == 0 
                    ? Color.black 
                    : CostBreakdownItemColours[CostBreakdownItemColours.Count - 1];

                do
                {
                    newColour = new Color(UnityEngine.Random.Range(0.0f, 1.0f),
                        UnityEngine.Random.Range(0.0f, 1.0f),
                        UnityEngine.Random.Range(0.0f, 1.0f));
                } while (CostBreakdownItemColours.Count != 0
                && (Mathf.Abs(newColour.r - prevColour.r) < 0.15f
                || Mathf.Abs(newColour.g - prevColour.g) < 0.15f
                || Mathf.Abs(newColour.b - prevColour.b) < 0.15f));

                CostBreakdownItemColours.Add(newColour);
                */

                HSBColor newHSBColour = new HSBColor();
                Color newColour = Color.black;
                Color prevColour = CostBreakdownItemColours.Count == 0
                    ? Color.black
                    : CostBreakdownItemColours[CostBreakdownItemColours.Count - 1];

                if (CostBreakdownItemColours.Count < categoryAmounts.Count - 1
                    || categoryAmounts.Count == 1)
                {
                    int loopCount = 0;
                    do
                    {
                        newHSBColour = GetRandomHSBColour();
                        newColour = newHSBColour.ToColor();
                        loopCount++;
                    } while (loopCount < 100
                             && CostBreakdownItemColours.Count != 0
                             && (Mathf.Abs(newColour.r - prevColour.r) < 0.1f
                                 || Mathf.Abs(newColour.g - prevColour.g) < 0.1f
                                 || Mathf.Abs(newColour.b - prevColour.b) < 0.1f));
                }
                else
                {
                    int loopCount = 0;
                    Color firstColor = CostBreakdownItemColours[0];
                    do
                    {
                        newHSBColour = GetRandomHSBColour();
                        newColour = newHSBColour.ToColor();
                        loopCount++;
                    } while (loopCount < 100
                             && (Mathf.Abs(newColour.r - prevColour.r) < 0.1f
                                 || Mathf.Abs(newColour.g - prevColour.g) < 0.1f
                                 || Mathf.Abs(newColour.b - prevColour.b) < 0.1f
                                 || Mathf.Abs(newColour.r - firstColor.r) < 0.1f
                                 || Mathf.Abs(newColour.g - firstColor.g) < 0.1f
                                 || Mathf.Abs(newColour.b - firstColor.b) < 0.1f));
                }
                

                CostBreakdownItemColours.Add(newColour);
            }

            CostBreakdownItems.Sort();
            mCostBreakdownList.Init(CostBreakdownItems.Count);

            foreach(GameObject pie in mCostBreakdownPieChartInstances)
            {
                mPoolMgr.DestroyObj(pie);
            }

            float percent = 1.0f;

            if (CostBreakdownItems.Count == 0)
            {
                CostBreakdownItemColours.Add(GetRandomHSBColour().ToColor());

                GameObject pieSlice = mPoolMgr.InstantiateObj(ObjectPoolType.PIE_SLICE,
                        mCostBreakdownPieChartContainer.transform);
                pieSlice.SetActive(true);
                pieSlice.GetComponent<PieChartItem>().SetColour(CostBreakdownItemColours[0]);
                pieSlice.GetComponent<PieChartItem>().SetPercentage(percent);

                mCostBreakdownPieChartInstances.Add(pieSlice);
            }
            else
            {
                for (int i = CostBreakdownItems.Count - 1; i >= 0; i--)
                {
                    GameObject pieSlice = mPoolMgr.InstantiateObj(ObjectPoolType.PIE_SLICE,
                        mCostBreakdownPieChartContainer.transform);
                    pieSlice.SetActive(true);
                    pieSlice.GetComponent<PieChartItem>().SetColour(CostBreakdownItemColours[i]);

                    pieSlice.GetComponent<PieChartItem>().SetPercentage(percent);

                    percent -= CostBreakdownItems[i].Percentage * 0.01f;

                    mCostBreakdownPieChartInstances.Add(pieSlice);
                }
            }

            mCostBreakdownNilText.SetActive(CostBreakdownItems.Count == 0);

            mCostBreakdownPieChartButton.SetActive(true);
            mCostBreakdownBarChartButton.SetActive(true);

            mCostBreakdownPieChartContainer.SetActive(!mCostBreakdownPieChartButton.GetComponent<Button>().interactable);
            mCostBreakdownBarChartContainer.SetActive(!mCostBreakdownBarChartButton.GetComponent<Button>().interactable);

            for (int i = 0; i < mCostBreakdownList.ItemList.Count; i++)
            {
                ((CostBreakdownListItem)mCostBreakdownList.ItemList[i]).IsColoursVisibile(!mCostBreakdownPieChartButton.GetComponent<Button>().interactable);
            }
        }

        foreach (GameObject bar in mCostBreakdownBarChartInstances)
        {
            mPoolMgr.DestroyObj(bar);
        }

        float barInteval = 0.0f;

        if (monthlyAmounts.Count <= 4)
        {
            mCostBreakdownBarChartScrollViewContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                mCostBreakdownBarChartWidth);

            barInteval = mCostBreakdownBarChartScrollViewContent.rect.width / (float)monthlyAmounts.Count;

            mCostBreakdownBarChartScrollView.horizontal = false;
            mCostBreakdownBarChartScrollBar.SetActive(false);
        }
        else
        {
            barInteval = mCostBreakdownBarChartWidth / COST_BREAKDOWN_MAX_BAR_INTERVAL;

            mCostBreakdownBarChartScrollViewContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                barInteval * monthlyAmounts.Count);

            mCostBreakdownBarChartScrollView.horizontal = true;
            mCostBreakdownBarChartScrollBar.SetActive(true);
        }

        float maxAmount = 0.0f;
        foreach (float amount in monthlyAmounts)
        {
            maxAmount = Mathf.Max(amount, maxAmount);
        }

        int magnitude = 1;
        while (maxAmount >= 10.0f)
        {
            maxAmount *= 0.1f;
            magnitude *= 10;
        }

        maxAmount = Mathf.Ceil(maxAmount);
        maxAmount *= magnitude;

        float baseLinePos = mCostBreakdownBarChartLineTrans[0].anchoredPosition.y;

        for (int i = 0; i < 5; i++)
        {
            float lineAmount = maxAmount * (0.2f * (i + 1));

            if (maxAmount < 100.0f)
            {
                mCostBreakdownBarChartLineAmounts[i].SetText("$" + lineAmount.ToString("0.00"));
            }
            else
            {
                mCostBreakdownBarChartLineAmounts[i].SetText("$" + Mathf.RoundToInt(lineAmount).ToString());
            }

            mCostBreakdownBarChartLineTrans[i + 1].anchoredPosition = 
                new Vector2(0.0f, baseLinePos + (lineInterval * (i + 1)));
        }

        mCostBreakdownBarChartLeftAxis.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
            (lineInterval * 5.0f) + 32.5f);

        mPoolMgr.GetPooledObjRef(ObjectPoolType.BAR_CHART_ITEM)
            .GetComponent<CostBreakdownBarItem>().MaxBarHeight = lineInterval * 5.0f;

        for (int i = 0; i < monthlyAmounts.Count; i++)
        {
            GameObject bar = mPoolMgr.InstantiateObj(ObjectPoolType.BAR_CHART_ITEM,
                mCostBreakdownBarChartScrollViewContent);

            bar.SetActive(true);

            DateTime barMonth = startingMonth.AddMonths(i);
            bool isSelectedMonth = Expenditures.DateRange.Year == 1 ||
                (Expenditures.DateRange.Year == barMonth.Year && Expenditures.DateRange.Month == barMonth.Month);

            bar.GetComponent<RectTransform>().anchoredPosition = new Vector2(barInteval * (i + 0.5f), 0.0f);
            bar.GetComponent<CostBreakdownBarItem>().Init(barMonth,
                monthlyAmounts[i] / maxAmount,
                isSelectedMonth);
            mCostBreakdownBarChartInstances.Add(bar);
        }

        mCostBreakdownNumMonths = monthlyAmounts.Count;
        RepositionCostBreakdownBarChart();

        mTotalSpendAmount.text = "$" + totalAmount.ToString("0.00");
    }

    private void RepositionCostBreakdownBarChart()
    {
        if (mCostBreakdownBarChartContainer.activeInHierarchy)
        {
            if (mCostBreakdownNumMonths > 4)
            {
                if (Expenditures.DateRange.Year == 1)
                {
                    mCostBreakdownBarChartScrollViewContent.anchoredPosition =
                        new Vector2(mCostBreakdownBarChartWidth - mCostBreakdownBarChartScrollViewContent.rect.width, 0.0f);
                }
                else
                {
                    DateTime startingMonth = Expenditures.Items[Expenditures.Items.Count - 1].Date;
                    startingMonth = new DateTime(startingMonth.Year, startingMonth.Month, 1);

                    float barInteval = mCostBreakdownBarChartWidth / COST_BREAKDOWN_MAX_BAR_INTERVAL;

                    int numMonths = 0;

                    while (!(startingMonth.AddMonths(numMonths).Year == Expenditures.DateRange.Year
                        && startingMonth.AddMonths(numMonths).Month == Expenditures.DateRange.Month))
                    {
                        numMonths++;
                    }

                    float pos = mCostBreakdownBarChartWidth * 0.5f - ((numMonths + 0.5f) * barInteval);

                    pos = Mathf.Min(0.0f, pos);
                    pos = Mathf.Max(mCostBreakdownBarChartWidth - mCostBreakdownBarChartScrollViewContent.rect.width, pos);

                    mCostBreakdownBarChartScrollViewContent.anchoredPosition =
                        new Vector2(pos, 0.0f);
                }
            }
        }
    }

    public void SetChartVisibility(bool isPieChart)
    {
        mCostBreakdownPieChartButton.GetComponent<Button>().interactable = !isPieChart;
        mCostBreakdownBarChartButton.GetComponent<Button>().interactable = isPieChart;

        mCostBreakdownPieChartContainer.SetActive(isPieChart);
        mCostBreakdownBarChartContainer.SetActive(!isPieChart);

        for (int i = 0; i < mCostBreakdownList.ItemList.Count; i++)
        {
            ((CostBreakdownListItem)mCostBreakdownList.ItemList[i]).IsColoursVisibile(isPieChart);
        }

        if (!isPieChart)
        {
            RepositionCostBreakdownBarChart();
        }
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
    private DailyExpenditureScrollList mRecurringExpenditureList;
    [SerializeField]
    private GameObject mRecurringExpenditureNilText;
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
                mRecurringExpenditureList.ItemList[i].CheckIfSelected(mRecurringEditIndex);
            }

            mEditRecurringExpenditureButton.interactable = mRecurringEditIndex != NULL_INDEX;
            mRemoveRecurringExpenditureButton.interactable = mRecurringEditIndex != NULL_INDEX;
        }
    }

    public void LoadRecurringExpenditureList()
    {
        mSelectedCostBreakdownCat = string.Empty;

        ResetEditIndices();

        mRecurringExpenditureListContainer.SetActive(true);
        mRecurringExpenditureList.Init(RecurringExpense.RecurringItemList);

        mRecurringExpenditureNilText.SetActive(RecurringExpense.RecurringItemList.Count == 0);
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
        ResetEditIndices();

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

        ResetEditIndices();

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

    #region Settings
    [Header("Settings")]
    [SerializeField]
    private GameObject mSettingsContainer;
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