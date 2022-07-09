﻿using System.Diagnostics.CodeAnalysis;

namespace ConvertX.To.API.Exceptions;

[Serializable]
[ExcludeFromCodeCoverage]
public class UnsupportedConversionException : ConvertXToPublicException
{
    public override string Reason => "Unsupported Conversion Type";

    public UnsupportedConversionException() { }
    public UnsupportedConversionException(string message) : base(message) { }
    public UnsupportedConversionException(string message, Exception inner) : base(message, inner) { }
    public UnsupportedConversionException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}