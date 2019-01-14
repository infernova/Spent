using System;
using UnityEngine;

[Serializable]
public struct SerializableDatetime : ISerializationCallbackReceiver
{
    public const string DateTimeFormatString = "yyyy-MM-dd HH:mm:ss K";

    [SerializeField]
    private string mDateTimeString;
    private DateTime mDateTime;

    public DateTime DateTime
    {
        get { return mDateTime; }
        set
        {
            mDateTime = value;
            mDateTimeString = mDateTime.ToString(DateTimeFormatString,
                                                 System.Globalization.CultureInfo.InvariantCulture);
        }
    }

    public SerializableDatetime(DateTime dt)
    {
        mDateTime = DateTime.SpecifyKind(dt.ToLocalTime(), DateTimeKind.Local);
        mDateTimeString = mDateTime.ToString(DateTimeFormatString,
                                             System.Globalization.CultureInfo.InvariantCulture);
    }

    #region ISerializationCallbackReceiver

    void UnityEngine.ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        if (!DateTime.TryParseExact((string)mDateTimeString,
                                    DateTimeFormatString,
                                    System.Globalization.CultureInfo.InvariantCulture,
                                    System.Globalization.DateTimeStyles.AssumeLocal,
                                    out mDateTime)
            && !DateTime.TryParse((string)mDateTimeString, out mDateTime))
        {
            DateTime = DateTime.MinValue;
        }
        else
        {
            DateTime = DateTime.SpecifyKind(mDateTime.ToLocalTime(), DateTimeKind.Local);
        }
    }

    void UnityEngine.ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        mDateTimeString = mDateTime.ToString(DateTimeFormatString,
                                             System.Globalization.CultureInfo.InvariantCulture);
    }

    #endregion

    public override string ToString()
    {
        return mDateTime.ToString();
    }

    public override bool Equals(object obj)
    {
        return mDateTime.Equals(obj);
    }

    public override int GetHashCode()
    {
        return mDateTime.GetHashCode();
    }

    public static implicit operator SerializableDatetime(DateTime value)
    {
        return new SerializableDatetime(value);
    }

    public static implicit operator DateTime(SerializableDatetime value)
    {
        return DateTime.SpecifyKind(value.DateTime.ToLocalTime(), DateTimeKind.Local);
    }

    public string ToString(string format)
    {
        return mDateTime.ToString(format);
    }
}