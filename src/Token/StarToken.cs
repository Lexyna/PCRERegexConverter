public class StarToken : Token
{

    public StarToken() : base("*", OP.Star)
    {
        if (Entry.verbose)
            Console.WriteLine("Creating StarToken");
    }

}