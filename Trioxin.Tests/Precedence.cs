using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trioxin.Tests;

public class Precedence
{
    // 🔹 Complex, nested AND/OR
    [Fact]
    public void ComplexNestedAndOrIsTrue()
    {
        var calc = new Calculator();
        Assert.True((bool)calc.Calculate("AND(LEN(LEFT(\"hello world\", 5)) = 5, OR(NOT(false), CBool(1)))")!);
    }

    // 🔹 Complex Full
    [Fact]
    public void ComplexNestedFullTrue()
    {
        var calc = new Calculator();
        Assert.Equal(107139.29M, calc.Calculate("ROUND(((IF(AND(LEN(LEFT(\"hello world\", 5)) = 5, OR(NOT(false), CBool(1))), ((CInt(10) + CByte(5)) * (CLong(100000) / CShort(2))), (CInt(20) - CInt(5))) + (CBool(Between(15, 10, 20)) * CBool(Within(8, 5, 10))) - (LEN(RIGHT(\"abcdef\", 2)) * (IF(CBool(0), CInt(5), CInt(10)) + CShort(3)))) / (CByte(2) + (NOT(AND(true, false, OR(true, false))) * 5))),2)")!);
    }
}
