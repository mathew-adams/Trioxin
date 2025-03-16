using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trioxin.Tests;

public class Aggregate
{
    // 🔹 Min
    [Fact]
    public void Min_ShouldReturnSmallestNumber()
    {
        var calc = new Calculator();
        Assert.Equal(2M, calc.Calculate("MIN(2, 3, 4, 5)")!);
    }

    // 🔹 Max
    [Fact]
    public void Max_ShouldReturnLargestNumber()
    {
        var calc = new Calculator();
        Assert.Equal(5M, calc.Calculate("MAX(2, 3, 4, 5)")!);
    }

    // 🔹 Sum
    [Fact]
    public void Sum_ShouldReturnTotalSum()
    {
        var calc = new Calculator();
        Assert.Equal(15M, calc.Calculate("SUM(1, 2, 3, 4, 5)")!);
    }

    // 🔹 Avg
    [Fact]
    public void Avg_ShouldReturnAverageValue()
    {
        var calc = new Calculator();
        Assert.Equal(3M, calc.Calculate("AVG(1, 2, 3, 4, 5)")!);
    }
}
