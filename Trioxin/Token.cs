using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trioxin;

internal enum TokenType
{
    //Types & operators
    Date,
    Comma,
    Number,
    String,
    Boolean,
    Operator,
    Variable,
    LeftParenthesis,
    RightParenthesis,
    //Comparables
    Equals,
    LessThan,
    NotEquals,
    GreaterThan,
    LessThanOrEqual,
    EqualsInsensitive,
    GreaterThanOrEqual,
    //Ranges
    In,
    Within,
    Between,
    //Strings
    Len,
    Left,
    Right,
    //Logic
    If,
    Or,
    And,
    Not,
    //Aggregate
    Min,
    Max,
    Sum,
    Avg,
    //Math
    Abs,
    Round,
    //Conversion
    CInt,
    CLong,
    CBool,
    CByte,
    CShort,
}

internal record class Token(TokenType type, object? Value = null);
