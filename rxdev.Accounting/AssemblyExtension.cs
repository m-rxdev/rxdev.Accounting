using System.Reflection;

namespace rxdev.Accounting;

public static class AssemblyExtension
{
    public static IEnumerable<Type> GetLocalTypes(string searchPath = "rxdev.*.dll", SearchOption searchOption = SearchOption.AllDirectories)
        => Assembly.GetExecutingAssembly().GetLocalTypes(searchPath, searchOption);

    public static IEnumerable<Type> GetLocalTypes(this Assembly assembly, string searchPath = "rxdev.*.dll", SearchOption searchOption = SearchOption.AllDirectories)
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        return Directory.GetFiles(Path.GetDirectoryName(assembly.Location)!, searchPath, searchOption)
            .Select(f => assemblies.FirstOrDefault(a => a.Location == f) ?? Assembly.LoadFrom(f))
            .SelectMany(a => a.GetTypes());
    }
}