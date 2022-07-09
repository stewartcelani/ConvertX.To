using System.Reflection;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ConvertX.To.API.Filters;

public static class RegisterFiltersFromAssemblyExtension
{
    public static void RegisterFiltersFromAssembly(this FilterCollection filterCollection, Assembly assembly)
    {
        var filters = assembly.ExportedTypes
            .Where(x => typeof(IFilterMetadata).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .ToList();
        filters.ForEach(filter => filterCollection.Add(filter));
    }
}