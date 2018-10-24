using System;
using UnityEngine;

[Serializable]
public struct SerializableDatetime : ISerializationCallbackReceiver
{
    [SerializeField]
    private string mDateTimeString;
    private DateTime mDateTime;

    public DateTime DateTime
    {
        get { return mDateTime; }
        set
        {
            mDateTime = value;
            mDateTimeString = mDateTime.ToString();
        }
    }

    public SerializableDatetime(DateTime dt)
    {
        mDateTime = dt;
        mDateTimeString = mDateTime.ToString();
    }

    #region ISerializationCallbackReceiver

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        if (!DateTime.TryParse(mDateTimeString, out mDateTime))
        {
            DateTime = DateTime.MinValue;
        }
        else
        {
            DateTime = mDateTime.ToUniversalTime();
        }
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        mDateTimeString = mDateTime.ToString();
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
        return value.DateTime;
    }
}