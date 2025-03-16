using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Trioxin;

/// <summary>
/// Provides validation functionality for models of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the model being validated.</typeparam>
/// <remarks>
/// This class is responsible for validating models dynamically. It uses a resolver
/// function to retrieve values or properties required for validation.
/// </remarks>
public class ModelValidator<T>
{
    #region Static type properties
    private static Type _type = typeof(T);
    private static readonly Dictionary<string, TrioxinRule[]> _fields = GetFieldsAndRules();
    private static Dictionary<string, TrioxinRule[]> GetFieldsAndRules()
    {
        Dictionary<string, TrioxinRule[]> rules = [];
        var properties = _type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var property in properties)
        {
            var trioxinRules = property.GetCustomAttributes<TrioxinRule>();
            rules.Add(property.Name, trioxinRules.ToArray());
        }
        return rules;
    }
    #endregion

    #region Private variables
    private readonly Calculator _calc;
    private T? _context;
    private Dictionary<string, RuleResult> _results = [];
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="ModelValidator"/> class.
    /// </summary>
    /// <param name="resolver">
    /// A function that takes a string key and returns the corresponding object.
    /// Used for resolving dependencies or model properties dynamically.
    /// </param>
    public ModelValidator(Func<string, object> resolver)
    {
        _calc = new Calculator((name) =>
        {
            if (_context.TryGetValue(name, out var value)) return value!;
            return resolver.Invoke(name);
        });
        foreach(var field in _fields) _results.Add(field.Key, new RuleResult()); //Initialize collection
    }

    /// <summary>
    /// Returns a <see cref="RuleResult"/> with the following properties:
    /// <list type="bullet">
    ///   <item><description>Visible</description></item>
    ///   <item><description>Editable</description></item>
    ///   <item><description>Error</description></item>
    ///   <item><description>Required</description></item>
    /// </list>
    /// </summary>
    /// <param name="key">Model property name</param>
    /// <returns>A <see cref="RuleResult"/></returns>
    public RuleResult this[string key]
    {
        get => _results[key];
    }

    /// <summary>
    /// Validates the given context, setting properties to calculated values.
    /// </summary>
    /// <typeparam name="T">The type of the context being validated.</typeparam>
    /// <param name="context">The context to validate, passed by reference.</param>
    public void Validate(ref T context)
    {
        _context = context;
        foreach (var field in _fields)
        {
            RuleResult result = new();
            foreach (var rule in field.Value)
            {
                if (string.IsNullOrEmpty(rule.Condition)
                    || (_calc.Calculate(rule.Condition) is bool conditionResult && conditionResult))
                {
                    var calcResult = _calc.Calculate(rule.Rule);
                    var boolResult = calcResult is bool b && b;
                    switch (rule.Type)
                    {
                        case Rule.Visible: result.Visible = boolResult; break;
                        case Rule.Enabled: result.Enabled = boolResult; break;
                        case Rule.Calculation:
                            {
                                PropertyInfo property = _type.GetProperty(field.Key)!;
                                if (property is null && !property!.CanWrite) continue;
                                property.SetValue(context, calcResult);
                                break;
                            }
                        case Rule.Required:
                            {
                                result.Error = boolResult;
                                result.Required = boolResult;
                                if (result.Required)
                                {
                                    if (string.IsNullOrEmpty(rule.MessageKey)) rule.MessageKey = $"{field.Key}.Required";
                                    result.MessageKeys.Add(rule.MessageKey);
                                }
                                break;
                            }
                        case Rule.Error:
                            {
                                result.Error = boolResult;
                                if (result.Error) result.MessageKeys.Add(rule.MessageKey);
                                break;
                            }
                        default:
                            throw new Exception($"Unknown rule type: {rule.Type}");
                    }
                }
                _results[field.Key] = result;
            }
        }
    }
}

/// <summary>
/// Provides extension methods for retrieving property values dynamically from objects.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Retrieves the value of a specified property from the given object.
    /// </summary>
    /// <typeparam name="T">The type of the object containing the property.</typeparam>
    /// <param name="context">The object instance from which to retrieve the property value.</param>
    /// <param name="name">The name of the property to retrieve.</param>
    /// <returns>The value of the specified property.</returns>
    /// <exception cref="Exception">
    /// Thrown if the specified property does not exist or cannot be retrieved.
    /// </exception>
    public static object? GetValue<T>(this T context, string name)
    {
        if(TryGetValue<T>(context, name, out object? value)) return value;
        throw new Exception($"Value not found in context: {typeof(T)}, name: {name}");
    }
    /// <summary>
    /// Attempts to retrieve the value of a specified property from the given object.
    /// </summary>
    /// <typeparam name="T">The type of the object containing the property.</typeparam>
    /// <param name="context">The object instance from which to retrieve the property value.</param>
    /// <param name="name">The name of the property to retrieve.</param>
    /// <param name="value">When this method returns, contains the value of the property if found; otherwise, <c>null</c>.</param>
    /// <returns><c>true</c> if the property was found and its value retrieved; otherwise, <c>false</c>.</returns>
    public static bool TryGetValue<T>(this T context, string name, out object? value)
    {
        value = null;
        Type type = typeof(T);
        PropertyInfo prop = type.GetProperty(name)!;
        if (prop == null) return false;
        if (!prop.CanRead) return false;

        value = prop.GetValue(context);
        return true;
    }
}