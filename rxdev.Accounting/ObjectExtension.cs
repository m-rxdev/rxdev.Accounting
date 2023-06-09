namespace rxdev.Accounting;

public static class ObjectExtension
{
    public static T? GetArg<T>(this object[] args, int index)
    {
        if (index >= args.Length
            || args[index] is not T)
            return default;

        return (T)args[index];
    }
}