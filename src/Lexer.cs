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
            CreateToken();

    }

    public List<Token> GetTokens() { return tokens; }

    private void CreateToken()
    {
        if (eol)
            return;

        char c = GetNextCharacter();

        if (c.Equals('\\'))
        {
            isEscaped = true;
            return;
        }

        if (isEscaped)
        {
            isEscaped = false;
            tokens.Add(new TerminalToken("\\" + c.ToString()));
            return;
        }

        Token.OP op = GetTokenOp(c);

        switch (op)
        {
            case Token.OP.Terminal:
                tokens.Add(new TerminalToken(c.ToString())); break;
            case Token.OP.Star:
                tokens.Add(new StarToken()); break;
            case Token.OP.Plus:
                tokens.Add(new PlusToken()); break;
            case Token.OP.Optional:
                tokens.Add(new OptionalToken()); break;
            case Token.OP.Alternate:
                tokens.Add(new AlternateToken()); break;
            case Token.OP.Repetition:
                CreateRepetitionToken(c); break;
            case Token.OP.Group:
                CreateGroupToken(c); break;
            case Token.OP.Class:
                CreateClassToken(c); break;
            case Token.OP.EOL:
                tokens.Add(new EOLToken()); eol = true; break;
            case Token.OP.Lookahead:
                CreateLookaheadToken(c); break;
            case Token.OP.NLookahead:
                CreateNegativeLookaheadToken(c); break;
            case Token.OP.Lookbehind:
                CreateLookbehindToken(c); break;
        }

    }

    private void CreateClassToken(char c)
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

        tokens.Add(new ClassToken(classString));

    }


    private void CreateGroupToken(char c)
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

            groupString += group_char;

            op = GetTokenOp(group_char);

            if (op == Token.OP.Group && group_char.Equals('('))
                innerGroups++;

            if (op == Token.OP.Group && group_char.Equals(')'))
                innerGroups--;

        } while ((op != Token.OP.Group && op != Token.OP.EOL) || innerGroups > 0);

        tokens.Add(new GroupToken(groupString));

    }

    private void CreateRepetitionToken(char c)
    {

        if (c.Equals(']'))
        {
            tokens.Add(new TerminalToken(c.ToString()));
            return;
        }

        string repetitionString = "{";

        char repetition_char = '\0';

        do
        {
            repetition_char = GetNextCharacter();
            repetitionString += repetition_char;
        } while ((GetTokenOp(repetition_char) != Token.OP.Repetition) && (GetTokenOp(repetition_char) != Token.OP.EOL));

        tokens.Add(new RepetitionToken(repetitionString));
    }

    private void CreateLookaheadToken(char c)
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

            lookaheadString += lookahead_char;

            op = GetTokenOp(lookahead_char);

            if (op == Token.OP.Group && lookahead_char.Equals('('))
                innerGroups++;

            if (op == Token.OP.Group && lookahead_char.Equals(')'))
                innerGroups--;

        } while ((op != Token.OP.Group && op != Token.OP.EOL) || innerGroups > 0);

        tokens.Add(new LookaheadToken(lookaheadString));

    }

    private void CreateNegativeLookaheadToken(char c)
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

        tokens.Add(new NLookaheadToken(lookaheadString));

    }

    private void CreateLookbehindToken(char c)
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
        tokens.Add(new GroupToken(lookbehindString));

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