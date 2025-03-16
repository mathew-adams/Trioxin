using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trioxin.Tests;

public class Logical
{
    // 🔹 If 
    [Theory]
    [InlineData(true, "Yes", "No", "Yes")]
    [InlineData(false, "Yes", "No", "No")]
    public void If_ShouldReturnCorrectValue(bool condition, string trueResult, string falseResult, string expected)
    {
        var calc = new Calculator();
        var result = calc.Calculate($"IF({condition},'{trueResult}','{falseResult}')");
        Assert.Equal(expected, result);
    }

    // 🔹 And
    [Fact]
    public void And_ShouldReturnCorrectValue()
    {
        var calc = new Calculator();
        Assert.True((bool)calc.Calculate("AND(true, true, true, true)")!);
        Assert.True((bool)calc.Calculate("AND(true, true)")!);
        Assert.False((bool)calc.Calculate("AND(true, false)")!);
    }

    // 🔹 Or
    [Fact]
    public void Or_ShouldReturnCorrectValue()
    {
        var calc = new Calculator();
        Assert.True((bool)calc.Calculate("OR(true, false)")!);
        Assert.False((bool)calc.Calculate("OR(false, false)")!);
    }

    // 🔹 Not
    [Fact]
    public void Not_ShouldReturnCorrectValue()
    {
        var calc = new Calculator();
        Assert.False((bool)calc.Calculate("NOT(1+1=2)")!);
        Assert.True((bool)calc.Calculate("NOT(1+2=2)")!);
    }
}
