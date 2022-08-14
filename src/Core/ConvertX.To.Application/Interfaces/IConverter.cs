﻿using ConvertX.To.Domain.Options;

namespace ConvertX.To.Application.Interfaces;

public interface IConverter
{
    Task<(string, Stream)> ConvertAsync(Stream source, ConversionOptions conversionOptions);
}