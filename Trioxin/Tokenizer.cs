using System;
using System.Buffers;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trioxin;
internal static class Tokenizer
{
    #region Private variables
    private readonly static SearchValues<char> _specialCharacters = SearchValues.Create("+-*/()#,=".ToCharArray());
    private readonly static SearchValues<char> _operators = SearchValues.Create("+-*/^".ToCharArray());
    #endregion

    /// <summary>
    /// Parses the given expression and generates a list of tokens.
    /// </summary>
    /// <param name="expression">The input string expression to tokenize.</param>
    /// <returns>A list of <see cref="Token"/> objects representing the parsed expression.</returns>
    internal static List<Token> CreateTokens(string expression)
    {
        var reader = new TokenReader(expression);

        List<Token> _tokens = new();
        while (!reader.EndOfStream)
        {
            reader.EatWhile(c => char.IsWhiteSpace(c));
            if (reader.Peek() is null) break;
            char current = (char)reader.Peek()!;
            if (char.IsNumber(current))
            {
                var sb = new StringBuilder();
                var dec = reader.EatWhile(c =>
                {
                    if (char.IsWhiteSpace(c)) return false;
                    if (_specialCharacters.Contains(c)) return false;
                    sb.Append(c);
                    if (char.IsNumber(c)) return true;
                    if (decimal.TryParse(sb.ToString(), CultureInfo.CurrentCulture, out decimal dec)) return true;
                    return false;
                });

                var next = reader.Peek();
                if (next is not null && char.IsLetterOrDigit(next.Value) && !_specialCharacters.Contains(next.Value))
                {
                    dec += reader.EatWhile(c => char.IsLetterOrDigit(c) || c == '_'); //Handle variable names that begin with a number
                    _tokens.Add(new Token(TokenType.Variable, dec));
                }
                else
                {
                    _tokens.Add(new Token(TokenType.Number, decimal.Parse(dec)));
                }
            }
            else if (current is '"' or '\'')
            {
                var queue = new Queue<char>();
                var value = reader.EatWhile(c =>
                {
                    if (queue.Count == 2) return false;
                    if (c == current) queue.Enqueue(c);
                    return true;
                });
                value = value.TrimStart(current);
                value = value.TrimEnd(current);
                _tokens.Add(new Token(TokenType.String, value));
            }
            else if (current == '#')
            {
                var queue = new Queue<char>();
                var date = reader.EatWhile(c =>
                {
                    if (queue.Count == 2) return false;
                    if (c == '#') queue.Enqueue(c);
                    return true;
                });
                _tokens.Add(new Token(TokenType.Date, DateTime.Parse(date, CultureInfo.CurrentCulture)));
            }
            else if (current == '<')
            {
                reader.Read();
                if (reader.Peek() == '=')
                {
                    reader.Read();
                    _tokens.Add(new Token(TokenType.LessThanOrEqual));
                }
                else
                {
                    _tokens.Add(new Token(TokenType.LessThan));
                }
            }
            else if (current == '!')
            {
                reader.Read();
                if (reader.Peek() == '=')
                {
                    reader.Read();
                    _tokens.Add(new Token(TokenType.NotEquals));
                }
                else throw new Exception($"Unknown expression for !, and next: {reader.Peek()}");
            }
            else if (current == '>')
            {
                reader.Read();
                if (reader.Peek() == '=')
                {
                    reader.Read();
                    _tokens.Add(new Token(TokenType.GreaterThanOrEqual));
                }
                else
                {
                    _tokens.Add(new Token(TokenType.GreaterThan));
                }
            }
            else if(current == '~')
            {
                reader.Read();
                if(reader.Peek() == '=')
                {
                    reader.Read();
                    _tokens.Add(new Token(TokenType.EqualsInsensitive));
                }
            }
            else if (char.IsLetter(current))
            {
                var operand = reader.EatWhile(c => char.IsLetterOrDigit(c) || c == '_');
                if (bool.TryParse(operand, out bool boolean))
                {
                    _tokens.Add(new Token(TokenType.Boolean, boolean));
                }
                else if (reader.Peek() == '(')
                {
                    _tokens.Add(operand switch
                    {
                        var x when x.Equals("IF", StringComparison.OrdinalIgnoreCase) => new Token(TokenType.If),
                        var x when x.Equals("OR", StringComparison.OrdinalIgnoreCase) => new Token(TokenType.Or),
                        var x when x.Equals("IN", StringComparison.OrdinalIgnoreCase) => new Token(TokenType.In),
                        var x when x.Equals("AND", StringComparison.OrdinalIgnoreCase) => new Token(TokenType.And),
                        var x when x.Equals("NOT", StringComparison.OrdinalIgnoreCase) => new Token(TokenType.Not),
                        var x when x.Equals("MIN", StringComparison.OrdinalIgnoreCase) => new Token(TokenType.Min),
                        var x when x.Equals("MAX", StringComparison.OrdinalIgnoreCase) => new Token(TokenType.Max),
                        var x when x.Equals("SUM", StringComparison.OrdinalIgnoreCase) => new Token(TokenType.Sum),
                        var x when x.Equals("AVG", StringComparison.OrdinalIgnoreCase) => new Token(TokenType.Avg),
                        var x when x.Equals("ABS", StringComparison.OrdinalIgnoreCase) => new Token(TokenType.Abs),
                        var x when x.Equals("LEN", StringComparison.OrdinalIgnoreCase) => new Token(TokenType.Len),
                        var x when x.Equals("LEFT", StringComparison.OrdinalIgnoreCase) => new Token(TokenType.Left),
                        var x when x.Equals("CINT", StringComparison.OrdinalIgnoreCase) => new Token(TokenType.CInt),
                        var x when x.Equals("CBOOL", StringComparison.OrdinalIgnoreCase) => new Token(TokenType.CBool),
                        var x when x.Equals("CBYTE", StringComparison.OrdinalIgnoreCase) => new Token(TokenType.CByte),
                        var x when x.Equals("CLONG", StringComparison.OrdinalIgnoreCase) => new Token(TokenType.CLong),
                        var x when x.Equals("RIGHT", StringComparison.OrdinalIgnoreCase) => new Token(TokenType.Right),
                        var x when x.Equals("ROUND", StringComparison.OrdinalIgnoreCase) => new Token(TokenType.Round),
                        var x when x.Equals("CSHORT", StringComparison.OrdinalIgnoreCase) => new Token(TokenType.CShort),
                        var x when x.Equals("WITHIN", StringComparison.OrdinalIgnoreCase) => new Token(TokenType.Within),
                        var x when x.Equals("BETWEEN", StringComparison.OrdinalIgnoreCase) => new Token(TokenType.Between),
                        _ => new Token(TokenType.Function, operand)
                    });
                }
                else
                {
                    _tokens.Add(new Token(TokenType.Variable, operand));
                }
            }
            else if (current == '(') _tokens.Add(new Token(TokenType.LeftParenthesis, reader.Read()!));
            else if (current == ')') _tokens.Add(new Token(TokenType.RightParenthesis, reader.Read()!));
            else if (current == ',') _tokens.Add(new Token(TokenType.Comma, reader.Read()!));
            else if (current == '=') _tokens.Add(new Token(TokenType.Equals, reader.Read()!));
            else if (_operators.Contains(current))
            {
                _tokens.Add(new Token(TokenType.Operator, reader.Read()!));
            }
            else
            {
                throw new Exception($"Unable to parse character: {current}");
            }
        }

        return _tokens;
    }
}



