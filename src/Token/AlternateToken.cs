public class AlternateToken : Token
{

    public AlternateToken() : base("|", OP.Alternate)
    {
        if (Entry.verbose)
            Console.WriteLine("Creating AlternateToken");
    }

}