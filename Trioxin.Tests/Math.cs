namespace Trioxin.Tests;
using Trioxin;

public class Math
{
    [Fact]
    public void Add_ShouldReturnCorrectValue()
    {
        var calc = new Calculator((name) => name switch
        {
            "a" => 1,
            "b" => 2,
            _ => $"Unknown operand: {name}"
        });
        Assert.Equal(calc.Calculate("a + b"), 3M);
    }

    [Fact]
    public void Subtract_ShouldReturnCorrectValue()
    {
        var calc = new Calculator((name) => name switch
        {
            "a" => 4,
            "b" => 2,
            _ => $"Unknown operand: {name}"
        });
        Assert.Equal(calc.Calculate("a - b"), 2M);
    }

    [Fact]
    public void Divide_ShouldReturnCorrectValue()
    {
        var calc = new Calculator((name) => name switch
        {
            "a" => 4,
            "b" => 2,
            _ => $"Unknown operand: {name}"
        });
        Assert.Equal(calc.Calculate("a / b"), 2M);
    }

    [Fact]
    public void Multiply_ShouldReturnCorrectValue()
    {
        var calc = new Calculator((name) => name switch
        {
            "a" => 4,
            "b" => 2,
            _ => $"Unknown operand: {name}"
        });
        Assert.Equal(calc.Calculate("a * b"), 8M);
    }

    // 🔹 Abs
    [Fact]
    public void Abs_ShouldReturnAbsoluteValue()
    {
        var calc = new Calculator();
        Assert.Equal(calc.Calculate("ABS(-1)"), 1M);
    }

    // 🔹 Round
    [Fact]
    public void Round_ShouldReturnCorrectValue()
    {
        var calc = new Calculator();
        Assert.Equal(calc.Calculate("ROUND(2.33544444, 2)"), 2.34M);
    }

}
