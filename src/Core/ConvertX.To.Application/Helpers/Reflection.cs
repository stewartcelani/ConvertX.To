using System.Reflection;

namespace ConvertX.To.Application.Helpers;

public static class Reflection
{
    public static List<Type> GetConcreteTypesInAssembly<T>(Assembly assembly)
    {
        return assembly.ExportedTypes
            .Where(x => typeof(T).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .ToList();
    }
}