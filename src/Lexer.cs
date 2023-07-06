using System;
using System.Collections;
using System.Collections.Generic;
public class Lexer
{
    public List<Token> tokens = new List<Token>();

    private string regex;
    private int index;
    private int lastCharacter;

    private Boolean isEscaped = false;

    private Boolean eol = false;

    public Lexer(string regex)
    {
        this.regex = regex;
        index = 0;
        lastCharacter = regex.Length;
    }

    public void Tokenize()
    {

        while (index < lastCharacter)
        {
            Token token = CreateToken();
            if (token.tokenOP == Token.OP.EOL && eol)
                break;
            tokens.Add(token);
        }

    }

    public List<Token> GetTokens() { return tokens; }

    private Token CreateToken()
    {
        if (eol)
            return new EOLToken();

        char c = GetNextCharacter();

        if (c.Equals('\\'))
        {
            isEscaped = true;
            return new EOLToken();
        }

        if (isEscaped)
        {
            isEscaped = false;
            return new TerminalToken("\\" + c.ToString());
        }

        Token.OP op = GetTokenOp(c);

        switch (op)
        {
            case Token.OP.Terminal:
                return new TerminalToken(c.ToString());
            case Token.OP.Star:
                return new StarToken();
            case Token.OP.Plus:
                return new PlusToken();
            case Token.OP.Optional:
                return new OptionalToken();
            case Token.OP.Alternate:
                return new AlternateToken();
            case Token.OP.Repetition:
                return CreateRepetitionToken(c);
            case Token.OP.Group:
                return CreateGroupToken();
            case Token.OP.Class:
                return CreateClassToken();
            case Token.OP.EOL:
                eol = true;
                return new EOLToken();
            case Token.OP.Lookahead:
                return CreateLookaheadToken();
            case Token.OP.NLookahead:
                return CreateNegativeLookaheadToken();
            case Token.OP.Lookbehind:
                return CreateLookbehindToken();
            default:
                eol = true;
                return new EOLToken();
        }

    }

    private ClassToken CreateClassToken()
    {

        string classString = "[";

        char class_char = '\0';

        Token.OP op = Token.OP.Terminal;
        do
        {
            class_char = GetNextCharacter();

            if (class_char.Equals('\\'))
            {
                isEscaped = true;
                continue;
            }

            if (isEscaped)
            {
                isEscaped = false;
                classString += "\\" + class_char;
                continue;
            }

            classString += class_char;

            op = GetTokenOp(class_char);

        } while (op != Token.OP.Class && op != Token.OP.EOL);

        //tokens.Add(new ClassToken(classString));
        return new ClassToken(classString);
    }


    private GroupToken CreateGroupToken()
    {

        string groupString = "(";

        char group_char = '\0';

        int innerGroups = 1;

        Token.OP op = Token.OP.Terminal;
        do
        {
            group_char = GetNextCharacter();

            if (group_char.Equals('\\'))
            {
                isEscaped = true;
                continue;
            }

            if (isEscaped)
            {
                isEscaped = false;
                groupString += "\\" + group_char;
                continue;
            }

            op = GetTokenOp(group_char);

            if (op == Token.OP.Lookahead)
            {

                LookaheadToken lookahead = CreateLookaheadToken();
                groupString += lookahead.symbol;

                continue;
            }
            else
            {
                groupString += group_char;
            }

            if (op == Token.OP.Group && group_char.Equals('('))
                innerGroups++;

            if (op == Token.OP.Group && group_char.Equals(')'))
                innerGroups--;

            if (innerGroups > 0 && op == Token.OP.EOL)
                throw new Exception("Unbalanced Parenthesizes, can't compute Regex.");

        } while ((op != Token.OP.Group && op != Token.OP.EOL) || innerGroups > 0);

        return new GroupToken(groupString);
    }

    private Token CreateRepetitionToken(char c)
    {

        if (c.Equals(']'))
        {
            //tokens.Add(new TerminalToken(c.ToString()));
            return new TerminalToken(c.ToString());
        }

        string repetitionString = "{";

        char repetition_char = '\0';

        do
        {
            repetition_char = GetNextCharacter();
            repetitionString += repetition_char;
        } while ((GetTokenOp(repetition_char) != Token.OP.Repetition) && (GetTokenOp(repetition_char) != Token.OP.EOL));

        //tokens.Add(new RepetitionToken(repetitionString));
        return new RepetitionToken(repetitionString);
    }

