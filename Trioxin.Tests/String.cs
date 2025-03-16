using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trioxin.Tests;

public class String
{
    // 🔹 Len
    [Fact]
    public void Len_ShouldReturnStringLength()
    {
        var calc = new Calculator();
        Assert.Equal(5, calc.Calculate("LEN('hello')")!);
    }

    // 🔹 Right
    [Fact]
    public void Right_ShouldReturnRightSubstring()
    {
        var calc = new Calculator();
        Assert.Equal("lo", (string)calc.Calculate("RIGHT('hello', 2)")!);
    }

    // 🔹 Left
    [Fact]
    public void Left_ShouldReturnLeftSubstring()
    {
        var calc = new Calculator();
        Assert.Equal("he", (string)calc.Calculate("LEFT('hello', 2)")!);
    }
}
