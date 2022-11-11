using System;
using System.Runtime.Serialization;

namespace HotChocolate.Extensions.Tracking.Exceptions;

[Serializable]
public sealed class FieldNotTrackableException : Exception
{
    public FieldNotTrackableException(string fieldName)
        : base($"This field can not be tracked: {fieldName}.")
    {
        FieldName = fieldName;
    }

    public string FieldName { get; }

    private FieldNotTrackableException(
        SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        FieldName = info.GetString(nameof(FieldName)) ?? string.Empty;
    }

    public override void GetObjectData(
        SerializationInfo info, StreamingContext context)
    {
        info.AddValue(nameof(FieldName), FieldName);
        base.GetObjectData(info, context);
    }
}
