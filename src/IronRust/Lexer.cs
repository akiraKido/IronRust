using System;

namespace IronRust
{
    internal enum Token
    {
        Reserved_fn,
        Reserved_let,
        Reserved_mut,

        Identifier,
        Integer,

        Equal,
        Semicolon,
        L_Paren,
        R_Paren,
        L_Bracket,
        R_Bracket,

        EndOfFile,
    }

    internal sealed class Lexer
    {
        internal (string value, Token token) Current { get; private set; }

        private readonly string _code;
        private int _offset;

        internal Lexer(string code) => this._code = code;

        internal bool MoveNext()
        {
            _offset += Current.value?.Length ?? 0;

            SkipWhitespace();
            if ( _offset >= _code.Length )
            {
                Current = ("", Token.EndOfFile);
                return false;
            }

            var currentChar = _code[_offset];

            switch ( currentChar )
            {
                case '(': Current = ("(", Token.L_Paren); return true;
                case ')': Current = (")", Token.R_Paren); return true;
                case '{': Current = ("{", Token.L_Bracket); return true;
                case '}': Current = ("}", Token.R_Bracket); return true;
                case ';': Current = (";", Token.Semicolon); return true;
                case '=': Current = ("=", Token.Equal); return true;
            }

            // identifiers
            if ( currentChar.IsLetter() || currentChar == '_' )
            {
                int length = 1;
                while ( _offset + length < _code.Length && _code[_offset + length].IsValidIdentifier() )
                {
                    length++;
                }
                var result = _code.Substring( _offset, length );
                Token token;
                switch ( result )
                {
                    case "fn": token = Token.Reserved_fn; break;
                    case "let": token = Token.Reserved_let; break;
                    case "mut": token = Token.Reserved_mut; break;
                    default: token = Token.Identifier; break;
                }
                Current = (result, token);
                return true;
            }

            // numbers
            if ( currentChar.IsDigit() )
            {
                // TODO: float

                int length = 1;
                while ( _offset + length < _code.Length && _code[_offset + length].IsDigit() )
                {
                    length++;
                }
                var result = _code.Substring( _offset, length );

                if ( !int.TryParse( result, out var intValue ) )
                {
                    throw new Exception("invalid integer");
                }

                Current = (result, Token.Integer);
                return true;
            }


            throw new NotImplementedException();
        }

        private void SkipWhitespace()
        {
            if (_offset >= _code.Length) return;
            while (_offset < _code.Length
                && _code[_offset].IsWhiteSpace())
            {
                _offset++;
            }
        }
    }

    internal static class CharExtensions
    {
        internal static bool IsWhiteSpace( this char c ) => char.IsWhiteSpace( c );
        internal static bool IsLetter( this char c ) => char.IsLetter( c );
        internal static bool IsLetterOrDigit( this char c ) => char.IsLetterOrDigit( c );
        internal static bool IsDigit( this char c ) => char.IsDigit( c );
        internal static bool IsValidIdentifier(this char c) => c.IsLetterOrDigit() || c == '_';
    }
}