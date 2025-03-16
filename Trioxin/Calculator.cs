using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace Trioxin;

/// <summary>
/// A simple calculator that processes mathematical expressions.
/// </summary>
public class Calculator
{
    /// <summary>
    /// A function that resolves variable values based on a string key.
    /// </summary>
    private readonly Func<string, object>? _resolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="Calculator"/> class.
    /// </summary>
    public Calculator(){ }

    /// <summary>
    /// Initializes a new instance of the <see cref="Calculator"/> class with a resolver function.
    /// </summary>
    /// <param name="resolver">
    /// A function that takes a string key and returns the corresponding object.
    /// Used to resolve variables in expressions.
    /// </param>
    public Calculator(Func<string, object>? resolver) =>  _resolver = resolver;

    /// <summary>
    /// Parses and evaluates a mathematical expression.
    /// </summary>
    /// <param name="input">The mathematical expression as a string.</param>
    /// <returns>
    /// The result of the evaluation as an object, or an empty string if the input is null.
    /// </returns>
    public object? Calculate(string input)
    {
        if (input == null) return "";
        var tokens = Tokenizer.CreateTokens(input);
        var rpns = Postfix.ConvertFromTokens(tokens);
        return Evaluator.Evaluate(rpns, _resolver);
    }
}
