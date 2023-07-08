public class PlusToken : Token
{

    public PlusToken() : base("+", OP.Plus)
    {
        if (Entry.verbose)
            Console.WriteLine("Creating PlusToken");
    }

}