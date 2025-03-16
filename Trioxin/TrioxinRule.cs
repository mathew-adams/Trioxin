using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trioxin;

public enum Rule
{
    None,
    Required,
    Error,
    Enabled,
    Visible,
    Calculation
}

/// <summary>
/// Represents the result of a validation rule, including visibility, errors, and messages.
/// </summary>
public class RuleResult()
{
    /// <summary>
    /// Indicates whether the rule is required.
    /// </summary>
    public bool Required { get; set; }
    /// <summary>
    /// Indicates whether the rule encountered an error.
    /// </summary>
    public bool Error { get; set; }
    /// <summary>
    /// Determines if the rule is enabled. Defaults to <c>true</c>.
    /// </summary>
    public bool Enabled { get; set; } = true;
    /// <summary>
    /// Determines if the rule is visible. Defaults to <c>true</c>.
    /// </summary>
    public bool Visible { get; set; } = true;
    /// <summary>
    /// A list of message keys associated with the rule result.
    /// </summary>
    public List<string> MessageKeys { get; set; } = [];
    /// <summary>
    /// Returns a string representation of the rule result.
    /// </summary>
    public override string ToString()
    {
        return $"Required: {Required}, Error: {Error}, Enabled: {Enabled}, Visible {Visible}, MessageKeys: {string.Join(",", MessageKeys)}";
    }
}

/// <summary>
/// Specifies a validation rule that can be applied to properties or fields.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class TrioxinRule : Attribute
{
    /// <summary>
    /// Gets or sets the type of the rule.
    /// </summary>
    public required Rule Type { get; set; }
    /// <summary>
    /// Gets or sets the condition expression that must be met for the rule to apply.
    /// Defaults to an empty string.
    /// </summary>
    public string Condition { get; set; } = "";
    /// <summary>
    /// Gets or sets the rule expression.
    /// </summary>
    public required string Rule { get; set; } = "";
    /// <summary>
    /// Gets or sets the message key associated with the rule.
    /// Defaults to an empty string.
    /// </summary>
    public string MessageKey { get; set; } = "";
}