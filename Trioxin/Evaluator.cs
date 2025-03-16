using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trioxin;

/// <summary>
/// Provides functionality to evaluate a sequence of tokens representing an expression.
/// </summary>
internal static class Evaluator
{
    /// <summary>
    /// Evaluates the given queue of tokens and returns the computed result.
    /// </summary>
    /// <param name="tokens">The queue of tokens to evaluate.</param>
    /// <param name="variableResolver">
    /// A function that resolves variables dynamically based on a string key.
    /// If <c>null</c>, an exception is thrown when a variable is encountered.
    /// </param>
    /// <returns>The evaluated result of the expression.</returns>
    /// <exception cref="Exception">
    /// Thrown if a required variable resolver is not provided or if an unknown token type is encountered.
    /// </exception>
    internal static object? Evaluate(Queue<Token> tokens, Func<string, object>? variableResolver = null)
    {
        var stack = new Stack<object>();
        while (tokens!.Count > 0)
        {
            var token = tokens.Dequeue();
            switch (token.type)
            {
                case TokenType.Number:
                case TokenType.Boolean:
                case TokenType.String:
                case TokenType.Date: stack.Push(token.Value!); break;
                case TokenType.Variable:
                    {
                        if (variableResolver == null) throw new Exception("No event for variable value specified");

                        var variable = (string)token.Value!;
                        switch (variable)
                        {
                            case var x when x.Equals("NOW", StringComparison.OrdinalIgnoreCase): stack.Push(DateTime.Now); break;
                            case var x when x.Equals("LOWDATE", StringComparison.OrdinalIgnoreCase): stack.Push(new DateTime(1900, 01, 01)); break;
                            case var x when x.Equals("HIGHDATE", StringComparison.OrdinalIgnoreCase): stack.Push(new DateTime(9999, 12, 31)); break;
                            case var x when x.Equals("NOW_DATE", StringComparison.OrdinalIgnoreCase): stack.Push(DateOnly.FromDateTime(DateTime.Now)); break;
                            case var x when x.Equals("NOW_TIME", StringComparison.OrdinalIgnoreCase): stack.Push(TimeOnly.FromDateTime(DateTime.Now)); break;
                            default:
                                {
                                    var result = variableResolver.Invoke(variable);
                                    var type = result.GetType();
                                    if (type.IsEnum)
                                    {
                                        var baseType = Enum.GetUnderlyingType(type);
                                        stack.Push(baseType switch
                                        {
                                            var x when x == typeof(byte) => (byte)result,
                                            var x when x == typeof(sbyte) => (sbyte)result,
                                            var x when x == typeof(ushort) => (ushort)result,
                                            var x when x == typeof(short) => (short)result,
                                            var x when x == typeof(uint) => (uint)result,
                                            var x when x == typeof(int) => (int)result,
                                            _ => result
                                        });
                                    }
                                    else
                                    {
                                        stack.Push(type switch
                                        {
                                            var x when x == typeof(bool) => (bool)result,
                                            var x when x == typeof(byte) => (byte)result,
                                            var x when x == typeof(sbyte) => (sbyte)result,
                                            var x when x == typeof(ushort) => (ushort)result,
                                            var x when x == typeof(short) => (short)result,
                                            var x when x == typeof(uint) => (uint)result,
                                            var x when x == typeof(int) => (int)result,
                                            var x when x == typeof(ulong) => (ulong)result,
                                            var x when x == typeof(long) => (long)result,
                                            var x when x == typeof(decimal) => (decimal)result,
                                            var x when x == typeof(double) => (double)result,
                                            var x when x == typeof(DateOnly) => (DateOnly)result,
                                            var x when x == typeof(TimeOnly) => (TimeOnly)result,
                                            var x when x == typeof(DateTime) => (DateTime)result,
                                            var x when x == typeof(Guid) => (Guid)result,
                                            _ => result
                                        });
                                    }
                                }
                                break;
                        }
                        break;
                    }
                case TokenType.EqualsInsensitive:
                    {
                        string right = (string)stack.Pop();
                        string left = (string)stack.Pop();
                        stack.Push(left.Equals(right, StringComparison.OrdinalIgnoreCase));
                    }
                    break;
                case TokenType.Equals:
                case TokenType.NotEquals:
                case TokenType.LessThan:
                case TokenType.LessThanOrEqual:
                case TokenType.GreaterThan:
                case TokenType.GreaterThanOrEqual:
                    {
                        object rightSide = stack.Pop();
                        object leftSide = stack.Pop();
                        stack.Push(CompareObjects(leftSide, rightSide, (left, right) =>
                        {
                            return token.type switch
                            {
                                TokenType.Equals => left.CompareTo(right) == 0,
                                TokenType.NotEquals => left.CompareTo(right) != 0,
                                TokenType.LessThan => left.CompareTo(right) < 0,
                                TokenType.LessThanOrEqual => left.CompareTo(right) <= 0,
                                TokenType.GreaterThan => left.CompareTo(right) > 0,
                                TokenType.GreaterThanOrEqual => left.CompareTo(right) >= 0,
                                _ => throw new Exception($"Invalid token type: {token.type}")
                            };
                        }));
                    }
                    break;
                case TokenType.Operator:
                    {
                        if (stack.Count == 1 && stack.Peek() is decimal dec)
                        {
                            stack.Push(-1 * dec); //Handle negative numbers
                        }
                        else if (stack.Count < 2) throw new Exception("Invalid expression");
                        else
                        {
                            object right = stack.Pop();
                            object left = stack.Pop();
                            if (left is DateTime date && double.TryParse(right.ToString(), out var days))
                            {
                                stack.Push(date.AddDays(days));
                            }
                            else if (right is string rs && left is string ls)
                            {
                                stack.Push(string.Concat(ls, rs));
                            }
                            else
                            {
                                decimal doubleRight = Convert.ToDecimal(right);
                                decimal doubleLeft = Convert.ToDecimal(left);
                                stack.Push(token.Value switch
                                {
                                    '+' => doubleLeft + doubleRight,
                                    '-' => doubleLeft - doubleRight,
                                    '*' => doubleLeft * doubleRight,
                                    '/' => doubleLeft / doubleRight,
                                    '^' => Math.Pow((double)doubleLeft, (double)doubleRight),
                                    _ => throw new Exception($"Unknown operator: {token.Value}")
                                });
                            }
                        }
                        break;
                    }
                case TokenType.In:
                    {
                        var args = new List<object>();
                        var argCount = (int)stack.Pop();
                        for (int i = 0; i < argCount - 1; i++)
                        {
                            args.Add(stack.Pop());
                        }
                        var term = stack.Pop();
                        stack.Push(args.Contains(term));
                    }
                    break;
                case TokenType.Within:
                    {
                        var end = stack.Pop();
                        var start = stack.Pop();
                        var term = stack.Pop();

                        var afterOrOnStart = CompareObjects(term, start, (left, right) => left.CompareTo(right) >= 0);
                        var beforeOrOnEnd = CompareObjects(term, end, (left, right) => left.CompareTo(right) <= 0);
                        stack.Push(afterOrOnStart && beforeOrOnEnd);
                    }
                    break;
                case TokenType.Between:
                    {
                        var end = stack.Pop();
                        var start = stack.Pop();
                        var term = stack.Pop();

                        var afterBegin = CompareObjects(term, start, (left, right) => left.CompareTo(right) > 0);
                        var beforEnd = CompareObjects(term, end, (left, right) => left.CompareTo(right) < 0);
                        stack.Push(afterBegin && beforEnd);
                    }
                    break;
                case TokenType.Or:
                case TokenType.And:
                case TokenType.Max:
                case TokenType.Min:
                case TokenType.Avg:
                case TokenType.Sum:
                    {
                        var args = new List<object>();
                        var argCount = (int)stack.Pop();
                        for (int i = 0; i < argCount; i++)
                        {
                            args.Add(stack.Pop());
                        }
                        stack.Push(token.type switch
                        {
                            TokenType.Min => args.Min()!,
                            TokenType.Max => args.Max()!,
                            TokenType.Avg => args.Average(a => (decimal)a),
                            TokenType.Sum => args.Sum(s => (decimal)s),
                            TokenType.And => args.All(a => a is bool b && b),
                            TokenType.Or => args.Any(a => a is bool b && b),
                            _ => throw new Exception("Unknown token type")
                        });
                    }
                    break;
                case TokenType.Not:
                    {
                        var result = stack.Pop();
                        stack.Push(!(bool)result);
                    }
                    break;
                case TokenType.If:
                    {
                        var resultFalse = stack.Pop();
                        var resultTrue = stack.Pop();
                        var condition = (bool)stack.Pop();
                        if (condition) stack.Push(resultTrue);
                        else stack.Push(resultFalse);
                    }
                    break;
                case TokenType.Round:
                    {
                        var digits = Convert.ToInt32(stack.Pop());
                        var number = (decimal)stack.Pop();
                        stack.Push(Math.Round(number, digits));
                    }
                    break;
                case TokenType.Abs:
                    {
                        var number = (decimal)stack.Pop();
                        stack.Push(Math.Abs(number));
                    }
                    break;
                case TokenType.Len:
                    {
                        var variable = (string)stack.Pop();
                        stack.Push(variable.Length);
                    }
                    break;
                case TokenType.Right:
                case TokenType.Left:
                    {
                        var count = Convert.ToInt32(stack.Pop());
                        var term = (string)stack.Pop();
                        stack.Push(token.type switch
                        {
                            TokenType.Right => term[^count..],
                            _ => term[..count]
                        });
                    }
                    break;
                case TokenType.CBool:
                case TokenType.CByte:
                case TokenType.CInt:
                case TokenType.CShort:
                case TokenType.CLong:
                    {
                        var result = stack.Pop();
                        stack.Push(token.type switch
                        {
                            TokenType.CBool => Convert.ToBoolean(result),
                            TokenType.CByte => Convert.ToByte(result),
                            TokenType.CShort => Convert.ToInt16(result),
                            TokenType.CInt => Convert.ToInt32(result),
                            TokenType.CLong => Convert.ToInt64(result),
                            _ => throw new NotImplementedException($"Unknown token type: {token.type}")
                        });
                        break;
                    }
                case TokenType.Function:
                    {
                        string function = (string)token.Value;
                        if (function.Equals("IF", StringComparison.OrdinalIgnoreCase))
                        {

                        }

                    }
                    break;
                default:
                    throw new Exception($"Unknown type: {token.type}");
            }
        }
        return stack.Pop();
    }
    /// <summary>
    /// Computes an operation based on token type.
    /// </summary>
    private static bool CompareObjects(object left, object right, Func<IComparable, IComparable, bool> comparer)
    {
        var typeLeft = left.GetType();
        var typeRight = right.GetType();
        if (typeLeft == typeof(bool) && typeRight == typeof(bool)) return comparer.Invoke((bool)left, (bool)right);
        if (typeLeft == typeof(byte) && typeRight == typeof(byte)) return comparer.Invoke((byte)left, (byte)right);
        if (typeLeft == typeof(sbyte) && typeRight == typeof(sbyte)) return comparer.Invoke((sbyte)left, (sbyte)right);
        if (typeLeft == typeof(ushort) && typeRight == typeof(ushort)) return comparer.Invoke((ushort)left, (ushort)right);
        if (typeLeft == typeof(short) && typeRight == typeof(short)) return comparer.Invoke((short)left, (short)right);
        if (typeLeft == typeof(uint) && typeRight == typeof(uint)) return comparer.Invoke((uint)left, (uint)right);
        if (typeLeft == typeof(int) && typeRight == typeof(int)) return comparer.Invoke((int)left, (int)right);
        if (typeLeft == typeof(ulong) && typeRight == typeof(ulong)) return comparer.Invoke((ulong)left, (ulong)right);
        if (typeLeft == typeof(long) && typeRight == typeof(long)) return comparer.Invoke((long)left, (long)right);
        if (typeLeft == typeof(decimal) && typeRight == typeof(decimal)) return comparer.Invoke((decimal)left, (decimal)right);
        if (typeLeft == typeof(double) && typeRight == typeof(double)) return comparer.Invoke((double)left, (double)right);
        if (typeLeft == typeof(string) && typeRight == typeof(string)) return comparer.Invoke((string)left, (string)right);
        if (typeLeft == typeof(DateTime) && typeRight == typeof(DateTime)) return comparer.Invoke((DateTime)left, (DateTime)right);
        if (typeLeft == typeof(DateOnly) && typeRight == typeof(DateOnly)) return comparer.Invoke((DateOnly)left, (DateOnly)right);
        if (typeLeft == typeof(Guid) && typeRight == typeof(Guid)) return comparer.Invoke((Guid)left, (Guid)right);

        //We are trying to deal with decimals when we encounter numerics
        if (decimal.TryParse(left.ToString(), out decimal decParseLeft) 
            && decimal.TryParse(right.ToString(), out decimal decParseRight)) return comparer.Invoke(decParseLeft, decParseRight);

        throw new Exception($"Unknown comparison for objects, Left: {left}, Right: {right}");
    }

}
