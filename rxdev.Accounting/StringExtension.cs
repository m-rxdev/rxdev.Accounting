namespace rxdev.Accounting;

public static class StringExtension
{
    public static string Remove(this string str, string toRemove)
        => str.Replace(toRemove, string.Empty);

    public static string Remove(this string str, params string[] toRemove)
    {
        string result = str;

        foreach (string s in toRemove)
            result = result.Remove(s);

        return result;
    }
}