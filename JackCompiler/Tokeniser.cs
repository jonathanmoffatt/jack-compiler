using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace JackCompiler
{
    public class Tokeniser : IDisposable, ITokeniser
    {
        private const string symbols = "{}()[].,;+-*/&|<>=~";
        private string[] keywords = new[] { "class", "constructor", "function", "method", "field", "static", "var", "int", "char", "boolean", "void", "true", "false", "null", "this", "let", "do", "if", "else", "while", "return" };
        private readonly StreamReader streamReader;
        private bool disposedValue;
        private int position = 0;
        private string buffer = "";

        public Tokeniser(string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            streamReader = new StreamReader(new MemoryStream(bytes));
        }

        public Tokeniser(Stream stream)
        {
            streamReader = new StreamReader(stream);
        }

        private bool IsWhitespace => Regex.IsMatch(Peek, @"\s");

        private bool IsLineComment => buffer.Substring(position).StartsWith("//");

        private bool IsBlockCommentStart => buffer.Substring(position).StartsWith("/*");

        private bool IsBlockCommentEnd => AtEnd || buffer.Substring(position).StartsWith("*/");

        private bool AtEnd => streamReader.EndOfStream && AtEndOfBuffer;

        private bool AtEndOfBuffer => position >= buffer.Length;

        private string Peek => AtEnd ? null : buffer.Substring(position, 1);

        private string Consume()
        {
            if (AtEnd) return null;
            string s = buffer.Substring(position, 1);
            position++;
            RefreshBuffer();
            return s;
        }

        private void Advance()
        {
            if (!AtEndOfBuffer) position++;
            RefreshBuffer();
        }

        public Token GetNextToken()
        {
            RefreshBuffer();
            SkipWhitespace();
            if (AtEnd) return null;
            string chunk = GetChunk();
            if (IsSymbol(chunk)) return new Token(NodeType.Symbol, chunk);
            if (IsInteger(chunk)) return new Token(NodeType.IntegerConstant, chunk);
            if (IsString(chunk)) return new Token(NodeType.StringConstant, chunk.Replace("\"", ""));
            if (IsKeyword(chunk)) return new Token(NodeType.Keyword, chunk);
            return new Token(NodeType.Identifier, chunk);
        }

        private void RefreshBuffer()
        {
            if (!streamReader.EndOfStream)
            {
                if (buffer == null || AtEndOfBuffer)
                {
                    buffer = streamReader.ReadLine() + "\n";
                    position = 0;
                }
            }
        }

        private string GetChunk()
        {
            if (IsSymbol(Peek)) return Consume();
            var chunk = new StringBuilder();
            bool insideStringConstant = false;
            while (!AtEnd && ((!IsWhitespace && !IsSymbol(Peek)) || insideStringConstant))
            {
                string c = Consume();
                if (c == "\"") insideStringConstant = !insideStringConstant;
                chunk.Append(c);
            }
            SkipWhitespace();
            return chunk.ToString();
        }

        private void SkipWhitespace()
        {
            while (!AtEnd && IsWhitespace) Advance();
            if (IsLineComment)
            {
                MoveToNextLine();
                SkipWhitespace();
            }
            if (IsBlockCommentStart)
            {
                while (!IsBlockCommentEnd) Advance();
                Advance();
                Advance();
                SkipWhitespace();
            }
        }

        private void MoveToNextLine()
        {
            position = buffer.Length;
            RefreshBuffer();
        }

        private bool IsSymbol(string s) => symbols.Contains(s);
        private bool IsInteger(string s) => Regex.IsMatch(s, @"^\d+$");
        private bool IsString(string s) => s.StartsWith('"') && s.EndsWith('"');
        private bool IsKeyword(string s) => keywords.Any(k => k == s);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    streamReader.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
