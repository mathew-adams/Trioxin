using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trioxin;

public class TokenReader(string input)
{
    private int _position = 0;
    public char Peek()
    {
        if (_position >= input.Length) return char.MaxValue;
        return input[_position];
    }

    public char Read()
    {
        if (_position > input.Length) return char.MaxValue;
        return input[_position++];
    }

    public bool EndOfStream => _position >= input.Length;

    public string EatWhile(Func<char, bool> condition)
    {
        int start = _position;
        while (_position < input.Length && condition(input[_position]))
        {
            _position++;
        }
        return input[start.._position];
    }
}