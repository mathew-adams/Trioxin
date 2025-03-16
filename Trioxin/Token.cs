using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trioxin;

internal enum TokenType
{
    //Types & operators
    Number,
    Date,
    String,
    Boolean,
    Variable,
    Operator,
    LeftParenthesis,
    RightParenthesis,
    Comma,
    //Comparables
    EqualsInsensitive,
    Equals,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
    NotEquals,
    //Ranges
    Between,
    Within,
    In,
    //Strings
    Len,
    Right,
    Left,
    //Logic
    If,
    And,
    Or,
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
    CBool,
    CByte,
    CInt,
    CShort,
    CLong,
    //Generic function
    Function
}

internal record class Token(TokenType type, object? Value = null);
