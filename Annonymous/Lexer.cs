using System;

namespace Annonymous
{
    internal interface ILexer
    {
        bool Check( TokenType tokenType );
        Token Match(TokenType tokenType);
    }

    internal class Lexer : ILexer
    {
        private readonly string _code;
        private int _offset;

        private Token _lookAhead;

        private bool _hasNext => _offset < _code.Length;
        private char _current => _hasNext ? _code[_offset] : '\0';

        private int _lineNumber;
        private int _columnNumber;

        internal Lexer(string code)
        {
            _code = code;
            AdvanceToken();
        }

        public bool Check( TokenType tokenType ) => _lookAhead.TokenType == tokenType;

        public Token Match( TokenType tokenType )
        {
            var isMatch = Check( tokenType );
            if(!isMatch) CreateException( $"expected {tokenType}; found {tokenType}" );
            var result = _lookAhead;
            AdvanceToken();
            return result;
        }

        internal void AdvanceToken()
        {
            SkipWhiteSpace();

            if (_hasNext == false)
            {
                // end of file
                _lookAhead = new Token(TokenType.EndOfFile, string.Empty);
                return;
            }

            switch ( _current )
            {
                case '+':
                case '-':
                    _lookAhead = new Token(
                        TokenType.AddOrSubtract,
                        _current.ToString()
                    );
                    _offset++;
                    return;
            }

            if (_current == '_' || _current.IsLetter())
            {
                // identifier
                var startPos = _offset;
                while (_current.IsLetterOrDigit() || _current == '_') AdvanceOffset();
                _lookAhead = new Token(
                    TokenType.Identifier,
                    _code.Substring(startPos, _offset - startPos)
                );
                return;
            }

            if (_current.IsDigit())
            {
                // number
                var startPos = _offset;
                var hasDecimal = false;
                while (_current.IsDigit() || _current == '.')
                {
                    AdvanceOffset();
                    if (_current == '.')
                    {
                        if (hasDecimal) CreateException("found second decimal");
                        hasDecimal = true;
                    }
                }
                _lookAhead = new Token(
                    TokenType.Number,
                    _code.Substring(startPos, _offset - startPos)
                );
                return;
            }

            CreateException($"unidentified token: {_current}");
        }

        private void CreateException(string message)
            => throw new LexerException($"[{_lineNumber}:{_columnNumber}] {message}.");

        private void SkipWhiteSpace()
        {
            while (_hasNext && _current.IsWhiteSpace())
            {
                AdvanceOffset();
            }
        }

        private void AdvanceOffset()
        {
            _offset++;
            _columnNumber++;

            if (_current == '\n')
            {
                _lineNumber++;
                _columnNumber = 0;
            }
        }
    }


    internal sealed class LexerException : Exception
    {
        internal LexerException(string message) : base(message) { }
    }

    internal enum TokenType
    {
        EndOfFile,

        Identifier,
        Number,

        AddOrSubtract,
    }

    internal struct Token
    {
        internal TokenType TokenType { get; }
        internal string Value { get; }

        internal Token(TokenType tokenType, string value)
            => (TokenType, Value) = (tokenType, value);
    }
}