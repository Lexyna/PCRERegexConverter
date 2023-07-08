public class EOLToken : Token
{

    public EOLToken() : base("", OP.EOL)
    {
        if (Entry.verbose)
            Console.WriteLine("Creating EOLToken");
    }

}