using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trioxin;

namespace Trioxin.Tests;

public class Comparison
{
    // 🔹 EqualsInsensitive (Case-insensitive string comparison)
    [Theory]
    [InlineData("hello", "HELLO", true)]
    [InlineData("world", "World", true)]
    [InlineData("test", "tesTing", false)]
    public void EqualsInsensitive_ShouldCompareStringsCorrectly(string a, string b, bool expected)
    {
        var calc = new Calculator((name) => name switch
        {
            "a" => a,
            "b" => b,
            _ => throw new Exception($"Unknown operand: {name}")
        });
        Assert.Equal(expected, (bool)calc.Calculate("a ~= b")!);
    }

    // 🔹 EqualsSensitive (Case-sensitive string comparison)
    [Theory]
    [InlineData("hello", "HELLO", false)]
    [InlineData("world", "World", false)]
    [InlineData("Hello", "Hello", true)]
    [InlineData("World", "World", true)]
    [InlineData("test", "tesTing", false)]
    public void EqualsSensitive_ShouldCompareStringsCorrectly(string a, string b, bool expected)
    {
        var calc = new Calculator((name) => name switch
        {
            "a" => a,
            "b" => b,
            _ => throw new Exception($"Unknown operand: {name}")
        });
        Assert.Equal(expected, (bool)calc.Calculate("a = b")!);
    }

    // 🔹 Equals
    [Theory]
    [InlineData(5, 5, true)]
    [InlineData(10, 5, false)]
    public void Equals_ShouldReturnCorrectValue(int a, int b, bool expected)
    {
        var calc = new Calculator((name) => name switch
        {
            "a" => a,
            "b" => b,
            _ => throw new Exception($"Unknown operand: {name}")
        });
        Assert.Equal(expected, calc.Calculate("a=b"));
    }

    // 🔹 GreaterThan
    [Fact]
    public void GreaterThan_ShouldReturnTrueWhenGreater()
    {
        var calc = new Calculator();
        Assert.True((bool)calc.Calculate("10>5")!);
    }

    // 🔹 GreaterThanOrEqual
    [Fact]
    public void GreaterThanOrEqual_ShouldReturnTrueWhenEqualOrGreater()
    {
        var calc = new Calculator();
        Assert.True((bool)calc.Calculate("10 >= 10")!);
        Assert.True((bool)calc.Calculate("10 >= 5")!);
    }

    // 🔹 LessThan
    [Fact]
    public void LessThan_ShouldReturnTrueWhenLess()
    {
        var calc = new Calculator();
        Assert.True((bool)calc.Calculate("3 < 5")!);
    }

    // 🔹 LessThanOrEqual
    [Fact]
    public void LessThanOrEqual_ShouldReturnTrueWhenEqualOrLess()
    {
        var calc = new Calculator();
        Assert.True((bool)calc.Calculate("5 <= 5")!);
        Assert.True((bool)calc.Calculate("3 <= 5")!);
    }

    // 🔹 NotEquals
    [Fact]
    public void NotEquals_ShouldReturnTrueWhenDifferent()
    {
        var calc = new Calculator();
        Assert.True((bool)calc.Calculate("5 != 10")!);
    }

    // 🔹 Between
    [Theory]
    [InlineData(5, 1, 10, true)]
    [InlineData(0, 1, 10, false)]
    public void Between_ShouldCheckIfNumberIsWithinRange(int value, int min, int max, bool expected)
    {
        var calc = new Calculator((name) => name switch
        {
            "min" => min,
            "max" => max,
            "value" => value,
            _ => throw new Exception($"Unknown operand: {name}")
        });
        Assert.Equal(expected, (bool)calc.Calculate("Between(value, min, max)")!);
    }

    // 🔹 Within (Inclusive between)
    [Theory]
    [InlineData(1, 1, 10, true)]
    [InlineData(10, 1, 10, true)]
    [InlineData(0, 1, 10, false)]
    public void Within_ShouldCheckIfWithinTolerance(int value, int min, int max, bool expected)
    {
        var calc = new Calculator((name) => name switch
        {
            "min" => min,
            "max" => max,
            "value" => value,
            _ => throw new Exception($"Unknown operand: {name}")
        });
        Assert.Equal(expected, (bool)calc.Calculate("Within(value, min, max)")!);
    }

    // 🔹 In (List Contains)
    [Theory]
    [InlineData("IN(1,1,2,3,4)", true)]
    [InlineData("IN(5,1,2,3,4)", false)]
    [InlineData("IN('one','one','two','three','four')", true)]
    [InlineData("IN('five','one','two','three','four')", false)]
    public void In_ShouldCheckIfElementExists(string expression, bool expected)
    {
        var calc = new Calculator();
        Assert.Equal(expected, (bool)calc.Calculate(expression)!);
    }
}
