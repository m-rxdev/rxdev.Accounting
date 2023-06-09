using rxdev.Accounting.Model;

namespace rxdev.Accounting.Persistence;

public static class EntityHelper
{
    public static IEnumerable<Type> GetEntityTypes()
        => AssemblyExtension
            .GetLocalTypes("rxdev.*.dll")
            .Where(t => !t.IsInterface && t.IsAssignableTo(typeof(Entity)) && !t.IsAbstract);
}