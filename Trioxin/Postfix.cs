using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Trioxin;

/// <summary>
/// Provides functions for Postfix or Reverse Polish Notation (RPN) translation
/// </summary>
internal class Postfix
{
    #region Private variables and functions
    private static readonly Dictionary<string, int> OperatorPrecedence = new()
    {
        { "^", 3 },
        { "*", 2 }, { "/", 2 },
        { "+", 1 }, { "-", 1 },
        { "<", 0 }, { "<=", 0 }, { ">", 0 }, { ">=", 0 }, { "!=", 0 }, { "=", 0 }, { "~=", 0 }
    };

    [DebuggerDisplay("{Type} with {Arguments} arg(s)")]
    private struct Function
    {
        public TokenType Type;
        public int Arguments;
    }

    private static bool IsFunction(TokenType type, out bool multiVariable)
    {
        multiVariable = IsMultiVariableFunction(type);
        if (multiVariable || IsKnownVariableFunction(type)) return true;
        return false;
    }

    private static bool IsMultiVariableFunction(TokenType type)
    {
        return type switch
        {
            TokenType.In => true,
            TokenType.And => true,
            TokenType.Or => true,
            TokenType.Max => true,
            TokenType.Min => true,
            TokenType.Avg => true,
            TokenType.Sum => true,
            _ => false
        };
    }

    private static bool IsKnownVariableFunction(TokenType type)
    {
        return type switch
        {
            TokenType.If => true,
            TokenType.Not => true,
            TokenType.Abs => true,
            TokenType.Len => true,
            TokenType.Left => true,
            TokenType.CInt => true,
            TokenType.Right => true,
            TokenType.CBool => true,
            TokenType.CByte => true,
            TokenType.CLong => true,
            TokenType.CShort => true,
            TokenType.Round => true,
            TokenType.Within => true,
            TokenType.Between => true,
            TokenType.Function => true,
            _ => false
        };
    }

    private static bool IsOperator(TokenType type)
    {
        return type switch
        {
            TokenType.Operator => true,
            TokenType.Equals => true,
            TokenType.LessThan => true,
            TokenType.LessThanOrEqual => true,
            TokenType.GreaterThan => true,
            TokenType.GreaterThanOrEqual => true,
            _ => false
        };
    }
    #endregion

    /// <summary>
    /// Converts a list of tokens into a Postfix or Reverse Polish Notation (RPN) queue for further processing.
    /// </summary>
    /// <param name="tokens">The list of <see cref="Token"/> objects to convert.</param>
    /// <returns>A queue of <see cref="Token"/> objects in the processed order.</returns>
    internal static Queue<Token> ConvertFromTokens(List<Token> tokens)
    {
        var output = new Queue<Token>(tokens.Count+1);
        var operators = new Stack<Token>();
        var functions = new Stack<Function>();
        foreach (var token in tokens)
        {
            switch (token.type)
            {
                case TokenType.Number:
                case TokenType.Boolean:
                case TokenType.String:
                case TokenType.Variable:
                case TokenType.Date:
                    output.Enqueue(token);
                    break;
                case var x when IsFunction(x, out _):
                    functions.Push(new Function() { Type = token.type });
                    operators.Push(token);
                    break;
                case TokenType.LeftParenthesis:
                    operators.Push(token);
                    break;
                case TokenType.RightParenthesis:
                    // Pop everything until '('
                    while (operators.Count > 0 && operators.Peek().type != TokenType.LeftParenthesis)
                    {
                        output.Enqueue(operators.Pop());
                    }

                    if (operators.Count == 0) throw new Exception("Mismatched parentheses in expression.");
                    operators.Pop(); // Remove '('

                    if(operators.Count > 0 && IsFunction(operators.Peek().type, out bool multiVariable))
                    {
                        var function = functions.Pop();
                        if(multiVariable)
                        {
                            function.Arguments++;
                            output.Enqueue(new Token(TokenType.Number, function.Arguments)); //Add function argument count
                        }
                        output.Enqueue(operators.Pop());
                    }
                    break;
                case TokenType.Comma:
                    // Ensure function arguments are separated correctly
                    while (operators.Count > 0 && operators.Peek().type != TokenType.LeftParenthesis)
                    {
                        output.Enqueue(operators.Pop());
                    }

                    if (functions.Count > 0)
                    {
                        var func = functions.Pop();
                        func.Arguments++;
                        functions.Push(func);
                    }

                    break;
                case TokenType.LessThan:
                case TokenType.LessThanOrEqual:
                case TokenType.GreaterThan:
                case TokenType.GreaterThanOrEqual:
                case TokenType.EqualsInsensitive:
                case TokenType.Equals:
                case TokenType.NotEquals:
                case TokenType.Operator:
                    while (operators.Count > 0 &&
                           IsOperator(operators.Peek().type) &&
                           OperatorPrecedence[operators.Peek().Value.ToString()!] >= OperatorPrecedence[token.Value.ToString()!])
                    {
                        output.Enqueue(operators.Pop());
                    }
                    operators.Push(token);
                    break;
            }
        }

        // Pop all remaining operators/functions from the stack
        while (operators.Count > 0)
        {
            if (operators.Peek().type == TokenType.LeftParenthesis)
            {
                throw new Exception("Mismatched parentheses in expression.");
            }
            output.Enqueue(operators.Pop());
        }

        return output;
    }


}
