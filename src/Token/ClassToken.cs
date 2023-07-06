using System;

public class ClassToken : Token
{

    private static string numberRange = "0123456789";
    private static string alphabeticalRange = "abcdefghijklmnopqrstuvwxyz";

    public ClassToken(string symbol) : base(symbol, OP.Class) { }

    public GroupToken ConvertToGroup()
    {

        string substring = symbol.Substring(1, symbol.Length - 2);

        string groupString = "(";

        int index = 0;

        do
        {
            Boolean isRange = IsRange(index, substring);

            if (isRange)
            {
                string range = BuildRangeString(index, substring);
                groupString += range;

                index += 3;

                if (index + 1 < substring.Length)
                    groupString += "|";
                continue;
            }

            groupString += substring[index];

            if (index + 1 < substring.Length)
                groupString += "|";

            index++;
        } while (index < substring.Length);

        groupString += ")";

        return new GroupToken(groupString);
    }

    private string BuildRangeString(int index, string range)
    {
        if (index + 2 >= range.Length)
            return "";

        char start = range[index];
        char end = range[index + 2];

        if (alphabeticalRange.Contains(start.ToString()) && alphabeticalRange.Contains(end.ToString()))
            return BuildLowerCaseAlphabeticalRange(start, end);

        if (alphabeticalRange.ToUpper().Contains(start.ToString()) && alphabeticalRange.ToUpper().Contains(end.ToString()))
            return BuildUpperCaseAlphabeticalRange(start, end);

        if (numberRange.Contains(start.ToString()) && numberRange.Contains(end.ToString()))
            return BuildNumberRange(start, end);

        return "";
    }

    private string BuildNumberRange(char start, char end)
    {
        int iter = numberRange.IndexOf(start);
        string rangeString = "";

        while (!numberRange[iter].Equals(end))
        {
            rangeString += numberRange[iter] + "|";
            iter++;
        }

        rangeString += end;

        return rangeString;
    }

    private string BuildUpperCaseAlphabeticalRange(char start, char end)
    {
        int iter = alphabeticalRange.ToUpper().IndexOf(start);
        string rangeString = "";

        while (!alphabeticalRange.ToUpper()[iter].Equals(end))
        {
            rangeString += alphabeticalRange.ToUpper()[iter] + "|";
            iter++;
        }

        rangeString += end;

        return rangeString;
    }

    private string BuildLowerCaseAlphabeticalRange(char start, char end)
    {
        int iter = alphabeticalRange.IndexOf(start);
        string rangeString = "";

        while (!alphabeticalRange[iter].Equals(end))
        {
            rangeString += alphabeticalRange[iter] + "|";
            iter++;
        }

        rangeString += end;

        return rangeString;
    }

    private Boolean IsRange(int index, string text)
    {
        if (index + 2 >= text.Length)
            return false;

        if (
            alphabeticalRange.Contains(text[index].ToString()) &&
            text[index + 1].Equals('-') &&
            alphabeticalRange.Contains(text[index + 2].ToString())
        )
            return true;

        if (
            alphabeticalRange.ToUpper().Contains(text[index].ToString()) &&
            text[index + 1].Equals('-') &&
            alphabeticalRange.ToUpper().Contains(text[index + 2].ToString())
        )
            return true;

        if (
            numberRange.Contains(text[index].ToString()) &&
            text[index + 1].Equals('-') &&
            numberRange.Contains(text[index + 2].ToString())
        )
            return true;

        return false;
    }

}