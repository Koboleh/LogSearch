using LogSearch;

public class WildcardToRegexConverter
{
    public static void Main()
    {
        var searcher = new LogSearcher();

        var res = searcher.GetAllMatches("*. and Warning*");

        foreach (var r in res)
        {
            Console.WriteLine(r);
        }
    }
}