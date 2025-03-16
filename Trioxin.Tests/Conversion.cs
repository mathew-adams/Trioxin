using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trioxin;

namespace Trioxin.Tests;

public class Conversion
{
    // 🔹 CBool
    [Theory]
    [InlineData(1, true)]
    [InlineData(0, false)]
    public void CBool_ShouldConvertToBoolean(int input, bool expected)
    {
        var calc = new Calculator();
        Assert.Equal(expected, calc.Calculate($"CBool({input})"));
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    public void CBool_ShouldConvertStringToBoolean(string input, bool expected)
    {
        var calc = new Calculator();
        Assert.Equal(expected, calc.Calculate($"CBool({input})"));
    }

    // 🔹 CByte
    [Fact]
    public void CByte_ShouldConvertToByte()
    {
        var calc = new Calculator();
        Assert.Equal((byte)10, calc.Calculate("CByte(10)"));
    }

    // 🔹 CInt
    [Fact]
    public void CInt_ShouldConvertToInteger()
    {
        var calc = new Calculator();
        Assert.Equal(10, calc.Calculate("CInt(10)"));
    }

    // 🔹 CShort
    [Fact]
    public void CShort_ShouldConvertToShort()
    {
        var calc = new Calculator();
        Assert.Equal((short)10, calc.Calculate("CShort(10)"));
    }

    // 🔹 CLong
    [Fact]
    public void CLong_ShouldConvertToLong()
    {
        var calc = new Calculator();
        Assert.Equal(100000L, calc.Calculate("CLong(100000)"));
    }
}