    private LookaheadToken CreateLookaheadToken()
    {

        string lookaheadString = "(?=";

        char lookahead_char = '\0';

        int innerGroups = 1;

        index += 2;

        Token.OP op = Token.OP.Terminal;
        do
        {
            lookahead_char = GetNextCharacter();

            if (lookahead_char.Equals('\\'))
            {
                isEscaped = true;
                continue;
            }

            if (isEscaped)
            {
                isEscaped = false;
                lookaheadString += "\\" + lookahead_char;
                continue;
            }

            op = GetTokenOp(lookahead_char);

            if (op == Token.OP.Lookahead)
            {

                LookaheadToken lookahead = CreateLookaheadToken();
                lookaheadString += lookahead.symbol;

                continue;
            }
            else
            {
                lookaheadString += lookahead_char;

            }

            if (op == Token.OP.Group && lookahead_char.Equals('('))
                innerGroups++;

            if (op == Token.OP.Group && lookahead_char.Equals(')'))
                innerGroups--;

        } while ((op != Token.OP.Group && op != Token.OP.EOL) || innerGroups > 0);

        return new LookaheadToken(lookaheadString);

    }

    private NLookaheadToken CreateNegativeLookaheadToken()
    {

        string lookaheadString = "(?!";

        char lookahead_char = '\0';

        int innerGroups = 1;

        index += 2;

        Token.OP op = Token.OP.Terminal;
        do
        {
            lookahead_char = GetNextCharacter();

            if (lookahead_char.Equals('\\'))
            {
                isEscaped = true;
                continue;
            }

            if (isEscaped)
            {
                isEscaped = false;
                lookaheadString += "\\" + lookahead_char;
                continue;
            }

            lookaheadString += lookahead_char;

            op = GetTokenOp(lookahead_char);

            if (op == Token.OP.Group && lookahead_char.Equals('('))
                innerGroups++;

            if (op == Token.OP.Group && lookahead_char.Equals(')'))
                innerGroups--;

        } while ((op != Token.OP.Group && op != Token.OP.EOL) || innerGroups > 0);

        return new NLookaheadToken(lookaheadString);
    }

    private GroupToken CreateLookbehindToken()
    {

        string lookbehindString = "(?<=";

        char lookbehind_char = '\0';

        int innerGroups = 1;

        index += 3;

        Token.OP op = Token.OP.Terminal;
        do
        {
            lookbehind_char = GetNextCharacter();

            if (lookbehind_char.Equals('\\'))
            {
                isEscaped = true;
                continue;
            }

            if (isEscaped)
            {
                isEscaped = false;
                lookbehindString += "\\" + lookbehind_char;
                continue;
            }

            lookbehindString += lookbehind_char;

            op = GetTokenOp(lookbehind_char);

            if (op == Token.OP.Group && lookbehind_char.Equals('('))
                innerGroups++;

            if (op == Token.OP.Group && lookbehind_char.Equals(')'))
                innerGroups--;

        } while ((op != Token.OP.Group && op != Token.OP.EOL) || innerGroups > 0);

        //Lookbehind token, placeholder for now
        return new GroupToken(lookbehindString);
    }

    private Token.OP GetTokenOp(char c)
    {

        switch (c)
        {
            case '[': return Token.OP.Class;
            case ']': return Token.OP.Class;
            case '(': return GetMultiCharToken(c);
            case ')': return Token.OP.Group;
            case '{': return Token.OP.Repetition;
            case '}': return Token.OP.Repetition;
            case '|': return Token.OP.Alternate;
            case '?': return Token.OP.Optional;
            case '+': return Token.OP.Plus;
            case '*': return Token.OP.Star;
            case '\0': return Token.OP.EOL;
            default: return Token.OP.Terminal;
        }
    }

    private Token.OP GetMultiCharToken(char c)
    {
        if (c.Equals(')') || (index + 2 > regex.Length))
            return Token.OP.Group;

        if (regex[index].Equals('?') && regex[index + 1].Equals('='))
            return Token.OP.Lookahead;

        if (regex[index].Equals('?') && regex[index + 1].Equals('!'))
            return Token.OP.NLookahead;

        if (regex[index].Equals('?') && regex[index + 1].Equals('<') && regex[index + 2].Equals('='))
            return Token.OP.Lookbehind;

        return Token.OP.Group;
    }

    private char GetNextCharacter()
    {
        if (index < lastCharacter)
        {
            index++;
            return regex[index - 1];
        }
        return '\0';
    }

}