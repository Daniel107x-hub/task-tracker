using System.Reflection.PortableExecutable;
using System.Text;

namespace JsonParserLib;

public class JsonParser
{
    private const char OPEN_BRACE = '{';
    private const char CLOSE_BRACE = '}';
    private const char OPEN_BRACKET = '[';
    private const char CLOSE_BRACKET = ']';
    private const char COLON = ';';
    private const char SEMICOLON = ':';
    private const char COMMA = ',';
    private const char QUOTES = '"';
    private const char SPACE = ' ';
    

    public List<Token> Lex(string json)
    {
        var tokens = new List<Token>();
        IIterator<char> iterator = new StringIterator(json);
        while (iterator.HasNext())
        {
            var c = iterator.Next();
            if (c is OPEN_BRACE or CLOSE_BRACE or OPEN_BRACKET or CLOSE_BRACKET or COLON or SEMICOLON or COMMA)
            {
                var token = new Token(TokenType.Syntax, c.ToString());
                tokens.Add(token);
            }
            else if (c is QUOTES) // Token may be a string
            {
                StringBuilder sb = new();
                while (iterator.HasNext() && iterator.Next() != QUOTES)
                {
                    c = iterator.Peek();
                    sb.Append(c);
                }

                if (!iterator.HasNext() && iterator.Peek() != QUOTES) throw new InvalidDataException();
                var token = new Token(TokenType.String, sb.ToString());
                tokens.Add(token);
            }
            else if (Char.IsDigit(c))
            {
                StringBuilder sb = new();
                sb.Append(c);
                while (iterator.HasNext() && iterator.Next() != SPACE)
                {
                    c = iterator.Peek();
                    sb.Append(c);
                }
                var token = new Token(TokenType.Number, sb.ToString());
                tokens.Add(token);
            }
        }
        return tokens;
    }

    interface IIterator<T>
    {
        bool HasNext();
        T Next();

        T Peek();
    }

    class StringIterator : IIterator<char>
    {
        private int _currentIndex;
        private string _s;

        public StringIterator(string s)
        {
            this._s = s;
        }

        public bool HasNext()
        {
            return _currentIndex < _s.Length;
        }

        public char Next()
        {
            if (!HasNext()) throw new IndexOutOfRangeException();
            return _s[_currentIndex++];
        }

        public char Peek()
        {
            if (_currentIndex > _s.Length) throw new IndexOutOfRangeException();
            return _s[_currentIndex - 1];
        }
        
        
    }
}