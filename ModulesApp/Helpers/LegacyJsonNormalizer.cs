using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ModulesApp.Helpers;

public static class LegacyJsonNormalizer
{
    public static string Normalize(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return "{}";
        }

        var parser = new Parser(input);
        return parser.ParseValueText();
    }

    private sealed class Parser
    {
        private readonly string _input;
        private int _pos;

        public Parser(string input) => _input = input;

        public string ParseValueText()
        {
            SkipWhitespace();
            if (_pos >= _input.Length)
            {
                return "null";
            }

            return Peek() switch
            {
                '{' => ParseObjectText(),
                '[' => ParseArrayText(),
                '"' => ReadQuotedString('"'),
                '\'' => ReadQuotedString('\''),
                _ => ParsePrimitiveText()
            };
        }

        private string ParseObjectText()
        {
            Expect('{');
            var sb = new StringBuilder();
            sb.Append('{');

            SkipWhitespace();
            if (TryConsume('}'))
            {
                sb.Append('}');
                return sb.ToString();
            }

            while (true)
            {
                var key = ReadKeyToken();
                sb.Append(JsonSerializer.Serialize(Regex.Unescape(key)));

                SkipWhitespace();
                Expect(':');
                sb.Append(':');

                sb.Append(ParseValueText());

                SkipWhitespace();
                if (TryConsume(','))
                {
                    sb.Append(',');
                    continue;
                }

                Expect('}');
                sb.Append('}');
                break;
            }

            return sb.ToString();
        }

        private string ParseArrayText()
        {
            Expect('[');
            var sb = new StringBuilder();
            sb.Append('[');

            SkipWhitespace();
            if (TryConsume(']'))
            {
                sb.Append(']');
                return sb.ToString();
            }

            while (true)
            {
                sb.Append(ParseValueText());

                SkipWhitespace();
                if (TryConsume(','))
                {
                    sb.Append(',');
                    continue;
                }

                Expect(']');
                sb.Append(']');
                break;
            }

            return sb.ToString();
        }

        private string ParsePrimitiveText()
        {
            var token = ReadUnquotedValueToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                return "null";
            }

            var trimmed = token.Trim();
            var lowered = trimmed.ToLowerInvariant();

            if (lowered is "true" or "false" or "null")
            {
                return lowered;
            }

            if (decimal.TryParse(trimmed, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
            {
                return trimmed;
            }

            var unescaped = Regex.Unescape(trimmed);
            return JsonSerializer.Serialize(unescaped);
        }

        private string ReadKeyToken()
        {
            SkipWhitespace();
            if (Peek() is '"' or '\'')
            {
                return ReadStringToken(Peek());
            }

            return ReadUntil(':');
        }

        private string ReadQuotedString(char quote)
        {
            var value = ReadStringToken(quote);
            return JsonSerializer.Serialize(value);
        }

        private string ReadStringToken(char quote)
        {
            Expect(quote);
            var sb = new StringBuilder();

            while (_pos < _input.Length)
            {
                var c = _input[_pos++];

                if (c == '\\' && _pos < _input.Length)
                {
                    sb.Append('\\').Append(_input[_pos++]);
                    continue;
                }

                if (c == quote)
                {
                    break;
                }

                sb.Append(c);
            }

            return Regex.Unescape(sb.ToString());
        }

        private string ReadUnquotedValueToken()
        {
            var start = _pos;
            while (_pos < _input.Length)
            {
                var c = _input[_pos];
                if (c == ',' || c == '}' || c == ']')
                {
                    break;
                }
                _pos++;
            }

            return _input[start.._pos];
        }

        private string ReadUntil(char endChar)
        {
            var start = _pos;
            while (_pos < _input.Length && _input[_pos] != endChar)
            {
                _pos++;
            }

            return _input[start.._pos].Trim();
        }

        private void SkipWhitespace()
        {
            while (_pos < _input.Length && char.IsWhiteSpace(_input[_pos]))
            {
                _pos++;
            }
        }

        private char Peek() => _input[_pos];

        private void Expect(char c)
        {
            if (_pos >= _input.Length || _input[_pos] != c)
            {
                throw new FormatException($"Expected '{c}' at position {_pos}.");
            }

            _pos++;
        }

        private bool TryConsume(char c)
        {
            if (_pos < _input.Length && _input[_pos] == c)
            {
                _pos++;
                return true;
            }

            return false;
        }
    }
}