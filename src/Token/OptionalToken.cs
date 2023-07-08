public class OptionalToken : Token
{

    public OptionalToken() : base("?", OP.Optional)
    {
        if (Entry.verbose)
            Console.WriteLine("Creating OptionalToken");
    }

}