using System.Diagnostics.CodeAnalysis;

namespace rxdev.Accounting;

public static class TypeExtension
{
    public static bool IsGenericAssignable(this Type type, Type targetType, [NotNullWhen(true)] out Type[]? args)
    {
        args = null;

        if (type.BaseType is null)
            return false;

        if ( type.BaseType.IsGenericType
            && type.BaseType.GetGenericTypeDefinition() == targetType)
        {
            args = type.BaseType.GetGenericArguments();
            return true;
        }

        return IsGenericAssignable(type.BaseType, targetType, out args);
    }
}