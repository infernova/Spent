using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class MainScreen : MonoBehaviour
{
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
        mDayField.text = DateTime.Now.ToString("dd");
        mMonthField.text = DateTime.Now.ToString("MM");
        mYearField.text = DateTime.Now.ToString("yyyy");

        OnDateEndEdit();

        mStats.LoadCategories();
    }

    private void LateUpdate()
    {
        if (!mPrimaryCatField.isFocused 
            && !mIsStallingForChoiceInput
            && mPrimaryCatDropdown.gameObject.activeSelf)
        {
            mPrimaryCatDropdown.gameObject.SetActive(false);
        }

        if (!mSecondaryCatField.isFocused
            && !mIsStallingForChoiceInput
            && mSecondaryCatDropdown.gameObject.activeSelf)
        {
            mSecondaryCatDropdown.gameObject.SetActive(false);
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
            for (int i = 0; i < mStats.SecondaryCategories.Count; i++)
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

        mSecondaryCatDropdown.UpdateOptions(mSecondaryCatOptions, false);
        mSecondaryCatDropdown.gameObject.SetActive(true);
    }
}
