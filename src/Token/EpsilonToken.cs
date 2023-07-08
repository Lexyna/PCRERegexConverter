public class EpsilonToken : Token
{

    public EpsilonToken() : base("", OP.Epsilon)
    {
        if (Entry.verbose)
            Console.WriteLine("Creating EpsilonToken");
    }

}